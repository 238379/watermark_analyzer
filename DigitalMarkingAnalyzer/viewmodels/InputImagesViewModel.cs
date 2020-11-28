using Common;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DigitalMarkingAnalyzer.viewmodels
{
	class InputImagesViewModel : ViewModel
	{
		private const int IMAGES_MEMORY = 10;

		private const double ACTIVE_BUTTON_OPACITY = 1;
		private const double NON_ACTIVE_BUTTON_OPACITY = 0.5;

		private ImageSource defaultOriginalSource;
		private ImageSource defaultWatermarkSource;
		private ImageSource defaultWatermarkedSource;

		private readonly DropoutStack<ImageSource> originalBitmaps;
		private readonly DropoutStack<ImageSource> watermarkBitmaps;
		private readonly DropoutStack<ImageSource> watermarkedBitmaps;

		private readonly MainWindow window;

		private readonly Image originalImage;
		private readonly Image watermarkImage;
		private readonly Image watermarkedImage;

		// TODO REFACTOR
		public InputImagesViewModel(MainWindow window, Image originalImage, Image watermarkImage, Image watermarkedImage, TextBlock errorMessageTextBlock) : base(errorMessageTextBlock)
		{
			this.window = window;
			originalBitmaps = new DropoutStack<ImageSource>(IMAGES_MEMORY);
			watermarkBitmaps = new DropoutStack<ImageSource>(IMAGES_MEMORY);
			watermarkedBitmaps = new DropoutStack<ImageSource>(IMAGES_MEMORY);

			this.originalImage = originalImage;
			this.watermarkImage = watermarkImage;
			this.watermarkedImage = watermarkedImage;
		}

		public override void Dispose()
		{
			window.BrowseOriginalButton.Click -= SetOriginalImageFromDrive;
			window.BrowseWatermarkButton.Click -= SetWatermarkImageFromDrive;
			window.BrowseWatermarkedButton.Click -= SetWatermarkedImageFromDrive;

			window.OriginalImage.SourceChanged -= RegisterOriginalImageSourceUpdated;
			window.WatermarkImage.SourceChanged -= RegisterWatermarkImageSourceUpdated;

			window.UndoOriginalButton.Click -= RevertOriginalImage;
			window.UndoWatermarkButton.Click -= RevertWatermarkImage;

			window.ToDefaultOriginalButton.Click -= ToDefaultOriginalImage;
			window.ToDefaultWatermarkButton.Click -= ToDefaultWatermarkImage;
		}

		public override void SetUp()
		{
			window.OriginalImage.SetSource(new Uri("/DigitalMarkingAnalyzer;component/Resources/c_corgi.jpg", UriKind.RelativeOrAbsolute));
			window.WatermarkImage.SetSource(new Uri("/DigitalMarkingAnalyzer;component/Resources/w_tekst_dolny.png", UriKind.RelativeOrAbsolute));
			window.WatermarkedImage.SetSource(new Uri("/DigitalMarkingAnalyzer;component/Resources/t_corgi_tekst_dolny.jpg", UriKind.RelativeOrAbsolute));

			InterfaceTools.RegisterOpenImageWindowOnClick(window, originalImage);
			InterfaceTools.RegisterOpenImageWindowOnClick(window, watermarkImage);
			InterfaceTools.RegisterOpenImageWindowOnClick(window, watermarkedImage);

			defaultOriginalSource = window.OriginalImage.Source;
			defaultWatermarkSource = window.WatermarkImage.Source;

			window.OriginalImage.SourceChanged += RegisterOriginalImageSourceUpdated;
			window.WatermarkImage.SourceChanged += RegisterWatermarkImageSourceUpdated;

			window.BrowseOriginalButton.Click += SetOriginalImageFromDrive;
			window.BrowseWatermarkButton.Click += SetWatermarkImageFromDrive;
			window.BrowseWatermarkedButton.Click += SetWatermarkedImageFromDrive;

			window.UndoOriginalButton.Click += RevertOriginalImage;
			window.UndoWatermarkButton.Click += RevertWatermarkImage;

			window.ToDefaultOriginalButton.Click += ToDefaultOriginalImage;
			window.ToDefaultWatermarkButton.Click += ToDefaultWatermarkImage;

			SetUndoOriginalVisibility();
			SetUndoWatermarkVisibility();

			SetToDefaultOriginalVisibility();
			SetToDefaultWatermarkVisibility();
		}

		protected override void OnSubmit()
		{
			throw new NotImplementedException();
		}

		private void SetOriginalImageFromDrive(object sender, RoutedEventArgs e)
		{
			Do(() => InterfaceTools.SetImageFromDrive(window.OriginalImage));
		}

		private void SetWatermarkImageFromDrive(object sender, RoutedEventArgs e)
		{
			Do(() => InterfaceTools.SetImageFromDrive(window.WatermarkImage));
		}

		private void SetWatermarkedImageFromDrive(object sender, RoutedEventArgs e)
		{
			Do(() => InterfaceTools.SetImageFromDrive(window.WatermarkedImage));
		}

		private void RegisterOriginalImageSourceUpdated(object source, ImageSourceChangedEventArgs args)
		{
			originalBitmaps.Push(args.OldImageSource);
			SetUndoOriginalVisibility();
			SetToDefaultOriginalVisibility();
		}

		private void RegisterWatermarkImageSourceUpdated(object source, ImageSourceChangedEventArgs args)
		{
			watermarkBitmaps.Push(args.OldImageSource);
			SetUndoWatermarkVisibility();
			SetToDefaultWatermarkVisibility();
		}

		private void SetUndoOriginalVisibility()
		{
			if(originalBitmaps.HasElements)
			{
				window.UndoOriginalButton.Visibility = Visibility.Visible;
			}
			else
			{
				window.UndoOriginalButton.Visibility = Visibility.Hidden;
			}
		}

		private void SetUndoWatermarkVisibility()
		{
			if (watermarkBitmaps.HasElements)
			{
				window.UndoWatermarkButton.Visibility = Visibility.Visible;
			}
			else
			{
				window.UndoWatermarkButton.Visibility = Visibility.Hidden;
			}
		}

		private void SetToDefaultOriginalVisibility()
		{
			if (window.OriginalImage.Source != defaultOriginalSource)
			{
				window.ToDefaultOriginalButton.IsHitTestVisible = true;
				window.ToDefaultOriginalButton.Opacity = ACTIVE_BUTTON_OPACITY;
			}
			else
			{
				window.ToDefaultOriginalButton.IsHitTestVisible = false;
				window.ToDefaultOriginalButton.Opacity = NON_ACTIVE_BUTTON_OPACITY;
			}
		}

		private void SetToDefaultWatermarkVisibility()
		{
			if (window.WatermarkImage.Source != defaultWatermarkSource)
			{
				window.ToDefaultWatermarkButton.IsHitTestVisible = true;
				window.ToDefaultWatermarkButton.Opacity = ACTIVE_BUTTON_OPACITY;
			}
			else
			{
				window.ToDefaultWatermarkButton.IsHitTestVisible = false;
				window.ToDefaultWatermarkButton.Opacity = NON_ACTIVE_BUTTON_OPACITY;
			}
		}

		private void RevertOriginalImage(object sender, RoutedEventArgs e)
		{
			Do(() =>
			{
				if (originalBitmaps.HasElements)
				{
					originalImage.Source = originalBitmaps.Pop();
					SetUndoOriginalVisibility();
					SetToDefaultOriginalVisibility();
				}
			});
		}

		private void RevertWatermarkImage(object sender, RoutedEventArgs e)
		{
			Do(() =>
			{
				if (watermarkBitmaps.HasElements)
				{
					watermarkImage.Source = watermarkBitmaps.Pop();
					SetUndoWatermarkVisibility();
					SetToDefaultWatermarkVisibility();
				}
			});
		}

		private void ToDefaultOriginalImage(object sender, RoutedEventArgs e)
		{
			Do(() =>
			{
				if (window.OriginalImage.Source != defaultOriginalSource)
				{
					window.OriginalImage.SetSource(defaultOriginalSource);
					SetToDefaultOriginalVisibility();
				}
			});
		}

		private void ToDefaultWatermarkImage(object sender, RoutedEventArgs e)
		{
			Do(() =>
			{
				if(window.WatermarkImage.Source != defaultWatermarkSource)
				{
					window.WatermarkImage.SetSource(defaultWatermarkSource);
					SetToDefaultWatermarkVisibility();
				}
			});
		}
	}
}
