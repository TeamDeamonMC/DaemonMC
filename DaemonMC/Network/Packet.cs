using DaemonMC.Network.Bedrock;
using DaemonMC.Network.RakNet;
using DaemonMC.Plugin;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network
{
    public abstract class Packet : IPacket
    {
        public abstract int Id { get; }

        public Packet GetDecodedPacket(PacketDecoder decoder)
        {
            Decode(decoder);
            return this;
        }

        public void DecodePacket(PacketDecoder decoder, PacketHandler handler = PacketHandler.Player)
        {
            try
            {
                Decode(decoder);
            }
            catch (Exception e)
            {
                if (decoder.player != null)
                {
                    decoder.player.Kick($"Handling {Id}\n {e}");
                }
                else
                {
                    PacketEncoder encoder = PacketEncoderPool.Get(decoder.clientEp);
                    var packet = new Disconnect
                    {
                        Message = $"Handling {Id}\n {e}"
                    };
                    packet.EncodePacket(encoder);
                }
                Log.warn($"Packet decoding error for {decoder.clientEp.Address}. \n Handling {Id}\n {e}");
                return;
            }
            if (PluginManager.PacketReceived(decoder.clientEp, this))
            {
                switch (handler)
                {
                    case PacketHandler.Player:
                        decoder.player.HandlePacket(this);
                        break;
                    case PacketHandler.Bedrock:
                        BedrockPacketProcessor.HandlePacket(this, decoder.clientEp);
                        break;
                    case PacketHandler.Raknet:
                        RakPacketProcessor.HandlePacket(this, decoder.clientEp);
                        break;
                }
            }
        }

        public void EncodePacket(PacketEncoder encoder)
        {
            if (PluginManager.PacketSent(encoder.clientEp, this))
            {
                switch (this)
                {
                    case UnconnectedPong:
                    case ACK:
                    case NACK:
                    case OpenConnectionReply1:
                    case OpenConnectionReply2:
                    case ConnectedPong:
                    case ConnectionRequestAccepted:
                    case Disconnect:
                    case GamePacket:
                        encoder.WriteByte((byte)Id);
                        break;
                    default:
                        encoder.PacketId(Id);
                        break;
                }
                Encode(encoder);
                switch (this)
                {
                    case UnconnectedPong:
                    case ACK:
                    case NACK:
                    case OpenConnectionReply1:
                    case OpenConnectionReply2:
                        encoder.SendPacket((byte)Id);
                        break;
                    case ConnectedPong:
                    case ConnectionRequestAccepted:
                    case Disconnect:
                    case GamePacket:
                        encoder.handlePacket("raknet");
                        break;
                    default:
                        encoder.handlePacket();
                        break;
                }
            }
        }

        protected abstract void Decode(PacketDecoder decoder);
        protected abstract void Encode(PacketEncoder encoder);
    }

    public enum PacketHandler
    {
        Player,
        Bedrock,
        Raknet
    }
}
