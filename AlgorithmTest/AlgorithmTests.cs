using FluentAssertions;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace AlgorithmTest
{
	public class AlgorithmTests
	{
        public static readonly string runningPath = AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string resourcesPath = Path.GetFullPath(Path.Combine(runningPath, @"..\..\..\", @"Resources\"));

        protected void AssertBitmapsAreEqual(Bitmap bitmap1, Color[,] bitmap2)
		{
			bitmap1.Width.Should().Be(bitmap2.GetLength(1));
			bitmap1.Height.Should().Be(bitmap2.GetLength(0));
			for (int i = 0; i < bitmap1.Width; i++)
			{
				for (int j = 0; j < bitmap1.Height; j++)
				{
					bitmap1.GetPixel(i, j).Should().Be(bitmap2[i, j]);
				}
			}
		}
        protected bool CompareBitmaps(Bitmap bmp1, Bitmap bmp2)
        {
            if (bmp1 == null || bmp2 == null)
                return false;
            if (Equals(bmp1, bmp2))
                return true;
            if (!bmp1.Size.Equals(bmp2.Size) || !bmp1.PixelFormat.Equals(bmp2.PixelFormat))
                return false;

            int bytes = bmp1.Width * bmp1.Height * (Image.GetPixelFormatSize(bmp1.PixelFormat) / 8);

            bool result = true;
            byte[] b1bytes = new byte[bytes];
            byte[] b2bytes = new byte[bytes];

            BitmapData bitmapData1 = bmp1.LockBits(new Rectangle(0, 0, bmp1.Width, bmp1.Height), ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bitmapData2 = bmp2.LockBits(new Rectangle(0, 0, bmp2.Width, bmp2.Height), ImageLockMode.ReadOnly, bmp2.PixelFormat);

            Marshal.Copy(bitmapData1.Scan0, b1bytes, 0, bytes);
            Marshal.Copy(bitmapData2.Scan0, b2bytes, 0, bytes);

            for (int n = 0; n <= bytes - 1; n++)
            {
                var diff = Math.Abs(b1bytes[n] - b2bytes[n]);
                if (diff > 32)
                {
                    result = false;
                    Console.WriteLine($"{b1bytes[n]} vs {b2bytes[n]}");
                    break;
                }
            }

            bmp1.UnlockBits(bitmapData1);
            bmp2.UnlockBits(bitmapData2);

            return result;
        }
    }
}
