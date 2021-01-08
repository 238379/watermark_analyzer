using System.Collections.Generic;
using System.Threading;

namespace Algorithms.common
{
	public abstract class Algorithm
	{
		private readonly string name;
		private readonly AlgorithmParameters parameters;

		public Algorithm(string name, AlgorithmParameters parameters)
		{
			this.name = name;
			this.parameters = parameters;
		}

		public string Description => $"{name} {parameters}";

		public abstract IAsyncEnumerable<AlgorithmResultElement> AddWatermark(CancellationToken ct);
		public abstract IAsyncEnumerable<AlgorithmResultElement> RemoveWatermark(CancellationToken ct);
	}

	public enum AlgorithmMode
	{
		AddWatermark,
		RemoveWatermark
	}
}
