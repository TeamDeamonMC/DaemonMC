using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class LevelSoundEventPacket : Packet
    {
        public override int Id => (int) Info.Bedrock.LevelSoundEventPacket;

        public int EventID { get; set; } = 0;
        public Vector3 Position { get; set; } = new Vector3();
        public int Data { get; set; } = 0;
        public string ActorIdentifier { get; set; } = "";
        public bool IsBaby { get; set; } = false;
        public bool IsGlobal { get; set; } = false;
        public long EntityId { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            EventID = decoder.ReadVarInt();
            Position = decoder.ReadVec3();
            Data = decoder.ReadSignedVarInt();
            ActorIdentifier = decoder.ReadString();
            IsBaby = decoder.ReadBool();
            IsGlobal = decoder.ReadBool();
            EntityId = decoder.ReadLong();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarInt(EventID);
            encoder.WriteVec3(Position);
            encoder.WriteSignedVarInt(Data);
            encoder.WriteString(ActorIdentifier);
            encoder.WriteBool(IsBaby);
            encoder.WriteBool(IsGlobal);
            encoder.WriteLong(EntityId);
        }
    }
}
