using Algorithms;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Drawing;

namespace AlgorithmTest
{
	[TestFixture]
	public class DctTests : AlgorithmTests
	{
		private Bitmap originalBitmap;
		private Bitmap watermarkBitmap;
		private Bitmap expectedDctBitmap;
		private Bitmap expectedDctWatermarkedBitmap;
		private Bitmap expectedWatermarkedBitmap;

		private int key;
		private double alpha;

		private DctParameters parameters;
		private Dct algorithm;

		private static readonly String myResourcesPath = resourcesPath + "/Dct/";

		[SetUp]
		public void Setup()
		{
			originalBitmap = new Bitmap(resourcesPath + "c_corgi.jpg");
			watermarkBitmap = new Bitmap(resourcesPath + "w_tekst_dolny.png");

			expectedDctBitmap = new Bitmap(myResourcesPath + "original_cosine_test.png");
			expectedDctWatermarkedBitmap = new Bitmap(myResourcesPath + "cosine_watermarked_test.png");
			expectedWatermarkedBitmap = new Bitmap(myResourcesPath + "dct_watermarked_test.png");

			key = 10;
			alpha = 0.01;
			parameters = new DctParameters(originalBitmap, watermarkBitmap, null, key, alpha);

			algorithm = new Dct(parameters);
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
		public void DctPlusWatermarkTest()
		{
			// Act
			var results = algorithm.AddWatermark();

			var fourierWatermarked = results[1];

			// Assert
			fourierWatermarked.Label.Should().Be("DCT + watermark");

			Assert.True(CompareBitmaps(fourierWatermarked.Image, expectedDctWatermarkedBitmap));
		}

		[Test]
		public void DctTest()
		{
			// Act
			var results = algorithm.AddWatermark();

			var originalFourier = results[0];

			// Assert
			originalFourier.Label.Should().Be("Cosine transform (DCT)");

			Assert.True(CompareBitmaps(originalFourier.Image, expectedDctBitmap));
		}
	}
}
