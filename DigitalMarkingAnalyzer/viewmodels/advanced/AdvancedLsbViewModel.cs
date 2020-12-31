using Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels.advanced
{
	public class AdvancedLsbViewModel : AlgorithmViewModel
	{
		private TextBox minBitsTextBox;
		private TextBox maxBitsTextBox;

		public AdvancedLsbViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
			AddParameterLabel("Min bits for watermark", 0, 0);
			minBitsTextBox = AddParameterTextBox("1", 1, 0);
			AddParameterLabel("Max bits for watermark", 0, 1);
			maxBitsTextBox = AddParameterTextBox("7", 1, 1);
		}

		protected override Task ProcessAdding(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		protected override Task ProcessRemoving(CancellationToken ct)
		{
			return Task.Run(async () =>
			{
				var ps = PrepareParameters();
				var results = ps.Select(p => {
					var algorithm = new Lsb(p);
					return new { Description=algorithm.Description, Results=algorithm.RemoveWatermark(ct) };
				});
				foreach(var result in results)
				{
					await ShowAlgorithmOutput(result.Results, result.Description);
				}
			});
		}

		private List<LsbParameters> PrepareParameters()
		{
			var (original, watermark, watermarked) = ReadInputBitmaps();

			string text = null;
			dispatcher.Invoke(() => text = minBitsTextBox.Text);

			if (int.TryParse(text, out var minBits) && minBits >= 1 && minBits <= 6)
			{
				dispatcher.Invoke(() => text = maxBitsTextBox.Text);
				if (int.TryParse(text, out var maxBits) && maxBits >= 2 && maxBits <= 7)
				{
					if (minBits < maxBits)
					{
						var list = new List<LsbParameters>();
						for(var p = minBits; p <= maxBits; p++)
						{
							list.Add(new LsbParameters(original, watermark, watermarked, p));
						}
						return list;
					}
					else
					{
						throw new ArgumentException($"Max bits for watermark have to be greater than min bits.");
					}
				}
				else
				{
					throw new ArgumentException($"Invalid max bits for watermark value. It should be between [2; 7] but it is: {text}");
				}
			}
			else
			{
				throw new ArgumentException($"Invalid min bits for watermark value. It should be between [1; 6] but it is: {text}");
			}
		}
	}
}
