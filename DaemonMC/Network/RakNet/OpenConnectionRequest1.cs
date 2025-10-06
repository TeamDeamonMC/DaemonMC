namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionRequest1 : Packet
    {
        public override int Id => (int) Info.RakNet.OpenConnectionRequest1;

        public string Magic { get; set; }
        public byte Protocol { get; set; }
        public short Mtu { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Magic = decoder.ReadMagic();
            Protocol = decoder.ReadByte();
            Mtu = decoder.ReadMTU(decoder.buffer.Length);
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteMagic(Magic);
            encoder.WriteByte(Protocol);
            encoder.WriteMTU(Mtu);
        }
    }
}
