using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class MovePlayer : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.MovePlayer;

        public long actorRuntimeId = 0;
        public Vector3 position = new Vector3();
        public Vector2 rotation = new Vector2();
        public float YheadRotation = 0;
        public byte positionMode = 0;
        public bool isOnGround = false;
        public long vehicleRuntimeId = 0;
        public long tick = 0;

        protected override void Decode(PacketDecoder decoder)
        {
            actorRuntimeId = decoder.ReadVarLong();
            position = decoder.ReadVec3();
            rotation = decoder.ReadVec2();
            YheadRotation = decoder.ReadFloat();
            positionMode = decoder.ReadByte();
            isOnGround = decoder.ReadBool();
            vehicleRuntimeId = decoder.ReadVarLong();
            tick = decoder.ReadVarLong();
        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong((ulong)actorRuntimeId);
            encoder.WriteVec3(position);
            encoder.WriteVec2(rotation);
            encoder.WriteFloat(YheadRotation);
            encoder.WriteByte(0);
            encoder.WriteBool(isOnGround);
            encoder.WriteVarLong((ulong)vehicleRuntimeId);
            encoder.WriteVarLong((ulong)tick);
        }
    }
}
