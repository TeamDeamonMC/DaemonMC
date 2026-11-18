using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class MoveActorDelta : Packet
    {
        public override int Id => (int) Info.Bedrock.MoveActorDelta;

        public long EntityId { get; set; } = 0;
        public ushort Header { get; set; } = 0;
        public Vector3 Position { get; set; } = new Vector3();
        public Vector2 Rotation { get; set; } = new Vector2();
        public float YheadRotation { get; set; } = 0;
        public long Tick { get; set; } = 0;

        public const ushort HasX = 0x01;
        public const ushort HasY = 0x02;
        public const ushort HasZ = 0x04;
        public const ushort HasPitch = 0x08;
        public const ushort HasYaw = 0x10;
        public const ushort HasHeadYaw = 0x20;
        public const ushort OnGround = 0x40;
        public const ushort Teleport = 0x80;
        public const ushort ForceMoveLocalEntity = 0x100;
        public const ushort ForceCompletion = 0x200;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(EntityId);
            if (encoder.protocolVersion >= Info.v1_26_40)
            {
                WriteOptional(encoder, HasX, () => encoder.WriteFloat(Position.X));
                WriteOptional(encoder, HasY, () => encoder.WriteFloat(Position.Y));
                WriteOptional(encoder, HasZ, () => encoder.WriteFloat(Position.Z));
                WriteOptional(encoder, HasPitch, () => encoder.WriteByte((byte)(Rotation.X / 1.40625)));
                WriteOptional(encoder, HasYaw, () => encoder.WriteByte((byte)(Rotation.Y / 1.40625)));
                WriteOptional(encoder, HasHeadYaw, () => encoder.WriteByte((byte)(YheadRotation / 1.40625)));
                encoder.WriteBool((Header & OnGround) != 0);
                encoder.WriteBool((Header & Teleport) != 0);
                encoder.WriteBool((Header & ForceMoveLocalEntity) != 0);
                encoder.WriteBool((Header & ForceCompletion) != 0);
                if (encoder.protocolVersion >= Info.v1_26_50)
                {
                    encoder.WriteVarLong(Tick);
                }
                return;
            }
            encoder.WriteShort(Header);
            encoder.WriteVec3(Position);
            encoder.WriteByte((byte)(Rotation.X / 1.40625));
            encoder.WriteByte((byte)(Rotation.Y / 1.40625));
            encoder.WriteByte((byte)(YheadRotation / 1.40625));
        }

        private void WriteOptional(PacketEncoder encoder, ushort flag, Action write)
        {
            bool present = (Header & flag) != 0;
            encoder.WriteBool(present);
            if (present)
            {
                write();
            }
        }
    }
}
