namespace DaemonMC.Level
{
    public enum Dimension
    {
        Overworld = 0,
        Nether = 1,
        End = 2
    }

    public static class DimensionExtensions
    {
        public static int MinY(this Dimension dimension) => dimension == Dimension.Overworld ? -64 : 0;

        public static int SubChunkCount(this Dimension dimension) => dimension switch
        {
            Dimension.Overworld => 24, // -64....319
            Dimension.Nether => 8,     // 0...127
            Dimension.End => 16,       // 0....255
            _ => 24
        };

        public static int MaxY(this Dimension dimension) => dimension.MinY() + dimension.SubChunkCount() * 16 - 1;
    }
}
