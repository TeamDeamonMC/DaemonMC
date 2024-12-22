using DaemonMC.Network;

namespace DaemonMC
{
    public static class ToDataTypes
    {
        public static byte[] GetByteSum(byte[] byte1, byte[] byte2)
        {
            var result = new byte[byte1.Length + byte2.Length];
            Buffer.BlockCopy(byte1, 0, result, 0, byte1.Length);
            Buffer.BlockCopy(byte2, 0, result, byte1.Length, byte2.Length);
            return result;
        }

        public static byte[] WriteVarint(int value)
        {
            List<byte> bytes = new List<byte>();
            while ((value & -128) != 0)
            {
                bytes.Add((byte)((value & 127) | 128));
                value >>= 7;
            }
            bytes.Add((byte)(value & 127));
            return bytes.ToArray();
        }

        public static int ReadVarInt(byte[] buffer)
        {
            int value = 0;
            int size = 0;

            while (true)
            {
                byte currentByte = buffer[size];
                value |= (currentByte & 0x7F) << (size * 7);

                if ((currentByte & 0x80) == 0)
                {
                    break;
                }

                size++;
            }

            return value;
        }

        public static uint ReadUInt32(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.Default, true))
            {
                return reader.ReadUInt32();
            }
        }

        public static void WriteUInt32(Stream stream, uint value)
        {
            using (BinaryWriter writer = new BinaryWriter(stream, System.Text.Encoding.Default, true))
            {
                writer.Write(value);
            }
        }
    }
}
