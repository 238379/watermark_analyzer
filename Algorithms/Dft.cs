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

		private readonly int key = 0;
		private readonly double alpha;
		private int[] v;
		private readonly Random random;
		private ComplexImage originalImage;
		private ComplexImage complexImage;
		private ComplexImage complexWatermark;

		public Dft(Dictionary<string, dynamic> parameters) : base(parameters)
		{
			this.key = parameters[BITS_PARAM];
			this.alpha = 0.1;
			this.random = new Random(key);
		}

		public Bitmap Watermark(Bitmap original, Bitmap watermark)
		{
			complexImage = ComplexImage.FromBitmap(original);
			complexWatermark = ComplexImage.FromBitmap(watermark, original.Width);
			originalImage = new ComplexImage(complexImage);

			complexImage.ForwardFourierTransform();
			complexWatermark.ForwardFourierTransform();
			EmbedWatermark();
            complexImage.BackwardFourierTransform();
            var watermarkedBitmap = complexImage.ToBitmap();

            return watermarkedBitmap;
		}

		public Bitmap CleanWatermark(Bitmap watermarked)
		{
			ComplexImage complexWatermarked = ComplexImage.FromBitmap(watermarked);
			complexWatermarked.ForwardFourierTransform();
			v = new int[complexWatermarked.Width];
			double[] vAlpha = new double[complexWatermarked.Width];
			for (int i = 0; i < complexWatermarked.Width; i++)
			{
				v[i] = random.Next(0, 2);
				vAlpha[i] = v[i] * alpha;
			}

			for (int y = 0; y < complexWatermarked.Height; y++)
				for (int x = 0; x < complexWatermarked.Width; x++)
				{
					double imageReal = complexWatermarked.Data[y, x].Real;
					double imageImaginary = complexWatermarked.Data[y, x].Imaginary;
					complexWatermarked.Data[y, x] = new System.Numerics.Complex(imageReal - vAlpha[x] * imageReal, imageImaginary - vAlpha[x] * imageImaginary);
				}
			complexWatermarked.BackwardFourierTransform();
			return complexWatermarked.ToBitmap();
		}

		public Bitmap ExtractWatermark(Bitmap watermarked)
		{
			ComplexImage complexWatermarked = ComplexImage.FromBitmap(watermarked);
			complexWatermarked.ForwardFourierTransform();
			v = new int[complexWatermarked.Width];
			double[] vAlpha = new double[complexWatermarked.Width];
			for (int i = 0; i < complexWatermarked.Width; i++)
			{
				v[i] = random.Next(0, 2);
				vAlpha[i] = v[i] * alpha;
			}

			for (int y = 0; y < complexWatermarked.Height; y++)
				for (int x = 0; x < complexWatermarked.Width; x++)
				{
					double imageReal = complexWatermarked.Data[y, x].Real;
					double imageImaginary = complexWatermarked.Data[y, x].Imaginary;
					complexWatermarked.Data[y, x] = new System.Numerics.Complex(vAlpha[x] * imageReal - imageReal, vAlpha[x] * imageImaginary - imageImaginary);
				}
			complexWatermarked.BackwardFourierTransform();
			return complexWatermarked.ToBitmap();
		}


		public void EmbedWatermark()
		{
			v = new int[complexImage.Width];
			double[] vAlpha = new double[complexImage.Width];
			for (int i = 0; i < complexImage.Width; i++)
            {
				v[i] = random.Next(0, 2);
				vAlpha[i] = v[i] * alpha;
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

		public override AlgorithmResult Run(Bitmap original, Bitmap watermark)
        {
			var watermarked = Watermark(original, watermark);
			var cleaned = CleanWatermark(watermarked);
			var extracted = ExtractWatermark(watermarked);
			return new AlgorithmResult(watermarked, cleaned, extracted);
		}
    }
}
