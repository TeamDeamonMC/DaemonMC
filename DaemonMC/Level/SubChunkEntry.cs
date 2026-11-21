using DaemonMC.Network.Enumerations;

namespace DaemonMC.Level
{
    public class SubChunkEntry
    {
        public sbyte OffsetX { get; set; } = 0;
        public sbyte OffsetY { get; set; } = 0;
        public sbyte OffsetZ { get; set; } = 0;
        public SubChunkRequestResult Result { get; set; } = SubChunkRequestResult.Undefined;
        public byte[] Payload { get; set; } = [];
        public HeightMapDataType HeightMapType { get; set; } = HeightMapDataType.NoData;
        public sbyte[] HeightMap { get; set; } = [];
        public HeightMapDataType RenderHeightMapType { get; set; } = HeightMapDataType.NoData;
        public sbyte[] RenderHeightMap { get; set; } = [];
    }
}
