using Algorithms;
using System;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public abstract class AlgorithmViewModel : ViewModel
	{
		public static AlgorithmViewModel Create(string algorithmName, Grid grid)
		{
			return algorithmName switch
			{
				Lsb.ALGORITHM_NAME => new LsbViewModel(grid),
				PixelAveraging.ALGORITHM_NAME => new PixelAveragingViewModel(grid),
				_ => throw new ArgumentException($"Unknown algorithmName '{algorithmName}'."),
			};
		}

		public AlgorithmViewModel(Grid grid) : base(grid)
		{
		}
	}
}
