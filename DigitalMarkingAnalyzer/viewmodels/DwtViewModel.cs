using Algorithms;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class DwtViewModel : AlgorithmViewModel
	{
		private TextBox layersTextBox;
		private TextBox alphaTextBox;

		public DwtViewModel(AlgorithmControls algorithmControls, TextBlock errorMessageTextBlock) : base(algorithmControls, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
			AddParameterLabel("Number of layers", 0, 0);
			layersTextBox = AddParameterTextBox("2", 1, 0);
			AddParameterLabel("Alpha", 0, 1);
			alphaTextBox = AddParameterTextBox("0.01", 1, 1);
		}

		protected override void ProcessAdding()
		{
			var p = ReadParameters();
			var algorithm = new Dwt(p);
			var result = algorithm.AddWatermark();
			ShowAlgorithmOutput(result);
		}

		protected override void ProcessRemoving()
		{
			throw new NotImplementedException();
		}

		private DwtParameters ReadParameters()
		{
			var (original, watermark) = ReadInputBitmaps();

			if (int.TryParse(layersTextBox.Text, out var layers) && layers >= 1 && layers <= 2)
			{
				if (double.TryParse(alphaTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var alpha) && alpha >= 0 && alpha <= 1)
					return new DwtParameters(original, watermark, layers, alpha);
				throw new ArgumentException($"Invalid alpha value. It should be between [0; 1] but it is: {alphaTextBox.Text}");
			}
			else
			{
				throw new ArgumentException($"Invalid number of layers value. It should be between [1; 2] but it is: {layersTextBox.Text}");
			}
		}
	}
}
