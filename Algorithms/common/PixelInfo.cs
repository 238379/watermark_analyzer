using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithms
{
	public struct PixelInfo
	{
		public readonly byte R;
		public readonly byte G;
		public readonly byte B;
		public readonly byte A;

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
	}
}
