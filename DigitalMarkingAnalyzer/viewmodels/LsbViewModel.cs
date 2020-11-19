using Algorithms;
using Algorithms.common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class LsbViewModel : AlgorithmViewModel
	{
		private TextBox bitsTextBox;

		public LsbViewModel(Grid grid, TextBlock errorTextBlock, System.Windows.Controls.Image originalImageContainer, System.Windows.Controls.Image watermarkImageContainer)
			: base(grid, errorTextBlock, originalImageContainer, watermarkImageContainer)
		{

		}

		public override void SetUp()
		{
			AddLabel("Bits for watermark", 0, 0);
			bitsTextBox = AddTextBox("2", 1, 0);
		}

		protected override void OnSubmit()
		{
			var p = ReadParameters();
			var algorithm = new Lsb(p);
			var (originalAsBitmap, watermarkAsBitmap) = ReadInputBitmaps();
			algorithmResult = algorithm.Run(originalAsBitmap, watermarkAsBitmap);
		}

		private Dictionary<string, dynamic> ReadParameters()
		{
			if (int.TryParse(bitsTextBox.Text, out var bitsForWatermark) && bitsForWatermark >= 1 && bitsForWatermark <= 8)
			{
				return new Dictionary<string, dynamic> { { Lsb.BITS_PARAM, bitsForWatermark } };
			}
			else
			{
				throw new ArgumentException($"Invalid bits for watermark value. It should be between [1; 8] but it is: {bitsTextBox.Text}");
			}
		}
	}
}
