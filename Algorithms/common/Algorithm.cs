using System.Threading.Tasks;

namespace Algorithms.common
{
	public abstract class Algorithm
	{
		public Algorithm()
		{
		}

		public abstract Task<AlgorithmResult> AddWatermark();
		public abstract Task<AlgorithmResult> RemoveWatermark();
	}

	public enum AlgorithmMode
	{
		AddWatermark,
		RemoveWatermark
	}
}
