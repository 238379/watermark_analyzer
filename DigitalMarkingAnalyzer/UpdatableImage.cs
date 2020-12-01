using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DigitalMarkingAnalyzer
{
	public class UpdatableImage
	{
		private readonly System.Windows.Controls.Image image;
		public ImageSource Source => image.Source;

		public delegate void ImageSourceChangedEventHandler(object source, ImageSourceChangedEventArgs args);
		public event ImageSourceChangedEventHandler SourceChanged;

		public UpdatableImage(System.Windows.Controls.Image image)
		{
			this.image = image;
		}

		public void SetSource(ImageSource source)
		{
			var oldImageSource = image.Source;
			image.Source = source;
			SourceChanged?.Invoke(this, new ImageSourceChangedEventArgs(oldImageSource, source));
		}

		public void SetSource(Bitmap bitmap)
		{
			SetSource(InterfaceTools.BitmapToImageSource(bitmap));
		}

		public void SetSource(Uri uri)
		{
			SetSource(new BitmapImage(uri));
		}
	}

	public class ImageSourceChangedEventArgs : EventArgs
	{
		public ImageSource OldImageSource;
		public ImageSource NewImageSource;

		public ImageSourceChangedEventArgs(ImageSource oldImageSource, ImageSource newImageSource)
		{
			OldImageSource = oldImageSource;
			NewImageSource = newImageSource;
		}
	}
}
