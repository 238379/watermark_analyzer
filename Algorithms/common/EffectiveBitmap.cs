using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.common
{
	public class EffectiveBitmap
	{
		public readonly int Width;
		public readonly int Height;
		public readonly int Depth;

		private readonly byte[] buffer;

		public EffectiveBitmap(int width, int height, int depth, byte[] buffer)
		{
			Width = width;
			Height = height;
			Depth = depth;
			this.buffer = buffer;
		}

		public EffectiveBitmap(int width, int height)
		{
			Width = width;
			Height = height;
			Depth = 4;
			buffer = new byte[Width * Height * Depth];
		}

		public EffectiveBitmap(byte[,,] arrayycbcr, int width, int height, int depth = 4)
		{
			Width = width;
			Height = height;
			Depth = depth;
			buffer = new byte[Width * Height * Depth];
			RunOnEveryPixel((i, j) =>
			{
				SetPixel(i, j, PixelInfo.FromYCbCr(arrayycbcr[i, j, 0], arrayycbcr[i, j, 1], arrayycbcr[i, j, 2]));
			});
		}

		public PixelInfo GetPixel(int x, int y)
		{
			var offset = ((y * Width) + x) * Depth;
			return new PixelInfo(buffer[offset + 0], buffer[offset + 1], buffer[offset + 2]);
		}

		public int GetDepth()
		{
			return Depth;
		}

		public Bitmap ToBitmap()
		{
			var resultBitmap = new Bitmap(Width, Height, Depth == 3 ? PixelFormat.Format24bppRgb : PixelFormat.Format32bppArgb);

			var resultData = resultBitmap.LockAllBitsReadWrite();
			Marshal.Copy(buffer, 0, resultData.Scan0, buffer.Length);
			resultBitmap.UnlockBits(resultData);

			return resultBitmap;
		}

		public void RunOnEveryPixel(Action<int, int> action)
		{
			for (int i = 0; i < Width; i++)
			{
				for (int j = 0; j < Height; j++)
				{
					action(i, j);
				}
			}
		}

		public EffectiveBitmap Resize(int width, int height)
		{
			if(Width != width || Height != height)
			{
				// todo nk better
				return ToBitmap().Resize(width, height).TransformToEffectiveBitmap();
			}
			return this;
		}

		public static EffectiveBitmap Create(int width, int height, PixelFormat pixelFormat, Func<int, int, PixelInfo> creator)
		{
			return Create(width, height, pixelFormat == PixelFormat.Format24bppRgb ? 3 : 4, creator);
		}

		public static EffectiveBitmap Create(int width, int height, int depth, Func<int, int, PixelInfo> creator)
		{
			var buffer = new byte[width * height * depth];

			Parallel.For(0, width, (i) => {
				Parallel.For(0, height, (j) => {
					var result = creator(i, j);
					var offset = ((j * width) + i) * depth;
					buffer[offset + 0] = result.R;
					buffer[offset + 1] = result.G;
					buffer[offset + 2] = result.B;
					if (depth > 3)
					{
						buffer[offset + 3] = result.A;
					}
				});
			});

			return new EffectiveBitmap(width, height, depth, buffer);
		}

		public byte[,,] Rgb2Ycbcr()
		{
			byte[,,] arrayycbcr = new byte[Height, Width, 3];

			RunOnEveryPixel((i, j) =>
			{
				byte[] YCbCr = YCbCrFromRGB(GetPixel(i, j).R, GetPixel(i, j).G, GetPixel(i, j).B);
				arrayycbcr[i, j, 0] = YCbCr[0];
				arrayycbcr[i, j, 1] = YCbCr[1];
				arrayycbcr[i, j, 2] = YCbCr[2];
			});
			return arrayycbcr;
		}

		private void SetPixel(int x, int y, PixelInfo pixelInfo)
		{
			var offset = ((y * Width) + x) * Depth;
			buffer[offset + 0] = pixelInfo.R;
			buffer[offset + 1] = pixelInfo.G;
			buffer[offset + 2] = pixelInfo.B;
			if (Depth > 3)
			{
				buffer[offset + 3] = pixelInfo.A;
			}
		}

		private byte[] YCbCrFromRGB(int R, int G, int B)
		{
			var Y = (byte)((0.257 * R) + (0.504 * G) + (0.098 * B) + 16);
			var Cb = (byte)(-(0.148 * R) - (0.291 * G) + (0.439 * B) + 128);
			var Cr = (byte)((0.439 * R) - (0.368 * G) - (0.071 * B) + 128);

			return new byte[3] { Y, Cb, Cr };
		}
	}
}
