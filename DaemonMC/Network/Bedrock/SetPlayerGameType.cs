namespace DaemonMC.Network.Bedrock
{
    public class SetPlayerGameType : Packet
    {
        public override int Id => (int) Info.Bedrock.SetPlayerGameType;

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
