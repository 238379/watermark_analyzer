using Algorithms;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels.basic
{
	public class RecognizerViewModel : AlgorithmViewModel
	{
		public const string ALGORITHM_NAME = "Unspecified";

		public RecognizerViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
		{
		}

		public override void SetUp()
		{
			
		}

		protected override Task ProcessAdding(CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		protected override Task ProcessRemoving(CancellationToken ct)
		{
			return Task.Run(async () =>
			{
				var parameters = ReadParameters();
				var algorithm = new Recognizer(parameters);
				var result = algorithm.RemoveWatermark(ct);
				await ShowAlgorithmOutput(result);
			});
		}

		private RecognizerParameters ReadParameters()
		{
			var (original, watermark, watermarked) = ReadInputBitmaps();
			return new RecognizerParameters(original, watermark, watermarked);
		}
	}
}
