namespace DaemonMC.Network.Bedrock
{
    public class MobEffect : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.MobEffect;

        public long EntityId { get; set; } = 0;
        public byte EventId { get; set; } = 0;
        public int EffectId { get; set; } = 0;
        public int EffectAmplifier { get; set; } = 0;
        public bool ShowParticles { get; set; } = true;
        public int Duration { get; set; } = 0;
        public long Tick { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(EntityId);
            encoder.WriteByte(EventId);
            encoder.WriteSignedVarInt(EffectId);
            encoder.WriteSignedVarInt(EffectAmplifier);
            encoder.WriteBool(ShowParticles);
            encoder.WriteSignedVarInt(Duration);
            encoder.WriteVarLong(Tick);
        }
    }
}
