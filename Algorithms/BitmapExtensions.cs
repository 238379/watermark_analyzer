using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Algorithms
{
    public static class BitmapExtensions
    {
        public static void RunOnEveryPixel(this Bitmap that, Action<int, int> action)
        {
            for (int i = 0; i < that.Width; i++)
            {
                for (int j = 0; j < that.Height; j++)
                {
                    action(i, j);
                }
            }
        }

        public static byte[,,] Rgb2Ycbcr(Bitmap bmp)
        {
            byte[,,] arrayycbcr = new byte[bmp.Height, bmp.Width, 3];

            bmp.RunOnEveryPixel((i, j) =>
            {
                byte[] YCbCr = RGB2YCbCr(bmp.GetPixel(i, j).R, bmp.GetPixel(i, j).G, bmp.GetPixel(i, j).B);
                arrayycbcr[i, j, 0] = YCbCr[0];
                arrayycbcr[i, j, 1] = YCbCr[1];
                arrayycbcr[i, j, 2] = YCbCr[2];
            });
            return arrayycbcr;
        }

        public static void Ycbcr2Rgb(this Bitmap that, byte[,,] arrayycbcr)
        {
            that.RunOnEveryPixel((i, j) =>
            {
                that.SetPixel(i, j, ColorFromYCbCr(arrayycbcr[i, j, 0], arrayycbcr[i, j, 1], arrayycbcr[i, j, 2]));
            });
        }

        public static Color ColorFromYCbCr(int Y, int Cb, int Cr)
        {
            int R = (int)Math.Max(0, Math.Min(255, (float)(Y + 1.403 * (Cr - 128))));
            int G = (int)Math.Max(0, Math.Min(255, (float)(Y - 0.344 * (Cb - 128) - 0.714 * (Cr - 128))));
            int B = (int)Math.Max(0, Math.Min(255, (float)(Y + 1.773 * (Cb - 128))));

            return Color.FromArgb(R, G, B);
        }

        private static byte[] RGB2YCbCr(int R, int G, int B)
        {
            var Y = (byte)((0.299 * R) + (0.587 * G) + (0.114 * B));
            var Cb = (byte)(-(0.169 * R) - (0.331 * G) + (0.5 * B) + 128);
            var Cr = (byte)((0.5 * R) - (0.419 * G) - (0.081 * B) + 128);
            return new byte[3] { Y, Cb, Cr };
        }
    }
}
