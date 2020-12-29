using Algorithms;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Drawing;
using System.Threading.Tasks;

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

		private static readonly string myResourcesPath = resourcesPath + "/Dft/";

		[SetUp]
		public void Setup()
		{
			originalBitmap = new Bitmap(resourcesPath + "c_corgi.png");
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
		public async Task WatermarkingTest()
		{
			// Act
			var results = await algorithm.AddWatermark();

			var watermarked = results[2];

			// Assert
			watermarked.Label.Should().Be("Watermarked");

			Assert.True(CompareBitmaps(watermarked.Image, expectedWatermarkedBitmap));
		}

		[Test]
		public async Task DftPlusWatermarkTest()
		{
			// Act
			var results = await algorithm.AddWatermark();

			var fourierWatermarked = results[1];

			// Assert
			fourierWatermarked.Label.Should().Be("DFT + watermark");

			Assert.True(CompareBitmaps(fourierWatermarked.Image, expectedDftWatermarkedBitmap));
		}

		[Test]
		public async Task DftTest()
		{
			// Act
			var results = await algorithm.AddWatermark();

			var originalFourier = results[0];

			// Assert
			originalFourier.Label.Should().Be("Fourier domain (DFT)");

			Assert.True(CompareBitmaps(originalFourier.Image, expectedDftBitmap));
		}
	}
}
