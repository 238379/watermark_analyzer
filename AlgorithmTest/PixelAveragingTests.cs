﻿using Algorithms;
using Algorithms.common;
using Common;
using FluentAssertions;
using NUnit.Framework;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace AlgorithmTest
{
	[TestFixture]
	public class PixelAveragingTests : AlgorithmTests
	{
		private const int WIDTH = 2;
		private const int HEIGHT = 2;

		private Bitmap originalBitmap;
		private Bitmap watermarkBitmap;
		private decimal ratio;

		private PixelAveragingParameters parameters;
		private PixelAveraging algorithm;

		[SetUp]
		public void Setup()
		{
			originalBitmap = CreateOriginal();
			watermarkBitmap = CreateWatermark();
			ratio = 0.5M;
			parameters = new PixelAveragingParameters(originalBitmap.TransformToEffectiveBitmap(), watermarkBitmap.TransformToEffectiveBitmap(), null, ratio);

			algorithm = new PixelAveraging(parameters);
		}

		[Test]
		public async Task BasicAddingTest()
		{
			// Act
			var results = await algorithm.AddWatermark(CancellationToken.None).ToListAsync();

			var watermarked = results[0];

			// Assert
			watermarked.Label.Should().Be("Watermarked");

			AssertBitmapsAreEqual(watermarked.Image, new Color[2, 2] {
				{ Color.FromArgb(177, 9, 16), Color.FromArgb(32, 127, 31) },
				{ Color.FromArgb(0, 0, 1), Color.FromArgb(33, 187, 65) }});
		}

		[Test]
		public async Task BasicRemovingTest()
		{
			// Act
			var results = await algorithm.AddWatermark(CancellationToken.None).ToListAsync();

			var watermarked = results[1];

			// Assert
			watermarked.Label.Should().Be("Cleaned");

			AssertBitmapsAreEqual(watermarked.Image, new Color[2, 2] {
				{ Color.FromArgb(254, 3, 30), Color.FromArgb(0, 254, 0) },
				{ Color.FromArgb(0, 0, 1), Color.FromArgb(33, 249, 2) }});
		}

		private Bitmap CreateOriginal()
		{
			var bmp = new Bitmap(WIDTH, HEIGHT);

			bmp.SetPixel(0, 0, Color.FromArgb(255, 4, 30)); bmp.SetPixel(0, 1, Color.FromArgb(0, 254, 0));
			bmp.SetPixel(1, 0, Color.FromArgb(0, 0, 3)); bmp.SetPixel(1, 1, Color.FromArgb(34, 250, 3));

			return bmp;
		}

		private Bitmap CreateWatermark()
		{
			var bmp = new Bitmap(WIDTH, HEIGHT);

			bmp.SetPixel(0, 0, Color.FromArgb(200, 30, 4)); bmp.SetPixel(0, 1, Color.FromArgb(128, 0, 127));
			bmp.SetPixel(1, 0, Color.FromArgb(1, 1, 1)); bmp.SetPixel(1, 1, Color.FromArgb(65, 250, 255));

			return bmp;
		}
	}
}
