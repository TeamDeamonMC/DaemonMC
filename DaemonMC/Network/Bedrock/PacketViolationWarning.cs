namespace DaemonMC.Network.Bedrock
{
    public class PacketViolationWarningPacket
    {
        public int type { get; set; }
        public int serverity { get; set; }
        public int packetId { get; set; }
        public string description { get; set; }
    }

    public class PacketViolationWarning
    {
        public const int id = 156;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new PacketViolationWarningPacket
            {
                type = decoder.ReadSignedVarInt(),
                serverity = decoder.ReadSignedVarInt(),
                packetId = decoder.ReadSignedVarInt(),
                description = decoder.ReadString(),
            };

            BedrockPacketProcessor.PacketViolationWarning(packet);
        }

        public static void Encode(PacketViolationWarningPacket fields)
        {

        }
    }
}
