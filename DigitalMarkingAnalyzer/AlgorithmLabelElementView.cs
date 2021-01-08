using System.Windows;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer
{
	class AlgorithmLabelElementView
    {
        public readonly Grid Grid;
        public readonly Label Label;

        public AlgorithmLabelElementView(string label)
        {
            if(label != null)
            {
                Grid = new Grid
                {
                    Height = 50
                };

                Label = new Label
                {
                    Content = label,
                    Width = 800,
                    Height = 50,
                    FontSize = 24,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };

                Grid.Children.Add(Label);
            }
            else
            {
                Grid = new Grid
                {
                    Height = 0,
                    Width = 0
                };
            }

        }
    }
}
