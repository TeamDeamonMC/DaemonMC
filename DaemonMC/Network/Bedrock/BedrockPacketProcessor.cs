using System.Net;
using DaemonMC.Level;
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

            PacketEncoder encoder6 = PacketEncoderPool.Get(clientEp);
            var pk6 = new NetworkChunkPublisherUpdatePacket
            {
                x = 0,
                y = 1,
                z = 0,
                radius = 20
            };
            NetworkChunkPublisherUpdate.Encode(pk6, encoder6);

              for (int x = -2; x <= 2; x++)
              {
                  for (int z = -2; z <= 2; z++)
                  {
                      PacketEncoder encoder21 = PacketEncoderPool.Get(clientEp);
                      var pk11 = new LevelChunkPacket
                      {
                          chunkX = x,
                          chunkZ = z,
                          count = 20,
                          data = testchunk.flat
                          // data = new byte[0]
                      };
                      LevelChunk.Encode(pk11, encoder21);
                  }
              }

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

        public static void Text(TextMessagePacket packet, IPEndPoint clientEp)
        {
            var player = RakSessionManager.getSession(clientEp);

            foreach (var dest in Server.onlinePlayers)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value.ep);
                var pk = new TextMessagePacket
                {
                    Username = player.username,
                    Message = packet.Message
                };
                TextMessage.Encode(pk, encoder);
            }
        }
    }
}
