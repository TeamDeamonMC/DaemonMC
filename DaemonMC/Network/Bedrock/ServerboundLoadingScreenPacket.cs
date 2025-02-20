namespace DaemonMC.Network.Bedrock
{
    public class ServerboundLoadingScreen : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ServerboundLoadingScreen;

        public int screenType = 0;
        public int? screenId = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            screenType = decoder.ReadVarInt();
            screenId = decoder.ReadOptional(() => decoder.ReadInt());
        }

        protected override void Encode(PacketEncoder encoder)
        {

        }
    }
}
