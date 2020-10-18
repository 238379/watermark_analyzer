using System;
using System.Drawing;

namespace Algorithms
{
	public class Lsb
	{
		// TODO use these width and height, scale if necessary
		private readonly int width;
		private readonly int height;
		private readonly byte watermarkThreshold;
		private readonly Random random;

		public Lsb(int width, int height, double watermarkThreshold)
		{
			this.width = width;
			this.height = height;
			this.watermarkThreshold = (byte)(byte.MaxValue * watermarkThreshold);
			this.random = new Random();
		}

		public Bitmap Watermark(Bitmap original, Bitmap watermark)
		{
			var watermarkedBitmap = new Bitmap(original.Width, original.Height);
			var simplifiedWatermark = PreprocessToSimplifiedWatermark(watermark);

			original.RunOnEveryPixel((i, j) =>
			{
				var pixel = original.GetPixel(i, j);
				var watermarkPixel = simplifiedWatermark.GetPixel(i, j);

				var r = pixel.R - pixel.R % 2 + watermarkPixel.R;
				var g = pixel.G - pixel.G % 2 + watermarkPixel.G;
				var b = pixel.B - pixel.B % 2 + watermarkPixel.B;

				watermarkedBitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
			});

			return watermarkedBitmap;
		}

		public Bitmap CleanWatermark(Bitmap watermarked)
		{
			var newBitmap = new Bitmap(watermarked.Width, watermarked.Height);

			watermarked.RunOnEveryPixel((i, j) =>
			{
				var pixel = watermarked.GetPixel(i, j);

				// TODO better method instead of random bit on last position
				var r = pixel.R - pixel.R % 2 + CreateRandomBit();
				var g = pixel.G - pixel.G % 2 + CreateRandomBit();
				var b = pixel.B - pixel.B % 2 + CreateRandomBit();

				newBitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
			});

			return newBitmap;
		}

		private Bitmap PreprocessToSimplifiedWatermark(Bitmap watermark)
		{
			Bitmap simplifiedBitmap = new Bitmap(watermark.Width, watermark.Height);

			watermark.RunOnEveryPixel((i, j) =>
			{
				var pixel = watermark.GetPixel(i, j);

				byte r = 0, g = 0, b = 0;
				if (pixel.R <= watermarkThreshold)
				{
					r = 1;
				}
				if(pixel.G <= watermarkThreshold)
				{
					g = 1;
				}
				if(pixel.B <= watermarkThreshold)
				{
					b = 1;
				}
				simplifiedBitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
			});

			return simplifiedBitmap;
		}

		private byte CreateRandomBit()
		{
			return (byte)random.Next(0, 2);
		}
	}
}
