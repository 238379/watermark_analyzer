using Algorithms;
using LoggerUtils;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace DigitalMarkingAnalyzer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Logger logger;

		public MainWindow()
		{
			logger = LoggerFactory.Create(GetType());
			InitializeComponent();
			ResultTab.Visibility = Visibility.Hidden;

			OriginalImage.Source = new BitmapImage(new System.Uri("/DigitalMarkingAnalyzer;component/Resources/c_corgi.jpg", System.UriKind.RelativeOrAbsolute));
			WatermarkImage.Source = new BitmapImage(new System.Uri("/DigitalMarkingAnalyzer;component/Resources/w_tekst_dolny.png", System.UriKind.RelativeOrAbsolute));

			StartLogConsole();

			logger.LogDebug("Created MainWindow");
		}

		public void StartLogConsole()
		{
			new LogConsole(this)
			{
				Left = Left + 0,
				Top = Top + 0
			}.Show();
		}

		private void BrowseOriginalButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked BrowseOriginalButton.");
			InterfaceTools.SetImageFromDrive(OriginalImage);
		}

		private void BrowseWatermarkButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked BrowseWatermarkButton.");
			InterfaceTools.SetImageFromDrive(WatermarkImage);
		}


		private void ProcessButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked ProcessButton.");
			var originalAsBitmapImage = (BitmapImage)OriginalImage.Source;
			var watermarkedAsBitmapImage = (BitmapImage)WatermarkImage.Source;

			var originalAsBitmap = originalAsBitmapImage.ToBitmap();
			var watermarkedAsBitmap = watermarkedAsBitmapImage.ToBitmap();

			var selectedAlgorithm = AlgorithmBox.SelectedItem.ToString();
			if (selectedAlgorithm == "LSB") {
				if (int.TryParse(BitsForWatermarkTextBox.Text, out var bitsForWatermark) && bitsForWatermark >= 1 && bitsForWatermark <= 8)
				{
					var lsb = new Lsb(originalAsBitmap.Width, originalAsBitmap.Height, bitsForWatermark);

					var watermarkingResult = lsb.Watermark(originalAsBitmap, watermarkedAsBitmap);
					var cleaningResult = lsb.CleanWatermark(watermarkingResult);
					var extractingResult = lsb.ExtractWatermark(watermarkingResult);

					PrintAlgorithmOutput(watermarkingResult, cleaningResult, extractingResult);
				}
				else
				{
					logger.LogError($"Could not process image. Bits for watermark value has to be a number between 0 and 8 but it is '{BitsForWatermarkTextBox.Text}'");
				}
			}else if (selectedAlgorithm == "PixelAveraging")
            {
				var pixelAveraging = new PixelAveraging(1);

				var watermarkingResult = pixelAveraging.Watermark(originalAsBitmap, watermarkedAsBitmap);
				var cleaningResult = pixelAveraging.CleanWatermark(watermarkingResult, watermarkedAsBitmap);
				var extractingResult = pixelAveraging.ExtractWatermark(watermarkingResult, originalAsBitmap);

				PrintAlgorithmOutput(watermarkingResult, cleaningResult, extractingResult);
            }
		}
		private void PrintAlgorithmOutput(Bitmap watermarkingResult, Bitmap cleaningResult, Bitmap extractingResult)
		{
			WatermarkedImage.Source = InterfaceTools.BitmapToImageSource(watermarkingResult);
			CleanedImage.Source = InterfaceTools.BitmapToImageSource(cleaningResult);
			ExtractedWatermarkImage.Source = InterfaceTools.BitmapToImageSource(extractingResult);

			ResultTab.Visibility = Visibility.Visible;
			Tabs.SelectedIndex = 1;
			logger.LogInfo("Processed image.");
		}

		private void CloseResultTabButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked CloseResultTabButton.");
			Tabs.SelectedIndex = 0;
			ResultTab.Visibility = Visibility.Hidden;
		}

		private void SaveWatermarkedButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked SaveWatermarkedButton.");
			InterfaceTools.SaveImageToDrive(WatermarkedImage);
		}

		private void SaveCleanedButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked SaveCleanedButton.");
			InterfaceTools.SaveImageToDrive(CleanedImage);
		}

		private void SaveExtractedWatermarkButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked SaveExtractedWatermarkButton.");
			InterfaceTools.SaveImageToDrive(ExtractedWatermarkImage);
		}	
	}
}

