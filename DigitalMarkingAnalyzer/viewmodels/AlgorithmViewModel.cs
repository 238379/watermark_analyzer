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
		public static AlgorithmViewModel Create(string algorithmName, MainWindow window)
		{
			return algorithmName switch
			{
				Lsb.ALGORITHM_NAME => new LsbViewModel(window),
				PixelAveraging.ALGORITHM_NAME => new PixelAveragingViewModel(window),
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
			window.WatermarkedImage.Source = InterfaceTools.BitmapToImageSource(result.Watermarked);
			window.CleanedImage.Source = InterfaceTools.BitmapToImageSource(result.Cleaned);
			window.ExtractedWatermarkImage.Source = InterfaceTools.BitmapToImageSource(result.ExtractedWatermark);

			window.ResultTab.Visibility = Visibility.Visible;
			window.Tabs.SelectedIndex = 1;
		}
	}
}
