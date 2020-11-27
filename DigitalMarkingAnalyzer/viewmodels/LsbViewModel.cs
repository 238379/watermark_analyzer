using Algorithms;
using System;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class LsbViewModel : AlgorithmViewModel
	{
		private TextBox bitsTextBox;

		public LsbViewModel(AlgorithmControls algorithmControls, TextBlock errorMessageTextBlock) : base(algorithmControls, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
			AddParameterLabel("Bits for watermark", 0, 0);
			bitsTextBox = AddParameterTextBox("2", 1, 0);
		}

		protected override void ProcessAdding()
		{
			var p = ReadParameters();
			var algorithm = new Lsb(p);
			var result = algorithm.AddWatermark();
			ShowAlgorithmOutput(result);
		}

		protected override void ProcessRemoving()
		{
			throw new NotImplementedException();
		}

		private LsbParameters ReadParameters()
		{
			var (original, watermark) = ReadInputBitmaps();

			if (int.TryParse(bitsTextBox.Text, out var bitsForWatermark) && bitsForWatermark >= 1 && bitsForWatermark <= 8)
			{
				return new LsbParameters(original, watermark, bitsForWatermark);
			}
			else
			{
				throw new ArgumentException($"Invalid bits for watermark value. It should be between [1; 8] but it is: {bitsTextBox.Text}");
			}
		}
	}
}
