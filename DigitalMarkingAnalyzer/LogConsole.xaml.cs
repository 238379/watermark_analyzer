using LoggerUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace DigitalMarkingAnalyzer
{
    public partial class LogConsole : Window
	{
        public LogConsole() { }

        public LogConsole(MainWindow mainWindow)
        {
            InitializeComponent();
            mainWindow.Closed += (_, __) => Close();

            LoggerFactory.SetConsoleLoggerWriteAction(log => Dispatcher.Invoke(() => OutputBlock.Text += log));
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            LoggerFactory.SetConsoleLoggerWriteAction(_ => { });
            base.OnClosing(e);
        }

        private void ClearConsoleButton_Click(object sender, RoutedEventArgs e)
        {
            OutputBlock.Text = "";
        }
    }
}
