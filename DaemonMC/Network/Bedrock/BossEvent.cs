using DaemonMC.Network.Enumerations;

namespace DaemonMC.Network.Bedrock
{
    public class BossEvent : Packet
    {
        public override int Id => (int) Info.Bedrock.BossEvent;

        public long EntityId { get; set; } = 0;
        public long PlayerId { get; set; } = 0;
        public ushort DarkenScreen { get; set; } = 0;
        public BossEventType EventType { get; set; } = 0;
        public string Title { get; set; } = "";
        public string FilteredTitle { get; set; } = "";
        public float Health { get; set; } = 0;
        public BossBarColor Color { get; set; } = BossBarColor.Purple;
        public byte Overlay { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            EntityId = decoder.ReadSignedVarLong();
            if (decoder.protocolVersion >= Info.v1_26_30)
            {
                PlayerId = decoder.ReadSignedVarLong();
                EventType = (BossEventType)decoder.ReadByte();
                Title = decoder.ReadString();
                FilteredTitle = decoder.ReadString();
                Health = decoder.ReadFloat();
                Color = (BossBarColor)decoder.ReadByte();
                Overlay = decoder.ReadByte();
            }
            else
            {
                EventType = (BossEventType)decoder.ReadVarInt();
                switch (EventType)
                {
                    case BossEventType.PlayerAdded:
                    case BossEventType.Remove:
                    case BossEventType.Query:
                        PlayerId = decoder.ReadSignedVarLong();
                        break;
                    case BossEventType.Add:
                        Title = decoder.ReadString();
                        FilteredTitle = decoder.ReadString();
                        Health = decoder.ReadFloat();
                        break;
                    case BossEventType.UpdateProperties:
                        DarkenScreen = decoder.ReadShort();
                        break;
                    case BossEventType.UpdateStyle:
                        Color = (BossBarColor)decoder.ReadVarInt();
                        Overlay = (byte)decoder.ReadVarInt();
                        break;
                    case BossEventType.UpdatePercent:
                        Health = decoder.ReadFloat();
                        break;
                    case BossEventType.UpdateName:
                        Title = decoder.ReadString();
                        FilteredTitle = decoder.ReadString();
                        break;
                }
            }
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarLong(EntityId);
            if (encoder.protocolVersion >= Info.v1_26_30)
            {
                encoder.WriteSignedVarLong(PlayerId);
                encoder.WriteByte((byte)EventType);
                encoder.WriteString(Title);
                encoder.WriteString(FilteredTitle);
                encoder.WriteFloat(Health);
                encoder.WriteByte((byte)Color);
                encoder.WriteByte(Overlay);
            }
            else
            {
                encoder.WriteVarInt((int)EventType);
                switch (EventType)
                {
                    case BossEventType.PlayerAdded:
                    case BossEventType.Remove:
                    case BossEventType.Query:
                        encoder.WriteSignedVarLong(PlayerId);
                        break;
                    case BossEventType.Add:
                        encoder.WriteString(Title);
                        encoder.WriteString(FilteredTitle);
                        encoder.WriteFloat(Health);
                        encoder.WriteShort(DarkenScreen);
                        encoder.WriteVarInt((int)Color);
                        encoder.WriteVarInt(Overlay);
                        break;
                    case BossEventType.UpdateProperties:
                        encoder.WriteShort(DarkenScreen);
                        break;
                    case BossEventType.UpdateStyle:
                        encoder.WriteVarInt((int)Color);
                        encoder.WriteVarInt(Overlay);
                        break;
                    case BossEventType.UpdatePercent:
                        encoder.WriteFloat(Health);
                        break;
                    case BossEventType.UpdateName:
                        encoder.WriteString(Title);
                        encoder.WriteString(FilteredTitle);
                        break;
                }
            }
        }
    }
}
