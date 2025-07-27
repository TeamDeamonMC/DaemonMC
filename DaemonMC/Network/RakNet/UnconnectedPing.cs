namespace DaemonMC.Network.RakNet
{
    public class UnconnectedPing : Packet
    {
        public override int Id => (int) Info.RakNet.UnconnectedPing;

        public long Time { get; set; }
        public byte[] Magic { get; set; }
        public long ClientId { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Time = decoder.ReadLongLE();
            Magic = decoder.ReadBytes(16);
            ClientId = decoder.ReadLong();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
