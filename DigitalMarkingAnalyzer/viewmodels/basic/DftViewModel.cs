using Algorithms;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels.basic
{
	public class DftViewModel : AlgorithmViewModel
	{
		private TextBox keyTextBox;
		private TextBox alphaTextBox;

		public DftViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{

		}

		public override void SetUp()
		{
			AddParameterLabel("Key", 0, 0);
			keyTextBox = AddParameterTextBox("10", 1, 0);
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
				var algorithm = new Dft(p);
				var result = algorithm.AddWatermark(ct);
				await ShowAlgorithmOutput(result, algorithm.Description);
			});
		}

		protected override Task ProcessRemoving(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		private DftParameters ReadParameters()
		{
			var (original, watermark, watermarked) = ReadInputBitmaps();

			string text = null;
			dispatcher.Invoke(() => text = keyTextBox.Text);

			if (int.TryParse(text, out var key) && key >= 0)
			{
				dispatcher.Invoke(() => text = alphaTextBox.Text);
				if (double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var alpha) && alpha >= 0 && alpha <= 1)
					return new DftParameters(original, watermark, watermarked, key, alpha);
				throw new ArgumentException($"Invalid alpha value. It should be between [0; 1] but it is: {text}");
			}
			else
			{
				throw new ArgumentException($"Invalid number of layers value. It should be greater or equal 0 but it is: {text}");
			}
		}
	}
}
