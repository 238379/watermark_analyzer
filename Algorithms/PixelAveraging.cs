using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Algorithms
{
	public class PixelAveraging
	{
		private readonly double ratio;

		public PixelAveraging(double ratio)
		{
			this.ratio = ratio;
		}

		public Bitmap Watermark(Bitmap original, Bitmap watermark)
		{
			Bitmap watermarkedBitmap = new Bitmap(original.Width, original.Height);
			original.RunOnEveryPixel((i, j) =>
			{
				var pixel = original.GetPixel(i, j);
				var watermarkPixel = watermark.GetPixel(i, j);

				var r = (int)(pixel.R + (watermarkPixel.R * ratio)) / 2;
				var g = (int)(pixel.G + (watermarkPixel.G * ratio)) / 2;
				var b = (int)(pixel.B + (watermarkPixel.B * ratio)) / 2;

				watermarkedBitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
			});

			return watermarkedBitmap;
		}

		public Bitmap CleanWatermark(Bitmap imgWatermarked, Bitmap watermark)
		{
			Bitmap newBitmap = new Bitmap(imgWatermarked.Width, imgWatermarked.Height);

			imgWatermarked.RunOnEveryPixel((i, j) =>
			{
				var imgPixel = imgWatermarked.GetPixel(i, j);
				var watermarkPixel = watermark.GetPixel(i, j);

				var r = (int)(imgPixel.R * 2 - watermarkPixel.R * ratio);
				var g = (int)(imgPixel.G * 2 - watermarkPixel.G * ratio);
				var b = (int)(imgPixel.B * 2 - watermarkPixel.B * ratio);

				r = r < 0 ? r + 1 : r;
				g = g < 0 ? g + 1 : g;
				b = b < 0 ? b + 1 : b;

				newBitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
			});

			return newBitmap;
		}

		public Bitmap ExtractWatermark(Bitmap imgWatermarked, Bitmap original)
		{
			Bitmap newBitmap = new Bitmap(imgWatermarked.Width, imgWatermarked.Height);

			imgWatermarked.RunOnEveryPixel((i, j) =>
			{
				var imgPixel = imgWatermarked.GetPixel(i, j);
				var originalPixel = original.GetPixel(i, j);

				var r = (int)((2 * imgPixel.R - originalPixel.R) * (1 / ratio));
				var g = (int)((2 * imgPixel.G - originalPixel.G) * (1 / ratio));
				var b = (int)((2 * imgPixel.B - originalPixel.B) * (1 / ratio));

				r = r < 0 ? 0 : r;
				g = g < 0 ? 0 : g;
				b = b < 0 ? 0 : b;

				newBitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
			});

			return newBitmap;
		}
	}
}
