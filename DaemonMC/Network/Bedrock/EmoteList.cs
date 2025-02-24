namespace DaemonMC.Network.Bedrock
{
    public class EmoteList : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.EmoteList;

        public long ActorRuntimeId = 0;
        public List<Guid> EmoteIds = new List<Guid>();

        protected override void Decode(PacketDecoder decoder)
        {
            ActorRuntimeId = decoder.ReadVarLong();
            var size = decoder.ReadVarInt();
            for (int v = 0; v < size; v++)
            {
                EmoteIds.Add(decoder.ReadUUID());
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong((ulong)ActorRuntimeId);
            encoder.WriteVarInt(EmoteIds.Count());
            foreach (var emote in EmoteIds)
            {
                encoder.WriteUUID(emote);
            }
        }
    }
}
