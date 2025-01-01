using System.Net;
using DaemonMC.Network.Handler;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class BedrockPacketProcessor
    {
        public static void RequestNetworkSettings(RequestNetworkSettings packet, IPEndPoint clientEp)
        {
            Log.debug($"New player ({RakSessionManager.getSession(clientEp).GUID}) log in with protocol version: {packet.protocolVersion}");

            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new NetworkSettings
            {
                compressionThreshold = 0,
                compressionAlgorithm = 0,
                clientThrottleEnabled = false,
                clientThrottleScalar = 0,
                clientThrottleThreshold = 0
            };
            pk.Encode(encoder);

            RakSessionManager.Compression(clientEp, true);
            RakSessionManager.getSession(clientEp).protocolVersion = packet.protocolVersion;

            if (!Info.protocolVersion.Contains(packet.protocolVersion))
            {
                PacketEncoder encoder2 = PacketEncoderPool.Get(clientEp);
                var packet2 = new Disconnect
                {
                    message = "Unsupported Minecraft version"
                };
                packet2.Encode(encoder2);
                RakSessionManager.deleteSession(clientEp);
            }
        }

        public static void Login(Login packet, IPEndPoint clientEp)
        {
            LoginHandler.execute(packet, clientEp);
            var session = RakSessionManager.getSession(clientEp);

            Player player = new Player();
            player.Username = session.username;

            long EntityId = Server.AddPlayer(player, clientEp);
            session.EntityID = EntityId;
            player.EntityID = EntityId;
        }

        public static void PacketViolationWarning(PacketViolationWarning packet)
        {
            Log.error($"Client reported that server sent failed packet '{(Info.Bedrock)packet.packetId}'");
            Log.error(packet.description);
        }

        public static void ClientCacheStatus(ClientCacheStatus packet, IPEndPoint clientEp)
        {
            var player = RakSessionManager.getSession(clientEp);
            Log.debug($"{player.username} ClientCacheStatus = {packet.status}");

            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk1 = new ResourcePacksInfo
            {
                force = false,
                isAddon = false,
                hasScripts = false,
            };
            pk1.Encode(encoder);
        }

        public static void ResourcePackClientResponse(ResourcePackClientResponse packet, IPEndPoint clientEp)
        {
            Log.debug($"ResourcePackClientResponse = {packet.response}");
            if (packet.response == 3)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new ResourcePackStack
                {
                    forceTexturePack = false,
                };
                pk.Encode(encoder);
            }
            else if (packet.response == 4) //start game
            {
                var player = Server.GetPlayer(RakSessionManager.getSession(clientEp).EntityID);
                player.currentLevel = Server.levels[0];
                player.spawn();
            }
        }

        public static void Interact(Interact packet)
        {
            //Log.debug($"Action for {packet.actorRuntimeId} / {packet.action}");
        }
    }
}
