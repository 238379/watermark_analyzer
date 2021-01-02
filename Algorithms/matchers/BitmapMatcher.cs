using Algorithms.common;

namespace Algorithms.matchers
{
	public abstract class BitmapMatcher
	{
		protected readonly EffectiveBitmap target;

		public BitmapMatcher(EffectiveBitmap target)
		{
			this.target = target;
		}

		public abstract double CalculateDifference(EffectiveBitmap bitmap);
	}
}