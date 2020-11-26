using Algorithms;
using Algorithms.common;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public abstract class AlgorithmViewModel : ViewModel
	{
		private const int RESULT_VIEW_COLUMNS = 2;

		public static AlgorithmViewModel Create(string algorithmName, MainWindow window)
		{
			return algorithmName switch
			{
				Lsb.ALGORITHM_NAME => new LsbViewModel(window),
				PixelAveraging.ALGORITHM_NAME => new PixelAveragingViewModel(window),
				Dwt.ALGORITHM_NAME => new DwtViewModel(window),
				_ => throw new ArgumentException($"Unknown algorithmName '{algorithmName}'."),
			};
		}

		public AlgorithmViewModel(MainWindow window) : base(window)
		{
		}

		protected (Bitmap, Bitmap) ReadInputBitmaps()
		{
			var originalAsBitmapImage = (BitmapImage)window.OriginalImage.Source;
			var watermarkAsBitmapImage = (BitmapImage)window.WatermarkImage.Source;

			return (originalAsBitmapImage.ToBitmap(), watermarkAsBitmapImage.ToBitmap());
		}

		protected void ShowAlgorithmOutput(AlgorithmResult result)
		{
			window.AlgorithmResultGrid.Children.Clear();

			result.ForEach((i, element) =>
			{
				int row = i / RESULT_VIEW_COLUMNS;
				int column = i % RESULT_VIEW_COLUMNS;

				var view = new AlgorithmResultElementView(element.Label, element.Image);

				AddAtPositionInResultGrid(view.Grid, row, column);

				if (column == 0) // it's a new row! :)
				{
					var rowDefinition = new RowDefinition
					{
						Height = GridLength.Auto
					};
					window.AlgorithmResultGrid.RowDefinitions.Add(rowDefinition);
				}
			});

			window.ResultTab.Visibility = Visibility.Visible;
			window.Tabs.SelectedIndex = 1;
		}

		protected Label AddParameterLabel(string labelContent, int x, int y)
		{
			var label = new Label
			{
				Content = labelContent,
				HorizontalContentAlignment = HorizontalAlignment.Left,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			AddAtPositionInParametersGrid(label, x, y);
			return label;
		}

		protected TextBox AddParameterTextBox(string initContent, int x, int y)
		{
			var textBox = new TextBox
			{
				Text = initContent,
				HorizontalContentAlignment = HorizontalAlignment.Right,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			AddAtPositionInParametersGrid(textBox, x, y);
			return textBox;
		}

		private void AddAtPositionInParametersGrid(UIElement element, int x, int y)
		{
			window.ParametersGrid.Children.Add(element);
			Grid.SetColumn(element, x);
			Grid.SetRow(element, y);
		}

		private void AddAtPositionInResultGrid(UIElement element, int x, int y)
		{
			window.AlgorithmResultGrid.Children.Add(element);
			Grid.SetColumn(element, x);
			Grid.SetRow(element, y);
		}
	}
}
