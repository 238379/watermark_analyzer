using System.Threading;
using System.Threading.Tasks;

namespace Algorithms.common
{
	public abstract class Algorithm
	{
		public Algorithm()
		{
		}

		public abstract Task<AlgorithmResult> AddWatermark(CancellationToken ct);
		public abstract Task<AlgorithmResult> RemoveWatermark(CancellationToken ct);
	}

	public enum AlgorithmMode
	{
		AddWatermark,
		RemoveWatermark
	}
}
