namespace DaemonMC.Utils
{
    public class ChunkUtils
    {
        public static List<(int x, int z)> GetSequence(int radius, int x, int z)
        {
            var positions = new List<(int x, int z)>();
            var visited = new HashSet<(int x, int z)>();

            positions.Add((0, 0));
            visited.Add((0, 0));

            var queue = new Queue<(int x, int z)>();
            queue.Enqueue((0, 0));

            while (queue.Count > 0)
            {
                (x, z) = queue.Dequeue();

                foreach (var (nx, nz) in GetNear(x, z))
                {
                    if (!visited.Contains((nx, nz)) && Math.Sqrt(nx * nx + nz * nz) <= radius)
                    {
                        visited.Add((nx, nz));
                        positions.Add((nx, nz));
                        queue.Enqueue((nx, nz));
                    }
                }
            }

            return positions;
        }

        private static List<(int x, int z)> GetNear(int x, int z)
        {
            return new List<(int, int)>
            {
                (x + 1, z),
                (x - 1, z),
                (x, z + 1),
                (x, z - 1)
            };
        }
    }
}
