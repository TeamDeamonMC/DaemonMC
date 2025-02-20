using System.Linq.Expressions;
using DaemonMC.Network.Bedrock;

namespace DaemonMC.Network
{
    public abstract class Packet : IPacket
    {
        public abstract Info.Bedrock Id { get; }

        public void DecodePacket(PacketDecoder decoder, PacketHandler handler = PacketHandler.Player)
        {
            Decode(decoder);
            switch (handler)
            {
                case PacketHandler.Player:
                    decoder.player.HandlePacket(this);
                    break;
                case PacketHandler.Bedrock:
                    BedrockPacketProcessor.HandlePacket(this, decoder.endpoint);
                    break;
                case PacketHandler.Raknet:
                    //todo
                    break;
            }
        }

        public void EncodePacket(PacketEncoder encoder)
        {
            encoder.PacketId(Id);
            Encode(encoder);
            encoder.handlePacket();
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
