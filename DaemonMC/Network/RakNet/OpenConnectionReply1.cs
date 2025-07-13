namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionReply1 : Packet
    {
        public override int Id => (int) Info.RakNet.OpenConnectionReply1;

        public string Magic { get; set; }
        public long GUID { get; set; }
        public bool Security { get; set; }
        public int Cookie { get; set; }
        public int Mtu { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteMagic(Magic);
            encoder.WriteLongLE(GUID);
            encoder.WriteByte(0);
            encoder.WriteShortBE((ushort)Mtu);
        }
    }
}
