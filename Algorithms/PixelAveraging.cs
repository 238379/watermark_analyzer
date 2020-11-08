using Algorithms.common;
using System.Collections.Generic;
using System.Drawing;

namespace Algorithms
{
	public class PixelAveraging : Algorithm
	{
		public const string ALGORITHM_NAME = "PixelAveraging";

		public const string RATIO_PARAM = "RATIO_PARAM";

		private readonly double ratio;

		public PixelAveraging(Dictionary<string, dynamic> parameters) : base(parameters)
		{
			this.ratio = parameters[RATIO_PARAM];
		}

		public override AlgorithmResult Run(Bitmap original, Bitmap watermark)
		{
			var watermarked = Watermark(original, watermark);
			var cleaned = CleanWatermark(watermarked, watermark);
			var extracted = ExtractWatermark(watermarked, original);
			return new AlgorithmResult(watermarked, cleaned, extracted);
		}

		private Bitmap Watermark(Bitmap original, Bitmap watermark)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var originalPixel = sources[0].GetPixel(i, j);
				var watermarkPixel = sources[1].GetPixel(i, j);

				var r = (int)(originalPixel.R + (watermarkPixel.R * ratio)) / 2;
				var g = (int)(originalPixel.G + (watermarkPixel.G * ratio)) / 2;
				var b = (int)(originalPixel.B + (watermarkPixel.B * ratio)) / 2;

				return new PixelInfo(r, g, b);
			}, original, watermark);
		}

		private Bitmap CleanWatermark(Bitmap imgWatermarked, Bitmap watermark)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkedImgPixel = sources[0].GetPixel(i, j);
				var watermarkPixel = sources[1].GetPixel(i, j);

				var r = (int)(watermarkedImgPixel.R * 2 - watermarkPixel.R * ratio);
				var g = (int)(watermarkedImgPixel.G * 2 - watermarkPixel.G * ratio);
				var b = (int)(watermarkedImgPixel.B * 2 - watermarkPixel.B * ratio);

				r = r < 0 ? r + 1 : r;
				g = g < 0 ? g + 1 : g;
				b = b < 0 ? b + 1 : b;

				return new PixelInfo(r, g, b);
			}, imgWatermarked, watermark);
		}

		private Bitmap ExtractWatermark(Bitmap imgWatermarked, Bitmap original)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkedImgPixel = sources[0].GetPixel(i, j);
				var originalPixel = sources[1].GetPixel(i, j);

				var r = (int)((2 * watermarkedImgPixel.R - originalPixel.R) * (1 / ratio));
				var g = (int)((2 * watermarkedImgPixel.G - originalPixel.G) * (1 / ratio));
				var b = (int)((2 * watermarkedImgPixel.B - originalPixel.B) * (1 / ratio));

				r = r < 0 ? 0 : r;
				g = g < 0 ? 0 : g;
				b = b < 0 ? 0 : b;

				return new PixelInfo(r, g, b);
			}, imgWatermarked, original);
		}
	}
}
