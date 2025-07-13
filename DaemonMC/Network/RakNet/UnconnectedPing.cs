namespace DaemonMC.Network.RakNet
{
    public class UnconnectedPing : Packet
    {
        public override int Id => (int) Info.RakNet.UnconnectedPing;

        public long Time { get; set; }
        public string Magic { get; set; }
        public long ClientId { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Time = decoder.ReadLongLE();
            Magic = decoder.ReadMagic();
            ClientId = decoder.ReadLong();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
