using System.Drawing;

namespace Algorithms.common
{
	public class AlgorithmParameters
	{
		public readonly EffectiveBitmap Original;
		public readonly EffectiveBitmap Watermark;
		public readonly EffectiveBitmap Watermarked;

		public AlgorithmParameters(EffectiveBitmap original, EffectiveBitmap watermark, EffectiveBitmap watermarked)
		{
			Original = original;
			Watermark = watermark;
			Watermarked = watermarked;
		}

		public AlgorithmParameters(Bitmap watermarked)
		{
			Watermarked = watermarked.TransformToEffectiveBitmap();
		}
	}
}
