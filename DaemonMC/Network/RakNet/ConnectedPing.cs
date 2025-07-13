namespace DaemonMC.Network.RakNet
{
    public class ConnectedPing : Packet
    {
        public override int Id => (int) Info.RakNet.ConnectedPing;

        public long Time { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Time = decoder.ReadLongLE();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteLongLE(Time);
        }
    }
}
