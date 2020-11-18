using Generators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	class GeneratorViewModel : ViewModel
	{
		public GeneratorViewModel(Grid grid) : base(grid)
		{
		}

		public override void PrepareControlls()
		{
			throw new NotImplementedException();
		}

		public override Dictionary<string, dynamic> ReadParameters()
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
