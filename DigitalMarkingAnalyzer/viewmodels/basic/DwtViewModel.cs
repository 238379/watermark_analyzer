using Algorithms;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels.basic
{
	public class DwtViewModel : AlgorithmViewModel
	{
		private TextBox layersTextBox;
		private TextBox alphaTextBox;

		public DwtViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
			AddParameterLabel("Number of layers", 0, 0);
			layersTextBox = AddParameterTextBox("2", 1, 0);
			AddParameterLabel("Alpha", 0, 1);
			alphaTextBox = AddParameterTextBox("0.01", 1, 1);
		}

		protected override Task ProcessAdding(CancellationToken ct)
		{
			return Task.Run(async () =>
			{
				ct.ThrowIfCancellationRequested();
				var p = ReadParameters();
				ct.ThrowIfCancellationRequested();
				var result = await new Dwt(p).AddWatermark(ct);
				ShowAlgorithmOutput(result);
			});
		}

		protected override Task ProcessRemoving(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		private DwtParameters ReadParameters()
		{
			var (original, watermark, watermarked) = ReadInputBitmaps();

			string text = null;
			dispatcher.Invoke(() => text = layersTextBox.Text);

			if (int.TryParse(text, out var layers) && layers >= 1 && layers <= 2)
			{
				dispatcher.Invoke(() => text = alphaTextBox.Text);
				if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var alpha) && alpha >= 0 && alpha <= 1)
					return new DwtParameters(original, watermark, watermarked, layers, alpha);
				throw new ArgumentException($"Invalid alpha value. It should be between [0; 1] but it is: {text}");
			}
			else
			{
				throw new ArgumentException($"Invalid number of layers value. It should be between [1; 2] but it is: {text}");
			}
		}
	}
}
