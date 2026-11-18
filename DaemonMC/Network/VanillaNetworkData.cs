namespace DaemonMC.Network
{
    public static class VanillaNetworkData
    {
        public static VanillaBlockProperties BlockProperties;
        public static VanillaVoxelShapes VoxelShapes;
    }

    public class VanillaBlockProperties
    {
        public int Count { get; set; } = 0;
        public byte[] Data { get; set; } = [];
    }

    public class VanillaVoxelShapes
    {
        public int ShapeCount { get; set; } = 0;
        public byte[] Shapes { get; set; } = [];
        public List<(string Name, ushort Handle)> NameMap { get; set; } = new();
    }
}
