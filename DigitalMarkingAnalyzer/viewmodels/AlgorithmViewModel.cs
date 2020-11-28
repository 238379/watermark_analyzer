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

		private readonly AlgorithmControls controls;

		public static AlgorithmViewModel Create(string algorithmName, AlgorithmControls algorithmControls, TextBlock errorMessageTextBlock)
		{
			return algorithmName switch
			{
				Lsb.ALGORITHM_NAME => new LsbViewModel(algorithmControls, errorMessageTextBlock),
				PixelAveraging.ALGORITHM_NAME => new PixelAveragingViewModel(algorithmControls, errorMessageTextBlock),
				Dwt.ALGORITHM_NAME => new DwtViewModel(algorithmControls, errorMessageTextBlock),
				_ => throw new ArgumentException($"Unknown algorithmName '{algorithmName}'."),
			};
		}

		public AlgorithmViewModel(AlgorithmControls algorithmControls, TextBlock errorMessageTextBlock) : base(errorMessageTextBlock)
		{
			this.controls = algorithmControls;
		}

		public override void Dispose()
		{
			controls.ParametersGrid.Children.Clear();
		}

		protected override void OnSubmit()
		{
			switch(controls.AlgorithmMode)
			{
				case AlgorithmMode.AddWatermark:
					ProcessAdding();
					break;
				case AlgorithmMode.RemoveWatermark:
					ProcessRemoving();
					break;
				default:
					throw new InvalidOperationException($"Unknown algorithm mode {controls.AlgorithmMode}.");
			}
		}

		protected abstract void ProcessAdding();

		protected abstract void ProcessRemoving();

		protected (Bitmap, Bitmap, Bitmap) ReadInputBitmaps()
		{
			var originalAsBitmapImage = (BitmapImage)controls.OriginalImage.Source;
			var watermarkAsBitmapImage = (BitmapImage)controls.WatermarkImage.Source;
			var watermarkedAsBitmapImage = (BitmapImage)controls.WatermarkedImage.Source;

			return (originalAsBitmapImage.ToBitmap(), watermarkAsBitmapImage.ToBitmap(), watermarkedAsBitmapImage.ToBitmap());
		}

		protected void ShowAlgorithmOutput(AlgorithmResult result)
		{
			controls.ResultGrid.Children.Clear();
			controls.ResultGrid.RowDefinitions.Clear();

			result.ForEach((i, element) =>
			{
				int row = i / RESULT_VIEW_COLUMNS;
				int column = i % RESULT_VIEW_COLUMNS;

				if (column == 0) // it's a new row! :)
				{
					var rowDefinition = new RowDefinition
					{
						Height = GridLength.Auto
					};
					controls.ResultGrid.RowDefinitions.Add(rowDefinition);
				}

				var view = new AlgorithmResultElementView(element.Label, element.Image);
				AddAtPositionInResultGrid(view.Grid, column, row);
			});

			controls.ResultTab.Visibility = Visibility.Visible;
			controls.TabControl.SelectedIndex = controls.ResultTabIndex;
			controls.ResultScrollViewer.ScrollToVerticalOffset(0);
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
			controls.ParametersGrid.Children.Add(element);
			Grid.SetColumn(element, x);
			Grid.SetRow(element, y);
		}

		private void AddAtPositionInResultGrid(UIElement element, int x, int y)
		{
			controls.ResultGrid.Children.Add(element);
			Grid.SetColumn(element, x);
			Grid.SetRow(element, y);
		}
	}
}
