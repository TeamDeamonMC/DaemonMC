using DaemonMC.Network.Bedrock;
using DaemonMC.Plugin.Plugin;

namespace DaemonMC.Network
{
    public abstract class Packet : IPacket
    {
        public abstract Info.Bedrock Id { get; }

        public void DecodePacket(PacketDecoder decoder, PacketHandler handler = PacketHandler.Player)
        {
            Decode(decoder);
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
