using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Algorithms
{
	public class PixelAveragingParameters : AlgorithmParameters
	{
		public readonly double Ratio;

		public PixelAveragingParameters(EffectiveBitmap original, EffectiveBitmap watermark, EffectiveBitmap watermarked, double ratio) : base(original, watermark, watermarked)
		{
			Ratio = ratio;
		}
	}

	public class PixelAveraging : Algorithm
	{
		public const string ALGORITHM_NAME = "PixelAveraging";

		private readonly PixelAveragingParameters parameters;


		public PixelAveraging(PixelAveragingParameters parameters) : base()
		{
			this.parameters = parameters;
		}

		public override Task<AlgorithmResult> AddWatermark()
		{
			var watermarked = Watermark(parameters.Original, parameters.Watermark);
			var cleaned = CleanWatermark(watermarked, parameters.Watermark);
			var extracted = ExtractWatermark(watermarked, parameters.Original);
			return Task.FromResult(new AlgorithmResult(("Watermarked", watermarked), ("Cleaned", cleaned), ("Extracted watermark", extracted)));
		}

		public override Task<AlgorithmResult> RemoveWatermark()
		{
			throw new NotImplementedException();
		}

		private EffectiveBitmap Watermark(EffectiveBitmap original, EffectiveBitmap watermark)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var originalPixel = sources[0].GetPixel(i, j);
				var watermarkPixel = sources[1].GetPixel(i, j);

				var r = (int)(originalPixel.R + (watermarkPixel.R * parameters.Ratio)) / 2;
				var g = (int)(originalPixel.G + (watermarkPixel.G * parameters.Ratio)) / 2;
				var b = (int)(originalPixel.B + (watermarkPixel.B * parameters.Ratio)) / 2;

				return new PixelInfo(r, g, b);
			}, original, watermark);
		}

		private EffectiveBitmap CleanWatermark(EffectiveBitmap imgWatermarked, EffectiveBitmap watermark)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkedImgPixel = sources[0].GetPixel(i, j);
				var watermarkPixel = sources[1].GetPixel(i, j);

				var r = (int)(watermarkedImgPixel.R * 2 - watermarkPixel.R * parameters.Ratio);
				var g = (int)(watermarkedImgPixel.G * 2 - watermarkPixel.G * parameters.Ratio);
				var b = (int)(watermarkedImgPixel.B * 2 - watermarkPixel.B * parameters.Ratio);

				r = r < 0 ? r + 1 : r;
				g = g < 0 ? g + 1 : g;
				b = b < 0 ? b + 1 : b;

				return new PixelInfo(r, g, b);
			}, imgWatermarked, watermark);
		}

		private EffectiveBitmap ExtractWatermark(EffectiveBitmap imgWatermarked, EffectiveBitmap original)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkedImgPixel = sources[0].GetPixel(i, j);
				var originalPixel = sources[1].GetPixel(i, j);

				var r = (int)((2 * watermarkedImgPixel.R - originalPixel.R) * (1 / parameters.Ratio));
				var g = (int)((2 * watermarkedImgPixel.G - originalPixel.G) * (1 / parameters.Ratio));
				var b = (int)((2 * watermarkedImgPixel.B - originalPixel.B) * (1 / parameters.Ratio));

				r = r < 0 ? 0 : r;
				g = g < 0 ? 0 : g;
				b = b < 0 ? 0 : b;

				return new PixelInfo(r, g, b);
			}, imgWatermarked, original);
		}
	}
}
