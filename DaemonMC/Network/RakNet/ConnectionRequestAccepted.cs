namespace DaemonMC.Network.RakNet
{
    public class ConnectionRequestAccepted : Packet
    {
        public override int Id => (int) Info.RakNet.ConnectionRequestAccepted;

        public IPAddressInfo ClientAddress { get; set; }
        public short SystemIndex { get; set; }
        public IPAddressInfo[] SystemAddress { get; set; }
        public long PingTime { get; set; }
        public long PongTime { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            ClientAddress = decoder.ReadAddress();
            SystemIndex = (short)decoder.ReadShort();
            SystemAddress = decoder.ReadInternalAddress();
            PingTime = decoder.ReadLongLE();
            PongTime = decoder.ReadLongLE();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteAddress(ClientAddress);
            encoder.WriteShort(SystemIndex);

            for (int i = 0; i < SystemAddress.Length; ++i)
            {
                encoder.WriteAddress(SystemAddress[i]);
            }

            encoder.WriteLongLE(PingTime);
            encoder.WriteLongLE(PongTime);
        }
    }
}
