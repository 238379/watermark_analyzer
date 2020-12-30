using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Algorithms.common
{
	public class AsyncAlgorithmResult
	{
		public Task<List<AlgorithmResult>> Task => taskCompletionSource.Task;

		private readonly List<AlgorithmResult> algorithmResults;
		private readonly Action<AlgorithmResult> onNextResult;
		private readonly TaskCompletionSource<List<AlgorithmResult>> taskCompletionSource = new TaskCompletionSource<List<AlgorithmResult>>();

		public AsyncAlgorithmResult(Action<AlgorithmResult> onNextResult)
		{
			this.algorithmResults = new List<AlgorithmResult>();
			this.onNextResult = onNextResult;
		}

		public void AddResult(AlgorithmResult result)
		{
			algorithmResults.Add(result);
			onNextResult?.Invoke(result);	// TODO consider making async
		}

		public void Complete(AlgorithmResult result)
		{
			AddResult(result);
			Complete();
		}

		public void Complete()
		{
			taskCompletionSource.SetResult(algorithmResults);
		}
	}
}
