namespace DaemonMC.Network.Bedrock
{
    public class ServerboundLoadingScreen : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ServerboundLoadingScreen;

        public int ScreenType { get; set; } = 0;
        public int? ScreenId { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            ScreenType = decoder.ReadVarInt();
            ScreenId = decoder.ReadOptional(decoder.ReadInt);
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
