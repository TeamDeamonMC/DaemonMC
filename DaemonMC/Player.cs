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
using DaemonMC.Entities;

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
        public World currentWorld { get; set; }
        public AttributesValues attributes { get; set; } = new AttributesValues(0.1f);
        public Dictionary<ActorData, Metadata> metadata { get; set; } = new Dictionary<ActorData, Metadata>();
        public List<AuthInputData> InputData = new List<AuthInputData>();

        private Queue<(int x, int z)> ChunkSendQueue = new Queue<(int x, int z)>();
        private bool SendQueueBusy = false;
        public bool Spawned = false;
        private int LastChunkX = 0;
        private int LastChunkZ = 0;

        internal void spawn()
        {
            SendStartGame();
            SendCommands();
            SendItemData();
            SendCreativeInventory();
            SendBiomeDefinitionList();
            SendPlayStatus(3);
            SendGameRules();
            UpdateAttributes();
            SendMetadata(true);
            currentWorld.addPlayer(this);
            Log.info($"{Username} spawned in World:'{currentWorld.levelName}' X:{Position.X} Y:{Position.Y} Z:{Position.Z}");
        }

        internal void SendStartGame()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new StartGame
            {
                LevelName = currentWorld.LevelDisplayName,
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
                Seed = currentWorld.RandomSeed,
                Generator = 1,
            };
            packet.EncodePacket(encoder);
        }

        internal void SendCommands()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new AvailableCommands
            {
                Commands = CommandManager.AvailableCommands
            };
            packet.EncodePacket(encoder);
        }

        internal void SendItemData()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new ItemRegistry
            {

            };
            packet.EncodePacket(encoder);
        }

        internal void SendCreativeInventory()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new CreativeContent
            {

            };
            packet.EncodePacket(encoder);
        }

        internal void SendBiomeDefinitionList()
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
            packet.EncodePacket(encoder);
        }

        public void SendPlayStatus(int status)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new PlayStatus
            {
                status = status,
            };
            packet.EncodePacket(encoder);
        }

        public void SendChunkToPlayer(int chunkX, int chunkZ)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            List<byte> chunkData = new List<byte>();
            int chunkCount = 0;

            if (currentWorld.temporary)
            {
                chunkData = new List<byte>(new SuperFlat().generateChunks());
                chunkCount = 20;
            }
            else
            {
                var chunkRaw = currentWorld.GetChunk(chunkX, chunkZ);
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
            chunk.EncodePacket(encoder);
        }

        public void UpdateChunkRadius(int radius)
        {
            drawDistance = radius;
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new ChunkRadiusUpdated
            {
                radius = drawDistance,
            };
            packet.EncodePacket(encoder);
        }

        public void UpdateAttributes()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new UpdateAttributes
            {
                EntityId = EntityID,
                Attributes = new List<AttributeValue> { attributes.Movement_speed() }
            };
            packet.EncodePacket(encoder);
        }

        public void SendMetadata(bool broadcast = true)
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
                players = currentWorld.onlinePlayers;
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
                packet.EncodePacket(encoder);
            }
        }

        public void SendGameRules()
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new GameRulesChanged
            {
                GameRules = currentWorld.GameRules
            };
            packet.EncodePacket(encoder);
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
            _ = Task.Run(async () => {
                await Task.Delay(1000);
                PacketEncoder encoder = PacketEncoderPool.Get(this);
                var packet = new Disconnect
                {
                    message = msg
                };
                packet.EncodePacket(encoder);
                PacketEncoder encoder2 = PacketEncoderPool.Get(this);
                var packet2 = new RakDisconnect
                {
                };
                packet2.Encode(encoder2);
                Server.RemovePlayer((long)EntityID);
                RakSessionManager.deleteSession(ep);
            });
        }

        public void SendMessage(string message)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var pk = new TextMessage
            {
                messageType = 1,
                Message = message
            };
            pk.EncodePacket(encoder);
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
            movePk.EncodePacket(encoder);
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
            SendMetadata(true);
        }

        public void SendLevelEvent(Vector3 pos, LevelEvents value, int data = 0)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            var packet = new LevelEvent
            {
                EventID = value,
                Position = pos,
                Data = data
            };
            packet.EncodePacket(encoder);
        }

        public void PlayAnimation(string animationID)
        {
            Animation animation = ResourcePackManager.Animations[animationID];
            foreach (var player in currentWorld.onlinePlayers.Values)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(player);
                var packet = new AnimateEntity
                {
                    mAnimation = animation.AnimationName,
                    mController = animation.ControllerName,
                    mRuntimeId = EntityID
                };
                packet.EncodePacket(encoder);
            }
        }


        ///////////////////////////// Packet handler /////////////////////////////

        private HashSet<(int x, int z)> sentChunks = new();

        internal void HandlePacket(Packet packet)
        {
            if (packet is PlayerAuthInput playerAuthInput)
            {
                Tick = playerAuthInput.Tick;
                if (!InputData.SequenceEqual(playerAuthInput.InputData))
                {
                    Log.debug($"Data: {string.Join(" | ", playerAuthInput.InputData)}");
                    UpdateFlags(playerAuthInput.InputData);
                    InputData = playerAuthInput.InputData;
                }
                if (Position != playerAuthInput.Position || Rotation != playerAuthInput.Rotation)
                {
                    Position = playerAuthInput.Position;
                    Rotation = playerAuthInput.Rotation;

                    ushort header = 0;
                    header |= 0x01;
                    header |= 0x02;
                    header |= 0x04;
                    header |= 0x08;
                    header |= 0x10;
                    header |= 0x20;

                    foreach (Player player in currentWorld.onlinePlayers.Values)
                    {
                        if (player == this) { continue; }
                        PacketEncoder encoder = PacketEncoderPool.Get(player);
                        var movePk = new MoveActorDelta
                        {
                            EntityId = EntityID,
                            Header = header,
                            Position = Position,
                            Rotation = Rotation,
                            YheadRotation = playerAuthInput.HeadRotation
                        };
                        movePk.EncodePacket(encoder);
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
                    packet2.EncodePacket(encoder2);

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

            if (packet is RequestChunkRadius requestChunkRadius)
            {
                var chunkRadius = Math.Min(DaemonMC.drawDistance, requestChunkRadius.radius);

                Log.debug($"{Username} requested chunks with radius {requestChunkRadius.radius} (sent {chunkRadius}). Max radius = {requestChunkRadius.maxRadius}");

                UpdateChunkRadius(chunkRadius);

                PacketEncoder encoder2 = PacketEncoderPool.Get(this);
                var packet2 = new NetworkChunkPublisherUpdate
                {
                    x = (int)Position.X,
                    y = (int)Position.Y,
                    z = (int)Position.Z,
                    radius = drawDistance
                };
                packet2.EncodePacket(encoder2);

                int radius = chunkRadius / 2;

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

            if (packet is TextMessage textMessage)
            {
                foreach (var dest in currentWorld.onlinePlayers)
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                    var pk = new TextMessage
                    {
                        messageType = 1,
                        Username = Username,
                        Message = textMessage.Message
                    };
                    pk.EncodePacket(encoder);
                }
            }

            if (packet is ServerboundLoadingScreen serverboundLoadingScreen)
            {
                if (serverboundLoadingScreen.screenType == 4)
                {
                    Spawned = true;
                    PluginManager.PlayerJoined(this);
                    foreach (var entity in currentWorld.Entities.Values)
                    {
                        _ = Task.Run(async () => {
                            await Task.Delay(2000);
                            if (entity is CustomEntity customEntity)
                            {
                                PacketEncoder encoder = PacketEncoderPool.Get(this);
                                var packet = new PlayerList
                                {
                                    action = 1,
                                    UUID = customEntity.UUID,
                                };
                                packet.EncodePacket(encoder);
                            }

                            if (ResourcePackManager.Animations.TryGetValue(entity.SpawnAnimation, out Animation spawnAnimation))
                            {
                                PacketEncoder encoder = PacketEncoderPool.Get(this);
                                var packet1 = new AnimateEntity
                                {
                                    mAnimation = spawnAnimation.AnimationName,
                                    mController = spawnAnimation.ControllerName,
                                    mRuntimeId = entity.EntityId
                                };
                                packet1.EncodePacket(encoder);
                            }
                            else
                            {
                                Log.warn($"Unable to find spawn animation by key:{entity.SpawnAnimation}. Make sure animation is registered.");
                            }
                        });
                    }
                }
            }

            if (packet is PlayerSkin playerSkin)
            {
                foreach (var dest in currentWorld.onlinePlayers)
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                    var pk = new PlayerSkin
                    {
                        UUID = UUID,
                        playerSkin = playerSkin.playerSkin,
                        Name = playerSkin.Name,
                        oldName = Skin.SkinId,
                        Trusted = playerSkin.Trusted,
                    };
                    pk.EncodePacket(encoder);
                }

                Skin = playerSkin.playerSkin;
            }

            if (packet is CommandRequest commandRequest)
            {
                CommandManager.Execute(commandRequest.Command.Substring(1), this);
            }

            if (packet is EmoteList emoteList) //todo save these values
            {
                Log.debug($"Received emote ids from {Username}");
                foreach (var dest in currentWorld.onlinePlayers)
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                    var pk = new EmoteList
                    {
                        ActorRuntimeId = EntityID,
                        EmoteIds = emoteList.EmoteIds
                    };
                    pk.EncodePacket(encoder);
                }
            }

            if (packet is Interact interact)
            {
                Log.debug($"{Username} interacted id {interact.action} at {interact.interactPosition}");
            }

            if (packet is Emote emote)
            {
                Log.debug($"{Username} emoted id {emote.EmoteID}");
                foreach (var dest in currentWorld.onlinePlayers)
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(dest.Value);
                    var pk = new Emote
                    {
                        ActorRuntimeId = EntityID,
                        EmoteID = emote.EmoteID,
                        EmoteTicks = emote.EmoteTicks,
                        XUID = emote.XUID,
                        PlatformID = emote.PlatformID,
                        Flags = emote.Flags
                    };
                    pk.EncodePacket(encoder);
                }
            }
        }
    }
}
