using System;
using System.Collections.Generic;

namespace Algorithms.common
{
	public class RangeParameter<T>
	{
		private readonly T min;
		private readonly T max;
		private readonly T interval;

		public RangeParameter(T min, T max, T interval)
		{
			this.min = min;
			this.max = max;
			this.interval = interval;
		}

		public List<T> Values()
		{
			var values = new List<T>();

			for(dynamic val = min; val <= max; val += interval)
			{
				values.Add(val);
			}

			if (Math.Abs((double)(dynamic)max - (double)(dynamic)values[values.Count - 1]) >= 0.00001)
			{
				values.Add(max);
			}

			return values;
		}
	}
}
