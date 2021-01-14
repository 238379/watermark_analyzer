using Algorithms.common;
using Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Algorithms
{
	public class RecognizerParameters : AlgorithmParameters
	{
		public RecognizerParameters(EffectiveBitmap original, EffectiveBitmap watermark, EffectiveBitmap watermarked) : base(original, watermark, watermarked)
		{
		}

		public override string ToString() => string.Empty;
	}

	public class Recognizer : Algorithm
	{
		public const string ALGORITHM_NAME = "Recognizer";

		private readonly RecognizerParameters parameters;

		public Recognizer(RecognizerParameters parameters) : base(ALGORITHM_NAME, parameters)
		{
			this.parameters = parameters;
		}

		public override string ToString() => string.Empty;

		public override IAsyncEnumerable<AlgorithmResultElement> AddWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public override async IAsyncEnumerable<AlgorithmResultElement> RemoveWatermark([EnumeratorCancellation] CancellationToken ct)
		{
			ct.ThrowIfCancellationRequested();

			var tasks = new List<(Algorithm, IAsyncEnumerable<AlgorithmResultElement>)>();

			var lsbParams = new RangeParameter<int>(1, 7, 1).Values();
			foreach (var p in lsbParams)
			{
				var algorithm = new Lsb(new LsbParameters(parameters.Original, parameters.Watermark, parameters.Watermarked, p));
				tasks.Add((algorithm, algorithm.RemoveWatermark(ct)));
			}

			ct.ThrowIfCancellationRequested();

			var pixelAveragingParams = new RangeParameter<decimal>(0, 1, 0.1M).Values();
			foreach (var p in pixelAveragingParams)
			{
				var algorithm = new PixelAveraging(new PixelAveragingParameters(parameters.Original, parameters.Watermark, parameters.Watermarked, p));
				tasks.Add((algorithm, algorithm.RemoveWatermark(ct)));
			}

			ct.ThrowIfCancellationRequested();

			foreach (var t in tasks)
			{
				await foreach(var r in t.Item2)
				{
					r.Description = new ResultDescription(t.Item1.Description);
					yield return r;
				}
			}
		}
	}
}
