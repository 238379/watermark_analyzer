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

		public GeneratorViewModel(MainWindow window, Image imageContainer) : base(window)
		{
			this.imageContainer = imageContainer;
		}
	}
}
