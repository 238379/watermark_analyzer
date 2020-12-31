using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels.basic
{
	public class UnspecifiedViewModel : AlgorithmViewModel
	{
		public const string ALGORITHM_NAME = "Unspecified";

		public UnspecifiedViewModel(AlgorithmControls algorithmControls, MainWindow mainWindow, TextBlock errorMessageTextBlock) : base(algorithmControls, mainWindow, errorMessageTextBlock)
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
			throw new NotImplementedException();
		}
	}
}
