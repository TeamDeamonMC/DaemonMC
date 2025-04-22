using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class ContainerOpen : Packet
    {
        public override Info.Bedrock Id => Info.Bedrock.ContainerOpen;

        public byte ContainerId { get; set; } = 0;
        public byte ContainerType { get; set; } = 0;
        public Vector3 Position { get; set; } = new Vector3();
        public long EntityId { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteByte(ContainerId);
            encoder.WriteByte(ContainerType);
            encoder.WriteSignedVarInt((int)Position.X);
            encoder.WriteSignedVarInt((int)Position.Y);
            encoder.WriteSignedVarInt((int)Position.Z);
            encoder.WriteSignedVarLong(EntityId);
        }
    }
}
