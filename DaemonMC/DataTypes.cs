using System.Numerics;
using System.Text;
using DaemonMC.Network;
using fNbt;

namespace DaemonMC
{
    public class IPAddressInfo
    {
        public byte[] IPAddress { get; set; }
        public ushort Port { get; set; }
    }

    public static class DataTypes
    {
        public static void HexDump(byte[] buffer, int lenght)
        {
            for (int i = 0; i < lenght; i++)
            {
                Console.Write(buffer[i].ToString("X2") + " ");
                if ((i + 1) % 16 == 0)
                {
                    Console.WriteLine();
                }
            }
            Console.WriteLine();
        }
    }
}
