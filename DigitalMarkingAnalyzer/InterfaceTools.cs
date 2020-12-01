using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DigitalMarkingAnalyzer
{
	public class InterfaceTools
	{
		public static void SaveImageToFile(System.Windows.Controls.Image image, string pathToSave)
		{
			var encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create((BitmapSource)image.Source));
			using (FileStream stream = new FileStream(pathToSave, FileMode.OpenOrCreate))
			{
				encoder.Save(stream);
			}
		}

		public static void SetImageFromDrive(System.Windows.Controls.Image imageContainer)
		{
			var path = GetFilePathFromDialog();
			if (path != null)
			{
				SetImageFromUri(imageContainer, new Uri(path, UriKind.Absolute));
			}
		}

		public static void SetImageFromDrive(UpdatableImage imageContainer)
		{
			var path = GetFilePathFromDialog();
			if (path != null)
			{
				imageContainer.SetSource(new Uri(path, UriKind.Absolute));
			}
		}

		public static void SetImageFromBitmap(System.Windows.Controls.Image imageContainer, Bitmap bitmap)
		{
			imageContainer.SetCurrentValue(System.Windows.Controls.Image.SourceProperty, BitmapToImageSource(bitmap));
		}

		public static void SetImageFromUri(System.Windows.Controls.Image imageContainer, Uri uri)
		{
			imageContainer.SetCurrentValue(System.Windows.Controls.Image.SourceProperty, new BitmapImage(uri));
		}

		public static void RegisterOpenImageWindowOnClick(MainWindow mainWindow, System.Windows.Controls.Image image)
		{
			image.MouseRightButtonUp += (object sender, System.Windows.Input.MouseButtonEventArgs e) => {
				new ImageWindow(mainWindow, image.Source);
			};		
		}

		public static void SaveImageToDrive(System.Windows.Controls.Image image)
		{
			var path = GetSavePathForPngFromDialog();
			if (path != null)
			{
				SaveImageToFile(image, path);
			}
		}

		private static string GetFilePathFromDialog()
		{
			var openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				return openFileDialog.FileName;
			}
			return null;
		}
		private static string GetSavePathForPngFromDialog()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				DefaultExt = ".png",
				Filter = "Pliki obrazów (*.png)|*.png"
			};

			if(saveFileDialog.ShowDialog() == true)
			{
				return saveFileDialog.FileName;
			}
			return null;
		}

		public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
		{
			using (MemoryStream outStream = new MemoryStream())
			{
				BitmapEncoder enc = new BmpBitmapEncoder();
				enc.Frames.Add(BitmapFrame.Create(bitmapImage));
				enc.Save(outStream);
				Bitmap bitmap = new Bitmap(outStream);

				return new Bitmap(bitmap);
			}
		}

		public static ImageSource BitmapToImageSource(Bitmap bitmap)
		{
			using (MemoryStream memory = new MemoryStream())
			{
				bitmap.Save(memory, ImageFormat.Png);
				memory.Position = 0;
				BitmapImage bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				return bitmapImage;
			}
		}
	}
}
