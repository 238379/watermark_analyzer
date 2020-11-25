using Algorithms;
using System;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class DwtViewModel : AlgorithmViewModel
	{
		private TextBox layersTextBox;

		public DwtViewModel(MainWindow window) : base(window)
		{
		}

		public override void SetUp()
		{
			AddLabel("Number of layers", 0, 0);
			layersTextBox = AddTextBox("2", 1, 0);
		}

		protected override void OnSubmit()
		{
			var p = ReadParameters();
			var algorithm = new Dwt(p);
			var result = algorithm.Run();
			ShowAlgorithmOutput(result);
		}

		private DwtParameters ReadParameters()
		{
			var (original, watermark) = ReadInputBitmaps();

			if (int.TryParse(layersTextBox.Text, out var layers) && layers >= 1 && layers <= 4)
			{
				return new DwtParameters(original, watermark, layers);
			}
			else
			{
				throw new ArgumentException($"Invalid number of layers value. It should be between [1; 4] but it is: {layersTextBox.Text}");
			}
		}
	}
}
