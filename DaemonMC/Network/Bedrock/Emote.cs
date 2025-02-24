namespace DaemonMC.Network.Bedrock
{
    public class Emote : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.Emote;

        public long ActorRuntimeId = 0;
        public string EmoteID = "";
        public int EmoteTicks = 0;
        public string XUID = "";
        public string PlatformID = "";
        public byte Flags = 0;

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
            encoder.WriteVarLong((ulong)ActorRuntimeId);
            encoder.WriteString(EmoteID);
            encoder.WriteVarInt(EmoteTicks);
            encoder.WriteString(XUID);
            encoder.WriteString(PlatformID);
            encoder.WriteByte(Flags);
        }
    }
}
