using Generators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class TextImageGeneratorViewModel : GeneratorViewModel
	{
		private TextImageGenerator generator;

		public TextImageGeneratorViewModel(GeneratorControls generatorControls, TextBlock errorMessageTextBlock) : base(generatorControls, errorMessageTextBlock)
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
			controls.GenerateButton.Click += GenerateWatermarkImage;
			generator = new TextImageGenerator(MockParameters());
		}

		public override void Dispose()
		{
			controls.GenerateButton.Click -= GenerateWatermarkImage;
			generator = null;
		}

		private void GenerateWatermarkImage(object sender, System.Windows.RoutedEventArgs e)
		{
			Submit();
		}

		private Dictionary<string, dynamic> MockParameters()
		{
			return new Dictionary<string, dynamic>
			{
				{ TextImageGenerator.TEXT_PARAM, new Func<string>(() => LoremIpsumGenerator.LoremIpsum(2, 4, 1, 2, 10)) },
				{ TextImageGenerator.WIDTH_PARAM, 800 },
				{ TextImageGenerator.HEIGHT_PARAM, 800 }
			};
		}
	}
}
