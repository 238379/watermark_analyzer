﻿using System.Drawing;

namespace Algorithms.common
{
	public class AlgorithmResultElement
	{
		public readonly string Label;
		public readonly Bitmap Image;

		public AlgorithmResultElement(string label, Bitmap image)
		{
			Label = label;
			Image = image;
		}
	}
}
