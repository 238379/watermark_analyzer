using Algorithms;
using Algorithms.common;
using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public abstract class AlgorithmViewModel : ViewModel
	{
		protected readonly System.Windows.Controls.Image originalImageContainer;
		protected readonly System.Windows.Controls.Image watermarkImageContainer;

		protected AlgorithmResult algorithmResult;
		public AlgorithmResult LastResult => algorithmResult; // TODO

		public static AlgorithmViewModel Create(string algorithmName, Grid grid, TextBlock errorTextBlock, System.Windows.Controls.Image originalImageContainer, System.Windows.Controls.Image watermarkImageContainer)
		{
			return algorithmName switch
			{
				Lsb.ALGORITHM_NAME => new LsbViewModel(grid, errorTextBlock, originalImageContainer, watermarkImageContainer),
				PixelAveraging.ALGORITHM_NAME => new PixelAveragingViewModel(grid, errorTextBlock, originalImageContainer, watermarkImageContainer),
				_ => throw new ArgumentException($"Unknown algorithmName '{algorithmName}'."),
			};
		}

		public AlgorithmViewModel(Grid parametersGrid, TextBlock errorTextBlock, System.Windows.Controls.Image orignalBitmap, System.Windows.Controls.Image watermarkBitmap) : base(parametersGrid, errorTextBlock)
		{
			this.originalImageContainer = orignalBitmap;
			this.watermarkImageContainer = watermarkBitmap;
		}

		protected (Bitmap, Bitmap) ReadInputBitmaps()
		{
			var originalAsBitmapImage = (BitmapImage)originalImageContainer.Source;
			var watermarkAsBitmapImage = (BitmapImage)watermarkImageContainer.Source;

			return (originalAsBitmapImage.ToBitmap(), watermarkAsBitmapImage.ToBitmap());
		}
	}
}
