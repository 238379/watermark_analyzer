using Algorithms;
using Algorithms.common;
using DigitalMarkingAnalyzer.common;
using DigitalMarkingAnalyzer.viewmodels.advanced;
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
		public enum ViewModelType {
			Basic,
			Advanced
		}

		protected readonly Dispatcher dispatcher;

		private const int RESULT_VIEW_COLUMNS = 2;

		private readonly AlgorithmControls controls;
		private readonly MainWindow mainWindow;

		private CancellationTokenSource cts;

		public static AlgorithmViewModel Create(string algorithmName, ViewModelType type, AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock)
		{
			return type switch
			{
				ViewModelType.Basic => algorithmName switch
				{
					Lsb.ALGORITHM_NAME => new LsbViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					PixelAveraging.ALGORITHM_NAME => new PixelAveragingViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					Dwt.ALGORITHM_NAME => new DwtViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					Dft.ALGORITHM_NAME => new DftViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					Dct.ALGORITHM_NAME => new DctViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					_ => throw new ArgumentException($"Unknown algorithmName '{algorithmName}'."),
				},
				ViewModelType.Advanced => algorithmName switch
				{
					Recognizer.ALGORITHM_NAME => new RecognizerViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					Lsb.ALGORITHM_NAME => new AdvancedLsbViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					PixelAveraging.ALGORITHM_NAME => new AdvancedPixelAveragingViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					Dwt.ALGORITHM_NAME => new AdvancedDwtViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					Dft.ALGORITHM_NAME => new AdvancedDftViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					Dct.ALGORITHM_NAME => new AdvancedDctViewModel(algorithmControls, mainWindow, errorMessageTextBlock),
					_ => throw new ArgumentException($"Unknown algorithmName '{algorithmName}'."),
				},
				_ => throw new ArgumentException($"Unknown view model type '{type}'."),
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
				originalAsBitmapImage = ((BitmapImage)controls.OriginalImage?.Source)?.ToBitmap()?.TransformToEffectiveBitmap();
				watermarkAsBitmapImage = ((BitmapImage)controls.WatermarkImage?.Source)?.ToBitmap()?.Resize(originalAsBitmapImage.Width, originalAsBitmapImage.Height)?.TransformToEffectiveBitmap();
				watermarkedAsBitmapImage = ((BitmapImage)controls.WatermarkedImage?.Source)?.ToBitmap()?.TransformToEffectiveBitmap();

			});

			return (originalAsBitmapImage, watermarkAsBitmapImage, watermarkedAsBitmapImage);
		}

		protected async Task ShowAlgorithmOutput(IAsyncEnumerable<AlgorithmResultElement> asyncResults)
		{
			bool first = true;
			await foreach (var result in asyncResults)
			{
				if(first)
				{
					first = false;
					dispatcher.Invoke(() =>
					{
						for (int column = controls.ResultGrid.Children.Count % RESULT_VIEW_COLUMNS; column != 0; column = controls.ResultGrid.Children.Count % RESULT_VIEW_COLUMNS)
						{
							// add placeholders
							int row = controls.ResultGrid.Children.Count / RESULT_VIEW_COLUMNS;
							AddAtPositionInResultGrid(new AlgorithmResultElementView(null, null, null).Grid, column, row);
						}

						{
							// now we have a new row
							var view = new AlgorithmLabelElementView(result.Description.ToString());
							controls.ResultGrid.RowDefinitions.Add(new RowDefinition
							{
								Height = new GridLength(view.Grid.Height)
							});
							int row = controls.ResultGrid.Children.Count / RESULT_VIEW_COLUMNS;
							int column = controls.ResultGrid.Children.Count % RESULT_VIEW_COLUMNS;
							AddAtPositionInResultGrid(view.Grid, column, row, 2);
							AddAtPositionInResultGrid(new AlgorithmLabelElementView(null).Grid, column + 1, row);
						}

					});
				}

				dispatcher.Invoke(() =>
				{
					int row = controls.ResultGrid.Children.Count / RESULT_VIEW_COLUMNS;
					int column = controls.ResultGrid.Children.Count % RESULT_VIEW_COLUMNS;

					if (column == 0) // it's a new row! :)
					{
						controls.ResultGrid.RowDefinitions.Add(new RowDefinition
						{
							Height = GridLength.Auto
						});
					}

					var view = new AlgorithmResultElementView(result.Label, result.Image, controls.OnUse);
					InterfaceTools.RegisterOpenImageWindowOnClick(mainWindow, view.Image);
					AddAtPositionInResultGrid(view.Grid, column, row);
				});
			}
		}

		protected RangeParameterView<int> AddIntRangeParameter(string paramName, int yPosition, (int, int) allowedRange, int defaultInterval)
		{
			var controls = AddRangeParameterControls(paramName, yPosition, allowedRange, defaultInterval);
			return new RangeParameterView<int>(paramName, allowedRange, controls.Item1, controls.Item2, controls.Item3, controls.Item4, Dispatcher.CurrentDispatcher);
		}

		protected RangeParameterView<decimal> AddDecimalRangeParameter(string paramName, int yPosition, (double, double) allowedRange, double defaultInterval)
		{
			var controls = AddRangeParameterControls(paramName, yPosition, allowedRange, defaultInterval);
			return new RangeParameterView<decimal>(paramName, allowedRange, controls.Item1, controls.Item2, controls.Item3, controls.Item4, Dispatcher.CurrentDispatcher);
		}

		private (Label, TextBox, TextBox, TextBox) AddRangeParameterControls(string paramName, int yPosition, (object, object) allowedRange, object defaultInterval)
		{
			var label = new Label
			{
				Content = paramName + " range",
				HorizontalContentAlignment = HorizontalAlignment.Left,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			var min = new TextBox
			{
				Text = allowedRange.Item1.ToString().Replace(",", "."),
				HorizontalContentAlignment = HorizontalAlignment.Right,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			var max = new TextBox
			{
				Text = allowedRange.Item2.ToString().Replace(",", "."),
				HorizontalContentAlignment = HorizontalAlignment.Right,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			var interval = new TextBox
			{
				Text = defaultInterval.ToString().Replace(",", "."),
				HorizontalContentAlignment = HorizontalAlignment.Right,
				VerticalContentAlignment = VerticalAlignment.Center
			};

			AddAtPositionInParametersGrid(label, 0, yPosition);
			AddAtPositionInParametersGrid(min, 1, yPosition);
			AddAtPositionInParametersGrid(max, 2, yPosition);
			AddAtPositionInParametersGrid(interval, 3, yPosition);

			return (label, min, max, interval);
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

		protected CheckBox AddParameterCheckBox(bool initState, int x, int y)
		{
			var checkBox = new CheckBox
			{
				IsChecked = initState,
				HorizontalContentAlignment = HorizontalAlignment.Right,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			AddAtPositionInParametersGrid(checkBox, x, y);
			return checkBox;
		}

		private void AddAtPositionInParametersGrid(UIElement element, int x, int y)
		{
			controls.ParametersGrid.Children.Add(element);
			Grid.SetColumn(element, x);
			Grid.SetRow(element, y);
		}

		private void AddAtPositionInResultGrid(UIElement element, int x, int y, int columnSpan = 1)
		{
			controls.ResultGrid.Children.Add(element);
			Grid.SetColumn(element, x);
			Grid.SetRow(element, y);
			Grid.SetColumnSpan(element, columnSpan);
		}
	}
}
