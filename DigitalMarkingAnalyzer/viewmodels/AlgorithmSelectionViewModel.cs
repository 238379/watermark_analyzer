using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class AlgorithmSelectionViewModel : ViewModel
	{
		private readonly ComboBox algorithmBox;
		private readonly AlgorithmControls controls;
		private readonly MainWindow mainWindow;

		private AlgorithmViewModel algorithmViewModel;

		public AlgorithmSelectionViewModel(ComboBox algorithmBox, AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(errorMessageTextBlock)
		{
			this.algorithmBox = algorithmBox;
			this.controls = algorithmControls;
			this.mainWindow = mainWindow;
		}

		public override void Dispose()
		{
			controls.ProcessButton.Click -= Process;
			algorithmBox.SelectionChanged -= ChangeSelectedAlgorithm;
			algorithmViewModel?.Dispose();
			algorithmViewModel = null;
		}

		public override void SetUp()
		{
			controls.ProcessButton.Click += Process;
			algorithmBox.SelectionChanged += ChangeSelectedAlgorithm;
		}

		private void Process(object sender, System.Windows.RoutedEventArgs e)
		{
			Submit();
		}

		protected override Task OnSubmit()
		{
			return algorithmViewModel?.Submit();
		}

		private void ChangeSelectedAlgorithm(object sender, SelectionChangedEventArgs e)
		{
			string algorithm = (sender as ComboBox).SelectedItem.ToString();
			logger.LogDebug($"Selected algorithm: {algorithm}.");

			algorithmViewModel?.Dispose();

			algorithmViewModel = AlgorithmViewModel.Create(algorithm, controls, mainWindow, errorMessageTextBlock);
			algorithmViewModel.SetUp();

			if (controls.ProcessButton.Visibility == Visibility.Hidden)
			{
				controls.ProcessButton.Visibility = Visibility.Visible;
			}
		}
	}
}
