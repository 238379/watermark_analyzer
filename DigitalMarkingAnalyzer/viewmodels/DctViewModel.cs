using Algorithms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class DctViewModel : AlgorithmViewModel
	{
		private TextBox keyTextBox;
		private TextBox alphaTextBox;

		public DctViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{

		}

		public override void SetUp()
		{
			AddParameterLabel("Key", 0, 0);
			keyTextBox = AddParameterTextBox("10", 1, 0);
			AddParameterLabel("Alpha", 0, 1);
			alphaTextBox = AddParameterTextBox("0.01", 1, 1);
		}

		protected override void ProcessAdding()
		{
			var p = ReadParameters();
			var algorithm = new Dct(p);
			var result = algorithm.AddWatermark();
			ShowAlgorithmOutput(result);
		}

		protected override void ProcessRemoving()
		{
			throw new NotImplementedException();
		}

		private DctParameters ReadParameters()
		{
			var (original, watermark, watermarked) = ReadInputBitmaps();

			if (int.TryParse(keyTextBox.Text, out var key) && key >= 0)
			{
				if (double.TryParse(alphaTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out var alpha) && alpha >= 0 && alpha <= 1)
                {
					return new DctParameters(original, watermark, watermarked, key, alpha);
				}
				throw new ArgumentException($"Invalid alpha value. It should be between [0; 1] but it is: {alphaTextBox.Text}");
			}
			else
			{
				throw new ArgumentException($"Invalid number of layers value. It should be greater or equal 0 but it is: {keyTextBox.Text}");
			}
		}
	}
}
