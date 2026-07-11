using System.Numerics;

namespace DaemonMC.Utils.Game
{
    public class VoxelShape
    {
        public string Name { get; set; } = "";
        public List<Vector3> Coordinates { get; set; } = new List<Vector3>();
        public List<VoxelCells> Cells { get; set; } = new List<VoxelCells>();

        public VoxelShape(string name, List<Vector3> coordinates, List<VoxelCells> cells)
        {
            Name = name;
            Coordinates = coordinates;
            Cells = cells;
        }
    }

    public class VoxelCells()
    {
        public List<byte> Storage { get; set; } = new List<byte>();
        public byte Xsize { get; set; } = 0;
        public byte Ysize { get; set; } = 0;
        public byte Zsize { get; set; } = 0;
    }
}
