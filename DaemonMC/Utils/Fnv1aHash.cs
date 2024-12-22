namespace DaemonMC.Utils
{
    public class Fnv1aHash
    {
        private const int FNV1_32_INIT = unchecked((int)0x811c9dc5);
        private const int FNV1_PRIME_32 = 0x01000193;

        public static int Hash32(byte[] data)
        {
            int hash = FNV1_32_INIT;
            foreach (var b in data)
            {
                hash ^= b;
                hash *= FNV1_PRIME_32;
            }
            return hash;
        }
    }
}
