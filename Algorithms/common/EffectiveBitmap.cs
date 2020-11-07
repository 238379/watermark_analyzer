using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithms
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

		public PixelInfo GetPixel(int x, int y)
		{
			var offset = ((y * Width) + x) * Depth;
			return new PixelInfo(buffer[offset + 0], buffer[offset + 1], buffer[offset + 2]);
		}
	}
}
