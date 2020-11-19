using Generators;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class InternetImageGeneratorViewModel : GeneratorViewModel
	{
		private InternetImageGenerator generator;

		public InternetImageGeneratorViewModel(Grid parametersGrid, TextBlock errorTextBlock, Image imageContainer) : base(parametersGrid, errorTextBlock, imageContainer)
		{
		}

		protected override void OnSubmit()
		{
			var bitmap = generator.Generate();
			InterfaceTools.SetImageFromBitmap(imageContainer, bitmap);
		}

		public override void SetUp()
		{
			generator = new InternetImageGenerator(MockParameters());
		}

		private Dictionary<string, dynamic> MockParameters()
		{
			return new Dictionary<string, dynamic>();
		}
	}
}
