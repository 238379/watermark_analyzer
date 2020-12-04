using System;
using System.Drawing;

namespace Algorithms.common
{
	public struct PixelInfo
	{
		public readonly byte R;
		public readonly byte G;
		public readonly byte B;
		public readonly byte A;

		public PixelInfo(Color color)
		{
			R = color.R;
			G = color.G;
			B = color.B;
			A = color.A;
		}

		public PixelInfo(byte r, byte g, byte b)
		{
			R = r;
			G = g;
			B = b;
			A = 255;
		}

		public PixelInfo(byte r, byte g, byte b, byte a)
		{
			R = r;
			G = g;
			B = b;
			A = a;
		}

		public PixelInfo(int r, int g, int b)
		{
			R = (byte)r;
			G = (byte)g;
			B = (byte)b;
			A = 255;
		}

		public PixelInfo(int r, int g, int b, int a)
		{
			R = (byte)r;
			G = (byte)g;
			B = (byte)b;
			A = (byte)a;
		}

		public static PixelInfo FromYCbCr(int y, int cb, int cr)
		{
			double Y = y;
			double Cb = cb;
			double Cr = cr;

			int R = (int)(Y + 1.40200 * (Cr - 0x80));
			int G = (int)(Y - 0.34414 * (Cb - 0x80) - 0.71414 * (Cr - 0x80));
			int B = (int)(Y + 1.77200 * (Cb - 0x80));

			R = Math.Max(0, Math.Min(255, R));
			G = Math.Max(0, Math.Min(255, G));
			B = Math.Max(0, Math.Min(255, B));

			return new PixelInfo(R, G, B);
		}
	}
}
