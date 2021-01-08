using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms.common
{
	public abstract class AlgorithmRunner<A, P> where A : Algorithm where P : AlgorithmParameters
	{
		public AlgorithmRunner()
		{
		}

		public abstract Task<AlgorithmResult> RemoveWatermark();
	}
}
