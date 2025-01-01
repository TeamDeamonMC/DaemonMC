namespace DaemonMC.Network.Bedrock
{
    public class ServerboundLoadingScreen
    {
        public Info.Bedrock id = Info.Bedrock.ServerboundLoadingScreen;

        public int screenType = 0;
        public int screenId = 0;

        public void Decode(PacketDecoder decoder)
        {
            var packet = new ServerboundLoadingScreen
            {
                screenType = decoder.ReadVarInt(),
                screenId = decoder.ReadInt()
            };

            decoder.player.PacketEvent_ServerboundLoadingScreen(packet);
        }

        public void Encode(PacketEncoder encoder)
        {

        }
    }
}
