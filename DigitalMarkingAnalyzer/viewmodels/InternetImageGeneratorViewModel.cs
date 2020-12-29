using Generators;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class InternetImageGeneratorViewModel : GeneratorViewModel
	{
		private InternetImageGenerator generator;

		public InternetImageGeneratorViewModel(GeneratorControls generatorControls, TextBlock errorMessageTextBlock) : base(generatorControls, errorMessageTextBlock)
		{
		}

		protected override Task OnSubmit()
		{
			var bitmap = generator.Generate();
			controls.ImageContainer.SetSource(bitmap);
			return Task.FromResult(0);
		}

		public override void SetUp()
		{
			controls.GenerateButton.Click += GenerateOriginalImage;
			generator = new InternetImageGenerator(MockParameters());
		}

		public override void Dispose()
		{
			controls.GenerateButton.Click -= GenerateOriginalImage;
			generator = null;
		}

		private void GenerateOriginalImage(object sender, System.Windows.RoutedEventArgs e)
		{
			Submit();
		}

		private Dictionary<string, dynamic> MockParameters()
		{
			return new Dictionary<string, dynamic>();
		}
	}
}
