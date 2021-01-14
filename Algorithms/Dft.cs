using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Algorithms
{
	public class DftParameters : AlgorithmParameters
	{
		public readonly int Key;
		public readonly double Alpha;
		public readonly decimal AlphaM;

		public DftParameters(EffectiveBitmap original, EffectiveBitmap watermark, EffectiveBitmap watermarked, int key, decimal alpha) : base(original, watermark, watermarked)
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

	public class Dft : Algorithm
	{
		public const string ALGORITHM_NAME = "DFT";

		private readonly DftParameters parameters;

		private ComplexImage complexImage;
		private ComplexImage complexWatermark;

		public override string ToString() => "DFT " + parameters;


		public Dft(DftParameters parameters) : base(ALGORITHM_NAME, parameters)
		{
			this.parameters = parameters;
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> AddWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			complexImage = ComplexImage.FromBitmap(parameters.Original);
			complexWatermark = ComplexImage.FromBitmap(parameters.Watermark, complexImage.Width);

			ct.ThrowIfCancellationRequested();

			complexImage.ForwardFourierTransform();
			var fourierDomain = complexImage.ToEffectiveBitmapFourier();

			yield return new AlgorithmResultElement("Fourier domain (DFT)", fourierDomain.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();

			complexWatermark.ForwardFourierTransform();
			complexImage.EmbedImageFourier(complexWatermark, parameters.Key, parameters.Alpha);
			var fourierDomainWatermarked = complexImage.ToEffectiveBitmapFourier();

			yield return new AlgorithmResultElement("DFT + watermark", fourierDomainWatermarked.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();

			complexImage.BackwardFourierTransform();
			var watermarked = complexImage.ToEffectiveBitmapFourier();

			yield return new AlgorithmResultElement("Watermarked", watermarked.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> RemoveWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();

			ComplexImage complexWatermarked = ComplexImage.FromBitmap(parameters.Watermarked);
			complexWatermarked.ForwardFourierTransform();

			ct.ThrowIfCancellationRequested();

			ComplexImage complexOriginal = ComplexImage.FromBitmap(parameters.Original);
			complexOriginal.ForwardFourierTransform();

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
					double watermarkedReal = complexWatermarked.Data[y, x].Real;
					double watermarkedImaginary = complexWatermarked.Data[y, x].Imaginary;
					double originalReal = complexOriginal.Data[y, x].Real;
					double originalImaginary = complexOriginal.Data[y, x].Imaginary;
					if (vAlpha[x] == 0)
					{
						complexWatermarked.Data[y, x] = new System.Numerics.Complex(0, 0);
					}
					else
					{
						complexWatermarked.Data[y, x] = new System.Numerics.Complex((watermarkedReal - originalReal) / vAlpha[x], (watermarkedImaginary - originalImaginary) / vAlpha[x]);
					}
				}
			}

			ct.ThrowIfCancellationRequested();

			yield return new AlgorithmResultElement("Extracted watermark DFT", complexWatermarked.ToEffectiveBitmapFourier(true).ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));

			complexWatermarked.BackwardFourierTransform();

			ct.ThrowIfCancellationRequested();

			yield return new AlgorithmResultElement("Extracted watermark", complexWatermarked.ToEffectiveBitmapFourier(true).ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));
		}
	}
}
