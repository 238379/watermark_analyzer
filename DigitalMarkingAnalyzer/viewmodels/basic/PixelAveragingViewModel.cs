using Algorithms;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels.basic
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

		protected override Task ProcessAdding(CancellationToken ct)
		{
			return Task.Run(async () =>
			{
				ct.ThrowIfCancellationRequested();
				var p = ReadParameters();
				ct.ThrowIfCancellationRequested();
				var result = new PixelAveraging(p).AddWatermark(ct);
				await AppendAlgorithmOutput(result);
			});
		}

		protected override Task ProcessRemoving(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		private PixelAveragingParameters ReadParameters()
		{
			var (original, watermark, watermarked) = ReadInputBitmaps();

			string text = null;
			dispatcher.Invoke(() => text = ratioTextBox.Text);

			if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var ratio) && ratio >= 0 && ratio <= 1)
			{
				return new PixelAveragingParameters(original, watermark, watermarked, ratio);
			}
			else
			{
				throw new ArgumentException($"Invalid value for ratio. It should be between [0; 1] but it is: {text}");
			}
		}
	}
}
