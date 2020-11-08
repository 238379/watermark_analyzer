using Algorithms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class LsbViewModel : AlgorithmViewModel
	{
		private TextBox bitsTextBox;

		public LsbViewModel(Grid grid) : base(grid)
		{

		}

		public override void PrepareControlls()
		{
			AddLabel("Bits for watermark", 0, 0);
			bitsTextBox = AddTextBox("2", 1, 0);
		}

		public override Dictionary<string, dynamic> ReadParameters()
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
