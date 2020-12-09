using Algorithms;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Drawing;
using System.IO;

namespace AlgorithmTest
{
	[TestFixture]
	public class DwtTests : AlgorithmTests
	{
		private Bitmap originalBitmap;
		private Bitmap watermarkBitmap;
		private Bitmap expectedWatermarkedBitmap;
		private Bitmap expectedDwtBitmap;
		private Bitmap expectedDwtPlusWatermarkBitmap;

		private int layers;
		private double alpha;

		private DwtParameters parameters;
		private Dwt algorithm;

		private static readonly String myResourcesPath = resourcesPath + "/DWT/";

		[SetUp]
		public void Setup()
		{
			Console.WriteLine(new System.IO.FileInfo(resourcesPath + "c_corgi.jpg").FullName);
			originalBitmap = new Bitmap(resourcesPath + "c_corgi.jpg");
			watermarkBitmap = new Bitmap(resourcesPath + "w_tekst_dolny.png");
			expectedWatermarkedBitmap = new Bitmap(myResourcesPath + "corgi_dolny_watermarked.png");
			expectedDwtBitmap = new Bitmap(myResourcesPath + "corgi_dolny_dwt.png");
			expectedDwtPlusWatermarkBitmap = new Bitmap(myResourcesPath + "corgi_dolny_dwt_plus_watermark.png");

			layers = 2;
			alpha = 0.01;
			parameters = new DwtParameters(originalBitmap, watermarkBitmap, null, layers, alpha);
			algorithm = new Dwt(parameters);
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
		public void DwtTest()
		{
			// Act
			var results = algorithm.AddWatermark();

			var haared = results[0];

			// Assert
			haared.Label.Should().Be("DWT");

			Assert.True(CompareBitmaps(haared.Image, expectedDwtBitmap));
		}

		[Test]
		public void DwtPlusWatermarkTest()
		{
			// Act
			var results = algorithm.AddWatermark();

			var haaredPlusWatermark = results[1];

			// Assert
			haaredPlusWatermark.Label.Should().Be("DWT + watermark");

			Assert.True(CompareBitmaps(haaredPlusWatermark.Image, expectedDwtPlusWatermarkBitmap));
		}

	}
}
