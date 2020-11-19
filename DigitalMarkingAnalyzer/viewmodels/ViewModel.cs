using LoggerUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public abstract class ViewModel : IDisposable
	{
		protected readonly Grid parametersGrid;
		protected readonly TextBlock errorTextBlock;
		protected readonly Logger logger;

		public ViewModel(Grid parametersGrid, TextBlock errorTextBlock)
		{
			this.parametersGrid = parametersGrid;
			this.errorTextBlock = errorTextBlock;
			logger = LoggerFactory.Create(GetType());
		}

		public abstract void SetUp();

		public void Submit()
		{
			var sw = Stopwatch.StartNew();
			try
			{
				errorTextBlock.Visibility = Visibility.Hidden;
				OnSubmit();
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				logger.LogDebug(ex.StackTrace);
				errorTextBlock.Text = ex.Message;
				errorTextBlock.Visibility = Visibility.Visible;
			}
			logger.LogInfo($"Processing time: {sw.ElapsedMilliseconds} ms");
		}

		public virtual void Dispose()
		{
			parametersGrid.Children.RemoveRange(0, parametersGrid.Children.Count);
		}

		protected abstract void OnSubmit();

		protected Label AddLabel(string labelContent, int x, int y)
		{
			var label = new Label
			{
				Content = labelContent,
				HorizontalContentAlignment = HorizontalAlignment.Left,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			AddAtPositionInGrid(label, x, y);
			return label;
		}

		protected TextBox AddTextBox(string initContent, int x, int y)
		{
			var textBox = new TextBox
			{
				Text = initContent,
				HorizontalContentAlignment = HorizontalAlignment.Right,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			AddAtPositionInGrid(textBox, x, y);
			return textBox;
		}

		private void AddAtPositionInGrid(UIElement element, int x, int y)
		{
			parametersGrid.Children.Add(element);
			Grid.SetColumn(element, x);
			Grid.SetRow(element, y);
		}
	}
}
