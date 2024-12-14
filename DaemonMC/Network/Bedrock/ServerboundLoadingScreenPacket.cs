namespace DaemonMC.Network.Bedrock
{
    public class ServerboundLoadingScreenPacket
    {
        public int screenType { get; set; }
        public int screenId { get; set; }
    }

    public class ServerboundLoadingScreen
    {
        public const int id = 312;
        public static void Decode(PacketDecoder decoder)
        {
            var packet = new ServerboundLoadingScreenPacket
            {
                screenType = decoder.ReadVarInt(),
                screenId = decoder.ReadInt()
            };

            BedrockPacketProcessor.ServerboundLoadingScreen(packet);
        }

        public static void Encode(ServerboundLoadingScreenPacket fields)
        {

        }
    }
}
