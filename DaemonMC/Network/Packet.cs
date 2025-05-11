using DaemonMC.Network.Bedrock;
using DaemonMC.Plugin;
using DaemonMC.Utils.Text;

namespace DaemonMC.Network
{
    public abstract class Packet : IPacket
    {
        public abstract Info.Bedrock Id { get; }

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
                        //todo
                        break;
                }
            }
        }

        public void EncodePacket(PacketEncoder encoder)
        {
            if (PluginManager.PacketSent(encoder.clientEp, this))
            {
                encoder.PacketId(Id);
                Encode(encoder);
                encoder.handlePacket();
            }
        }

        protected abstract void Decode(PacketDecoder decoder);
        protected abstract void Encode(PacketEncoder encoder);
    }

    public interface IPacket
    {
        void DecodePacket(PacketDecoder decoder, PacketHandler handler);
        void EncodePacket(PacketEncoder encoder);
    }

    public enum PacketHandler
    {
        Player,
        Bedrock,
        Raknet
    }
}
