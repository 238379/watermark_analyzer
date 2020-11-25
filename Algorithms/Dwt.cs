using Algorithms.common;
using System;
using System.Drawing;

namespace Algorithms
{
	public class DwtParameters : AlgorithmParameters
	{
		public readonly int Layers;

		public DwtParameters(Bitmap original, Bitmap watermark, int layers) : base(original, watermark)
		{
			Layers = layers;
		}
	}
	public class Dwt: Algorithm
	{
		public const string ALGORITHM_NAME = "DWT";

		private readonly DwtParameters parameters;

		private const double w0 = 0.5;
		private const double w1 = -0.5;
		private const double s0 = 0.5;
		private const double s1 = 0.5;

		public Dwt(DwtParameters parameters) : base()
		{
			this.parameters = parameters;
		}

		public override AlgorithmResult Run()
		{
			var watermarked = ProcessHaar(parameters.Original, false, parameters.Layers);
			var cleaned = ProcessHaar(watermarked, true, parameters.Layers);
			var extracted = watermarked;
			return new AlgorithmResult(watermarked, cleaned, extracted);
		}

		private void FWT(HaarColor[] data)
		{
			HaarColor[] temp = new HaarColor[data.Length];

			int h = data.Length / 2;
			for (int i = 0; i < h; i++)
			{
				int k = (i * 2);
				temp[i] =
					new HaarColor(
						data[k].r * s0 + data[k + 1].r * s1,
						data[k].g * s0 + data[k + 1].g * s1,
						data[k].b * s0 + data[k + 1].b * s1
				);

				temp[i + h] =
					new HaarColor(
						data[k].r * w0 + data[k + 1].r * w1,
						data[k].g * w0 + data[k + 1].g * w1,
						data[k].b * w0 + data[k + 1].b * w1
				);
			}

			for (int i = 0; i < data.Length; i++)
				data[i] = temp[i];
		}

		public void FWT(HaarColor[,] data, int iterations)
		{
			int rows = data.GetLength(0);
			int cols = data.GetLength(1);

			HaarColor[] row;
			HaarColor[] col;

			for (int k = 0; k < iterations; k++)
			{
				int lev = (int)Math.Pow(2, k);

				int levCols = cols / lev;
				int levRows = rows / lev;

				row = new HaarColor[levCols];
				for (int i = 0; i < levRows; i++)
				{
					for (int j = 0; j < row.Length; j++)
						row[j] = data[i, j];

					FWT(row);

					for (int j = 0; j < row.Length; j++)
						data[i, j] = row[j];
				}


				col = new HaarColor[levRows];
				for (int j = 0; j < levCols; j++)
				{
					for (int i = 0; i < col.Length; i++)
						col[i] = data[i, j];

					FWT(col);

					for (int i = 0; i < col.Length; i++)
						data[i, j] = col[i];
				}
			}
		}

		public Bitmap ProcessHaar(Bitmap image, Boolean isRevert, int layers)
		{
			var haarColors = new HaarColor[image.Height, image.Width];

			for (int i = 0; i < image.Height; i++)
			{
				for (int j = 0; j < image.Width; j++)
				{
					var color = image.GetPixel(j, i);
					var haarColor = new HaarColor(color);
					haarColors[i, j] = haarColor;
				}
			}

			if (!isRevert)
			{
				FWT(haarColors, layers);
			}
			else
			{
				IWT(haarColors, layers);
			}

			var haarBitmap = new Bitmap(image.Width, image.Height);
			for (int i = 0; i < haarBitmap.Height; i++)
			{
				for (int j = 0; j < haarBitmap.Width; j++)
				{
					var haarColor = haarColors[i, j];
					haarBitmap.SetPixel(j, i, haarColor.GetColor());
				}
			}

			return haarBitmap;
		}

		public void IWT(HaarColor[] data)
		{
			var temp = new HaarColor[data.Length];

			int h = data.Length / 2;
			for (int i = 0; i < h; i++)
			{
				int k = (i * 2);

				temp[k] =
				new HaarColor(
					(data[i].r * s0 + data[i + h].r * w0) / w0,
					(data[i].g * s0 + data[i + h].g * w0) / w0,
					(data[i].b * s0 + data[i + h].b * w0) / w0
				);

				temp[k + 1] =
					new HaarColor(
						(data[i].r * s0 + data[i + h].r * w1) / s0,
						(data[i].g * s0 + data[i + h].g * w1) / s0,
						(data[i].b * s0 + data[i + h].b * w1) / s0
				);
			}

			for (int i = 0; i < data.Length; i++)
				data[i] = temp[i];
		}

		public void IWT(HaarColor[,] data, int iterations)
		{
			int rows = data.GetLength(0);
			int cols = data.GetLength(1);

			HaarColor[] col;
			HaarColor[] row;

			for (int k = iterations - 1; k >= 0; k--)
			{
				int lev = (int)Math.Pow(2, k);

				int levCols = cols / lev;
				int levRows = rows / lev;

				col = new HaarColor[levRows];
				for (int j = 0; j < levCols; j++)
				{
					for (int i = 0; i < col.Length; i++)
						col[i] = data[i, j];

					IWT(col);

					for (int i = 0; i < col.Length; i++)
						data[i, j] = col[i];
				}

				row = new HaarColor[levCols];
				for (int i = 0; i < levRows; i++)
				{
					for (int j = 0; j < row.Length; j++)
						row[j] = data[i, j];

					IWT(row);

					for (int j = 0; j < row.Length; j++)
						data[i, j] = row[j];
				}
			}
		}
	}
}
