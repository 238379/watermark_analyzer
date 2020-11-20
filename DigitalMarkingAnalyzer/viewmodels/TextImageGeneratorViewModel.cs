using Generators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class TextImageGeneratorViewModel : GeneratorViewModel
	{
		private TextImageGenerator generator;

		public TextImageGeneratorViewModel(MainWindow window, Image imageContainer) : base(window, imageContainer)
		{
		}

		protected override void OnSubmit()
		{
			var bitmap = generator.Generate();
			InterfaceTools.SetImageFromBitmap(imageContainer, bitmap);
		}

		public override void SetUp()
		{
			generator = new TextImageGenerator(MockParameters());
		}
		private Dictionary<string, dynamic> MockParameters()
		{
			return new Dictionary<string, dynamic>
			{
				{ TextImageGenerator.TEXT_PARAM, LoremIpsumGenerator.LoremIpsum(2, 4, 1, 2, 10) },
				{ TextImageGenerator.WIDTH_PARAM, 800 },
				{ TextImageGenerator.HEIGHT_PARAM, 800 }
			};
		}
	}
}
