using DigitalMarkingAnalyzer.viewmodels;
using LoggerUtils;
using System;
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
		private const int ADVANCED_REMOVING_TAB_INDEX = 4;
		private const int ADVANCED_REMOVING_RESULT_TAB_INDEX = 5;

		#region Private Adding
		private readonly UpdatableImage originalImage;
		private readonly UpdatableImage watermarkImage;

		private readonly AlgorithmControls addingWatermarkAlgorithmControls;
		private readonly AlgorithmSelectionViewModel addingAlgorithmSelectionViewModel;
		private readonly InternetImageGeneratorViewModel originalGeneratorViewModel;
		private readonly TextImageGeneratorViewModel watermarkGeneratorViewModel;
		#endregion

		#region Private Removing
		private readonly UpdatableImage watermarkedImage;

		private readonly AlgorithmControls removingWatermarkAlgorithmControls;
		private readonly AlgorithmSelectionViewModel removingAlgorithmSelectionViewModel;
		private readonly InternetImageGeneratorViewModel watermarkedGeneratorViewModel;
		#endregion

		#region Private AdvancedRemoving
		private readonly UpdatableImage advancedWatermarkedImage;

		private readonly AlgorithmControls advancedRemovingWatermarkAlgorithmControls;
		private readonly AlgorithmSelectionViewModel advancedRemovingAlgorithmSelectionViewModel;
		private readonly InternetImageGeneratorViewModel advancedWatermarkedGeneratorViewModel;
		#endregion

		private readonly Logger logger;

		public MainWindow()
		{
			logger = LoggerFactory.Create(GetType());
			InitializeComponent();
			AddingResultTab.Visibility = Visibility.Collapsed;
			RemovingResultTab.Visibility = Visibility.Collapsed;
			AdvancedRemovingResultTab.Visibility = Visibility.Collapsed;

			StartLogConsole();

			originalImage = new UpdatableImage(OriginalImageControl);
			watermarkImage = new UpdatableImage(WatermarkImageControl);
			watermarkedImage = new UpdatableImage(WatermarkedImageControl);
			advancedWatermarkedImage = new UpdatableImage(AdvancedWatermarkedImageControl);

			addingWatermarkAlgorithmControls = new AlgorithmControls(Algorithms.common.AlgorithmMode.AddWatermark, AddingParametersGrid, AddingProcess,
				OriginalImageControl, WatermarkImageControl, WatermarkedImageControl,
				Tabs, AddingResultTab, ADDING_RESULT_TAB_INDEX, AddingResultGrid, AddingResultScrollViewer, CloseAddingResult, CancelButton);

			removingWatermarkAlgorithmControls = new AlgorithmControls(Algorithms.common.AlgorithmMode.RemoveWatermark, RemovingParametersGrid, RemovingProcess,
				OriginalImageControl, WatermarkImageControl, WatermarkedImageControl,
				Tabs, RemovingResultTab, REMOVING_RESULT_TAB_INDEX, RemovingResultGrid, RemovingResultScrollViewer, CloseRemovingResult, CancelButton);

			advancedRemovingWatermarkAlgorithmControls = new AlgorithmControls(Algorithms.common.AlgorithmMode.RemoveWatermark, AdvancedRemovingParametersGrid, AdvancedRemovingProcess,
				OriginalImageControl, WatermarkImageControl, AdvancedWatermarkedImageControl,
				Tabs, AdvancedRemovingResultTab, ADVANCED_REMOVING_RESULT_TAB_INDEX, AdvancedRemovingResultGrid, AdvancedRemovingResultScrollViewer, AdvancedCloseRemovingResult, CancelButton);

			SetUpInputImagesViewModels();

			addingAlgorithmSelectionViewModel = new AlgorithmSelectionViewModel(AddingAlgorithmBox, addingWatermarkAlgorithmControls, this, AddingErrorMessage);
			addingAlgorithmSelectionViewModel.SetUp();

			removingAlgorithmSelectionViewModel = new AlgorithmSelectionViewModel(RemovingAlgorithmBox, removingWatermarkAlgorithmControls, this, RemovingErrorMessage);
			removingAlgorithmSelectionViewModel.SetUp();

			advancedRemovingAlgorithmSelectionViewModel = new AlgorithmSelectionViewModel(AdvancedRemovingAlgorithmBox, advancedRemovingWatermarkAlgorithmControls, this, AdvancedRemovingErrorMessage);
			advancedRemovingAlgorithmSelectionViewModel.SetUp();

			originalGeneratorViewModel = new InternetImageGeneratorViewModel(new GeneratorControls(GenerateOriginalButton, originalImage), AddingErrorMessage);
			originalGeneratorViewModel.SetUp();

			watermarkGeneratorViewModel = new TextImageGeneratorViewModel(new GeneratorControls(GenerateWatermarkButton, watermarkImage), AddingErrorMessage);
			watermarkGeneratorViewModel.SetUp();

			watermarkedGeneratorViewModel = new InternetImageGeneratorViewModel(new GeneratorControls(GenerateWatermarkedButton, watermarkedImage), RemovingErrorMessage);
			watermarkedGeneratorViewModel.SetUp();

			advancedWatermarkedGeneratorViewModel = new InternetImageGeneratorViewModel(new GeneratorControls(AdvancedGenerateWatermarkedButton, advancedWatermarkedImage), AdvancedRemovingErrorMessage);
			advancedWatermarkedGeneratorViewModel.SetUp();

			logger.LogDebug("Created MainWindow");
		}

		private void SetUpInputImagesViewModels()
		{
			// WatermarkImageControl, WatermarkedImageControl
			var original = new InputImageViewModel(this, OriginalImageControl, originalImage, new Uri("/DigitalMarkingAnalyzer;component/Resources/c_corgi.jpg", UriKind.RelativeOrAbsolute),
				BrowseOriginalButton, UndoOriginalButton, ToDefaultOriginalButton, AddingErrorMessage);
			original.SetUp();

			var watermark = new InputImageViewModel(this, WatermarkImageControl, watermarkImage, new Uri("/DigitalMarkingAnalyzer;component/Resources/w_tekst_dolny.png", UriKind.RelativeOrAbsolute),
				BrowseWatermarkButton, UndoWatermarkButton, ToDefaultWatermarkButton, AddingErrorMessage);
			watermark.SetUp();

			var watermarked = new InputImageViewModel(this, WatermarkedImageControl, watermarkedImage, new Uri("/DigitalMarkingAnalyzer;component/Resources/t_corgi_tekst_dolny.jpg", UriKind.RelativeOrAbsolute),
				BrowseWatermarkedButton, UndoWatermarkedButton, ToDefaultWatermarkedButton, RemovingErrorMessage);
			watermarked.SetUp();

			var advancedWatermarked = new InputImageViewModel(this, AdvancedWatermarkedImageControl, advancedWatermarkedImage, new Uri("/DigitalMarkingAnalyzer;component/Resources/t_corgi_tekst_dolny.jpg", UriKind.RelativeOrAbsolute),
				AdvancedBrowseWatermarkedButton, AdvancedUndoWatermarkedButton, AdvancedToDefaultWatermarkedButton, AdvancedRemovingErrorMessage);
			advancedWatermarked.SetUp();
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
			Tabs.SelectedIndex = ADDING_TAB_INDEX;
			AddingResultTab.Visibility = Visibility.Collapsed;
		}

		private void CloseRemovingResultTabButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked CloseRemovingResultTabButton.");
			Tabs.SelectedIndex = REMOVING_TAB_INDEX;
			RemovingResultTab.Visibility = Visibility.Collapsed;
		}

		private void CloseAdvancedRemovingResultTabButton_Click(object sender, RoutedEventArgs e)
		{
			logger.LogDebug("Clicked CloseAdvancedRemovingResultTabButton.");
			Tabs.SelectedIndex = ADVANCED_REMOVING_TAB_INDEX;
			AdvancedRemovingResultTab.Visibility = Visibility.Collapsed;
		}
	}
}