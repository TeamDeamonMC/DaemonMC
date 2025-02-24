namespace DaemonMC.Network.Bedrock
{
    public class Emote : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.Emote;

        public long ActorRuntimeId { get; set; } = 0;
        public string EmoteID { get; set; } = "";
        public int EmoteTicks { get; set; } = 0;
        public string XUID { get; set; } = "";
        public string PlatformID { get; set; } = "";
        public byte Flags { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            ActorRuntimeId = decoder.ReadVarLong();
            EmoteID = decoder.ReadString();
            EmoteTicks = decoder.ReadVarInt();
            XUID = decoder.ReadString();
            PlatformID = decoder.ReadString();
            Flags = decoder.ReadByte();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(ActorRuntimeId);
            encoder.WriteString(EmoteID);
            encoder.WriteVarInt(EmoteTicks);
            encoder.WriteString(XUID);
            encoder.WriteString(PlatformID);
            encoder.WriteByte(Flags);
        }
    }
}
