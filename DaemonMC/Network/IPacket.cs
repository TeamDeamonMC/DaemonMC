namespace DaemonMC.Network
{
    public interface IPacket
    {
        void DecodePacket(PacketDecoder decoder, PacketHandler handler);
        void EncodePacket(PacketEncoder encoder);
    }
}
