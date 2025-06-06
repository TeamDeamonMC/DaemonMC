using System.Net;
using DaemonMC.Network.Bedrock;
using DaemonMC.Network;
using DaemonMC.Utils.Text;
using System.Numerics;
using DaemonMC.Level;
using DaemonMC.Utils;
using DaemonMC.Network.Enumerations;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Game;
using DaemonMC.Plugin;
using DaemonMC.Entities;
using DaemonMC.Blocks;
using DaemonMC.Forms;
using Newtonsoft.Json;
using DaemonMC.Plugin.Events;

namespace DaemonMC
{
    public class Player
    {
        private PlayerUtils _player;
        public string Username { get; set; } = "";
        public string NameTag { get; set; } = "";
        public Skin Skin { get; set; } = new Skin();
        public Guid UUID { get; set; } = new Guid();
        public string XUID { get; set; }
        public int GameMode { get; set; } = 0;
        public long EntityID { get; set; }
        internal long dataValue { get; set; }
        public long Tick { get; set; }
        public Vector3 Position { get; set; } = new Vector3(0, 100, 0);
        public Vector2 Rotation { get; set; } = new Vector2(0, 0);
        public bool onGround { get; set; }
        public int drawDistance { get; set; }
        public IPEndPoint ep { get; set; }
        public World CurrentWorld { get; set; }
        public AttributesValues Attributes { get; set; } = new AttributesValues(0.1f);
        public Dictionary<ActorData, Metadata> Metadata { get; set; } = new Dictionary<ActorData, Metadata>();
        public List<AuthInputData> InputData { get; set; } = new List<AuthInputData>();
        public List<AbilitiesData> Abilities { get; set; } = new List<AbilitiesData>() { new AbilitiesData(1, 262143, new PermissionSet(), 0.05f, 0.1f, 0.1f) };
        public PlayerInventory Inventory { get; set; }
        public Dictionary<Effects, bool> AllEffects { get; set; } = new Dictionary<Effects, bool>();
        public bool Spawned { get; set; } = false;
        private int LastChunkX = 0;
        private int LastChunkZ = 0;

        public Player()
        {
            _player = new PlayerUtils(this);
            Inventory = new PlayerInventory(this);
        }

        internal void spawn()
        {
            _player.SendStartGame();
            SendPlayStatus(3);
            SendGameRules();
            UpdateAttributes();
            SendMetadata(true);
            SendAbilities();
            CurrentWorld.AddPlayer(this);
            SendHud(HudElements.AirBubbles, false); //tempfix for bubbles. todo fix
        }

        public void Send(Packet packet)
        {
            PacketEncoder encoder = PacketEncoderPool.Get(this);
            packet.EncodePacket(encoder);
        }

        public void SendPlayStatus(int status)
        {
            var packet = new PlayStatus
            {
                Status = status,
            };
            Send(packet);
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
                NameTag = Username;
                Metadata[ActorData.NAME] = new Metadata(Username);
                Metadata[ActorData.NAMETAG_ALWAYS_SHOW] = new Metadata((byte) 1);
                SetFlag(ActorFlags.CAN_SHOW_NAME, true);
                SetFlag(ActorFlags.HAS_COLLISION, true);
                SetFlag(ActorFlags.HAS_GRAVITY, true);
                SetFlag(ActorFlags.FIRE_IMMUNE, true);
            }

            Metadata[ActorData.RESERVED_0] = new Metadata(dataValue);

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
                Server.RemovePlayer(EntityID);
                RakSessionManager.deleteSession(ep);
            });
        }

        public void SendMessage(string message)
        {
            var packet = new TextMessage
            {
                MessageType = 0,
                Message = message
            };
            Send(packet);
        }

        public void SendChatMessage(string message, string from)
        {
            var packet = new TextMessage
            {
                MessageType = 1,
                Message = message,
                Username = from
            };
            Send(packet);
        }

        public void SendPopup(string message)
        {
            var packet = new TextMessage
            {
                MessageType = 3,
                Message = message
            };
            Send(packet);
        }

        public void SendJukeboxPopup(string message)
        {
            var packet = new TextMessage
            {
                MessageType = 4,
                Message = message
            };
            Send(packet);
        }

        public void SendTip(string message)
        {
            var packet = new TextMessage
            {
                MessageType = 5,
                Message = message
            };
            Send(packet);
        }

        public void SendTitle(string title, string subtitle = "", int fadeInTime = 1, int stayTime = 1, int fadeOutTime = 1)
        {
            var packet = new SetTitle
            {
                Type = 2,
                Text = title,
                FadeIn = fadeInTime * 20,
                Stay = stayTime * 20,
                FadeOut = fadeOutTime * 20,
                XUID = XUID
            };
            Send(packet);

            if (subtitle != "")
            {
                var packet2 = new SetTitle
                {
                    Type = 3,
                    Text = subtitle,
                    FadeIn = fadeInTime * 20,
                    Stay = stayTime * 20,
                    FadeOut = fadeOutTime * 20,
                    XUID = XUID
                };
                Send(packet2);
            }
        }

        public void SendActionBarTitle(string title, int fadeInTime = 1, int stayTime = 1, int fadeOutTime = 1)
        {
            var packet = new SetTitle
            {
                Type = 4,
                Text = title,
                FadeIn = fadeInTime * 20,
                Stay = stayTime * 20,
                FadeOut = fadeOutTime * 20,
                XUID = XUID
            };
            Send(packet);
        }

        public void ClearTitle()
        {
            var packet = new SetTitle
            {
                Type = 0,
                XUID = XUID
            };
            Send(packet);
        }

        public void Teleport(Vector3 position)
        {
            Position = position;
            var packet = new MovePlayer
            {
                ActorRuntimeId = EntityID,
                Position = position,
                Teleport = true,
                Tick = Tick
            };
            Send(packet);
        }

        public void MoveTo(Vector3 position)
        {
            Position = position;
            var packet = new MovePlayer
            {
                ActorRuntimeId = EntityID,
                Position = position,
                Tick = Tick
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

        public void SendAbilities(byte playerPermissions = 3, byte commandPermissions = 0)
        {
            UpdateAbilities packet = new UpdateAbilities()
            {
                EntityId = EntityID,
                PlayerPermissions = playerPermissions,
                CommandPermissions = commandPermissions,
                Layers = Abilities,
            };
            Send(packet);
        }

        public void SetPermissions(PermissionSet permissions)
        {
            Abilities[0].AbilityValues = permissions;
            SendAbilities();
        }

        private void SendEntities()
        {
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

        public void ChangeWorld(World world)
        {
            doChangeWorld(world, new Vector3(CurrentWorld.SpawnX, CurrentWorld.SpawnY, CurrentWorld.SpawnZ));
        }

        public void ChangeWorld(World world, Vector3 position)
        {
            doChangeWorld(world, position);
        }

        public async void doChangeWorld(World world, Vector3 position)
        {
            CurrentWorld.RemovePlayer(this);
            foreach (var player in CurrentWorld.OnlinePlayers.Values)
            {
                var packet = new PlayerList
                {
                    Action = 1,
                    UUID = player.UUID,
                };
                Send(packet);
                var packet3 = new RemoveActor
                {
                    EntityId = player.EntityID
                };
                Send(packet3);
            }
            foreach (var entity in CurrentWorld.Entities.Values)
            {
                if (entity is CustomEntity customEntity)
                {
                    var packet = new PlayerList
                    {
                        Action = 1,
                        UUID = customEntity.UUID,
                    };
                    Send(packet);
                }
                var packet3 = new RemoveActor
                {
                    EntityId = entity.EntityId
                };
                Send(packet3);
            }
            CurrentWorld = world;
            world.AddPlayer(this);
            _player.ClearChunkCache();
            SendEntities();
            await Task.Delay(500); //need a little bit time to clear chunk cache and process packets
            _player.SendChunks();
            await Task.Delay(500);
            Teleport(position);
        }

        public void SetNameTag(string nameTag)
        {
            NameTag = nameTag;
            Metadata[ActorData.NAME] = new Metadata(nameTag);
            SendMetadata();
        }

        public void SendBlock(Block block, int x, int y, int z)
        {
            var packet = new UpdateBlock()
            {
                Block = block,
                Position = new Vector3(x, y, z)
            };
            Send(packet);
        }

        public void SendBlock(Block block, Vector3 playerPos)
        {
            SendBlock(block, (int)(playerPos.X < 0 ? playerPos.X - 1 : playerPos.X), (int)playerPos.Y, (int)(playerPos.Z < 0 ? playerPos.Z - 1 : playerPos.Z));
        }

        public void SendForm(Form form, Action<Player, string> callback)
        {
            var packet = new ModalFormRequest()
            {
                ID = form.Id,
                Data = JsonConvert.SerializeObject(form, new JsonSerializerSettings() { ContractResolver = FormManager.contractResolver })
            };
            FormManager.PendingForms[form.Id] = (form, callback);
            Send(packet);
        }

        public void Transfer(string address, ushort port)
        {
            var packet = new TransferPlayer()
            {
                IpAddress = address,
                Port = port
            };
            Send(packet);
        }

        public void SendTime(int time)
        {
            var packet = new SetTime()
            {
                Time = time,
            };
            Send(packet);
        }

        public void SendHud(HudElements element, bool visible)
        {
            SendHud(new Dictionary<HudElements, bool>() { { element, visible } });
        }

        public void SendHud(Dictionary<HudElements, bool> elements)
        {
            var packet = new SetHud
            {
                HudElements = elements,
            };
            Send(packet);
        }

        public void UpdateVisibleEffects()
        {
            long effectsData = 0;

            foreach (var effect in AllEffects)
            {
                if (effect.Value)
                {
                    effectsData = (effectsData << 7) | (((int)effect.Key & 0x3F) << 1) | 0;
                }
            }

            Metadata[ActorData.VISIBLE_MOB_EFFECTS] = new Metadata(effectsData);

            SendMetadata();
        }

        public void AddEffect(Effects effect, int duration = -1, bool showParticles = true)
        {
            if (AllEffects.ContainsKey(effect))
            {
                Log.warn($"Effect '{effect}' already applied to {Username}");
                return;
            }

            AllEffects.Add(effect, showParticles);

            if (showParticles)
            {
                UpdateVisibleEffects();
            }

            var packet = new MobEffect
            {
                EntityId = EntityID,
                EventId = 1,
                EffectId = (int)effect,
                ShowParticles = showParticles,
                Duration = duration,
                Tick = Tick
            };
            Send(packet);

            if (duration > 0)
            {
                _ = Task.Run(async () => {
                    await Task.Delay(duration * 50);
                    Log.debug($"Effect '{effect}' removed from {Username}");
                    AllEffects.Remove(effect);
                    UpdateVisibleEffects();
                });
            }
        }

        public void RemoveEffect(Effects effect)
        {
            if (!AllEffects.Remove(effect))
            {
                Log.warn($"Couldn't remove effect '{effect}' from {Username}. Effect is not applied.");
                return;
            }

            var packet = new MobEffect
            {
                EntityId = EntityID,
                EventId = 3,
                EffectId = (int)effect,
                Tick = Tick
            };
            Send(packet);

            Log.debug($"Effect '{effect}' removed from {Username}");
            AllEffects.Remove(effect);
            UpdateVisibleEffects();
        }

        ///////////////////////////// Packet handler /////////////////////////////

        internal void HandlePacket(Packet packet)
        {
            if (packet is PlayerAuthInput playerAuthInput)
            {
                Tick = playerAuthInput.Tick;
                if (!InputData.SequenceEqual(playerAuthInput.InputData))
                {
                    Log.debug($"Data: {string.Join(" | ", playerAuthInput.InputData)}");
                    _player.UpdateFlags(playerAuthInput.InputData);
                    InputData = playerAuthInput.InputData;
                }
                if (Vector3.Distance(Position, playerAuthInput.Position) > 0.01f || Vector2.Distance(Rotation, playerAuthInput.Rotation) > 0.01f)
                {
                    PluginManager.PlayerMove(this);

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
                        Y = (int)Position.Y,
                        Z = (int)Position.Z,
                        Radius = drawDistance
                    };
                    Send(packet2);

                    HashSet<(int x, int z)> newChunks = new(ChunkUtils.GetSequence(drawDistance/2, currentChunkX, currentChunkZ));

                    foreach (var chunk in newChunks)
                    {
                        if (!_player.sentChunks.Contains(chunk))
                        {
                            _player.sentChunks.Add(chunk);
                            _player.ChunkSendQueue.Enqueue(chunk);
                        }
                    }

                    foreach (var chunk in _player.sentChunks.ToList())
                    {
                        if (!newChunks.Contains(chunk))
                        {
                            _player.sentChunks.Remove(chunk);
                        }
                    }

                    _player.ProcessSendQueue();
                }
            }

            if (packet is RequestChunkRadius requestChunkRadius)
            {
                var chunkRadius = Math.Min(DaemonMC.DrawDistance, requestChunkRadius.Radius);

                Log.debug($"{Username} requested chunks with radius {requestChunkRadius.Radius} (sent {chunkRadius}). Max radius = {requestChunkRadius.MaxRadius}");

                UpdateChunkRadius(chunkRadius);

                _player.SendChunks();
            }

            if (packet is TextMessage textMessage)
            {
                PluginManager.PlayerSentMessage(this, textMessage);
            }

            if (packet is ServerboundLoadingScreen serverboundLoadingScreen)
            {
                if (serverboundLoadingScreen.ScreenType == 4)
                {
                    PluginManager.PlayerJoined(this);
                    SendEntities();
                }
            }

            if (packet is PlayerSkin playerSkin)
            {
                PluginManager.PlayerSkinChanged(this, playerSkin);
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
                if (interact.Action == 6)
                {
                    var pk = new ContainerOpen
                    {
                        ContainerId = 0,
                        ContainerType = 255,
                        EntityId = EntityID
                    };
                    Send(pk);
                }
            }

            if (packet is ContainerClose containerClose)
            {
                var pk = new ContainerClose
                {
                    ContainerId = 0,
                    ContainerType = 255,
                };
                Send(pk);
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
                        PluginManager.PlayerAttackedEntity(this, entity);
                    }
                    if (CurrentWorld.OnlinePlayers.TryGetValue(inventoryTransaction.Transaction.EntityId, out Player player))
                    {
                        PluginManager.PlayerAttackedPlayer(this, player);
                    }
                }
            }

            if (packet is MobEquipment mobEquipment)
            {
                if (Inventory.Inventory.TryGetValue(mobEquipment.Slot, out var expectedItem))
                {
                    if (expectedItem.Name != mobEquipment.Item.Name)
                    {
                        Log.warn($"{mobEquipment.EntityId} Inventory mismatch. Expected {Inventory.Inventory[mobEquipment.Slot].Name}, player have {mobEquipment.Item.Name}. Resending item from server side inventory...");
                        Inventory.Send(0, mobEquipment.Slot, expectedItem);
                    }
                    Log.debug($"{mobEquipment.EntityId} holding {mobEquipment.Item.Name} in slot {mobEquipment.Slot}");
                    Inventory.HandSlot = mobEquipment.Slot;
                    var pk1 = new MobEquipment
                    {
                        EntityId = mobEquipment.EntityId,
                        Item = expectedItem,
                        Slot = 0,
                    };
                    CurrentWorld.Send(pk1, EntityID);
                }
                else
                {
                    Kick("Inventory error");
                }
            }

            if (packet is ModalFormResponse formResponse)
            {
                if (FormManager.PendingForms.TryGetValue(formResponse.ID, out var entry))
                {
                    FormManager.Process(this, formResponse, entry);
                }
                else
                {
                    Log.warn($"Received response for unknown form ID: {formResponse.ID}");
                }
            }

            if (packet is ClientMovementPredictionSync clientMovementPredictionSync)
            {
                Log.debug($"ClientMovementPredictionSync: {string.Join(" | ", clientMovementPredictionSync.ActorData)}");
            }
        }
    }
}
