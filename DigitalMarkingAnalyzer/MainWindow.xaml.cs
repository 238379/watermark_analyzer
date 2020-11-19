using Algorithms;
using Algorithms.common;
using DigitalMarkingAnalyzer.viewmodels;
using Generators;
using LoggerUtils;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DigitalMarkingAnalyzer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly Logger logger;

		private readonly InternetImageGeneratorViewModel internetImageGeneratorViewModel;
		private readonly TextImageGeneratorViewModel textImageGeneratorViewModel;

		private AlgorithmViewModel algorithmViewModel;

		public MainWindow()
		{
			logger = LoggerFactory.Create(GetType());
			InitializeComponent();
			ResultTab.Visibility = Visibility.Hidden;

			OriginalImage.Source = new BitmapImage(new Uri("/DigitalMarkingAnalyzer;component/Resources/c_corgi.jpg", System.UriKind.RelativeOrAbsolute));
			WatermarkImage.Source = new BitmapImage(new Uri("/DigitalMarkingAnalyzer;component/Resources/w_tekst_dolny.png", System.UriKind.RelativeOrAbsolute));

			StartLogConsole();

			internetImageGeneratorViewModel = new InternetImageGeneratorViewModel(null, ErrorMessage, OriginalImage);
			internetImageGeneratorViewModel.SetUp();
			textImageGeneratorViewModel = new TextImageGeneratorViewModel(null, ErrorMessage, WatermarkImage);
			textImageGeneratorViewModel.SetUp();

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

		private void GenerateOriginalButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked GenerateOriginalButton.");
			var sw = Stopwatch.StartNew();

			internetImageGeneratorViewModel.Submit();

			logger.LogDebug($"Generated image in {sw.ElapsedMilliseconds}ms.");
		}		

		private void GenerateWatermarkButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked GenerateWatermarkButton.");
			var sw = Stopwatch.StartNew();

			textImageGeneratorViewModel.Submit();

			logger.LogDebug($"Generated image in {sw.ElapsedMilliseconds}ms.");
		}

		private void ProcessButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked ProcessButton.");

			algorithmViewModel.Submit();

			try
			{
				ErrorMessage.Visibility = Visibility.Hidden;

				ShowAlgorithmOutput(algorithmViewModel.LastResult);
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				logger.LogDebug(ex.StackTrace);
				ErrorMessage.Text = ex.Message;
				ErrorMessage.Visibility = Visibility.Visible;
			}
		}

		private void ShowAlgorithmOutput(AlgorithmResult result)
		{
			WatermarkedImage.Source = InterfaceTools.BitmapToImageSource(result.Watermarked);
			CleanedImage.Source = InterfaceTools.BitmapToImageSource(result.Cleaned);
			ExtractedWatermarkImage.Source = InterfaceTools.BitmapToImageSource(result.ExtractedWatermark);

			ResultTab.Visibility = Visibility.Visible;
			Tabs.SelectedIndex = 1;
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

		private void AlgorithmBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string algorithm = (sender as ComboBox).SelectedItem.ToString();
			logger.LogDebug($"Selected algorithm: {algorithm}.");

			algorithmViewModel?.Dispose();

			algorithmViewModel = AlgorithmViewModel.Create(algorithm, ParametersGrid, ErrorMessage, OriginalImage, WatermarkImage);
			algorithmViewModel.SetUp();

			if(Process.Visibility == Visibility.Hidden)
			{
				Process.Visibility = Visibility.Visible;
			}
		}
	}
}

