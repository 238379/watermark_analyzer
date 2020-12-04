using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Algorithms.common
{
	public class BitmapOperations
	{
		// TODO what if they are not the same size?
		// TODO obsolete
		public static EffectiveBitmap Create(Func<EffectiveBitmap[], int, int, PixelInfo> creator, params EffectiveBitmap[] sources)
		{
			var source = sources[0];
			var buffer = new byte[source.Width * source.Height * source.GetDepth()];

			Process(sources, buffer, source.Width, source.Height, source.GetDepth(), creator);

			return new EffectiveBitmap(source.Width, source.Height, source.Depth, buffer);
		}

		private static void Process(EffectiveBitmap[] sources, byte[] buffer, int width, int height, int depth,
			Func<EffectiveBitmap[], int, int, PixelInfo> creator)
		{
			Parallel.For(0, width, (i) => {
				Parallel.For(0, height, (j) => {
					var result = creator(sources, i, j);
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
