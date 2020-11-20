using Algorithms;
using System;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class LsbViewModel : AlgorithmViewModel
	{
		private TextBox bitsTextBox;

		public LsbViewModel(MainWindow window) : base(window)
		{
		}

		public override void SetUp()
		{
			AddLabel("Bits for watermark", 0, 0);
			bitsTextBox = AddTextBox("2", 1, 0);
		}

		protected override void OnSubmit()
		{
			var p = ReadParameters();
			var algorithm = new Lsb(p);
			var result = algorithm.Run();
			ShowAlgorithmOutput(result);
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
