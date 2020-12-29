using Algorithms;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class PixelAveragingViewModel : AlgorithmViewModel
	{
		private TextBox ratioTextBox;

		public PixelAveragingViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
			AddParameterLabel("Ratio", 0, 0);
			ratioTextBox = AddParameterTextBox("0.5", 1, 0);
		}

		protected override void ProcessAdding()
		{
			var p = ReadParameters();
			var algorithm = new PixelAveraging(p);
			var result = algorithm.AddWatermark().GetAwaiter().GetResult();
			ShowAlgorithmOutput(result);
		}

		protected override void ProcessRemoving()
		{
			throw new NotImplementedException();
		}

		private PixelAveragingParameters ReadParameters()
		{
			var (original, watermark, watermarked) = ReadInputBitmaps();

			if (double.TryParse(ratioTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var ratio) && ratio >= 0 && ratio <= 1)
			{
				return new PixelAveragingParameters(original, watermark, watermarked, ratio);
			}
			else
			{
				throw new ArgumentException($"Invalid value for ratio. It should be between [0; 1] but it is: {ratioTextBox.Text}");
			}
		}
	}
}
