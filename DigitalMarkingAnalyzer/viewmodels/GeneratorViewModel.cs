using Generators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public abstract class GeneratorViewModel : ViewModel
	{
		protected GeneratorControls controls;

		public GeneratorViewModel(GeneratorControls generatorControls, TextBlock errorMessageTextBlock) : base(errorMessageTextBlock)
		{
			this.controls = generatorControls;
		}
	}
}
