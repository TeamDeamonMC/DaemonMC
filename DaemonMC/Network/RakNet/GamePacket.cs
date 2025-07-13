namespace DaemonMC.Network.RakNet
{
    public class GamePacket : Packet
    {
        public override int Id => (int) Info.RakNet.GamePacket;

        public byte[] Payload { get; set; }

        protected override void Decode(PacketDecoder decoder)
        {
            Payload = decoder.ReadBytes(decoder.buffer.Length - 1);
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBytes(Payload, false);
        }
    }
}
