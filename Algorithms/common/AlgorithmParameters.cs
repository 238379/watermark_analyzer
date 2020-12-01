using System.Drawing;

namespace Algorithms.common
{
	public class AlgorithmParameters
	{
		public readonly Bitmap Original;
		public readonly Bitmap Watermark;
		public readonly Bitmap Watermarked;

		public AlgorithmParameters(Bitmap original, Bitmap watermark, Bitmap watermarked)
		{
			Original = original;
			Watermark = watermark.Resize(original.Width, original.Height);  // TODO better method
			Watermarked = watermarked;
		}

		public AlgorithmParameters(Bitmap watermarked)
		{
			Watermarked = watermarked;
		}
	}
}
