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
		protected readonly MainWindow window;
		protected readonly Logger logger;

		public ViewModel(MainWindow window)
		{
			this.window = window;
			logger = LoggerFactory.Create(GetType());
		}

		public abstract void SetUp();

		public void Submit()
		{
			var sw = Stopwatch.StartNew();
			try
			{
				window.ErrorMessage.Visibility = Visibility.Hidden;
				OnSubmit();
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				logger.LogDebug(ex.StackTrace);
				window.ErrorMessage.Text = ex.Message;
				window.ErrorMessage.Visibility = Visibility.Visible;
			}
			logger.LogInfo($"Processing time: {sw.ElapsedMilliseconds} ms");
		}

		public virtual void Dispose()
		{
			window.ParametersGrid.Children.RemoveRange(0, window.ParametersGrid.Children.Count);
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
			window.ParametersGrid.Children.Add(element);
			Grid.SetColumn(element, x);
			Grid.SetRow(element, y);
		}
	}
}
