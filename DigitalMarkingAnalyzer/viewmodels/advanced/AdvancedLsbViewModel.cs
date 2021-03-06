﻿using Algorithms;
using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Common;
using Algorithms.matchers;
using DigitalMarkingAnalyzer.common;

namespace DigitalMarkingAnalyzer.viewmodels.advanced
{
	public class AdvancedLsbViewModel : AlgorithmViewModel
	{
		private CheckBox useOriginalImageCheckBox;
		private RangeParameterView<int> bitsRangeParameterControls;

		public AdvancedLsbViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
			AddParameterLabel("Find best", 0, 0);
			useOriginalImageCheckBox = AddParameterCheckBox(false, 1, 0);

			bitsRangeParameterControls = AddIntRangeParameter("Bits for watermark", 1, (1, 7), 1);
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

			var ratioValues = bitsRangeParameterControls.Read().Values();

			return ratioValues.Select(x => new LsbParameters(original, watermark, watermarked, x)).ToList();
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
