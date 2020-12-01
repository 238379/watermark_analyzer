namespace Algorithms.common
{
	public abstract class Algorithm
	{
		public Algorithm()
		{
		}

		public abstract AlgorithmResult AddWatermark();
		public abstract AlgorithmResult RemoveWatermark();
	}

	public enum AlgorithmMode
	{
		AddWatermark,
		RemoveWatermark
	}
}
