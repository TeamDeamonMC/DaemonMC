using System.Net;

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
                internalAddress = decoder.ReadInternalAddress(20),
                incommingTime = decoder.ReadLong(),
                serverTime = decoder.ReadLong()
            };

            RakPacketProcessor.NewIncomingConnection(packet);
        }

        public static void Encode(NewIncomingConnectionPacket fields)
        {

        }
    }
}
