using System.Net;
using System.Numerics;
using System.Security.Cryptography;
using DaemonMC.Biomes;
using DaemonMC.Entities;
using DaemonMC.Items;
using DaemonMC.Level;
using DaemonMC.Network.Handler;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class BedrockPacketProcessor
    {
        internal static void HandlePacket(Packet packet, IPEndPoint clientEp)
        {
            if (packet is RequestNetworkSettings requestNetworkSettings)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new NetworkSettings
                {
                    CompressionThreshold = (ushort)(DaemonMC.Compression == CompressionTypes.None ? 0 : 1), //looks like since 1.21.110 if CompressionThreshold > 0 client will use compression even if CompressionAlgorithm is set to 255 (none). What?
                    CompressionAlgorithm = (ushort)DaemonMC.Compression,
                    ClientThrottleEnabled = false,
                    ClientThrottleScalar = 0,
                    ClientThrottleThreshold = 0
                };
                pk.EncodePacket(encoder);

                RakSessionManager.Compression(clientEp, true);
                RakSessionManager.getSession(clientEp).protocolVersion = requestNetworkSettings.ProtocolVersion;

                if (!Info.ProtocolVersion.Contains(requestNetworkSettings.ProtocolVersion))
                {
                    PacketEncoder encoder2 = PacketEncoderPool.Get(clientEp);
                    var packet2 = new Disconnect
                    {
                        Message = $"Unsupported Minecraft version \nSupported protocol versions: {string.Join(", ", Info.ProtocolVersion)}"
                    };
                    packet2.EncodePacket(encoder2);
                    RakSessionManager.deleteSession(clientEp);
                }
            }

            if (packet is Login login)
            {
                var session = RakSessionManager.getSession(clientEp);
                if (session.EntityID != 0)
                {
                    return; // already doing login
                }
                try
                {
                    LoginHandler.handleRequest(login, clientEp);
                }
                catch (Exception e)
                {
                    Log.error($"Login error: {e}");
                    PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                    var packet1 = new Disconnect
                    {
                        Message = $"Login error: {e}"
                    };
                    packet1.EncodePacket(encoder);
                    return;
                }

                session.protocolVersion = login.ProtocolVersion;

                Player player = new Player();
                player.Username = session.username;
                player.UUID = session.identity == null ? new Guid() : new Guid(session.identity);
                player.XUID = session.XUID;
                player.Skin = session.skin;

                long EntityId = Server.AddPlayer(player, clientEp);
                session.EntityID = EntityId;
                player.EntityID = EntityId;
                player.GameMode = DaemonMC.GameMode;

                if (player.GameMode == 2)
                {
                    player.Abilities[0].AbilityValues.MayFly = true;
                }
            }

            if (packet is ClientToServerHandshake clientToServerHandshake)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new PlayStatus
                {
                    Status = 0,
                };
                pk.EncodePacket(encoder);
            }

            if (packet is PacketViolationWarning packetViolationWarning)
            {
                Log.error($"Client reported that server sent failed packet '{(Info.Bedrock)packetViolationWarning.PacketId}'");
                Log.error(packetViolationWarning.Description);
            }

            if (packet is ClientCacheStatus clientCacheStatus)
            {
                var player = RakSessionManager.getSession(clientEp);
                Log.debug($"{player.username} ClientCacheStatus = {clientCacheStatus.Status}");

                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk1 = new ResourcePacksInfo
                {
                    Force = ResourcePackManager.ForcePacks,
                    Packs = Server.Packs
                };
                pk1.EncodePacket(encoder);
            }

            if (packet is ResourcePackClientResponse resourcePackClientResponse)
            {
                Log.debug($"ResourcePackClientResponse = {resourcePackClientResponse.Response}");
                if (resourcePackClientResponse.Response == 2)
                {
                    foreach (var pack in resourcePackClientResponse.Packs)
                    {
                        ResourcePack resourcePack = Server.Packs.FirstOrDefault(p => p.ContentId == pack.Substring(0, 36));

                        PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                        var pk = new ResourcePackDataInfo
                        {
                            PackName = resourcePack.ContentId,
                            ChunkSize = ResourcePackManager.ChunkSize,
                            ChunkCount = (int)Math.Ceiling((double)resourcePack.PackContent.Length / ResourcePackManager.ChunkSize),
                            PackSize = resourcePack.PackContent.Length,
                            Hash = SHA256.Create().ComputeHash(resourcePack.PackContent),
                            IsPremium = false,
                            PackType = 6
                        };
                        pk.EncodePacket(encoder);
                    }
                }
                else if (resourcePackClientResponse.Response == 3)
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                    var pk = new ResourcePackStack
                    {
                        Packs = Server.Packs
                    };
                    pk.EncodePacket(encoder);
                }
                else if (resourcePackClientResponse.Response == 4) //start game
                {
                    var session = RakSessionManager.getSession(clientEp);
                    var player = Server.GetPlayer(session.EntityID);

                    if (player.Spawned) //something weird happens under load
                    {
                        return;
                    }

                    World spawnWorld = Server.Levels.FirstOrDefault(w => w.LevelName == DaemonMC.DefaultWorld);
                    if (spawnWorld != null)
                    {
                        player.CurrentWorld = spawnWorld;
                        player.Position = new Vector3(spawnWorld.SpawnX, spawnWorld.SpawnY, spawnWorld.SpawnZ);
                    }
                    else
                    {
                        player.CurrentWorld = Server.Levels[0];
                    }

                    if (!player.CurrentWorld.OnlinePlayers.TryAdd(player.EntityID, player)) { return; }
                    player.spawn();

                    var items = new ItemRegistry
                    {
                        Items = ItemPalette.items
                    };
                    player.Send(items);

                    var commands = new AvailableCommands
                    {
                        EnumValues = CommandManager.EnumValues,
                        Enums = CommandManager.RealEnums,
                        Commands = CommandManager.AvailableCommands
                    };
                    player.Send(commands);

                    var creativeInventory = new CreativeContent
                    {

                    };
                    player.Send(creativeInventory);

                    var biomes = new BiomeDefinitionList
                    {
                        BiomeData = BiomeManager.Biomes
                    };
                    player.Send(biomes);

                    foreach (var actorData in ActorProperties.PropertyData)
                    {
                        var actorProperties = new SyncActorProperty
                        {
                            Data = actorData,
                        };
                        player.Send(actorProperties);
                    }

                    player.Spawned = true;
                }
            }

            if (packet is ResourcePackChunkRequest resourcePackChunkRequest)
            {
                ResourcePack resourcePack = Server.Packs.FirstOrDefault(p => p.ContentId == resourcePackChunkRequest.PackName);

                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new ResourcePackChunkData
                {
                    PackName = resourcePackChunkRequest.PackName,
                    Chunk = resourcePackChunkRequest.Chunk,
                    Offset = ResourcePackManager.ChunkSize * resourcePackChunkRequest.Chunk,
                    Data = ResourcePackManager.GetData(resourcePackChunkRequest.PackName, resourcePackChunkRequest.Chunk)
                };
                pk.EncodePacket(encoder);
            }
        }
    }
}
