using Algorithms;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels.basic
{
	public class LsbViewModel : AlgorithmViewModel
	{
		private TextBox bitsTextBox;

		public LsbViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
			AddParameterLabel("Bits for watermark", 0, 0);
			bitsTextBox = AddParameterTextBox("2", 1, 0);
		}

		protected override Task ProcessAdding(CancellationToken ct)
		{
			return Task.Run(async () =>
			{
				ct.ThrowIfCancellationRequested();
				var p = ReadParameters();
				ct.ThrowIfCancellationRequested();
				var result = await new Lsb(p).AddWatermark(ct);
				ShowAlgorithmOutput(result);
			});
		}

		protected override Task ProcessRemoving(CancellationToken ct)
		{
			return Task.Run(async () =>
			{
				var p = ReadParameters();
				var algorithm = new Lsb(p);
				var result = await algorithm.RemoveWatermark(ct);
				ShowAlgorithmOutput(result);
			});
		}

		private LsbParameters ReadParameters()
		{
			var (original, watermark, watermarked) = ReadInputBitmaps();

			string text = null;
			dispatcher.Invoke(() => text = bitsTextBox.Text);

			if (int.TryParse(text, out var bitsForWatermark) && bitsForWatermark >= 1 && bitsForWatermark <= 8)
			{
				return new LsbParameters(original, watermark, watermarked, bitsForWatermark);
			}
			else
			{
				throw new ArgumentException($"Invalid bits for watermark value. It should be between [1; 8] but it is: {text}");
			}
		}
	}
}
