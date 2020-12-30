using Algorithms.common;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer
{
	public class AlgorithmControls
	{
		public readonly AlgorithmMode AlgorithmMode;
		public readonly Grid ParametersGrid;
		public readonly Button ProcessButton;
		public readonly Image OriginalImage;
		public readonly Image WatermarkImage;
		public readonly TabItem ResultTab;
		public readonly int ResultTabIndex;
		public readonly Image WatermarkedImage;
		public readonly TabControl TabControl;
		public readonly Grid ResultGrid;
		public readonly ScrollViewer ResultScrollViewer;
		public readonly Button CloseButton;
		public readonly Button CancelButton;

		public AlgorithmControls(AlgorithmMode algorithmMode, Grid parametersGrid, Button processButton, Image originalImage, Image watermarkImage, Image watermarkedImage,
			TabControl tabControl, TabItem resultTab, int resultTabIndex, Grid resultGrid, ScrollViewer resultScrollViewer, Button closeButton, Button cancelButton)
		{
			AlgorithmMode = algorithmMode;
			ParametersGrid = parametersGrid;
			ProcessButton = processButton;
			OriginalImage = originalImage;
			WatermarkImage = watermarkImage;
			WatermarkedImage = watermarkedImage;
			TabControl = tabControl;
			ResultTab = resultTab;
			ResultTabIndex = resultTabIndex;
			ResultGrid = resultGrid;
			ResultScrollViewer = resultScrollViewer;
			CloseButton = closeButton;
			CancelButton = cancelButton;
		}
	}
}
