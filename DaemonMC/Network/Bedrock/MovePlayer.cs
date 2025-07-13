using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class MovePlayer : Packet
    {
        public override int Id => (int) Info.Bedrock.MovePlayer;

        public long ActorRuntimeId { get; set; } = 0;
        public Vector3 Position { get; set; } = new Vector3();
        public Vector2 Rotation { get; set; } = new Vector2();
        public float YheadRotation { get; set; } = 0;
        public byte PositionMode { get; set; } = 0;
        public bool Teleport { get; set; } = false;
        public bool IsOnGround { get; set; } = false;
        public long VehicleRuntimeId { get; set; } = 0;
        public long Tick { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(ActorRuntimeId);
            encoder.WriteVec3(Position);
            encoder.WriteVec2(Rotation);
            encoder.WriteFloat(YheadRotation);
            encoder.WriteByte((byte)(Teleport ? 2 : 0));
            encoder.WriteBool(IsOnGround);
            encoder.WriteVarLong(VehicleRuntimeId);
            if (Teleport)
            {
                encoder.WriteInt(0); //tp cause
                encoder.WriteInt(0); //???source actor type
            }
            encoder.WriteVarLong(Tick);
        }
    }
}
