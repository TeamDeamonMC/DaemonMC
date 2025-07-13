using System.Numerics;
using DaemonMC.Blocks;

namespace DaemonMC.Network.Bedrock
{
    public class UpdateBlock : Packet
    {
        public override int Id => (int) Info.Bedrock.UpdateBlock;

        public Vector3 Position { get; set; } = new Vector3();
        public Block Block { get; set; } = new Air();
        public int Flags { get; set; } = 2;
        public int Layer { get; set; } = 0;

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteBlockNetPos(Position);
            encoder.WriteVarInt_Signed(Block.GetHash());
            encoder.WriteVarInt(Flags);
            encoder.WriteVarInt(Layer);
        }
    }
}
