using System.Net;
using System.Reflection.PortableExecutable;

namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionRequest2Packet
    {
        public string Magic { get; set; }
        public IPAddressInfo Address { get; set; }
        public short Mtu { get; set; }
        public long ClientId { get; set; }
    }

    public class OpenConnectionRequest2
    {
        public static byte id = 7;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new OpenConnectionRequest2Packet
            {
                Magic = decoder.ReadMagic(),
                Address = decoder.ReadAddress(),
                Mtu = decoder.ReadShort(),
                ClientId = decoder.ReadLong()
            };
            RakPacketProcessor.OpenConnectionRequest2(packet, decoder.endpoint);
        }

        public static void Encode(OpenConnectionRequest2Packet fields)
        {

        }
    }
}
