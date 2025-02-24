using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class MovePlayer : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.MovePlayer;

        public long ActorRuntimeId { get; set; } = 0;
        public Vector3 Position { get; set; } = new Vector3();
        public Vector2 Rotation { get; set; } = new Vector2();
        public float YheadRotation { get; set; } = 0;
        public byte PositionMode { get; set; } = 0;
        public bool IsOnGround { get; set; } = false;
        public long VehicleRuntimeId { get; set; } = 0;
        public long Tick { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            ActorRuntimeId = decoder.ReadVarLong();
            Position = decoder.ReadVec3();
            Rotation = decoder.ReadVec2();
            YheadRotation = decoder.ReadFloat();
            PositionMode = decoder.ReadByte();
            IsOnGround = decoder.ReadBool();
            VehicleRuntimeId = decoder.ReadVarLong();
            Tick = decoder.ReadVarLong();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong((ulong)ActorRuntimeId);
            encoder.WriteVec3(Position);
            encoder.WriteVec2(Rotation);
            encoder.WriteFloat(YheadRotation);
            encoder.WriteByte(0);
            encoder.WriteBool(IsOnGround);
            encoder.WriteVarLong((ulong)VehicleRuntimeId);
            encoder.WriteVarLong((ulong)Tick);
        }
    }
}
