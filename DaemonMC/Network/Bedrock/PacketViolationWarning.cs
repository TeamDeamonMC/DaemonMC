namespace DaemonMC.Network.Bedrock
{

    public class PacketViolationWarning : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.PacketViolationWarning;

        public int Type { get; set; } = 0;
        public int Serverity { get; set; } = 0;
        public int PacketId { get; set; } = 0;
        public string Description { get; set; } = "";

        protected override void Decode(PacketDecoder decoder)
        {
            Type = decoder.ReadSignedVarInt();
            Serverity = decoder.ReadSignedVarInt();
            PacketId = decoder.ReadSignedVarInt();
            Description = decoder.ReadString();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
