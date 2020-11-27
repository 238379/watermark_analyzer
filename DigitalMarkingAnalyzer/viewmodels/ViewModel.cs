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

		protected void Do(Action action)
		{
			try
			{
				window.ErrorMessage.Visibility = Visibility.Hidden;
				action();
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				logger.LogDebug(ex.StackTrace);
				window.ErrorMessage.Text = ex.Message;
				window.ErrorMessage.Visibility = Visibility.Visible;
			}
		}

		public abstract void Dispose();

		protected abstract void OnSubmit();
	}
}
