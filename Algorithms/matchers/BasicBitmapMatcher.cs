using Algorithms.common;
using System;

namespace Algorithms.matchers
{
	public class BasicBitmapMatcher : BitmapMatcher
	{
		public BasicBitmapMatcher(EffectiveBitmap target) : base(target)
		{
		}

		public override double CalculateDifference(EffectiveBitmap bitmap)
		{
			bitmap = bitmap.Resize(target.Width, target.Height);

			double differenceSum = 0;
			target.RunOnEveryPixel((x, y) =>
			{
				var p1 = target.GetPixel(x, y);
				var p2 = bitmap.GetPixel(x, y);
				differenceSum += Math.Abs(p1.R - p2.R) + Math.Abs(p1.G - p2.G) + Math.Abs(p1.B - p2.B);
			});

			return differenceSum;
		}
	}
}
