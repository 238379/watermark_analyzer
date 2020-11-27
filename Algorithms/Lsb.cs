using Algorithms.common;
using System;
using System.Drawing;

namespace Algorithms
{
	public class LsbParameters : AlgorithmParameters
	{
		public readonly int BitsForWatermark;

		public LsbParameters(Bitmap original, Bitmap watermark, int bitsForWatermark) : base(original, watermark)
		{
			BitsForWatermark = bitsForWatermark;
		}
	}

	public class Lsb : Algorithm
	{
		public const string ALGORITHM_NAME = "LSB";

		private readonly LsbParameters parameters;

		// TODO width and height
		private readonly Random random = new Random();

		public Lsb(LsbParameters parameters) : base()
		{
			this.parameters = parameters;
		}

		public override AlgorithmResult AddWatermark()
		{
			var watermarked = Watermark(parameters.Original, parameters.Watermark);
			var cleaned = CleanWatermark(watermarked);
			var extracted = ExtractWatermark(watermarked);
			return new AlgorithmResult(("Watermarked", watermarked), ("Cleaned", cleaned), ("Extracted watermark", extracted));
		}

		public override AlgorithmResult RemoveWatermark()
		{
			throw new NotImplementedException();
		}

		private Bitmap Watermark(Bitmap original, Bitmap watermark)
		{
			var simplifiedWatermark = PreprocessToSimplifiedWatermark(watermark);

			return BitmapOperations.Create((sources, i, j) =>
			{
				var originalPixel = sources[0].GetPixel(i, j);
				var watermarkPixel = sources[1].GetPixel(i, j);

				var divider = (int)Math.Pow(2, parameters.BitsForWatermark);
				var r = (byte)(Math.Clamp(originalPixel.R - originalPixel.R % divider + watermarkPixel.R, 0, 255));
				var g = (byte)(Math.Clamp(originalPixel.G - originalPixel.G % divider + watermarkPixel.G, 0, 255));
				var b = (byte)(Math.Clamp(originalPixel.B - originalPixel.B % divider + watermarkPixel.B, 0, 255));

				return new PixelInfo(r, g, b);
			}, original, simplifiedWatermark);
		}

		private Bitmap CleanWatermark(Bitmap watermarked)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkedImgPixel = sources[0].GetPixel(i, j);

				var divider = (int)Math.Pow(2, parameters.BitsForWatermark);
				// TODO better method instead of random bit on last position
				var r = (byte)(watermarkedImgPixel.R - watermarkedImgPixel.R % divider + CreateRandomNumber(divider));
				var g = (byte)(watermarkedImgPixel.G - watermarkedImgPixel.G % divider + CreateRandomNumber(divider));
				var b = (byte)(watermarkedImgPixel.B - watermarkedImgPixel.B % divider + CreateRandomNumber(divider));

				return new PixelInfo(r, g, b);
			}, watermarked);
		}

		private Bitmap ExtractWatermark(Bitmap watermarked)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkedImgPixel = sources[0].GetPixel(i, j);

				var divider = (int)Math.Pow(2, parameters.BitsForWatermark);
				var multiplier = 255 / (divider - 1);
				byte r = (byte)(watermarkedImgPixel.R % divider * multiplier);
				byte g = (byte)(watermarkedImgPixel.G % divider * multiplier);
				byte b = (byte)(watermarkedImgPixel.B % divider * multiplier);

				return new PixelInfo(r, g, b);
			}, watermarked);
		}

		private Bitmap PreprocessToSimplifiedWatermark(Bitmap watermark)
		{
			var divider = (byte)(Math.Ceiling(255 / Math.Pow(2, parameters.BitsForWatermark)) + 0.5);

			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkPixel = sources[0].GetPixel(i, j);

				byte r = (byte)(watermarkPixel.R / divider);
				byte g = (byte)(watermarkPixel.G / divider);
				byte b = (byte)(watermarkPixel.B / divider);

				return new PixelInfo(r, g, b);
			}, watermark);
		}

		private byte CreateRandomNumber(int upperExcluded)
		{
			return (byte)random.Next(0, upperExcluded);
		}
	}
}
