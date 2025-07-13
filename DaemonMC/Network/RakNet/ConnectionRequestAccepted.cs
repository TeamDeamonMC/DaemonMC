namespace DaemonMC.Network.RakNet
{
    public class ConnectionRequestAccepted : Packet
    {
        public override int Id => (int) Info.RakNet.ConnectionRequestAccepted;

        public long Time { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteAddress();
            encoder.WriteShort(0);

            for (int i = 0; i < 20; ++i)
            {
                encoder.WriteAddress();
            }

            encoder.WriteLongLE(Time);
            encoder.WriteLongLE(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }
    }
}
