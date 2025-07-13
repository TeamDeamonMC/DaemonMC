namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionReply2 : Packet
    {
        public override int Id => (int) Info.RakNet.OpenConnectionReply2;

        public string Magic { get; set; }
        public long GUID { get; set; }
        public IPAddressInfo clientAddress { get; set; }
        public int Mtu { get; set; }
        public bool Encryption { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteMagic(Magic);
            encoder.WriteLongLE(GUID);
            encoder.WriteAddress();
            encoder.WriteShortBE((ushort)Mtu);
            encoder.WriteByte(0);
        }
    }
}
