namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionRequest1 : Packet
    {
        public override int Id => (int) Info.RakNet.OpenConnectionRequest1;

        public byte[] Magic { get; set; }
        public byte Protocol { get; set; }
        public int Mtu { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Magic = decoder.ReadBytes(16);
            Protocol = decoder.ReadByte();
            Mtu = decoder.ReadMTU(decoder.buffer.Length);
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
