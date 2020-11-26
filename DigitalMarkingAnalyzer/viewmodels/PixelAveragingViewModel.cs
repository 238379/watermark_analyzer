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

		public PixelAveragingViewModel(MainWindow window) : base(window)
		{
		}

		public override void SetUp()
		{
			AddParameterLabel("Ratio", 0, 0);
			ratioTextBox = AddParameterTextBox("0.5", 1, 0);
		}

		protected override void OnSubmit()
		{
			var p = ReadParameters();
			var algorithm = new PixelAveraging(p);
			var result = algorithm.Run();
			ShowAlgorithmOutput(result);
		}

		private PixelAveragingParameters ReadParameters()
		{
			var (original, watermark) = ReadInputBitmaps();

			if (double.TryParse(ratioTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var ratio) && ratio >= 0 && ratio <= 1)
			{
				return new PixelAveragingParameters(original, watermark, ratio);
			}
			else
			{
				throw new ArgumentException($"Invalid value for ratio. It should be between [0; 1] but it is: {ratioTextBox.Text}");
			}
		}
	}
}
