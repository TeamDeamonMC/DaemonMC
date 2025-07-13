namespace DaemonMC.Network.RakNet
{
    public class ConnectionRequest : Packet
    {
        public override int Id => (int) Info.RakNet.ConnectionRequest;

        public long Time { get; set; }
        public long GUID { get; set; }
        public byte Security { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            GUID = decoder.ReadLong();
            Time = decoder.ReadLongLE();
            Security = decoder.ReadByte();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteLong(GUID);
            encoder.WriteLongLE(Time);
            encoder.WriteByte(Security);
        }
    }
}
