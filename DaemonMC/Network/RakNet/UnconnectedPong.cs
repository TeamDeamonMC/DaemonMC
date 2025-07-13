namespace DaemonMC.Network.RakNet
{
    public class UnconnectedPong : Packet
    {
        public override int Id => (int) Info.RakNet.UnconnectedPong;

        public long Time { get; set; }
        public long GUID { get; set; }
        public string Magic { get; set; }
        public string MOTD { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteLongLE(Time);
            encoder.WriteLongLE(GUID);
            encoder.WriteMagic(Magic);
            encoder.WriteRakString(MOTD);
        }
    }
}
