using System;
using System.Drawing;

namespace Algorithms
{
	public static class BitmapExtensions
	{
		public static void RunOnEveryPixel(this Bitmap that, Action<int, int> action)
		{
			for (int i = 0; i < that.Width; i++)
			{
				for (int j = 0; j < that.Height; j++)
				{
					action(i, j);
				}
			}
		}
	}
}
