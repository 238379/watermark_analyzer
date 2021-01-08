using Common;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DigitalMarkingAnalyzer.viewmodels
{
	class InputImageViewModel : ViewModel
	{
		private const int IMAGES_MEMORY = 10;

		private const double ACTIVE_BUTTON_OPACITY = 1;
		private const double NON_ACTIVE_BUTTON_OPACITY = 0.5;

		private ImageSource defaultImageSource;

		private readonly DropoutStack<ImageSource> bitmapsBuffer;
		private readonly MainWindow window;
		private readonly Image image;
		private readonly UpdatableImage updatableImage;
		private readonly Uri defaultBitmap;
		private readonly Button browseImageButton;
		private readonly Button undoButton;
		private readonly Button defaultButton;

		public InputImageViewModel(MainWindow window, Image image, UpdatableImage updatableImage, Uri defaultBitmap, Button browseImageButton, Button undoButton, Button defaultButton,
			TextBlock errorMessageTextBlock) : base(errorMessageTextBlock)
		{
			bitmapsBuffer = new DropoutStack<ImageSource>(IMAGES_MEMORY);
			this.window = window;
			this.image = image;
			this.updatableImage = updatableImage;
			this.defaultBitmap = defaultBitmap;
			this.browseImageButton = browseImageButton;
			this.undoButton = undoButton;
			this.defaultButton = defaultButton;
		}

		public override void Dispose()
		{
			browseImageButton.Click -= SetImageFromDrive;
			updatableImage.SourceChanged -= RegisterOriginalImageSourceUpdated;
			undoButton.Click -= RevertOriginalImage;
			defaultButton.Click -= ToDefaultImage;
		}

		public override void SetUp()
		{
			updatableImage.SetSource(defaultBitmap);
			InterfaceTools.RegisterOpenImageWindowOnClick(window, image);
			defaultImageSource = updatableImage.Source;

			updatableImage.SourceChanged += RegisterOriginalImageSourceUpdated;
			browseImageButton.Click += SetImageFromDrive;
			undoButton.Click += RevertOriginalImage;
			defaultButton.Click += ToDefaultImage;

			SetUndoVisibility();
			SetToDefaultVisibility();
		}

		protected override Task OnSubmit()
		{
			throw new NotImplementedException();
		}

		private void SetImageFromDrive(object sender, RoutedEventArgs e)
		{
			Do(() => InterfaceTools.SetImageFromDrive(image));
		}

		private void RegisterOriginalImageSourceUpdated(object source, ImageSourceChangedEventArgs args)
		{
			bitmapsBuffer.Push(args.OldImageSource);
			SetUndoVisibility();
			SetToDefaultVisibility();
		}

		private void SetUndoVisibility()
		{
			if(bitmapsBuffer.HasElements)
			{
				undoButton.Visibility = Visibility.Visible;
			}
			else
			{
				undoButton.Visibility = Visibility.Hidden;
			}
		}

		private void SetToDefaultVisibility()
		{
			if (updatableImage.Source != defaultImageSource)
			{
				defaultButton.IsHitTestVisible = true;
				defaultButton.Opacity = ACTIVE_BUTTON_OPACITY;
			}
			else
			{
				defaultButton.IsHitTestVisible = false;
				defaultButton.Opacity = NON_ACTIVE_BUTTON_OPACITY;
			}
		}

		private void RevertOriginalImage(object sender, RoutedEventArgs e)
		{
			Do(() =>
			{
				if (bitmapsBuffer.HasElements)
				{
					image.Source = bitmapsBuffer.Pop();
					SetUndoVisibility();
					SetToDefaultVisibility();
				}
			});
		}

		private void ToDefaultImage(object sender, RoutedEventArgs e)
		{
			Do(() =>
			{
				if (updatableImage.Source != defaultImageSource)
				{
					updatableImage.SetSource(defaultImageSource);
					SetToDefaultVisibility();
				}
			});
		}
	}
}
