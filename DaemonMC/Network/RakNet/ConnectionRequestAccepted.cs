using System.Text;

namespace DaemonMC.Network.RakNet
{
    public class ConnectionRequestAccepted : Packet
    {
        public override int Id => (int) Info.RakNet.ConnectionRequestAccepted;

        public IPAddressInfo clientAddress { get; set; }
        public ushort systemIndex { get; set; }
        public IPAddressInfo[] internalAddress { get; set; }
        public long requestTime { get; set; }
        public long Time { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            clientAddress = decoder.ReadAddress();
            systemIndex = decoder.ReadShort();

            for (int i = 0; i < 20; ++i)
            {
                decoder.ReadAddress();
            }

            requestTime = decoder.ReadLong();
            Time = decoder.ReadLong();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteAddress();
            encoder.WriteShort(systemIndex);

            for (int i = 0; i < 20; ++i)
            {
                encoder.WriteAddress();
            }

            encoder.WriteLongLE(Time);
            encoder.WriteLongLE(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }
    }
}
