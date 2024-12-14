using DaemonMC.Network.RakNet;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network.Bedrock
{
    public class BedrockPacketDecoder
    {
        public static void BedrockDecoder(PacketDecoder decoder)
        {
            if (RakSessionManager.getSession(decoder.endpoint) != null)
            {
                if (RakSessionManager.getSession(decoder.endpoint).initCompression)
                {
                    decoder.ReadByte();
                }
            }
            var size = decoder.ReadVarInt(); //packet size
            var pkid = decoder.ReadVarInt();
            Log.debug($"[Server] <-- [{decoder.endpoint.Address,-16}:{decoder.endpoint.Port}] {(Info.Bedrock)pkid}");

            switch (pkid)
            {
                case RequestNetworkSettings.id:
                    RequestNetworkSettings.Decode(decoder);
                    break;
                case Login.id:
                    Login.Decode(decoder);
                    break;
                case PacketViolationWarning.id:
                    PacketViolationWarning.Decode(decoder);
                    break;
                case ClientCacheStatus.id:
                    ClientCacheStatus.Decode(decoder);
                    break;
                case ResourcePackClientResponse.id:
                    ResourcePackClientResponse.Decode(decoder);
                    break;
                case RequestChunkRadius.id:
                    RequestChunkRadius.Decode(decoder);
                    break;
                case MovePlayer.id:
                    MovePlayer.Decode(decoder);
                    break;
                case ServerboundLoadingScreen.id:
                    ServerboundLoadingScreen.Decode(decoder);
                    break;
                case Interact.id:
                    Interact.Decode(decoder);
                    break;
                case TextMessage.id:
                    TextMessage.Decode(decoder);
                    break;

                default:
                    Log.error($"[Server] Unknown Bedrock packet: {pkid}");
                    DataTypes.HexDump(decoder.buffer, decoder.buffer.Length);
                    break;
            }
        }
    }
}
