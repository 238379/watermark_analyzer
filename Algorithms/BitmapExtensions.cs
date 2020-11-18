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
                byte[] YCbCr = YCbCrFromRGB(bmp.GetPixel(i, j).R, bmp.GetPixel(i, j).G, bmp.GetPixel(i, j).B);
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

        private static Color ColorFromYCbCr(int y, int cb, int cr)
        {
            double Y = y;
            double Cb = cb;
            double Cr = cr;

            int R = (int)(Y + 1.40200 * (Cr - 0x80));
            int G = (int)(Y - 0.34414 * (Cb - 0x80) - 0.71414 * (Cr - 0x80));
            int B = (int)(Y + 1.77200 * (Cb - 0x80));

            R = Math.Max(0, Math.Min(255, R));
            G = Math.Max(0, Math.Min(255, G));
            B = Math.Max(0, Math.Min(255, B));

            return Color.FromArgb(R, G, B);
        }

        private static byte[] YCbCrFromRGB(int R, int G, int B)
        {
            var Y = (byte)((0.257 * R) + (0.504 * G) + (0.098 * B) + 16);
            var Cb = (byte)(-(0.148 * R) - (0.291 * G) + (0.439 * B) + 128);
            var Cr = (byte)((0.439 * R) - (0.368 * G) - (0.071 * B) + 128);

            return new byte[3] { Y, Cb, Cr };
        }
    }
}
