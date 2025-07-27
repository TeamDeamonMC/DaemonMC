namespace DaemonMC.Network.RakNet
{
    public class UnconnectedPong : Packet
    {
        public override int Id => (int) Info.RakNet.UnconnectedPong;

        public long Time { get; set; }
        public long GUID { get; set; }
        public byte[] Magic { get; set; }
        public string MOTD { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Time = decoder.ReadLongLE();
            GUID = decoder.ReadLongLE();
            Magic = decoder.ReadBytes(16);
            MOTD = decoder.ReadRakString();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteLongLE(Time);
            encoder.WriteLongLE(GUID);
            encoder.WriteBytes(Magic, false);
            encoder.WriteRakString(MOTD);
        }
    }
}
