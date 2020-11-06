using System;
using System.Drawing;

namespace Algorithms
{
	public class Lsb
	{
		// TODO use these width and height, scale if necessary
		private readonly int width;
		private readonly int height;
		private readonly int bitsForWatermark;
		private readonly Random random;

		public Lsb(int width, int height, int bitsForWatermark)
		{
			this.width = width;
			this.height = height;
			this.bitsForWatermark = bitsForWatermark;
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

				var divider = (int)Math.Pow(2, bitsForWatermark);
				var r = Math.Clamp(pixel.R - pixel.R % divider + watermarkPixel.R, 0, 255);
				var g = Math.Clamp(pixel.G - pixel.G % divider + watermarkPixel.G, 0, 255);
				var b = Math.Clamp(pixel.B - pixel.B % divider + watermarkPixel.B, 0, 255);

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

				var divider = (int)Math.Pow(2, bitsForWatermark);
				// TODO better method instead of random bit on last position
				var r = pixel.R - pixel.R % divider + CreateRandomNumber(divider);
				var g = pixel.G - pixel.G % divider + CreateRandomNumber(divider);
				var b = pixel.B - pixel.B % divider + CreateRandomNumber(divider);

				newBitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
			});

			return newBitmap;
		}

		public Bitmap ExtractWatermark(Bitmap watermarked)
		{
			var newBitmap = new Bitmap(watermarked.Width, watermarked.Height);

			watermarked.RunOnEveryPixel((i, j) =>
			{
				var pixel = watermarked.GetPixel(i, j);

				var divider = (int)Math.Pow(2, bitsForWatermark);
				var multiplier = 255 / (divider - 1);
				byte r = (byte)(pixel.R % divider * multiplier);
				byte g = (byte)(pixel.G % divider * multiplier);
				byte b = (byte)(pixel.B % divider * multiplier);

				newBitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
			});

			return newBitmap;
		}

		private Bitmap PreprocessToSimplifiedWatermark(Bitmap watermark)
		{
			Bitmap simplifiedBitmap = new Bitmap(watermark.Width, watermark.Height);

			var divider = (byte)(Math.Ceiling(255 / Math.Pow(2, bitsForWatermark)) + 0.5);

			watermark.RunOnEveryPixel((i, j) =>
			{
				var pixel = watermark.GetPixel(i, j);

				byte r = (byte)(pixel.R / divider);
				byte g = (byte)(pixel.G / divider);
				byte b = (byte)(pixel.B / divider);

				simplifiedBitmap.SetPixel(i, j, Color.FromArgb(r, g, b));
			});
			return simplifiedBitmap;
		}

		private byte CreateRandomNumber(int upperExcluded)
		{
			return (byte)random.Next(0, upperExcluded);
		}
	}
}
