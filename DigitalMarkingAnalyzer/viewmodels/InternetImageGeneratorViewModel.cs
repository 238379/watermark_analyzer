using Generators;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public class InternetImageGeneratorViewModel : GeneratorViewModel
	{
		private InternetImageGenerator generator;

		public InternetImageGeneratorViewModel(MainWindow window, UpdatableImage imageContainer) : base(window, imageContainer)
		{
		}

		protected override void OnSubmit()
		{
			var bitmap = generator.Generate();
			imageContainer.SetSource(bitmap);
		}

		public override void SetUp()
		{
			window.GenerateOriginalButton.Click += GenerateOriginalImage;
			generator = new InternetImageGenerator(MockParameters());
		}

		public override void Dispose()
		{
			window.GenerateOriginalButton.Click -= GenerateOriginalImage;
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
