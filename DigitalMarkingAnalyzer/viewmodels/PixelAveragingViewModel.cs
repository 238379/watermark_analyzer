using Algorithms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class PixelAveragingViewModel : AlgorithmViewModel
	{
		private TextBox ratioTextBox;

		public PixelAveragingViewModel(Grid grid, TextBlock errorTextBlock, System.Windows.Controls.Image originalImageContainer, System.Windows.Controls.Image watermarkImageContainer)
			: base(grid, errorTextBlock, originalImageContainer, watermarkImageContainer)
		{
		}

		public override void SetUp()
		{
			AddLabel("Ratio", 0, 0);
			ratioTextBox = AddTextBox("0.5", 1, 0);
		}

		protected override void OnSubmit()
		{
			var p = ReadParameters();
			var algorithm = new PixelAveraging(p);
			var (originalAsBitmap, watermarkAsBitmap) = ReadInputBitmaps();
			algorithmResult = algorithm.Run(originalAsBitmap, watermarkAsBitmap);
		}

		private Dictionary<string, dynamic> ReadParameters()
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
