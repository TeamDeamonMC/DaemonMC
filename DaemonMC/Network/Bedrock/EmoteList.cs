namespace DaemonMC.Network.Bedrock
{
    public class EmoteList : Packet
    {
        public override int Id => (int) Info.Bedrock.EmoteList;

        public long ActorRuntimeId { get; set; } = 0;
        public List<Guid> EmoteIds { get; set; } = new List<Guid>();

        protected override void Decode(PacketDecoder decoder)
        {
            ActorRuntimeId = decoder.ReadVarLong();
            EmoteIds = decoder.ReadEmotes();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(ActorRuntimeId);
            encoder.WriteEmotes(EmoteIds);
        }
    }
}
