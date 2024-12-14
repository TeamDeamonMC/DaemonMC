namespace DaemonMC.Network.Bedrock
{
    public class InteractPacket
    {
        public byte action { get; set; }
        public long actorRuntimeId { get; set; }
    }

    public class Interact
    {
        public const int id = 33;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new InteractPacket
            {
                action = decoder.ReadByte(),
                actorRuntimeId = decoder.ReadVarLong()
            };

            BedrockPacketProcessor.Interact(packet);
        }

        public static void Encode(InteractPacket fields)
        {

        }
    }
}
