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
using DigitalMarkingAnalyzer.common;

namespace DigitalMarkingAnalyzer.viewmodels.advanced
{
	public class RecognizerViewModel : AlgorithmViewModel
	{
		public RecognizerViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
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
				var lsbResults = ps.Item1.Select(p => new Lsb(p).RemoveWatermark(ct));
				var pixelAveraginResults = ps.Item2.Select(p => new PixelAveraging(p).RemoveWatermark(ct));
				var results = lsbResults.Concat(pixelAveraginResults);

				foreach (var result in results)
				{
					ct.ThrowIfCancellationRequested();
					await ShowAlgorithmOutput(result);
				}

				ct.ThrowIfCancellationRequested();
				var theBestResult = await GuessResult(results, ps.Item1.First().Original, ct);
				theBestResult.First().Description = new ResultDescription("Best result: " + theBestResult.First().Description); // hack
				await ShowAlgorithmOutput(theBestResult.ToIAsyncEnumerable());
			});
		}

		private (List<LsbParameters>, List<PixelAveragingParameters>) PrepareParameters()
		{
			var (original, watermark, watermarked) = ReadInputBitmaps();

			var lsbParams = new RangeParameter<int>(1, 7, 1).Values();
			var pixelAveragingParams = new RangeParameter<decimal>(0, 1, 0.1M).Values();

			return (lsbParams.Select(x => new LsbParameters(original, watermark, watermarked, x)).ToList(),
				pixelAveragingParams.Select(x => new PixelAveragingParameters(original, watermark, watermarked, x)).ToList());
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
