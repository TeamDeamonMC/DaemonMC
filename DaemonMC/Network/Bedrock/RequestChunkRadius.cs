namespace DaemonMC.Network.Bedrock
{
    public class RequestChunkRadius : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.RequestChunkRadius;

        public int radius = 0;
        public byte maxRadius = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            radius = decoder.ReadVarInt();
            maxRadius = decoder.ReadByte();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
