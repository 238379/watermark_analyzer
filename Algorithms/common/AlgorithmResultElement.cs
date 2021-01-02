using System.Drawing;

namespace Algorithms.common
{
	public class AlgorithmResultElement
	{
		public readonly string Label;
		public readonly Bitmap Image;
		public ResultDescription Description;


		public AlgorithmResultElement(string label, EffectiveBitmap image, ResultDescription description) : this(label, image.ToBitmap(), description)
		{
		}

		public AlgorithmResultElement(string label, Bitmap image, ResultDescription description)
		{
			Label = label;
			Image = image;
			Description = description;
		}
	}
}
