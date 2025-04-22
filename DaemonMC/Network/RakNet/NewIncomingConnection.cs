namespace DaemonMC.Network.RakNet
{
    public class NewIncomingConnectionPacket
    {
        public IPAddressInfo serverAddress { get; set; }

        public IPAddressInfo[] internalAddress { get; set; }
        public long incommingTime { get; set; }
        public long serverTime { get; set; }
    }

    public class NewIncomingConnection
    {
        public static byte id = 19;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new NewIncomingConnectionPacket
            {
                serverAddress = decoder.ReadAddress(),
                internalAddress = decoder.ReadInternalAddress(),//todo
                incommingTime = decoder.ReadLong(),
                serverTime = decoder.ReadLong()
            };

            RakPacketProcessor.NewIncomingConnection(packet, decoder.clientEp);
        }

        public static void Encode(NewIncomingConnectionPacket fields)
        {

        }
    }
}
