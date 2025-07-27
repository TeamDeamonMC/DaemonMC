using System.Net;

namespace DaemonMC.Network.RakNet
{
    public class OpenConnectionRequest2 : Packet
    {
        public override int Id => (int) Info.RakNet.OpenConnectionRequest2;

        public byte[] Magic { get; set; }
        public IPAddressInfo Address { get; set; }
        public short Mtu { get; set; }
        public long ClientId { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Magic = decoder.ReadBytes(16);
            Address = decoder.ReadAddress();
            Mtu = decoder.ReadSignedShort();
            ClientId = decoder.ReadLong();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
