﻿using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Algorithms
{
	public class LsbParameters : AlgorithmParameters
	{
		public readonly int BitsForWatermark;

		public LsbParameters(EffectiveBitmap original, EffectiveBitmap watermark, EffectiveBitmap watermarked, int bitsForWatermark) : base(original, watermark, watermarked)
		{
			BitsForWatermark = bitsForWatermark;
		}

		public override string ToString()
		{
			return "{" + $"Bits for watermark={BitsForWatermark}" + "}";
		}
	}

	public class Lsb : Algorithm
	{
		public const string ALGORITHM_NAME = "LSB";

		private readonly LsbParameters parameters;

		// TODO width and height
		private readonly Random random = new Random();

		public override string ToString() => "LSB " + parameters;

		public Lsb(LsbParameters parameters) : base(ALGORITHM_NAME, parameters)
		{
			this.parameters = parameters;
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> AddWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();
			var watermarked = Watermark(parameters.Original, parameters.Watermark);

			yield return new AlgorithmResultElement("Watermarked", watermarked, new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();
			var cleaned = CleanWatermark(watermarked);

			yield return new AlgorithmResultElement("Cleaned", cleaned, new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();
			var extracted = ExtractWatermark(watermarked);

			yield return new AlgorithmResultElement("Extracted watermark", extracted, new ResultDescription(ToString()));
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> RemoveWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();
			var cleaned = CleanWatermark(parameters.Watermarked);

			yield return new AlgorithmResultElement("Cleaned", cleaned, new ResultDescription(ToString()));

			ct.ThrowIfCancellationRequested();
			var extracted = ExtractWatermark(parameters.Watermarked);

			yield return new AlgorithmResultElement("Extracted watermark", extracted, new ResultDescription(ToString()));
		}

		private EffectiveBitmap Watermark(EffectiveBitmap original, EffectiveBitmap watermark)
		{
			var simplifiedWatermark = PreprocessToSimplifiedWatermark(watermark);

			return EffectiveBitmap.Create(original.Width, original.Height, original.Depth, (i, j) =>
			{
				var originalPixel = original.GetPixel(i, j);
				var watermarkPixel = simplifiedWatermark.GetPixel(i, j);

				var divider = (int)Math.Pow(2, parameters.BitsForWatermark);
				var r = (byte)(Math.Clamp(originalPixel.R - originalPixel.R % divider + watermarkPixel.R, 0, 255));
				var g = (byte)(Math.Clamp(originalPixel.G - originalPixel.G % divider + watermarkPixel.G, 0, 255));
				var b = (byte)(Math.Clamp(originalPixel.B - originalPixel.B % divider + watermarkPixel.B, 0, 255));

				return new PixelInfo(r, g, b);
			});
		}

		private EffectiveBitmap CleanWatermark(EffectiveBitmap watermarked)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkedImgPixel = sources[0].GetPixel(i, j);

				var divider = (int)(Math.Pow(2, parameters.BitsForWatermark) + 0.5);
				// TODO better method instead of random bit on last position
				var r = (byte)(watermarkedImgPixel.R - watermarkedImgPixel.R % divider + CreateRandomNumber(divider));
				var g = (byte)(watermarkedImgPixel.G - watermarkedImgPixel.G % divider + CreateRandomNumber(divider));
				var b = (byte)(watermarkedImgPixel.B - watermarkedImgPixel.B % divider + CreateRandomNumber(divider));

				return new PixelInfo(r, g, b);
			}, watermarked);
		}

		private EffectiveBitmap ExtractWatermark(EffectiveBitmap watermarked)
		{
			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkedImgPixel = sources[0].GetPixel(i, j);

				var divider = (int)Math.Pow(2, parameters.BitsForWatermark);
				var multiplier = 255 / (divider - 1);
				byte r = (byte)(watermarkedImgPixel.R % divider * multiplier);
				byte g = (byte)(watermarkedImgPixel.G % divider * multiplier);
				byte b = (byte)(watermarkedImgPixel.B % divider * multiplier);

				return new PixelInfo(r, g, b);
			}, watermarked);
		}

		private EffectiveBitmap PreprocessToSimplifiedWatermark(EffectiveBitmap watermark)
		{
			var divider = (byte)(Math.Ceiling(255 / Math.Pow(2, parameters.BitsForWatermark)) + 0.5);

			return BitmapOperations.Create((sources, i, j) =>
			{
				var watermarkPixel = sources[0].GetPixel(i, j);

				byte r = (byte)(watermarkPixel.R / divider);
				byte g = (byte)(watermarkPixel.G / divider);
				byte b = (byte)(watermarkPixel.B / divider);

				return new PixelInfo(r, g, b);
			}, watermark);
		}

		private byte CreateRandomNumber(int upperExcluded)
		{
			return (byte)random.Next(0, upperExcluded);
		}
	}
}
