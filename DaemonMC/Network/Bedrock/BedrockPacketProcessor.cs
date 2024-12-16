using System.Net;
using DaemonMC.Level;
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
        }

        public static void Login(Login packet, IPEndPoint clientEp)
        {
            LoginHandler.execute(packet, clientEp);
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
                preSpawn.execute(clientEp);
            }
        }

        public static void RequestChunkRadius(RequestChunkRadius packet, IPEndPoint clientEp)
        {
            var player = RakSessionManager.getSession(clientEp);
            Log.debug($"{player.username} requested chunks with radius {packet.radius}. Max radius = {packet.maxRadius}");

            PacketEncoder encoder = PacketEncoderPool.Get(clientEp);
            var pk = new ChunkRadiusUpdated
            {
                radius = packet.radius,
            };
            pk.Encode(encoder);

            PacketEncoder encoder6 = PacketEncoderPool.Get(clientEp);
            var pk6 = new NetworkChunkPublisherUpdate
            {
                x = 0,
                y = 1,
                z = 0,
                radius = 20
            };
            pk6.Encode(encoder6);

              for (int x = -2; x <= 2; x++)
              {
                  for (int z = -2; z <= 2; z++)
                  {
                      PacketEncoder encoder21 = PacketEncoderPool.Get(clientEp);
                      var pk11 = new LevelChunk
                      {
                          chunkX = x,
                          chunkZ = z,
                          count = 20,
                          data = testchunk.flat
                      };
                      pk11.Encode(encoder21);
                  }
              }

        }

        public static void MovePlayer(MovePlayer packet)
        {
            //Log.debug($"{packet.actorRuntimeId} / {packet.position.X} : {packet.position.Y} : {packet.position.Z}");
        }

        public static void ServerboundLoadingScreen(ServerboundLoadingScreen packet)
        {

        }

        public static void Interact(Interact packet)
        {
            //Log.debug($"Action for {packet.actorRuntimeId} / {packet.action}");
        }

        public static void Text(TextMessage packet, IPEndPoint clientEp)
        {
            var player = RakSessionManager.getSession(clientEp);

            foreach (var dest in Server.onlinePlayers)
            {
                PacketEncoder encoder = PacketEncoderPool.Get(dest.Value.ep);
                var pk = new TextMessage
                {
                    Username = player.username,
                    Message = packet.Message
                };
                pk.Encode(encoder);
            }
        }
    }
}
