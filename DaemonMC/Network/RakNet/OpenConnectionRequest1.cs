using System.Net;

namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionRequest1Packet
    {
        public string Magic { get; set; }
        public byte Protocol { get; set; }
        public int Mtu { get; set; }
    }

    public class OpenConnectionRequest1
    {
        public static byte id = 5;
        public static void Decode(PacketDecoder decoder, int recv)
        {
            var packet = new OpenConnectionRequest1Packet
            {
                Magic = decoder.ReadMagic(),
                Protocol = decoder.ReadByte(),
                Mtu = decoder.ReadMTU(recv),
            };

            RakPacketProcessor.OpenConnectionRequest1(packet, decoder.endpoint);
        }

        public static void Encode(OpenConnectionRequest1Packet fields)
        {

        }
    }
}
