using Algorithms;
using Algorithms.common;
using DigitalMarkingAnalyzer.viewmodels.basic;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public abstract class AlgorithmViewModel : ViewModel
	{
		protected readonly Dispatcher dispatcher;

		private const int RESULT_VIEW_COLUMNS = 2;

		private readonly AlgorithmControls controls;
		private readonly MainWindow mainWindow;

		private CancellationTokenSource cts;

		public static AlgorithmViewModel Create(string algorithmName, AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock)
		{
			return algorithmName switch
			{
				Lsb.ALGORITHM_NAME => new LsbViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
				PixelAveraging.ALGORITHM_NAME => new PixelAveragingViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
				Dwt.ALGORITHM_NAME => new DwtViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
				Dft.ALGORITHM_NAME => new DftViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
				_ => throw new ArgumentException($"Unknown algorithmName '{algorithmName}'."),
			};
		}

		public AlgorithmViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(errorMessageTextBlock)
		{
			this.controls = algorithmControls;
			this.mainWindow = mainWindow;
			this.dispatcher = mainWindow.Dispatcher;
		}

		public override void Dispose()
		{
			controls.ParametersGrid.Children.Clear();
		}

		protected override async Task OnSubmit()
		{
			var sourceTabIndex = controls.TabControl.SelectedIndex;

			controls.ResultGrid.Children.Clear();
			controls.ResultGrid.RowDefinitions.Clear();

			controls.ResultTab.Visibility = Visibility.Visible;
			controls.TabControl.SelectedIndex = controls.ResultTabIndex;
			controls.ResultScrollViewer.ScrollToVerticalOffset(0);

			cts = new CancellationTokenSource();

			controls.CancelButton.Click += Cancel;
			controls.CancelButton.Visibility = Visibility.Visible;
			controls.CloseButton.Click += Cancel;

			try
			{
				switch(controls.AlgorithmMode)
				{
					case AlgorithmMode.AddWatermark:
						await ProcessAdding(cts.Token);
						break;
					case AlgorithmMode.RemoveWatermark:
						await ProcessRemoving(cts.Token);
						break;
					default:
						throw new InvalidOperationException($"Unknown algorithm mode {controls.AlgorithmMode}.");
				};
			}
			catch
			{
				dispatcher.Invoke(() =>
				{
					controls.TabControl.SelectedIndex = sourceTabIndex;
				});
				throw;
			}
			finally
			{
				controls.CancelButton.Click -= Cancel;
				controls.CancelButton.Visibility = Visibility.Hidden;
				controls.CloseButton.Click -= Cancel;
			}
		}

		private void Cancel(object sender, RoutedEventArgs e)
		{
			cts.Cancel();
		}

		protected abstract Task ProcessAdding(CancellationToken ct);

		protected abstract Task ProcessRemoving(CancellationToken ct);

		protected (EffectiveBitmap, EffectiveBitmap, EffectiveBitmap) ReadInputBitmaps()
		{
			EffectiveBitmap originalAsBitmapImage = null;
			EffectiveBitmap watermarkAsBitmapImage = null;
			EffectiveBitmap watermarkedAsBitmapImage = null;

			dispatcher.Invoke(() =>
			{
				originalAsBitmapImage = ((BitmapImage)controls.OriginalImage.Source).ToBitmap().TransformToEffectiveBitmap();
				watermarkAsBitmapImage = ((BitmapImage)controls.WatermarkImage.Source).ToBitmap().Resize(originalAsBitmapImage.Width, originalAsBitmapImage.Height).TransformToEffectiveBitmap();
				watermarkedAsBitmapImage = ((BitmapImage)controls.WatermarkedImage.Source).ToBitmap().TransformToEffectiveBitmap();
			});

			return (originalAsBitmapImage, watermarkAsBitmapImage, watermarkedAsBitmapImage);
		}

		protected async Task AppendAlgorithmOutput(IAsyncEnumerable<AlgorithmResultElement> asyncResults)
		{
			await foreach (var result in asyncResults)
			{
				dispatcher.Invoke(() =>
				{
					int row = controls.ResultGrid.Children.Count / RESULT_VIEW_COLUMNS;
					int column = controls.ResultGrid.Children.Count % RESULT_VIEW_COLUMNS;

					if (column == 0) // it's a new row! :)
					{
						var rowDefinition = new RowDefinition
						{
							Height = GridLength.Auto
						};
						controls.ResultGrid.RowDefinitions.Add(rowDefinition);
					}

					var view = new AlgorithmResultElementView(result.Label, result.Image);
					InterfaceTools.RegisterOpenImageWindowOnClick(mainWindow, view.Image);
					AddAtPositionInResultGrid(view.Grid, column, row);
				});
			}
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
