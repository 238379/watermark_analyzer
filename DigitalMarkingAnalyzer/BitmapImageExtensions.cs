using System.Drawing;
using System.Windows.Media.Imaging;

namespace DigitalMarkingAnalyzer
{
	public static class BitmapImageExtensions
	{
		public static Bitmap ToBitmap(this BitmapImage that)
		{
			return InterfaceTools.BitmapImageToBitmap(that);
		}
	}
}
