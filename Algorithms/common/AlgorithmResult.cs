using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Algorithms.common
{
	public class AlgorithmResult
	{
		public readonly Bitmap Watermarked;
		public readonly Bitmap Cleaned;
		public readonly Bitmap ExtractedWatermark;

		public AlgorithmResult(Bitmap watermarked, Bitmap cleaned, Bitmap extractedWatermark)
		{
			Watermarked = watermarked;
			Cleaned = cleaned;
			ExtractedWatermark = extractedWatermark;
		}
	}
}
