using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Algorithms.common
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

        public static EffectiveBitmap TransformToEffectiveBitmap(this Bitmap that)
        {
            var data = that.LockAllBitsReadOnly();
            var depth = that.GetDepth();
            var buffer = new byte[that.Width * that.Height * depth];

            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            that.UnlockBits(data);

            return new EffectiveBitmap(that.Width, that.Height, depth, buffer);
        }

        public static int GetDepth(this Bitmap that)
        {
            return Image.GetPixelFormatSize(that.PixelFormat) / 8;
        }

        public static BitmapData LockAllBitsReadOnly(this Bitmap that)
        {
            var rect = new Rectangle(0, 0, that.Width, that.Height);
            return that.LockBits(rect, ImageLockMode.ReadOnly, that.PixelFormat);
        }

        public static BitmapData LockAllBitsReadWrite(this Bitmap that)
        {
            var rect = new Rectangle(0, 0, that.Width, that.Height);
            return that.LockBits(rect, ImageLockMode.ReadWrite, that.PixelFormat);
        }

        public static Bitmap Resize(this Bitmap that, Size size)
        {
            return Resize(that, size.Width, size.Height);
        }

		public static Bitmap Resize(this Bitmap that, int width, int height)
		{
			if (that.Width == width && that.Height == height)
				return that;

			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

            destImage.SetResolution(that.HorizontalResolution, that.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(that, destRect, 0, 0, that.Width, that.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}