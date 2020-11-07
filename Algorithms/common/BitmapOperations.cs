﻿using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Algorithms
{
	public class BitmapOperations
	{
		// TODO what if they are not the same size?
		public static Bitmap Create(Func<EffectiveBitmap[], int, int, PixelInfo> creator, params Bitmap[] sources)
		{
			var effectiveBitmaps = TransformToEffectiveBitmaps(sources);

			var source = sources[0];
			var buffer = new byte[source.Width * source.Height * source.GetDepth()];

			Process(effectiveBitmaps, buffer, source.Width, source.Height, source.Width, source.GetDepth(), creator);

			var resultBitmap = new Bitmap(source.Width, source.Height, source.PixelFormat);

			var resultData = resultBitmap.LockAllBitsReadWrite();
			Marshal.Copy(buffer, 0, resultData.Scan0, buffer.Length);
			resultBitmap.UnlockBits(resultData);

			return resultBitmap;
		}

		private static void Process(EffectiveBitmap[] sources, byte[] buffer, int endx, int endy, int width, int depth,
			Func<EffectiveBitmap[], int, int, PixelInfo> creator)
		{
			for (int i = 0; i < endx; i++)
			{
				for (int j = 0; j < endy; j++)
				{
					var result = creator(sources, i, j);
					var offset = ((j * width) + i) * depth;
					buffer[offset + 0] = result.R;
					buffer[offset + 1] = result.G;
					buffer[offset + 2] = result.B;
					if(depth > 3)
					{
						buffer[offset + 3] = result.A;
					}
				}
			}
		}

		private static EffectiveBitmap[] TransformToEffectiveBitmaps(Bitmap[] sources)
		{
			var effectiveBitmaps = new EffectiveBitmap[sources.Length];
			for (int i = 0; i < sources.Length; i++)
			{
				effectiveBitmaps[i] = sources[i].TransformToEffectiveBitmap();
			}
			return effectiveBitmaps;
		}
	}
}
