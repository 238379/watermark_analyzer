using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Algorithms.common
{
	public abstract class Algorithm
	{
		public static Algorithm Create(string algorithmName, Dictionary<string, dynamic> parameters)
		{
			return algorithmName switch
			{
				Lsb.ALGORITHM_NAME => new Lsb(parameters),
				PixelAveraging.ALGORITHM_NAME => new PixelAveraging(parameters),
				Dft.ALGORITHM_NAME => new Dft(parameters),
				_ => throw new ArgumentException($"Unknown algorithmName '{algorithmName}'."),
			};
		}

		public Algorithm(Dictionary<string, dynamic> parameters)
		{

		}

		public abstract AlgorithmResult Run(Bitmap original, Bitmap watermark);
	}
}
