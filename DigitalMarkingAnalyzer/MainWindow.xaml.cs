using DigitalMarkingAnalyzer.viewmodels;
using LoggerUtils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DigitalMarkingAnalyzer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public UpdatableImage OriginalImage { get; }
		public UpdatableImage WatermarkImage { get; }

		private readonly Logger logger;

		private readonly InputImagesViewModel inputImagesViewModel;

		private readonly InternetImageGeneratorViewModel internetImageGeneratorViewModel;
		private readonly TextImageGeneratorViewModel textImageGeneratorViewModel;

		private AlgorithmViewModel algorithmViewModel;

		public MainWindow()
		{
			logger = LoggerFactory.Create(GetType());
			InitializeComponent();
			ResultTab.Visibility = Visibility.Hidden;

			StartLogConsole();

			OriginalImage = new UpdatableImage(OriginalImageControl);
			WatermarkImage = new UpdatableImage(WatermarkImageControl);

			inputImagesViewModel = new InputImagesViewModel(this, OriginalImageControl, WatermarkImageControl);
			inputImagesViewModel.SetUp();

			internetImageGeneratorViewModel = new InternetImageGeneratorViewModel(this, OriginalImage);
			internetImageGeneratorViewModel.SetUp();
			textImageGeneratorViewModel = new TextImageGeneratorViewModel(this, WatermarkImage);
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

		private void ProcessButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked ProcessButton.");
			algorithmViewModel.Submit();
		}

		private void CloseResultTabButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked CloseResultTabButton.");
			Tabs.SelectedIndex = 0;
			ResultTab.Visibility = Visibility.Hidden;
		}

		private void AlgorithmBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string algorithm = (sender as ComboBox).SelectedItem.ToString();
			logger.LogDebug($"Selected algorithm: {algorithm}.");

			algorithmViewModel?.Dispose();

			algorithmViewModel = AlgorithmViewModel.Create(algorithm, this);
			algorithmViewModel.SetUp();

			if(Process.Visibility == Visibility.Hidden)
			{
				Process.Visibility = Visibility.Visible;
			}
		}
	}
}