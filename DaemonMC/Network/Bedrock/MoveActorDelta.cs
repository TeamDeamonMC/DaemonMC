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

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVarLong(EntityId);
            encoder.WriteShort(Header);
            encoder.WriteVec3(Position);
            encoder.WriteByte((byte)(Rotation.X / 1.40625));
            encoder.WriteByte((byte)(Rotation.Y / 1.40625));
            encoder.WriteByte((byte)(YheadRotation / 1.40625));
        }
    }
}
