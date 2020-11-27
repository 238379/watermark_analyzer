using Generators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public abstract class GeneratorViewModel : ViewModel
	{
		protected UpdatableImage imageContainer;

		public GeneratorViewModel(MainWindow window, UpdatableImage imageContainer) : base(window)
		{
			this.imageContainer = imageContainer;
		}
	}
}
