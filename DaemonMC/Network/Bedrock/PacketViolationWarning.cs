namespace DaemonMC.Network.Bedrock
{

    public class PacketViolationWarning : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.PacketViolationWarning;

        public int type = 0;
        public int serverity = 0;
        public int packetId = 0;
        public string description = "";

        protected override void Decode(PacketDecoder decoder)
        {
            type = decoder.ReadSignedVarInt();
            serverity = decoder.ReadSignedVarInt();
            packetId = decoder.ReadSignedVarInt();
            description = decoder.ReadString();
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
