namespace DaemonMC.Network.Bedrock
{

    public class PacketViolationWarning
    {
        public Info.Bedrock id = Info.Bedrock.PacketViolationWarning;

        public int type = 0;
        public int serverity = 0;
        public int packetId = 0;
        public string description = "";

        public void Decode(PacketDecoder decoder)
        {
            var packet = new PacketViolationWarning
            {
                type = decoder.ReadSignedVarInt(),
                serverity = decoder.ReadSignedVarInt(),
                packetId = decoder.ReadSignedVarInt(),
                description = decoder.ReadString(),
            };

            BedrockPacketProcessor.PacketViolationWarning(packet);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}
