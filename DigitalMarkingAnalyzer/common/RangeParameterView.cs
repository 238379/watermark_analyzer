using Algorithms.common;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DigitalMarkingAnalyzer.common
{
	public class RangeParameterView<T>
	{
		private readonly string paramName;
		private readonly (dynamic, dynamic) allowedRange;
		private readonly Label label;
		private readonly TextBox min;
		private readonly TextBox max;
		private readonly TextBox interval;
		private readonly Dispatcher dispatcher;

		public RangeParameterView(string paramName, (dynamic, dynamic) allowedRange, Label label, TextBox min, TextBox max, TextBox interval, Dispatcher dispatcher)
		{
			if (allowedRange.Item1 >= allowedRange.Item2)
				throw new ArgumentOutOfRangeException();
			this.paramName = paramName;
			this.allowedRange = allowedRange;
			this.label = label;
			this.min = min;
			this.max = max;
			this.interval = interval;
			this.dispatcher = dispatcher;
		}

		public RangeParameter<T> Read()
		{
			string minText = null, maxText = null, intervalText = null;
			dispatcher.Invoke(() => {
				minText = min.Text;
				maxText = max.Text;
				intervalText = interval.Text;
			});

			if (double.TryParse(minText, NumberStyles.Any, CultureInfo.InvariantCulture, out var minVal) && minVal >= allowedRange.Item1 && minVal < allowedRange.Item2)
			{
				if (double.TryParse(maxText, NumberStyles.Any, CultureInfo.InvariantCulture, out var maxVal) && maxVal > allowedRange.Item1 && maxVal <= allowedRange.Item2)
				{
					var intevalAllowedRange = (0.001, (allowedRange.Item2 - allowedRange.Item1));
					if (double.TryParse(intervalText, NumberStyles.Any, CultureInfo.InvariantCulture, out var intervalVal) && intervalVal >= intevalAllowedRange.Item1 && intervalVal <= intevalAllowedRange.Item2)
					{
						return new RangeParameter<T>((T)Convert.ChangeType(minVal, typeof(T)), (T)Convert.ChangeType(maxVal, typeof(T)), (T)Convert.ChangeType(intervalVal, typeof(T)));
					}
					else
					{
						throw new ArgumentException($"Interval value for parameter {paramName} is invalid. It should be between [{intevalAllowedRange.Item1}; {intevalAllowedRange.Item2}] but it is: {intervalText}");
					}
				}
				else
				{
					throw new ArgumentException($"Invalid max {paramName} value. It should be between ({allowedRange.Item1}; {allowedRange.Item2}] but it is: {maxText}");
				}
			}
			else
			{
				throw new ArgumentException($"Invalid min {paramName} value. It should be between [{allowedRange.Item1}; {allowedRange.Item2}) but it is: {minText}");
			}
		}
	}
}
