using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Threading;

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

		public override IAsyncEnumerable<AlgorithmResultElement> AddWatermark(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public override IAsyncEnumerable<AlgorithmResultElement> RemoveWatermark(CancellationToken ct)
		{
			return new Lsb(
				new LsbParameters(parameters.Original, parameters.Watermark, parameters.Watermarked, 2)
				).RemoveWatermark(ct);
		}
	}
}
