using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace Algorithms.common
{
	public class AlgorithmResult
	{
		public int Count => results.Count;

		private readonly List<AlgorithmResultElement> results;

		public AlgorithmResult()
		{
			this.results = new List<AlgorithmResultElement>();
		}

		public AlgorithmResult(List<AlgorithmResultElement> results)
		{
			this.results = results;
		}

		public AlgorithmResult(params AlgorithmResultElement[] results)
		{
			this.results = new List<AlgorithmResultElement>(results);
		}

		public AlgorithmResult(params (string, Bitmap)[] results)
		{
			this.results = new List<AlgorithmResultElement>(results.Select(x => new AlgorithmResultElement(x.Item1, x.Item2)));
		}

		public AlgorithmResult(params (string, EffectiveBitmap)[] results)
		{
			this.results = new List<AlgorithmResultElement>(results.Select(x => new AlgorithmResultElement(x.Item1, x.Item2.ToBitmap())));
		}

		public AlgorithmResultElement Get(int index)
		{
			return results[index];
		}

		public AlgorithmResultElement this[int i]
		{
			get { return results[i]; }
		}

		public void ForEach(Action<AlgorithmResultElement> action)
		{
			for(int i = 0; i < Count; i++)
			{
				action(results[i]);
			}
		}

		public void ForEach(Action<int, AlgorithmResultElement> action)
		{
			for (int i = 0; i < Count; i++)
			{
				action(i, results[i]);
			}
		}
	}
}
