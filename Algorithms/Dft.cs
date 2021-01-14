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
			return "{"+ $"Key={Key}, Alpha={AlphaM}" + "}";
		}
	}

	public class Dft : Algorithm
	{
		public const string ALGORITHM_NAME = "DFT";

		private readonly DftParameters parameters;

		private int[] v;
		private Random random;
		private ComplexImage complexImage;
		private ComplexImage complexWatermark;

		public override string ToString() => "DFT " + parameters;


		public Dft(DftParameters parameters) : base(ALGORITHM_NAME, parameters)
		{
			this.parameters = parameters;
		}

		public EffectiveBitmap CleanWatermark(EffectiveBitmap watermarked)
		{
			ComplexImage complexWatermarked = ComplexImage.FromBitmap(watermarked);
			complexWatermarked.ForwardFourierTransform();
			v = new int[complexWatermarked.Width];
			double[] vAlpha = new double[complexWatermarked.Width];
			random = new Random(parameters.Key);
			for (int i = 0; i < complexWatermarked.Width; i++)
			{
				v[i] = random.Next(0, 2);
				vAlpha[i] = v[i] * parameters.Alpha;
			}

			for (int y = 0; y < complexWatermarked.Height; y++)
			{
				for (int x = 0; x < complexWatermarked.Width; x++)
				{
					double imageReal = complexWatermarked.Data[y, x].Real;
					double imageImaginary = complexWatermarked.Data[y, x].Imaginary;
					complexWatermarked.Data[y, x] = new System.Numerics.Complex(imageReal - vAlpha[x] * imageReal, imageImaginary - vAlpha[x] * imageImaginary);
				}
			}
			complexWatermarked.BackwardFourierTransform();
			return complexWatermarked.ToEffectiveBitmap();
		}

		public EffectiveBitmap ExtractWatermark(EffectiveBitmap watermarked)
		{
			ComplexImage complexWatermarked = ComplexImage.FromBitmap(watermarked);
			complexWatermarked.ForwardFourierTransform();
			v = new int[complexWatermarked.Width];
			double[] vAlpha = new double[complexWatermarked.Width];
			random = new Random(parameters.Key);
			for (int i = 0; i < complexWatermarked.Width; i++)
			{
				v[i] = random.Next(0, 2);
				vAlpha[i] = v[i] * parameters.Alpha;
			}

			for (int y = 0; y < complexWatermarked.Height; y++)
				for (int x = 0; x < complexWatermarked.Width; x++)
				{
					double imageReal = complexWatermarked.Data[y, x].Real;
					double imageImaginary = complexWatermarked.Data[y, x].Imaginary;
					complexWatermarked.Data[y, x] = new System.Numerics.Complex(vAlpha[x] * imageReal - imageReal, vAlpha[x] * imageImaginary - imageImaginary);
				}
			complexWatermarked.BackwardFourierTransform();
			return complexWatermarked.ToEffectiveBitmap();
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
			complexImage = ComplexImage.FromBitmap(parameters.Original);
			complexWatermark = ComplexImage.FromBitmap(parameters.Watermark, complexImage.Width);

			ct.ThrowIfCancellationRequested();

			complexImage.ForwardFourierTransform();
			var fourierDomain = complexImage.ToEffectiveBitmap();

			yield return new AlgorithmResultElement("Fourier domain (DFT)", fourierDomain.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();

			complexWatermark.ForwardFourierTransform();
			EmbedWatermark();
			var fourierDomainWatermarked = complexImage.ToEffectiveBitmap();

			yield return new AlgorithmResultElement("DFT + watermark", fourierDomainWatermarked.ToBitmap(parameters.Original.Size), new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();

			complexImage.BackwardFourierTransform();
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
