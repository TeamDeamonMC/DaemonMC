using DaemonMC.Utils.Game;

namespace DaemonMC.Network.Bedrock
{
    public class VoxelShapes : Packet
    {
        public override int Id => (int) Info.Bedrock.VoxelShapes;

        public List<VoxelShape> Shapes { get; set; } = new List<VoxelShape>();

        protected override void Decode(PacketDecoder decoder)
        {

        }

        protected override void Encode(PacketEncoder encoder)
        {
            encoder.WriteVoxelShapes(Shapes);
            encoder.WriteVoxelNameMap(Shapes);
            encoder.WriteShort(0);//?? todo
        }
    }
}
