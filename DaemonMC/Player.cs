using System.Net;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network;
using DaemonMC.Utils.Text;
using fNbt;
using System.Numerics;
using DaemonMC.Level;
using DaemonMC.Utils;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Game;
using DaemonMC.Level.Generators;
using DaemonMC.Plugin.Plugin;

namespace DaemonMC
{
    public class Player
    {
        public string Username { get; set; }
        public Skin Skin { get; set; } = new Skin();
        public Guid UUID { get; set; } = new Guid();
        public string XUID { get; set; }
        public long EntityID { get; set; }
        private long dataValue { get; set; }
        public long Tick { get; set; }
        public Vector3 Position { get; set; } = new Vector3(0, 1, 0);
        public Vector2 Rotation { get; set; } = new Vector2(0, 0);
        public int drawDistance { get; set; }
        public IPEndPoint ep { get; set; }
        public World currentLevel { get; set; }
        public AttributesValues attributes { get; set; } = new AttributesValues(0.1f);
        public Dictionary<ActorData, Metadata> metadata { get; set; } = new Dictionary<ActorData, Metadata>();
        public List<AuthInputData> InputData = new List<AuthInputData>();

        private Queue<(int x, int z)> ChunkSendQueue = new Queue<(int x, int z)>();
        private bool SendQueueBusy = false;
        public bool Spawned = false;
        private int LastChunkX = 0;
        private int LastChunkZ = 0;

        public void spawn()
        {
            SendStartGame();
            SendItemData();
            SendCreativeInventory();
            SendBiomeDefinitionList();
            SendPlayStatus(3);
            SendGameRules();
            UpdateAttributes();
            SendMetadata(true);
            currentLevel.addPlayer(this);
            Log.info($"{Username} spawned in World:'{currentLevel.levelName}' X:{Position.X} Y:{Position.Y} Z:{Position.Z}");
        }

        public void SendStartGame()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new StartGame
            {
                LevelName = currentLevel.LevelDisplayName,
                EntityId = EntityID,
                GameType = 0,
                GameMode = 2,
                Position = new Vector3(Position.X, Position.Y, Position.Z),
                Rotation = new Vector2(0, 0),
                SpawnBlockX = (int)Position.X,
                SpawnBlockY = (int)Position.Y,
                SpawnBlockZ = (int)Position.Z,
                Difficulty = 1,
                Dimension = 0,
                Seed = currentLevel.RandomSeed,
                Generator = 1,
            };
            packet.Encode(encoder);
        }

        public void SendItemData()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new ItemRegistry
            {

            };
            packet.Encode(encoder);
        }

        public void SendCreativeInventory()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new CreativeContent
            {

            };
            packet.Encode(encoder);
        }

        public void SendBiomeDefinitionList()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new BiomeDefinitionList
            {
                biomeData = new fNbt.NbtCompound("")
                {
                new NbtCompound("plains")
                    {
                        new NbtFloat("downfall", 0.4f),
                        new NbtFloat("temperature", 0.8f),
                    }
                }
            };
            packet.Encode(encoder);
        }

        public void SendPlayStatus(int status)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new PlayStatus
            {
                status = status,
            };
            packet.Encode(encoder);
        }

        public void SendChunkToPlayer(int chunkX, int chunkZ)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            List<byte> chunkData = new List<byte>();
            int chunkCount = 0;

            if (currentLevel.temporary)
            {
                chunkData = new List<byte>(new SuperFlat().generateChunks());
                chunkCount = 20;
            }
            else
            {
                var chunkRaw = currentLevel.GetChunk(chunkX, chunkZ);
                chunkData = new List<byte>(chunkRaw.networkSerialize(this));
                chunkCount = chunkRaw.chunks.Count();
                if (chunkRaw.chunks.Count == 0)
                {
                    chunkData = new List<byte>(new SuperFlat().generateChunks());
                    chunkCount = 20;
                }
            }

            var chunk = new LevelChunk
            {
                chunkX = chunkX,
                chunkZ = chunkZ,
                count = chunkCount,
                data = chunkData.ToArray()
            };
            chunk.Encode(encoder);
        }

        public void UpdateChunkRadius(int radius)
        {
            drawDistance = radius;
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new ChunkRadiusUpdated
            {
                radius = drawDistance,
            };
            packet.Encode(encoder);
        }

        public void UpdateAttributes()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new UpdateAttributes
            {
                EntityId = EntityID,
                Attributes = new List<AttributeValue> { attributes.Movement_speed() }
            };
            packet.Encode(encoder);
        }

        public void SendMetadata(bool broadcast = false)
        {
            if (dataValue == 0)
            {
                SetFlag(ActorFlags.ALWAYS_SHOW_NAME, true);
                SetFlag(ActorFlags.HAS_COLLISION, true);
                SetFlag(ActorFlags.HAS_GRAVITY, true);
                SetFlag(ActorFlags.FIRE_IMMUNE, true);
            }

            metadata[ActorData.RESERVED_0] = new Metadata(dataValue);

            Dictionary<long, Player> players = new Dictionary<long, Player>(){ { EntityID, this } };

            if (broadcast)
            {
                players = currentLevel.onlinePlayers;
            }

            foreach (var dest in players)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                var packet = new SetActorData
                {
                    EntityId = EntityID,
                    Metadata = metadata,
                    Tick = Tick
                };
                packet.Encode(encoder);
            }
        }

        public void SendGameRules()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new GameRulesChanged
            {
                GameRules = currentLevel.GameRules
            };
            packet.Encode(encoder);
        }

        private async void ProcessSendQueue()
        {
            if (SendQueueBusy) { return; }
            SendQueueBusy = true;
            while (ChunkSendQueue.Count > 0)
            {
                if (!Server.onlinePlayers.ContainsValue(this)) {
                    ChunkSendQueue.Clear();
                    break;
                }
                var (chunkX, chunkZ) = ChunkSendQueue.Dequeue();
                SendChunkToPlayer(chunkX, chunkZ);

                await Task.Delay(10);
            }
            SendQueueBusy = false;
        }

        public void Kick(string msg)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new Disconnect
            {
                message = msg
            };
            packet.Encode(encoder);
            Server.RemovePlayer((long)EntityID);
            RakSessionManager.deleteSession(ep);
        }

        public void SendMessage(string message)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var pk = new TextMessage
            {
                messageType = 1,
                Message = message
            };
            pk.Encode(encoder);
        }

        public void Teleport(Vector3 position)
        {
            Position = position;
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var movePk = new MovePlayer
            {
                actorRuntimeId = EntityID,
                position = position
            };
            movePk.Encode(encoder);
        }

        public void SetFlag(ActorFlags flag, bool enable)
        {
            if (enable)
            {
                dataValue |= (1L << (int)flag);
            }
            else
            {
                dataValue &= ~(1L << (int)flag);
            }
            SendMetadata(true);
        }

        public void UpdateFlags(List<AuthInputData> flags)
        {
            if (flags.Contains(AuthInputData.Sneaking))
            {
                SetFlag(ActorFlags.SNEAKING, true);
            }
            if (flags.Contains(AuthInputData.StopSneaking))
            {
                SetFlag(ActorFlags.SNEAKING, false);
            }
            if (flags.Contains(AuthInputData.StartSprinting))
            {
                SetFlag(ActorFlags.SPRINTING, true);
            }
            if (flags.Contains(AuthInputData.StopSprinting))
            {
                SetFlag(ActorFlags.SPRINTING, false);
            }
        }


        //
        //Packet processors for spawned player
        //


        private HashSet<(int x, int z)> sentChunks = new();

        public void PacketEvent_PlayerAuthInput(PlayerAuthInput packet)
        {
            Tick = packet.Tick;
            if (!InputData.SequenceEqual(packet.InputData))
            {
                Log.debug($"Data: {string.Join(" | ", packet.InputData)}");
                UpdateFlags(packet.InputData);
                InputData = packet.InputData;
            }
            if (Position != packet.Position || Rotation != packet.Rotation)
            {
                Position = packet.Position;
                Rotation = packet.Rotation;

                ushort header = 0;
                header |= 0x01;
                header |= 0x02;
                header |= 0x04;
                header |= 0x08;
                header |= 0x10;
                header |= 0x20;

                foreach (Player player in currentLevel.onlinePlayers.Values)
                {
                    if (player == this) { continue; }
                    PacketEncoder encoder = PacketEncoderPool.Get(player);
                    var movePk = new MoveActorDelta
                    {
                        EntityId = EntityID,
                        Header = header,
                        Position = Position,
                        Rotation = Rotation,
                        YheadRotation = packet.HeadRotation
                    };
                    movePk.Encode(encoder);
                }

                int currentChunkX = (int)Math.Floor(Position.X / 16.0);
                int currentChunkZ = (int)Math.Floor(Position.Z / 16.0);

                if (currentChunkX == LastChunkX && currentChunkZ == LastChunkZ)
                    return; // No movement, no update needed

                LastChunkX = currentChunkX;
                LastChunkZ = currentChunkZ;

                PacketEncoder encoder2 = PacketEncoderPool.Get(this);
                var packet2 = new NetworkChunkPublisherUpdate
                {
                    x = (int)Position.X,
                    y = (int)0,
                    z = (int)Position.Z,
                    radius = 20
                };
                packet2.Encode(encoder2);

                HashSet<(int x, int z)> newChunks = new(ChunkUtils.GetSequence(20, currentChunkX, currentChunkZ));

                foreach (var chunk in newChunks)
                {
                    if (!sentChunks.Contains(chunk))
                    {
                        sentChunks.Add(chunk);
                        ChunkSendQueue.Enqueue(chunk);
                    }
                }

                foreach (var chunk in sentChunks.ToList())
                {
                    if (!newChunks.Contains(chunk))
                    {
                        sentChunks.Remove(chunk);
                    }
                }

                ProcessSendQueue();
            }
        }

        public void PacketEvent_MovePlayer(MovePlayer packet)
        {
            if (packet.position != Position)
            {
                Position = packet.position;

                ushort header = 0; //todo
                header |= 0x01;
                header |= 0x02;
                header |= 0x04;
                /*header |= 0x08;
                header |= 0x10;
                header |= 0x20;*/

                foreach (Player player in currentLevel.onlinePlayers.Values)
                {
                    if (player == this) { continue; }
                    PacketEncoder encoder = PacketEncoderPool.Get(player);
                    var movePk = new MoveActorDelta
                    {
                        EntityId = EntityID,
                        Header = header,
                        Position = Position
                    };
                    movePk.Encode(encoder);
                }
                Log.debug($"{packet.actorRuntimeId} / {packet.position.X} : {packet.position.Y} : {packet.position.Z} / onground {packet.isOnGround}");
            }
        }

        public void PacketEvent_RequestChunkRadius(RequestChunkRadius packet)
        {
            Log.debug($"{Username} requested chunks with radius {packet.radius}. Max radius = {packet.maxRadius}");

            UpdateChunkRadius(packet.radius);

            PacketEncoder encoder2 = PacketEncoderPool.Get(this);
            var packet2 = new NetworkChunkPublisherUpdate
            {
                x = (int) Position.X,
                y = (int) Position.Y,
                z = (int) Position.Z,
                radius = drawDistance
            };
            packet2.Encode(encoder2);

            int radius = Math.Min(packet.radius, packet.maxRadius) / 2;

            int currentChunkX = (int)Math.Floor(Position.X / 16.0);
            int currentChunkZ = (int)Math.Floor(Position.Z / 16.0);

            List<(int x, int z)> chunkPositions = ChunkUtils.GetSequence(radius, currentChunkX, currentChunkZ);

            foreach (var (x, z) in chunkPositions)
            {
                if (!sentChunks.Contains((x, z)))
                {
                    sentChunks.Add((x, z));
                    ChunkSendQueue.Enqueue((x, z));
                }
            }

            ProcessSendQueue();
        }

        public void PacketEvent_Text(TextMessage packet)
        {
            foreach (var dest in currentLevel.onlinePlayers)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                var pk = new TextMessage
                {
                    messageType = 1,
                    Username = Username,
                    Message = packet.Message
                };
                pk.Encode(encoder);
            }
        }

        public void PacketEvent_ServerboundLoadingScreen(ServerboundLoadingScreen packet)
        {
            if (packet.screenType == 4)
            {
                Spawned = true;
            }
            PluginManager.OnPlayerJoin(this);
        }

        public void PacketEvent_PlayerSkin(PlayerSkin packet)
        {
            foreach (var dest in currentLevel.onlinePlayers)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                var pk = new PlayerSkin
                {
                    UUID = UUID,
                    playerSkin = packet.playerSkin,
                    Name = packet.Name,
                    oldName = Skin.SkinId,
                    Trusted = packet.Trusted,
                };
                pk.Encode(encoder);
            }

            Skin = packet.playerSkin;
        }
    }
}
