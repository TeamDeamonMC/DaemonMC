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
                if (Server.onlinePlayers.ContainsKey(RakSessionManager.getSession(decoder.endpoint).EntityID))
                {
                    decoder.player = Server.GetPlayer(RakSessionManager.getSession(decoder.endpoint).EntityID);
                }
            }

            while (decoder.readOffset < decoder.buffer.Length)
            {
                var startOffset = decoder.readOffset;
                var size = decoder.ReadVarInt(); //packet size

                var pkid = (Info.Bedrock)decoder.ReadVarInt();
                Log.packetIn(decoder.endpoint, pkid);

                switch (pkid)
                {
                    case Info.Bedrock.RequestNetworkSettings:
                        new RequestNetworkSettings().Decode(decoder);
                        break;
                    case Info.Bedrock.Login:
                        new Login().Decode(decoder);
                        break;
                    case Info.Bedrock.PacketViolationWarning:
                        new PacketViolationWarning().Decode(decoder);
                        break;
                    case Info.Bedrock.ClientCacheStatus:
                        new ClientCacheStatus().Decode(decoder);
                        break;
                    case Info.Bedrock.ResourcePackClientResponse:
                        new ResourcePackClientResponse().Decode(decoder);
                        break;
                    case Info.Bedrock.RequestChunkRadius:
                        new RequestChunkRadius().Decode(decoder);
                        break;
                    case Info.Bedrock.MovePlayer:
                        new MovePlayer().Decode(decoder);
                        break;
                    case Info.Bedrock.ServerboundLoadingScreen:
                        new ServerboundLoadingScreen().Decode(decoder);
                        break;
                    case Info.Bedrock.Interact:
                        new Interact().Decode(decoder);
                        break;
                    case Info.Bedrock.TextMessage:
                        new TextMessage().Decode(decoder);
                        break;
                    case Info.Bedrock.PlayerAuthInput:
                        new PlayerAuthInput().Decode(decoder);
                        break;
                    case Info.Bedrock.PlayerSkin:
                        new PlayerSkin().Decode(decoder);
                        break;
                    case Info.Bedrock.ResourcePackChunkRequest:
                        new ResourcePackChunkRequest().Decode(decoder);
                        break;

                    default:
                        Log.error($"[Server] Unknown Bedrock packet: {pkid}");
                        DataTypes.HexDump(decoder.buffer, decoder.buffer.Length);
                        break;
                }

                int expectedOffset = startOffset + size;
                if (decoder.readOffset < expectedOffset)
                {
                    Log.warn($"{expectedOffset - decoder.readOffset} bytes left while reading {pkid}.");
                    break;

                }
            }
        }
    }
}
