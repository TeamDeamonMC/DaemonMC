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
        public int GameMode { get; set; } = 0;
        public long EntityID { get; set; }
        private long dataValue { get; set; }
        public long Tick { get; set; }
        public Vector3 Position { get; set; } = new Vector3(0, 1, 0);
        public Vector2 Rotation { get; set; } = new Vector2(0, 0);
        public bool onGround { get; set; }
        public int drawDistance { get; set; }
        public IPEndPoint ep { get; set; }
        public World CurrentWorld { get; set; }
        public AttributesValues Attributes { get; set; } = new AttributesValues(0.1f);
        public Dictionary<ActorData, Metadata> Metadata { get; set; } = new Dictionary<ActorData, Metadata>();
        public List<AuthInputData> InputData { get; set; } = new List<AuthInputData>();
        public List<AbilitiesData> Abilities { get; set; } = new List<AbilitiesData>() { new AbilitiesData(1, 262143, 0, 0.05f, 0.1f, 0.1f) };

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
            SendAbilities();
            CurrentWorld.AddPlayer(this);
            Log.info($"{Username} spawned in World:'{CurrentWorld.LevelName}' X:{Position.X} Y:{Position.Y} Z:{Position.Z}");
        }

        public void Send(Packet packet)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            packet.EncodePacket(encoder);
        }

        internal void SendStartGame()
        {
            var packet = new StartGame
            {
                LevelName = CurrentWorld.LevelDisplayName,
                EntityId = EntityID,
                GameType = GameMode,
                GameMode = GameMode,
                Position = new Vector3(Position.X, Position.Y, Position.Z),
                Rotation = new Vector2(0, 0),
                SpawnBlockX = (int)Position.X,
                SpawnBlockY = (int)Position.Y,
                SpawnBlockZ = (int)Position.Z,
                Difficulty = 1,
                Dimension = 0,
                Seed = CurrentWorld.RandomSeed,
                Generator = 1,
            };
            Send(packet);
        }

        internal void SendCommands()
        {
            var packet = new AvailableCommands
            {
                Commands = CommandManager.AvailableCommands
            };
            Send(packet);
        }

        internal void SendItemData()
        {
            var packet = new ItemRegistry
            {

            };
            Send(packet);
        }

        internal void SendCreativeInventory()
        {
            var packet = new CreativeContent
            {

            };
            Send(packet);
        }

        internal void SendBiomeDefinitionList()
        {
            var packet = new BiomeDefinitionList
            {
                BiomeData = new fNbt.NbtCompound("")
                {
                new NbtCompound("plains")
                    {
                        new NbtFloat("downfall", 0.4f),
                        new NbtFloat("temperature", 0.8f),
                    }
                }
            };
            Send(packet);
        }

        public void SendPlayStatus(int status)
        {
            var packet = new PlayStatus
            {
                Status = status,
            };
            Send(packet);
        }

        public void SendChunkToPlayer(int chunkX, int chunkZ)
        {
            List<byte> chunkData = new List<byte>();
            int chunkCount = 0;

            if (CurrentWorld.Temporary)
            {
                chunkData = new List<byte>(new SuperFlat().generateChunks());
                chunkCount = 20;
            }
            else
            {
                var chunkRaw = CurrentWorld.GetChunk(chunkX, chunkZ);
                chunkData = new List<byte>(chunkRaw.NetworkSerialize(this));
                chunkCount = chunkRaw.Chunks.Count();
                if (chunkRaw.Chunks.Count == 0)
                {
                    chunkData = new List<byte>(new SuperFlat().generateChunks());
                    chunkCount = 20;
                }
            }

            var chunk = new LevelChunk
            {
                ChunkX = chunkX,
                ChunkZ = chunkZ,
                Count = chunkCount,
                Data = chunkData.ToArray()
            };
            Send(chunk);
        }

        public void UpdateChunkRadius(int radius)
        {
            drawDistance = radius;
            var packet = new ChunkRadiusUpdated
            {
                Radius = drawDistance,
            };
            Send(packet);
        }

        public void UpdateAttributes()
        {
            var packet = new UpdateAttributes
            {
                EntityId = EntityID,
                Attributes = new List<AttributeValue> { Attributes.Movement_speed() },
                Tick = Tick
            };
            Send(packet);
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

            Metadata[ActorData.RESERVED_0] = new Metadata(dataValue);

            Dictionary<long, Player> players = new Dictionary<long, Player>(){ { EntityID, this } };

            var packet = new SetActorData
            {
                EntityId = EntityID,
                Metadata = Metadata,
                Tick = Tick
            };

            if (broadcast)
            {
                CurrentWorld.Send(packet);
            }
            else
            {
                Send(packet);
            }
        }

        public void SendGameRules()
        {
            var packet = new GameRulesChanged
            {
                GameRules = CurrentWorld.GameRules
            };
            Send(packet);
        }

        private async void ProcessSendQueue()
        {
            if (SendQueueBusy) { return; }
            SendQueueBusy = true;
            while (ChunkSendQueue.Count > 0)
            {
                if (!Server.OnlinePlayers.ContainsValue(this)) {
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

                var packet = new Disconnect
                {
                    Message = msg
                };
                Send(packet);

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
            var packet = new TextMessage
            {
                MessageType = 1,
                Message = message
            };
            Send(packet);
        }

        public void Teleport(Vector3 position)
        {
            Position = position;
            var packet = new MovePlayer
            {
                ActorRuntimeId = EntityID,
                Position = position
            };
            Send(packet);
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
            var packet = new LevelEvent
            {
                EventID = value,
                Position = pos,
                Data = data
            };
            Send(packet);
        }

        public void PlayAnimation(string animationID, bool broadcast = true)
        {
            Animation animation = ResourcePackManager.Animations[animationID];
            var packet = new AnimateEntity
            {
                Animation = animation.AnimationName,
                Controller = animation.ControllerName,
                RuntimeId = EntityID
            };

            if (broadcast)
            {
                CurrentWorld.Send(packet);
            }
            else
            {
                Send(packet);
            }
        }

        public void SetGameMode(int gameMode)
        {
            GameMode = gameMode;

            SetPlayerGameType packet = new SetPlayerGameType()
            {
                GameMode = gameMode
            };
            Send(packet);
        }

        public void SendAbilities()
        {
            UpdateAbilities packet = new UpdateAbilities()
            {
                EntityId = EntityID,
                PlayerPermissions = 0,
                CommandPermissions = 0,
                Layers = Abilities,
            };
            Send(packet);
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
                    if (playerAuthInput.InputData.Contains(AuthInputData.VerticalCollision))
                    {
                        header |= 0x40;
                    }

                    foreach (Player player in CurrentWorld.OnlinePlayers.Values)
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

                    var packet2 = new NetworkChunkPublisherUpdate
                    {
                        X = (int)Position.X,
                        Y = (int)0,
                        Z = (int)Position.Z,
                        Radius = 20
                    };
                    Send(packet2);

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
                var chunkRadius = Math.Min(DaemonMC.DrawDistance, requestChunkRadius.Radius);

                Log.debug($"{Username} requested chunks with radius {requestChunkRadius.Radius} (sent {chunkRadius}). Max radius = {requestChunkRadius.MaxRadius}");

                UpdateChunkRadius(chunkRadius);

                var packet2 = new NetworkChunkPublisherUpdate
                {
                    X = (int)Position.X,
                    Y = (int)Position.Y,
                    Z = (int)Position.Z,
                    Radius = drawDistance
                };
                Send(packet2);

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
                var msg = new TextMessage
                {
                    MessageType = 1,
                    Username = Username,
                    Message = textMessage.Message
                };
                CurrentWorld.Send(msg);
            }

            if (packet is ServerboundLoadingScreen serverboundLoadingScreen)
            {
                if (serverboundLoadingScreen.ScreenType == 4)
                {
                    Spawned = true;
                    PluginManager.PlayerJoined(this);
                    foreach (var entity in CurrentWorld.Entities.Values)
                    {
                        _ = Task.Run(async () => {
                            await Task.Delay(2000);
                            if (entity is CustomEntity customEntity)
                            {
                                var packet = new PlayerList
                                {
                                    Action = 1,
                                    UUID = customEntity.UUID,
                                };
                                Send(packet);
                            }

                            if (ResourcePackManager.Animations.TryGetValue(entity.SpawnAnimation, out Animation spawnAnimation))
                            {
                                var packet1 = new AnimateEntity
                                {
                                    Animation = spawnAnimation.AnimationName,
                                    Controller = spawnAnimation.ControllerName,
                                    RuntimeId = entity.EntityId
                                };
                                Send(packet1);
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
                var pk = new PlayerSkin
                {
                    UUID = UUID,
                    Skin = playerSkin.Skin,
                    Name = playerSkin.Name,
                    OldName = Skin.SkinId,
                    Trusted = playerSkin.Trusted,
                };
                CurrentWorld.Send(pk);

                Skin = playerSkin.Skin;
            }

            if (packet is CommandRequest commandRequest)
            {
                CommandManager.Execute(commandRequest.Command.Substring(1), this);
            }

            if (packet is EmoteList emoteList) //todo save these values
            {
                Log.debug($"Received emote ids from {Username}");
                CurrentWorld.Send(emoteList);
            }

            if (packet is Interact interact)
            {
                Log.debug($"{Username} interacted id:{interact.Action} at {interact.InteractPosition}");
            }

            if (packet is Emote emote)
            {
                Log.debug($"{Username} emoted id:{emote.EmoteID}");
                CurrentWorld.Send(emote);
            }

            if (packet is Animate animate)
            {
                CurrentWorld.Send(animate);
                Log.debug($"{Username} animation action:{animate.Action}");
            }

            if (packet is InventoryTransaction inventoryTransaction)
            {
                if (inventoryTransaction.Transaction.Type is TransactionType.ItemUseOnEntityTransaction)
                {
                    if (CurrentWorld.Entities.TryGetValue(inventoryTransaction.Transaction.EntityId, out Entity entity))
                    {
                        PluginManager.EntityAttack(this, entity);
                    }
                    if (CurrentWorld.OnlinePlayers.TryGetValue(inventoryTransaction.Transaction.EntityId, out Player player))
                    {
                        PluginManager.PlayerAttack(this, player);
                    }
                }
            }
        }
    }
}
