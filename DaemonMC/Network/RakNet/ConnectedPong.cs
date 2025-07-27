namespace DaemonMC.Network.RakNet
{
    public class ConnectedPong : Packet
    {
        public override int Id => (int) Info.RakNet.ConnectedPong;

        public long pingTime { get; set; }
        public long pongTime { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            pingTime = decoder.ReadLongLE();
            pongTime = decoder.ReadLongLE();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteLongLE(pingTime);
            encoder.WriteLongLE(pongTime);
        }
    }
}
