using System.Net;
using System.Text;
using DaemonMC.Network.Handler;
using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class BedrockPacketProcessor
    {
        public static void RequestNetworkSettings(RequestNetworkSettingsPacket packet, IPEndPoint clientEp)
        {
            Log.debug($"New player ({RakSessionManager.getSession(clientEp).GUID}) log in with protocol version: {packet.protocolVersion}");

            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new NetworkSettingsPacket
            {
                compressionThreshold = 0,
                compressionAlgorithm = 0,
                clientThrottleEnabled = false,
                clientThrottleScalar = 0,
                clientThrottleThreshold = 0
            };
            NetworkSettings.Encode(pk, encoder);

            RakSessionManager.Compression(clientEp, true);
        }

        public static void Login(LoginPacket packet, IPEndPoint clientEp)
        {
            Handler.Login.execute(packet, clientEp);
        }

        public static void PacketViolationWarning(PacketViolationWarningPacket packet)
        {
            Log.error($"Client reported that server sent failed packet '{(Info.Bedrock)packet.packetId}'");
            Log.error(packet.description);
        }

        public static void ClientCacheStatus(ClientCacheStatusPacket packet, IPEndPoint clientEp)
        {
            var player = RakSessionManager.getSession(clientEp);
            Log.debug($"{player.username} ClientCacheStatus = {packet.status}");

            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk1 = new ResourcePacksInfoPacket
            {
                force = false,
                isAddon = false,
                hasScripts = false,
            };
            ResourcePacksInfo.Encode(pk1, encoder);
        }

        public static void ResourcePackClientResponse(ResourcePackClientResponsePacket packet, IPEndPoint clientEp)
        {
            Log.debug($"ResourcePackClientResponse = {packet.response}");
            if (packet.response == 3)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
                var pk = new ResourcePackStackPacket
                {
                    forceTexturePack = false,
                };
                ResourcePackStack.Encode(pk, encoder);
            }
            else if (packet.response == 4) //start game
            {
                preSpawn.execute(clientEp);
            }
        }

        public static void RequestChunkRadius(RequestChunkRadiusPacket packet, IPEndPoint clientEp)
        {
            var player = RakSessionManager.getSession(clientEp);
            Log.debug($"{player.username} requested chunks with radius {packet.radius}. Max radius = {packet.maxRadius}");

            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new ChunkRadiusUpdatedPacket
            {
                radius = packet.radius,
            };
            ChunkRadiusUpdated.Encode(pk, encoder);

            return;
         /*   for (int x = -20; x <= 20; x++)
            {
                for (int z = -20; z <= 20; z++)
                {
                    Log.debug($"({x}, {z})");
                    var pk1 = new LevelChunkPacket
                    {
                        chunkX = x,
                        chunkZ = z,
                        data = ""
                    };
                    LevelChunk.Encode(pk1);
                }
            }*/
        }

        public static void MovePlayer(MovePlayerPacket packet)
        {
            //Log.debug($"{packet.actorRuntimeId} / {packet.position.X} : {packet.position.Y} : {packet.position.Z}");
        }

        public static void ServerboundLoadingScreen(ServerboundLoadingScreenPacket packet)
        {

        }

        public static void Interact(InteractPacket packet)
        {
            //Log.debug($"Action for {packet.actorRuntimeId} / {packet.action}");
        }
    }
}
