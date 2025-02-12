using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class MoveActorDelta
    {
        public Info.Bedrock id = Info.Bedrock.MoveActorDelta;

        public long EntityId = 0;
        public ushort Header = 0;
        public Vector3 Position = new Vector3();
        public Vector2 Rotation = new Vector2();
        public float YheadRotation = 0;

        public void Decode(PacketDecoder decoder)
        {

        }

        public void Encode(PacketEncoder encoder)
        {
            encoder.PacketId(id);
            encoder.WriteVarLong((ulong)EntityId);
            encoder.WriteShort(Header);
            encoder.WriteVec3(Position);
            encoder.WriteByte((byte)(Rotation.X / 1.40625));
            encoder.WriteByte((byte)(Rotation.Y / 1.40625));
            encoder.WriteByte((byte)(YheadRotation / 1.40625));
            encoder.handlePacket();
        }
    }
}
