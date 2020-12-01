using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DigitalMarkingAnalyzer
{
    public partial class ImageWindow : Window
	{
        public ImageWindow(MainWindow mainWindow, ImageSource imageSource)
        {
            InitializeComponent();
            mainWindow.Closed += (_, __) => Close();

            // magic
            if(Height > imageSource.Height + 38)
            {
                Height = imageSource.Height + 38;
            }
            if (Width > imageSource.Width + 15)
            {
                Width = imageSource.Width + 15;
            }

            Image.Source = imageSource;
            Show();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }
    }
}
