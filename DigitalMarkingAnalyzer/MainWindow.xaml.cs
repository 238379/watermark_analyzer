using Algorithms;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DigitalMarkingAnalyzer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			ResultTab.Visibility = Visibility.Hidden;

			OriginalImage.Source = new BitmapImage(new System.Uri("/DigitalMarkingAnalyzer;component/Resources/c_corgi.jpg", System.UriKind.RelativeOrAbsolute));
			WatermarkImage.Source = new BitmapImage(new System.Uri("/DigitalMarkingAnalyzer;component/Resources/w_tekst_dolny.png", System.UriKind.RelativeOrAbsolute));
		}

		private void BrowseOriginalButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceTools.SetImageFromDrive(OriginalImage);
		}

		private void BrowseWatermarkButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceTools.SetImageFromDrive(WatermarkImage);
		}


		private void ProcessButton_Click(object sender, RoutedEventArgs e)
		{
			var originalAsBitmapImage = (BitmapImage)OriginalImage.Source;
			var watermarkedAsBitmapImage = (BitmapImage)WatermarkImage.Source;

			var originalAsBitmap = originalAsBitmapImage.ToBitmap();
			var watermarkedAsBitmap = watermarkedAsBitmapImage.ToBitmap();

			var lsb = new Lsb(originalAsBitmap.Width, originalAsBitmap.Height, 0.5);

			var watermarkingResult = lsb.Watermark(originalAsBitmap, watermarkedAsBitmap);
			var cleaningResult = lsb.CleanWatermark(watermarkingResult);

			WatermarkedImage.Source = InterfaceTools.BitmapToImageSource(watermarkingResult);
			CleanedImage.Source = InterfaceTools.BitmapToImageSource(cleaningResult);

			ResultTab.Visibility = Visibility.Visible;
			Tabs.SelectedIndex = 1;
		}


		private void CloseResultTabButton_Click(object sender, RoutedEventArgs e)
		{
			Tabs.SelectedIndex = 0;
			ResultTab.Visibility = Visibility.Hidden;
		}

		private void SaveWatermarkedButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceTools.SaveImageToDrive(WatermarkedImage);
		}

		private void SaveCleanedButton_Click(object sender, RoutedEventArgs e)
		{
			InterfaceTools.SaveImageToDrive(CleanedImage);
		}
	}
}
