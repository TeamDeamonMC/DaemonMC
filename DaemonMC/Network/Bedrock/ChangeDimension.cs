using System.Numerics;

namespace DaemonMC.Network.Bedrock
{
    public class ChangeDimension : Packet
    {
        public override int Id => (int) Info.Bedrock.ChangeDimension;

        public int Dimension { get; set; } = 0;
        public Vector3 Position { get; set; } = new Vector3();
        public bool Respawn { get; set; } = false;
        public uint? LoadingScreenId { get; set; } = null;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteSignedVarInt(Dimension);
            encoder.WriteVec3(Position);
            encoder.WriteBool(Respawn);
            encoder.WriteOptional(LoadingScreenId == null ? null : () => encoder.WriteInt((int)LoadingScreenId.Value));
        }
    }
}
