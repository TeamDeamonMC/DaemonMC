using System.Drawing;

namespace DaemonMC.Utils
{
    public class Texture
    {
        public static byte[] PngToBytes(string filename)
        {
            using (Bitmap bitmap = new Bitmap(filename))
            {
                int size = bitmap.Height * bitmap.Width * 4;

                byte[] bytes = new byte[size];

                int i = 0;
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color color = bitmap.GetPixel(x, y);
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
