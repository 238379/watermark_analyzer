using Algorithms;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Drawing;

namespace AlgorithmTest
{
	[TestFixture]
	public class DftTests : AlgorithmTests
	{
		private Bitmap originalBitmap;
		private Bitmap watermarkBitmap;
		private Bitmap expectedDftBitmap;
		private Bitmap expectedDftWatermarkedBitmap;
		private Bitmap expectedWatermarkedBitmap;

		private int key;
		private double alpha;

		private DftParameters parameters;
		private Dft algorithm;

		private static readonly String myResourcesPath = resourcesPath + "/Dft/";

		[SetUp]
		public void Setup()
		{
			originalBitmap = new Bitmap(resourcesPath + "c_corgi.jpg");
			watermarkBitmap = new Bitmap(resourcesPath + "w_tekst_dolny.png");

			expectedDftBitmap = new Bitmap(myResourcesPath + "original_fourier_test.png");
			expectedDftWatermarkedBitmap = new Bitmap(myResourcesPath + "fourier_watermarked_test.png");
			expectedWatermarkedBitmap = new Bitmap(myResourcesPath + "dft_watermarked_test.png");

			key = 10;
			alpha = 0.01;
			parameters = new DftParameters(originalBitmap, watermarkBitmap, null, key, alpha);

			algorithm = new Dft(parameters);
		}

		[Test]
		public void WatermarkingTest()
		{
			// Act
			var results = algorithm.AddWatermark();

			var watermarked = results[2];

			// Assert
			watermarked.Label.Should().Be("Watermarked");

			Assert.True(CompareBitmaps(watermarked.Image, expectedWatermarkedBitmap));
		}

		[Test]
		public void DftPlusWatermarkTest()
		{
			// Act
			var results = algorithm.AddWatermark();

			var fourierWatermarked = results[1];

			// Assert
			fourierWatermarked.Label.Should().Be("DFT + watermark");

			Assert.True(CompareBitmaps(fourierWatermarked.Image, expectedDftWatermarkedBitmap));
		}

		[Test]
		public void DftTest()
		{
			// Act
			var results = algorithm.AddWatermark();

			var originalFourier = results[0];

			// Assert
			originalFourier.Label.Should().Be("Fourier domain (DFT)");

			Assert.True(CompareBitmaps(originalFourier.Image, expectedDftBitmap));
		}
	}
}
