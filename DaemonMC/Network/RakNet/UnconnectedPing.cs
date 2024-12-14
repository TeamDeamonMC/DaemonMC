using System.Net;

namespace DaemonMC.Network.RakNet
{
    public class UnconnectedPingPacket
    {
        public long Time { get; set; }
        public string Magic { get; set; }
        public long ClientId { get; set; }
    }

    public class UnconnectedPing
    {
        public static byte id = 1;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new UnconnectedPingPacket
            {
                Time = decoder.ReadLongLE(),
                Magic = decoder.ReadMagic(),
                ClientId = decoder.ReadLong()
            };

            RakPacketProcessor.UnconnectedPing(packet, decoder.endpoint);
        }

        public static void Encode(UnconnectedPingPacket fields)
        {

        }
    }
}
