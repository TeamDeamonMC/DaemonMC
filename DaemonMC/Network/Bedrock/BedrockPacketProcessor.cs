using System.Net;
using DaemonMC.Level;
using DaemonMC.Network.Handler;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Text;
using System.Security.Cryptography;

namespace DaemonMC.Network.Bedrock
{
    public class BedrockPacketProcessor
    {
        internal static void HandlePacket(Packet packet, IPEndPoint clientEp)
        {
            if (packet is RequestNetworkSettings requestNetworkSettings)
            {
                Log.debug($"New player ({RakSessionManager.getSession(clientEp).GUID}) log in with protocol version: {requestNetworkSettings.protocolVersion}");

                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new NetworkSettings
                {
                    compressionThreshold = 0,
                    compressionAlgorithm = 0,
                    clientThrottleEnabled = false,
                    clientThrottleScalar = 0,
                    clientThrottleThreshold = 0
                };
                pk.EncodePacket(encoder);

                RakSessionManager.Compression(clientEp, true);
                RakSessionManager.getSession(clientEp).protocolVersion = requestNetworkSettings.protocolVersion;

                if (!Info.protocolVersion.Contains(requestNetworkSettings.protocolVersion))
                {
                    PacketEncoder encoder2 = PacketEncoderPool.Get(clientEp);
                    var packet2 = new Disconnect
                    {
                        message = $"Unsupported Minecraft version \nSupported protocol versions: {string.Join(", ", Info.protocolVersion)}"
                    };
                    packet2.EncodePacket(encoder2);
                    RakSessionManager.deleteSession(clientEp);
                }
            }

            if (packet is Login login)
            {
                LoginHandler.execute(login, clientEp);
                var session = RakSessionManager.getSession(clientEp);
                session.protocolVersion = login.protocolVersion;

                Player player = new Player();
                player.Username = session.username;
                player.UUID = session.identity == null ? new Guid() : new Guid(session.identity);
                player.XUID = session.XUID;
                player.Skin = session.skin;

                long EntityId = Server.AddPlayer(player, clientEp);
                session.EntityID = EntityId;
                player.EntityID = EntityId;
            }

            if (packet is PacketViolationWarning packetViolationWarning)
            {
                Log.error($"Client reported that server sent failed packet '{(Info.Bedrock)packetViolationWarning.packetId}'");
                Log.error(packetViolationWarning.description);
            }

            if (packet is ClientCacheStatus clientCacheStatus)
            {
                var player = RakSessionManager.getSession(clientEp);
                Log.debug($"{player.username} ClientCacheStatus = {clientCacheStatus.status}");

                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk1 = new ResourcePacksInfo
                {
                    force = ResourcePackManager.ForcePacks,
                    packs = Server.packs
                };
                pk1.EncodePacket(encoder);
            }

            if (packet is ResourcePackClientResponse resourcePackClientResponse)
            {
                Log.debug($"ResourcePackClientResponse = {resourcePackClientResponse.response}");
                if (resourcePackClientResponse.response == 2)
                {
                    foreach (var pack in resourcePackClientResponse.packs)
                    {
                        ResourcePack resourcePack = Server.packs.FirstOrDefault(p => p.ContentId == pack.Substring(0, 36));

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
                else if (resourcePackClientResponse.response == 3)
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                    var pk = new ResourcePackStack
                    {
                        packs = Server.packs
                    };
                    pk.EncodePacket(encoder);
                }
                else if (resourcePackClientResponse.response == 4) //start game
                {
                    var player = Server.GetPlayer(RakSessionManager.getSession(clientEp).EntityID);

                    World spawnWorld = Server.levels.FirstOrDefault(w => w.levelName == DaemonMC.defaultWorld);
                    if (spawnWorld != null)
                    {
                        player.currentLevel = spawnWorld;
                    }
                    else
                    {
                        player.currentLevel = Server.levels[0];
                    }

                    if (!player.currentLevel.onlinePlayers.TryAdd(player.EntityID, player)) { return; }
                    player.spawn();
                }
            }

            if (packet is ResourcePackChunkRequest resourcePackChunkRequest)
            {
                ResourcePack resourcePack = Server.packs.FirstOrDefault(p => p.ContentId == resourcePackChunkRequest.PackName);

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
