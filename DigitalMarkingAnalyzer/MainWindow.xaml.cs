using DigitalMarkingAnalyzer.viewmodels;
using LoggerUtils;
using System.Windows;

namespace DigitalMarkingAnalyzer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private const int ADDING_TAB_INDEX = 0;
		private const int ADDING_RESULT_TAB_INDEX = 1;
		private const int REMOVING_TAB_INDEX = 2;
		private const int REMOVING_RESULT_TAB_INDEX = 3;

		#region Public Adding
		public UpdatableImage OriginalImage { get; }
		public UpdatableImage WatermarkImage { get; }
		#endregion

		#region Public Removing
		public UpdatableImage WatermarkedImage { get; }
		#endregion

		#region Private Adding
		private readonly AlgorithmControls addingWatermarkAlgorithmControls;
		private readonly AlgorithmSelectionViewModel addingAlgorithmSelectionViewModel;
		private readonly InternetImageGeneratorViewModel originalGeneratorViewModel;
		private readonly TextImageGeneratorViewModel watermarkGeneratorViewModel;
		#endregion

		#region Private Removing
		private readonly AlgorithmControls removingWatermarkAlgorithmControls;
		private readonly AlgorithmSelectionViewModel removingAlgorithmSelectionViewModel;
		private readonly InternetImageGeneratorViewModel watermarkedGeneratorViewModel;
		#endregion

		private readonly InputImagesViewModel inputImagesViewModel; // TODO
		private readonly Logger logger;

		public MainWindow()
		{
			logger = LoggerFactory.Create(GetType());
			InitializeComponent();
			AddingResultTab.Visibility = Visibility.Collapsed;
			RemovingResultTab.Visibility = Visibility.Collapsed;

			StartLogConsole();

			OriginalImage = new UpdatableImage(OriginalImageControl);
			WatermarkImage = new UpdatableImage(WatermarkImageControl);
			WatermarkedImage = new UpdatableImage(WatermarkedImageControl);

			addingWatermarkAlgorithmControls = new AlgorithmControls(Algorithms.common.AlgorithmMode.AddWatermark, AddingParametersGrid, AddingProcess, OriginalImageControl, WatermarkImageControl, WatermarkedImageControl,
				Tabs, AddingResultTab, ADDING_RESULT_TAB_INDEX, AddingResultGrid, AddingResultScrollViewer, CloseAddingResult);

			removingWatermarkAlgorithmControls = new AlgorithmControls(Algorithms.common.AlgorithmMode.RemoveWatermark, RemovingParametersGrid, RemovingProcess, OriginalImageControl, WatermarkImageControl, WatermarkedImageControl,
				Tabs, RemovingResultTab, REMOVING_RESULT_TAB_INDEX, RemovingResultGrid, RemovingResultScrollViewer, CloseRemovingResult);

			inputImagesViewModel = new InputImagesViewModel(this, OriginalImageControl, WatermarkImageControl, WatermarkedImageControl, AddingErrorMessage);// TODO
			inputImagesViewModel.SetUp();

			addingAlgorithmSelectionViewModel = new AlgorithmSelectionViewModel(AddingAlgorithmBox, addingWatermarkAlgorithmControls, AddingErrorMessage);
			addingAlgorithmSelectionViewModel.SetUp();

			removingAlgorithmSelectionViewModel = new AlgorithmSelectionViewModel(RemovingAlgorithmBox, removingWatermarkAlgorithmControls, RemovingErrorMessage);
			removingAlgorithmSelectionViewModel.SetUp();

			originalGeneratorViewModel = new InternetImageGeneratorViewModel(new GeneratorControls(GenerateOriginalButton, OriginalImage), AddingErrorMessage);
			originalGeneratorViewModel.SetUp();

			watermarkGeneratorViewModel = new TextImageGeneratorViewModel(new GeneratorControls(GenerateWatermarkButton, WatermarkImage), AddingErrorMessage);
			watermarkGeneratorViewModel.SetUp();

			watermarkedGeneratorViewModel = new InternetImageGeneratorViewModel(new GeneratorControls(GenerateWatermarkedButton, WatermarkedImage), RemovingErrorMessage);
			watermarkedGeneratorViewModel.SetUp();

			logger.LogDebug("Created MainWindow");
		}

		public void StartLogConsole()
		{
			new LogConsole(this)
			{
				Left = Left,
				Top = Top
			}.Show();
		}

		private void CloseAddingResultTabButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked CloseResultTabButton.");
			Tabs.SelectedIndex = 0;
			AddingResultTab.Visibility = Visibility.Collapsed;
		}

		private void CloseRemovingResultTabButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked CloseRemovingResultTabButton.");
			Tabs.SelectedIndex = 2;
			RemovingResultTab.Visibility = Visibility.Collapsed;
		}
	}
}