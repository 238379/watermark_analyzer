using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Algorithms
{
	public class PixelAveragingParameters : AlgorithmParameters
	{
		public readonly decimal Ratio;

		public PixelAveragingParameters(EffectiveBitmap original, EffectiveBitmap watermark, EffectiveBitmap watermarked, decimal ratio) : base(original, watermark, watermarked)
		{
			Ratio = ratio;
		}

		public override string ToString()
		{
			return "{" + $"Ratio={Ratio}" + "}";
		}
	}

	public class PixelAveraging : Algorithm
	{
		public const string ALGORITHM_NAME = "PixelAveraging";

		private readonly PixelAveragingParameters parameters;

		public override string ToString() => "PixelAveraging " + parameters;

		public PixelAveraging(PixelAveragingParameters parameters) : base(ALGORITHM_NAME, parameters)
		{
			this.parameters = parameters;
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> AddWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();
			var watermarked = Watermark(parameters.Original, parameters.Watermark);

			yield return new AlgorithmResultElement("Watermarked", watermarked, new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();
			var cleaned = CleanWatermark(watermarked, parameters.Watermark);

			yield return new AlgorithmResultElement("Cleaned", cleaned, new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();
			var extracted = ExtractWatermark(watermarked, parameters.Original);

			yield return new AlgorithmResultElement("Extracted watermark", extracted, new ResultDescription(ToString()));
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> RemoveWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			throw new NotImplementedException();
			yield return null;
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
