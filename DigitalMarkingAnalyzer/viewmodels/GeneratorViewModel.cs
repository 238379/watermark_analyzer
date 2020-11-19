using Generators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public abstract class GeneratorViewModel : ViewModel
	{
		protected Image imageContainer;

		public GeneratorViewModel(Grid parametersGrid, TextBlock errorTextBlock, Image imageContainer) : base(parametersGrid, errorTextBlock)
		{
			this.imageContainer = imageContainer;
		}
	}
}
