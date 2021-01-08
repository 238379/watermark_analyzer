using Algorithms;
using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Common;
using Algorithms.matchers;

namespace DigitalMarkingAnalyzer.viewmodels.advanced
{
	public class AdvancedLsbViewModel : AlgorithmViewModel
	{
		private CheckBox useOriginalImageCheckBox;
		private TextBox minBitsTextBox;
		private TextBox maxBitsTextBox;

		public AdvancedLsbViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
			AddParameterLabel("Use original bitmap", 0, 0);
			useOriginalImageCheckBox = AddParameterCheckBox(false, 1, 0);

			AddParameterLabel("Min bits for watermark", 0, 1);
			minBitsTextBox = AddParameterTextBox("1", 1, 1);

			AddParameterLabel("Max bits for watermark", 0, 2);
			maxBitsTextBox = AddParameterTextBox("7", 1, 2);
		}

		protected override Task ProcessAdding(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		protected override Task ProcessRemoving(CancellationToken ct)
		{
			return Task.Run(async () =>
			{
				ct.ThrowIfCancellationRequested();

				var ps = PrepareParameters();
				var results = ps.Select(p => new Lsb(p).RemoveWatermark(ct));

				foreach (var result in results)
				{
					ct.ThrowIfCancellationRequested();
					await ShowAlgorithmOutput(result);
				}
				bool? isChecked = null;
				dispatcher.Invoke(() => isChecked = useOriginalImageCheckBox.IsChecked);
				if (isChecked.Value)
				{
					ct.ThrowIfCancellationRequested();
					var theBestResult = await GuessResult(results, ps.First().Original, ct);
					theBestResult.First().Description = new ResultDescription("Best result: " + theBestResult.First().Description);	// hack
					await ShowAlgorithmOutput(theBestResult.ToIAsyncEnumerable());
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

		private Task<List<AlgorithmResultElement>> GuessResult(IEnumerable<IAsyncEnumerable<AlgorithmResultElement>> results, EffectiveBitmap target, CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();
			var evaluatedResults = results.Select(async x => await x.ToListAsync()).ToList();

			var bitmapMatcher = new BasicBitmapMatcher(target);

			var bests = evaluatedResults.OrderBy(x => bitmapMatcher
				.CalculateDifference(
					x.Result.Where(r => r.Label == "Cleaned").First().Image.TransformToEffectiveBitmap()
				)
			);

			return bests.First(); ;
		}
	}
}
