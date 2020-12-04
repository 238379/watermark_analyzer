using Algorithms;
using FluentAssertions;
using NUnit.Framework;
using System.Drawing;

namespace AlgorithmTest
{
	[TestFixture]
	public class DwtTests : AlgorithmTests
	{
		private const int WIDTH = 2;
		private const int HEIGHT = 2;

		private Bitmap originalBitmap;
		private Bitmap watermarkBitmap;
		private int layers;
		private double alpha;

		private DwtParameters parameters;
		private Dwt algorithm;

		[SetUp]
		public void Setup()
		{
			originalBitmap = CreateOriginal();
			watermarkBitmap = CreateWatermark();
			layers = 2;
			alpha = 0.01;
			parameters = new DwtParameters(originalBitmap, watermarkBitmap, null, layers, alpha);

			algorithm = new Dwt(parameters);
		}

		[Test]
		public void BasicAddingTest()
		{
			// Act
			var results = algorithm.AddWatermark();

			//var watermarked = results[0];

			// Assert
			//watermarked.Label.Should().Be("Watermarked");

			//AssertBitmapsAreEqual(watermarked.Image, new Color[2, 2] {
			//	{ Color.FromArgb(255, 0, 0), Color.FromArgb(1, 254, 0) },
			//	{ Color.FromArgb(0, 0, 2), Color.FromArgb(0, 0, 3) }});

			Assert.Fail("Remove if test is implemented.");
		}

		private Bitmap CreateOriginal()
		{
			var bmp = new Bitmap(WIDTH, HEIGHT);

			bmp.SetPixel(0, 0, Color.FromArgb(255, 0, 0)); bmp.SetPixel(0, 1, Color.FromArgb(0, 254, 0));
			bmp.SetPixel(1, 0, Color.FromArgb(0, 0, 3)); bmp.SetPixel(1, 1, Color.FromArgb(0, 0, 3));

			return bmp;
		}

		private Bitmap CreateWatermark()
		{
			var bmp = new Bitmap(WIDTH, HEIGHT);

			bmp.SetPixel(0, 0, Color.FromArgb(200, 0, 0)); bmp.SetPixel(0, 1, Color.FromArgb(128, 0, 127));
			bmp.SetPixel(1, 0, Color.FromArgb(1, 1, 1)); bmp.SetPixel(1, 1, Color.FromArgb(0, 0, 255));

			return bmp;
		}
	}
}
