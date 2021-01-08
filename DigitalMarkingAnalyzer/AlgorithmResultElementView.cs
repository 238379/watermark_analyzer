using System;
using System.Windows;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer
{
	class AlgorithmResultElementView
    {
        public readonly Grid Grid;
        public readonly Label Label;
        public readonly Image Image;
        public readonly Button SaveButton;
        public readonly Button UseAsWatermarkedButton;

        public AlgorithmResultElementView(string label, System.Drawing.Bitmap bitmap, Action<System.Drawing.Bitmap> onUse)
        {
            Grid = new Grid
            {
                Height = 500
            };

            if(label != null)
            {
                Label = new Label
                {
                    Content = label,
                    Width = 400,
                    Height = 50,
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };

                Grid.Children.Add(Label);
            }

            if (bitmap != null)
            {
                Image = new Image()
                {
                    Source = InterfaceTools.BitmapToImageSource(bitmap),
                    Width = 400,
                    Height = 400,
                    Margin = new Thickness(0, 50, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Stretch = System.Windows.Media.Stretch.Fill
                };

                SaveButton = new Button()
                {
                    Width = 100,
                    Height = 30,
                    Content = "Save",
                    Margin = new Thickness(10, 460, 10, 0),
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                SaveButton.Click += (_, __) => InterfaceTools.SaveImageToDrive(Image);

                UseAsWatermarkedButton = new Button()
                {
                    Width = 100,
                    Height = 30,
                    Content = "Use",
                    Margin = new Thickness(10, 460, 10, 0),
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                UseAsWatermarkedButton.Click += (_, __) => onUse(bitmap);

                Grid.Children.Add(Image);
                Grid.Children.Add(SaveButton);
                Grid.Children.Add(UseAsWatermarkedButton);
            }
        }
    }
}
