namespace DaemonMC.Network.Bedrock
{
    public class UpdateAdventureSettings : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.UpdateAdventureSettings;

        public bool noPvM { get; set; } = false;
        public bool noMvP { get; set; } = false;
        public bool ImmutableWorld { get; set; } = false;
        public bool ShowNameTags { get; set; } = false;
        public bool AutoJump { get; set; } = false;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBool(noPvM);
            encoder.WriteBool(noMvP);
            encoder.WriteBool(ImmutableWorld);
            encoder.WriteBool(ShowNameTags);
            encoder.WriteBool(AutoJump);
        }
    }
}
