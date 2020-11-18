using Algorithms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class PixelAveragingViewModel : AlgorithmViewModel
	{
		private TextBox ratioTextBox;

		public PixelAveragingViewModel(Grid grid) : base(grid)
		{
		}

		public override void PrepareControlls()
		{
			AddLabel("Ratio", 0, 0);
			ratioTextBox = AddTextBox("0.5", 1, 0);
		}

		public override Dictionary<string, dynamic> ReadParameters()
		{
			if (double.TryParse(ratioTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var ratio) && ratio >= 0 && ratio <= 1)
			{
				return new Dictionary<string, dynamic> { { PixelAveraging.RATIO_PARAM, ratio } };
			}
			else
			{
				throw new ArgumentException($"Invalid value for ratio. It should be between [0; 1] but it is: {ratioTextBox.Text}");
			}
		}
	}
}
