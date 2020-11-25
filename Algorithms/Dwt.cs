using Algorithms.common;
using System;
using System.Drawing;

namespace Algorithms
{
	public class DwtParameters : AlgorithmParameters
	{
		public readonly int Layers;
		public readonly double Alpha;

		public DwtParameters(Bitmap original, Bitmap watermark, int layers, double alpha) : base(original, watermark)
		{
			Layers = layers;
			Alpha = alpha;
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
			var haared = ProcessHaar(parameters.Original, false, parameters.Layers);

			var haaredWatermarked = BitmapOperations.Create((sources, i, j) =>
			{
				var layers = parameters.Layers;
				var originalPixel = sources[0].GetPixel(i, j);
				var watermarkPixel = sources[1].GetPixel(i, j);

				//TODO watermark scaling
				if (i>sources[0].Height * (2 * layers - 1) / (2 * layers ) && j<sources[0].Width/(2 * layers)) {
					var alpha = parameters.Alpha;
	
					var r = (int)(originalPixel.R + (watermarkPixel.R * alpha)) % 255;
					var g = (int)(originalPixel.G + (watermarkPixel.G * alpha)) % 255;
					var b = (int)(originalPixel.B + (watermarkPixel.B * alpha)) % 255;

					return new PixelInfo(r, g, b);
				}

				return originalPixel;
				
				
			}, haared, parameters.Watermark);

			var watermarked = ProcessHaar(haaredWatermarked, true, parameters.Layers);

			return new AlgorithmResult(watermarked, haared, haaredWatermarked);
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
					{
						int x = j;
						if (k == 1) x += levCols;
						row[j] = data[i, x];
					}

					FWT(row);

					for (int j = 0; j < row.Length; j++)
					{
						int x = j;
						if (k == 1) x += levCols;
						data[i, x] = row[j];
					}
				}

				col = new HaarColor[levRows];
				for (int j = 0; j < levCols; j++)
				{
					for (int i = 0; i < col.Length; i++)
					{
						int x = j;
						if (k == 1) x += levCols;
						col[i] = data[i, x];
					}

					FWT(col);

					for (int i = 0; i < col.Length; i++)
					{
						int x = j;
						if (k == 1) x += levCols;
						data[i, x] = col[i];
					}
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

				row = new HaarColor[levCols];
				for (int i = 0; i < levRows; i++)
				{
					for (int j = 0; j < row.Length; j++)
					{
						int x = j;
						if (k == 1) x += 400;
						row[j] = data[i, x];
					}

					IWT(row);

					for (int j = 0; j < row.Length; j++)
					{
						int x = j;
						if (k == 1) x += 400;
						data[i, x] = row[j];
					}
				}

				col = new HaarColor[levRows];
				for (int j = 0; j < levCols; j++)
				{
					for (int i = 0; i < col.Length; i++)
					{
						int x = j;
						if (k == 1) x += 400;
						col[i] = data[i, x];
					}

					IWT(col);

					for (int i = 0; i < col.Length; i++)
					{
						int x = j;
						if (k == 1) x += 400;
						data[i, x] = col[i];
					}
				}
			}
		}
	}
}
