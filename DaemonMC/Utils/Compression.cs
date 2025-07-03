using System.IO.Compression;
using Snappier;

namespace DaemonMC.Utils
{
    public class Compression
    {
        public static byte[] DecompressZLib(byte[] compressedData)
        {
            using (var input = new MemoryStream(compressedData))
            using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                deflate.CopyTo(output);
                return output.ToArray();
            }
        }

        public static byte[] CompressZLib(byte[] input)
        {
            using var output = new MemoryStream();
            using (var deflater = new DeflateStream(output, CompressionLevel.Fastest))
            {
                deflater.Write(input, 0, input.Length);
            }
            return output.ToArray();
        }

        public static byte[] DecompressSnappy(byte[] compressed)
        {
            return Snappy.DecompressToArray(compressed);
        }

        public static byte[] CompressSnappy(byte[] input)
        {
            return Snappy.CompressToArray(input);
        }
    }

    public enum CompressionTypes
    {
        ZLib,
        Snappy,
        None = 255
    }
}
