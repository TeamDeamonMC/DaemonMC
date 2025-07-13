namespace DaemonMC.Network.RakNet
{
    public class NewIncomingConnection : Packet
    {
        public override int Id => (int) Info.RakNet.NewIncomingConnection;

        public IPAddressInfo serverAddress { get; set; }
        public IPAddressInfo[] internalAddress { get; set; }
        public long incommingTime { get; set; }
        public long serverTime { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            serverAddress = decoder.ReadAddress();
            internalAddress = decoder.ReadInternalAddress();
            incommingTime = decoder.ReadLongLE();
            serverTime = decoder.ReadLongLE();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
