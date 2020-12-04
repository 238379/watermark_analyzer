using System.Drawing;

namespace Algorithms.common
{
	public class AlgorithmParameters
	{
		public readonly EffectiveBitmap Original;
		public readonly EffectiveBitmap Watermark;
		public readonly EffectiveBitmap Watermarked;

		public AlgorithmParameters(Bitmap original, Bitmap watermark, Bitmap watermarked)
		{
			Original = original.TransformToEffectiveBitmap();
			Watermark = watermark.Resize(original.Width, original.Height).TransformToEffectiveBitmap();  // TODO better method
			Watermarked = watermarked.TransformToEffectiveBitmap();
		}

		public AlgorithmParameters(Bitmap watermarked)
		{
			Watermarked = watermarked.TransformToEffectiveBitmap();
		}
	}
}
