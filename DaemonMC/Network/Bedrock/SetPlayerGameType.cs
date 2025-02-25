namespace DaemonMC.Network.Bedrock
{
    public class SetPlayerGameType : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.SetPlayerGameType;

        public int GameMode { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(GameMode);
        }
    }
}
