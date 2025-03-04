using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace DaemonMC.Utils
{
    public class Texture
    {
        public static byte[] PngToBytes(string filename)
        {
            using (Image<Rgba32> bitmap = Image.Load<Rgba32>(filename))
            {
                int size = bitmap.Width * bitmap.Height * 4;
                byte[] bytes = new byte[size];

                int i = 0;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Rgba32 color = bitmap[x, y];
                        bytes[i++] = color.R;
                        bytes[i++] = color.G;
                        bytes[i++] = color.B;
                        bytes[i++] = color.A;
                    }
                }

                return bytes;
            }
        }
    }
}
