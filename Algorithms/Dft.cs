﻿using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Algorithms
{
	public class DftParameters : AlgorithmParameters
	{
		public readonly int Key;
		public readonly double Alpha;

		public DftParameters(Bitmap original, Bitmap watermark, Bitmap watermarked, int key, double alpha) : base(original, watermark, watermarked)
		{
			Key = key;
			Alpha = alpha;
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

		public Dft(DftParameters parameters) : base()
		{
			this.parameters = parameters;
		}

		public Bitmap CleanWatermark(Bitmap watermarked)
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
			return complexWatermarked.ToBitmap();
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

        public override AlgorithmResult AddWatermark()
        {
			complexImage = ComplexImage.FromBitmap(parameters.Original);
			complexWatermark = ComplexImage.FromBitmap(parameters.Watermark, parameters.Original.Width);

			complexImage.ForwardFourierTransform();
			var fourierDomain = complexImage.ToBitmap();

			complexWatermark.ForwardFourierTransform();
			EmbedWatermark();
			var fourierDomainWatermarked = complexImage.ToBitmap();

			complexImage.BackwardFourierTransform();
			var watermarked = complexImage.ToBitmap();
			return new AlgorithmResult(("Fourier domain (DFT)", fourierDomain), ("DFT + watermark", fourierDomainWatermarked), ("Watermarked", watermarked));
		}

        public override AlgorithmResult RemoveWatermark()
        {
            throw new NotImplementedException();
        }
    }
}