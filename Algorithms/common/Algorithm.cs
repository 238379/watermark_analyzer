using System.Collections.Generic;
using System.Threading;

namespace Algorithms.common
{
	public abstract class Algorithm
	{
		public Algorithm()
		{
		}

		public abstract IAsyncEnumerable<AlgorithmResultElement> AddWatermark(CancellationToken ct);
		public abstract IAsyncEnumerable<AlgorithmResultElement> RemoveWatermark(CancellationToken ct);
	}

	public enum AlgorithmMode
	{
		AddWatermark,
		RemoveWatermark
	}
}
