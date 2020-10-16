using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DigitalMarkingAnalyzer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void BrowseOriginalButton_Click(object sender, RoutedEventArgs e)
		{
			BrowseImage(OriginalImage);
		}

		private void BrowseWatermarkButton_Click(object sender, RoutedEventArgs e)
		{
			BrowseImage(WatermarkImage);
		}

		private void BrowseImage(Image imageContainer)
		{
			var openFileDialog = new OpenFileDialog();
			if (openFileDialog.ShowDialog() == true)
			{
				imageContainer.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
			}
		}

		private void ProcessButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void CloseResultTabButton_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
