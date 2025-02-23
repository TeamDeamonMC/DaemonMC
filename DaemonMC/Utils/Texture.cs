using SkiaSharp;

namespace DaemonMC.Utils
{
    public class Texture
    {
        public static byte[] PngToBytes(string filename)
        {
            using (SKBitmap bitmap = SKBitmap.Decode(filename))
            {
                byte[] bytes = new byte[bitmap.Width * bitmap.Height * 4];

                int i = 0;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        SKColor color = bitmap.GetPixel(x, y);
                        bytes[i++] = color.Red;
                        bytes[i++] = color.Green;
                        bytes[i++] = color.Blue;
                        bytes[i++] = color.Alpha;
                    }
                }

                return bytes;
            }
        }
    }
}
