using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Algorithms
{
	public class Dft : Algorithm
	{
		public const string ALGORITHM_NAME = "DFT";

		public const string BITS_PARAM = "BITS_PARAM";

		private readonly int bitsForWatermark;
		private readonly double alpha;
		private readonly Random random;

		public Dft(Dictionary<string, dynamic> parameters) : base(parameters)
		{
			this.bitsForWatermark = parameters[BITS_PARAM];
			this.alpha = 0.1;
			this.random = new Random();
		}

		public Bitmap Watermark(Bitmap original, Bitmap watermark)
		{
			var watermarkedBitmap = new Bitmap(original.Width, original.Height);
			byte[,,] ycbcr = BitmapExtensions.Rgb2Ycbcr(original);

			watermarkedBitmap.RunOnEveryPixel((i, j) =>
			{
				watermarkedBitmap.SetPixel(i, j, Color.FromArgb(ycbcr[i, j, 0], ycbcr[i, j, 0], ycbcr[i, j, 0]));
			});

			ComplexImage complexImage = ComplexImage.FromBitmap(watermarkedBitmap);
            complexImage.ForwardFourierTransform();
            complexImage.BackwardFourierTransform();
            watermarkedBitmap = complexImage.ToBitmap();

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
			var divider = (byte)(Math.Ceiling(255 / Math.Pow(2, bitsForWatermark)) + 0.5);

			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkPixel = sources[0].GetPixel(i, j);

				byte r = (byte)(watermarkPixel.R / divider);
				byte g = (byte)(watermarkPixel.G / divider);
				byte b = (byte)(watermarkPixel.B / divider);

				return new PixelInfo(r, g, b);
			}, watermark);
		}

		public Bitmap ExtractWatermark(Bitmap watermarked)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkedImgPixel = sources[0].GetPixel(i, j);

				var divider = (int)Math.Pow(2, bitsForWatermark);
				var multiplier = 255 / (divider - 1);
				byte r = (byte)(watermarkedImgPixel.R % divider * multiplier);
				byte g = (byte)(watermarkedImgPixel.G % divider * multiplier);
				byte b = (byte)(watermarkedImgPixel.B % divider * multiplier);

				return new PixelInfo(r, g, b);
			}, watermarked);
		}

		private byte CreateRandomBit()
		{
			return (byte)random.Next(0, 2);
		}

        public override AlgorithmResult Run(Bitmap original, Bitmap watermark)
        {
			var watermarked = Watermark(original, watermark);
			var cleaned = CleanWatermark(watermarked);
			var extracted = ExtractWatermark(watermarked);
			return new AlgorithmResult(watermarked, cleaned, extracted);
		}
    }
}
