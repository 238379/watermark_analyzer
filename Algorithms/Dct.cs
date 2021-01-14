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

		private int[] v;
		private Random random;
		private ComplexImage complexImage;
		private ComplexImage complexWatermark;

		public override string ToString() => "DCT " + parameters;

		public Dct(DctParameters parameters) : base(ALGORITHM_NAME, parameters)
		{
			this.parameters = parameters;
		}

		public EffectiveBitmap CleanWatermark(EffectiveBitmap watermarked)
		{
			throw new NotImplementedException();
		}

		public EffectiveBitmap ExtractWatermark(EffectiveBitmap watermarked)
		{
			throw new NotImplementedException();
		}

		public void EmbedWatermark()
		{
			v = new int[complexImage.Width];
			double[] vAlpha = new double[complexImage.Width];
			random = new Random(parameters.Key);
			for (int i = 0; i < complexImage.Width; i++)
			{
				v[i] = random.Next(0, 2);
				vAlpha[i] = v[i] * parameters.Alpha;
			}

			for (int y = 0; y < complexImage.Height; y++)
				for (int x = 0; x < complexImage.Width; x++)
				{
					double imageReal = complexImage.Data[y, x].Real;
					double imageImaginary = complexImage.Data[y, x].Imaginary;
					double watermarkReal = complexWatermark.Data[y, x].Real;
					double watermarkImaginary = complexWatermark.Data[y, x].Imaginary;
					complexImage.Data[y, x] = new System.Numerics.Complex(imageReal + vAlpha[x] * watermarkReal, imageImaginary + vAlpha[x] * watermarkImaginary);
				}
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> AddWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			int size = 0;
            if (Math.Max(parameters.Original.Width, parameters.Original.Width) > 512)
            {
				size = Math.Max(parameters.Original.Width, parameters.Original.Width) / 2;
			}
			complexImage = ComplexImage.FromBitmap(parameters.Original, size);
			complexWatermark = ComplexImage.FromBitmap(parameters.Watermark, complexImage.Width);

			ct.ThrowIfCancellationRequested();

			complexImage.DCT();
			var cosineTransform = complexImage.ToEffectiveBitmap();
			
			yield return new AlgorithmResultElement("Cosine transform (DCT)", cosineTransform.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();

			complexWatermark.DCT();
			EmbedWatermark();
			var cosineWatermarked = complexImage.ToEffectiveBitmap();

			yield return new AlgorithmResultElement("DCT + watermark", cosineWatermarked.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();

			complexImage.IDCT();
			var watermarked = complexImage.ToEffectiveBitmap();

			yield return new AlgorithmResultElement("Watermarked", watermarked.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> RemoveWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			throw new NotImplementedException();
			yield return null;
		}
	}
}
