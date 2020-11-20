using System.Drawing;

namespace Algorithms.common
{
	public class AlgorithmParameters
	{
		public readonly Bitmap Original;
		public readonly Bitmap Watermark;

		public AlgorithmParameters(Bitmap original, Bitmap watermark)
		{
			Original = original;
			Watermark = watermark.Resize(original.Width, original.Height);	// TODO better method
		}
	}
}
