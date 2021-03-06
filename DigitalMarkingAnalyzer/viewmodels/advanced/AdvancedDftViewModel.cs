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
	public class AdvancedDftViewModel : AlgorithmViewModel
	{
		private RangeParameterView<int> keyRangeParameterControls;
		private RangeParameterView<decimal> alphaRangeParameterControls;

		public AdvancedDftViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
			keyRangeParameterControls = AddIntRangeParameter("Key", 1, (0, 5), 1);
			alphaRangeParameterControls = AddDecimalRangeParameter("Alpha", 2, (0, 1), 0.2);
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
				var results = ps.Select(p => new Dft(p).RemoveWatermark(ct));

				foreach (var result in results)
				{
					ct.ThrowIfCancellationRequested();
					await ShowAlgorithmOutput(result);
				}
			});
		}

		private List<DftParameters> PrepareParameters()
		{
			var (original, watermark, watermarked) = ReadInputBitmaps();

			var keyValues = keyRangeParameterControls.Read().Values();
			var alphaValues = alphaRangeParameterControls.Read().Values();
			var combinedValues = keyValues.Combine(alphaValues);

			return combinedValues.Select(x => new DftParameters(original, watermark, watermarked, x.Item1, x.Item2)).ToList();
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
