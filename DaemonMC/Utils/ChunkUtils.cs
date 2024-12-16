using System.Net.Sockets;

namespace DaemonMC.Utils
{
    public class ChunkUtils
    {
        public static List<(int x, int z)> GetSequence(int radius, int posx, int posz)
        {
            var positions = new List<(int x, int z)>();
            int x = (int)Math.Floor(posx / 16.0);
            int z = (int)Math.Floor(posz / 16.0);
            int dx = 0, dz = -1;

            for (int i = 0; i < (2 * radius + 1) * (2 * radius + 1); i++)
            {
                if (Math.Abs(x) <= radius && Math.Abs(z) <= radius)
                {
                    positions.Add((x, z));
                }

                if (x == z || (x < 0 && x == -z) || (x > 0 && x == 1 - z))
                {
                    int temp = dx;
                    dx = -dz;
                    dz = temp;
                }

                x += dx;
                z += dz;
            }

            return positions;
        }
    }
}
