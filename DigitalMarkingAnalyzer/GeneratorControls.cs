using System.Windows.Controls;

namespace DigitalMarkingAnalyzer
{
	public class GeneratorControls
	{
		public readonly Button GenerateButton;
		public readonly UpdatableImage ImageContainer;

		public GeneratorControls(Button generateButton, UpdatableImage imageContainer)
		{
			this.GenerateButton = generateButton;
			this.ImageContainer = imageContainer;
		}
	}
}
