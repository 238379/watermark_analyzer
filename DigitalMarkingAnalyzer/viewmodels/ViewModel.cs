using LoggerUtils;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public abstract class ViewModel : IDisposable
	{
		protected readonly TextBlock errorMessageTextBlock;
		protected readonly Logger logger;

		public ViewModel(TextBlock errorMessageTextBlock)
		{
			this.errorMessageTextBlock = errorMessageTextBlock;
			logger = LoggerFactory.Create(GetType());
		}

		public abstract void SetUp();

		public void Submit()
		{
			var sw = Stopwatch.StartNew();
			try
			{
				errorMessageTextBlock.Visibility = Visibility.Hidden;
				OnSubmit();
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				logger.LogDebug(ex.StackTrace);
				errorMessageTextBlock.Text = ex.Message;
				errorMessageTextBlock.Visibility = Visibility.Visible;
			}
			logger.LogInfo($"Processing time: {sw.ElapsedMilliseconds} ms");
		}

		protected void Do(Action action)
		{
			try
			{
				errorMessageTextBlock.Visibility = Visibility.Hidden;
				action();
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				logger.LogDebug(ex.StackTrace);
				errorMessageTextBlock.Text = ex.Message;
				errorMessageTextBlock.Visibility = Visibility.Visible;
			}
		}

		public abstract void Dispose();

		protected abstract void OnSubmit();
	}
}
