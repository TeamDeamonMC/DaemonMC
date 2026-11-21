using System.Numerics;
using DaemonMC.Level;
using fNbt;

namespace DaemonMC.Network.Bedrock
{
    public class BlockActorData : Packet
    {
        public override int Id => (int) Info.Bedrock.BlockActorData;

        public Vector3 Position { get; set; } = new Vector3();
        public NbtCompound Nbt { get; set; } = new NbtCompound("");

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBlockNetPos(Position);
            encoder.WriteBytes(ChunkColumn.SerializeNetworkNbt(Nbt), false);
        }
    }
}
