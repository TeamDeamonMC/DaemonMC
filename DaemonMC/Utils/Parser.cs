using System.Numerics;

namespace DaemonMC.Utils
{
    internal class Parser
    {
        public static bool Vector3(string[] args, out Vector3 result)
        {
            result = new Vector3();

            if (Vector3(args, 0, out Vector3 data))
            {
                result = data;
                return true;
            }
            return false;
        }

        public static bool Vector3(string[] args, int index, out Vector3 result)
        {
            result = new Vector3();

            if (index + 2 >= args.Length) { return false; }

            if (float.TryParse(args[index], out float x) && float.TryParse(args[index + 1], out float y) && float.TryParse(args[index + 2], out float z))
            {
                result = new Vector3(x, y, z);
                return true;
            }
            return false;
        }
    }
}
