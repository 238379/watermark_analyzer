using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Algorithms
{
	public class DctParameters : AlgorithmParameters
	{
		public readonly int Key;
		public readonly double Alpha;
		public readonly decimal AlphaM;

		public DctParameters(EffectiveBitmap original, EffectiveBitmap watermark, EffectiveBitmap watermarked, int key, decimal alpha) : base(original, watermark, watermarked)
		{
			Key = key;
			Alpha = (double)alpha;
			AlphaM = alpha;
		}

		public override string ToString()
		{
			return "{" + $"Key={Key}, Alpha={AlphaM}" + "}";
		}
	}

	public class Dct : Algorithm
	{
		public const string ALGORITHM_NAME = "DCT";

		private readonly DctParameters parameters;

		private ComplexImage complexImage;
		private ComplexImage complexWatermark;

		public override string ToString() => "DCT " + parameters;

		public Dct(DctParameters parameters) : base(ALGORITHM_NAME, parameters)
		{
			this.parameters = parameters;
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> AddWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			int size = 0;
			if (Math.Max(parameters.Original.Width, parameters.Original.Height) > 512)
			{
				size = Math.Max(parameters.Original.Width, parameters.Original.Height) / 2;
			}
			complexImage = ComplexImage.FromBitmap(parameters.Original, size);
			complexWatermark = ComplexImage.FromBitmap(parameters.Watermark, complexImage.Width);

			ct.ThrowIfCancellationRequested();

			complexImage.DCT();
			var cosineTransform = complexImage.ToEffectiveBitmapCosine();

			yield return new AlgorithmResultElement("Cosine transform (DCT)", cosineTransform.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();

			complexWatermark.DCT();

			ct.ThrowIfCancellationRequested();

			complexImage.EmbedImageCosine(complexWatermark, parameters.Key, parameters.Alpha);
			var cosineWatermarked = complexImage.ToEffectiveBitmapCosine();

			yield return new AlgorithmResultElement("DCT + watermark", cosineWatermarked.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();

			complexImage.IDCT();
			var watermarked = complexImage.ToEffectiveBitmapCosine();

			yield return new AlgorithmResultElement("Watermarked", watermarked.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> RemoveWatermark([EnumeratorCancellation] CancellationToken ct)
		{

			int size = 0;
			if (Math.Max(parameters.Watermarked.Width, parameters.Watermarked.Height) > 512)
			{
				size = Math.Max(parameters.Watermarked.Width, parameters.Watermarked.Height) / 2;
			}
			ComplexImage complexWatermarked = ComplexImage.FromBitmap(parameters.Watermarked, size);

			ct.ThrowIfCancellationRequested();

			complexWatermarked.DCT();

			ct.ThrowIfCancellationRequested();

			ComplexImage complexOriginal = ComplexImage.FromBitmap(parameters.Original, complexWatermarked.Width);
			complexOriginal.DCT();

			ct.ThrowIfCancellationRequested();

			int[] v = new int[complexWatermarked.Width];
			double[] vAlpha = new double[complexWatermarked.Width];
			Random random = new Random(parameters.Key);

			ct.ThrowIfCancellationRequested();

			for (int i = 0; i < complexWatermarked.Width; i++)
			{
				v[i] = random.Next(0, 2);
				vAlpha[i] = v[i] * parameters.Alpha;
			}

			ct.ThrowIfCancellationRequested();

			for (int y = 0; y < complexWatermarked.Height; y++)
			{
				for (int x = 0; x < complexWatermarked.Width; x++)
				{
					if (vAlpha[x] == 0)
					{
						complexWatermarked.DCTData[y, x] = 0;
					}
					else
					{
						complexWatermarked.DCTData[y, x] = (complexWatermarked.DCTData[y, x] - complexOriginal.DCTData[y, x]) / vAlpha[x];
					}
				}
			}

			ct.ThrowIfCancellationRequested();

			yield return new AlgorithmResultElement("Extracted watermark DCT", complexWatermarked.ToEffectiveBitmapCosine(true).ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));

			complexWatermarked.IDCT();

			ct.ThrowIfCancellationRequested();

			yield return new AlgorithmResultElement("Extracted watermark", complexWatermarked.ToEffectiveBitmapCosine(true).ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));
		}
	}
}
