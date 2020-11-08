using Algorithms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace DigitalMarkingAnalyzer.viewmodels
{
	public abstract class AlgorithmViewModel : IDisposable
	{
		protected readonly Grid Grid;

		public static AlgorithmViewModel Create(string algorithmName, Grid grid)
		{
			return algorithmName switch
			{
				Lsb.ALGORITHM_NAME => new LsbViewModel(grid),
				PixelAveraging.ALGORITHM_NAME => new PixelAveragingViewModel(grid),
				_ => throw new ArgumentException($"Unknown algorithmName '{algorithmName}'."),
			};
		}

		public AlgorithmViewModel(Grid grid)
		{
			Grid = grid;
		}


		public abstract void PrepareControlls();
		public abstract Dictionary<string, dynamic> ReadParameters();

		public virtual void Dispose()
		{
			Grid.Children.RemoveRange(0, Grid.Children.Count);
		}


		protected Label AddLabel(string labelContent, int x, int y)
		{
			var label = new Label
			{
				Content = labelContent,
				HorizontalContentAlignment = HorizontalAlignment.Left,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			AddAtPositionInGrid(label, x, y);
			return label;
		}

		protected TextBox AddTextBox(string initContent, int x, int y)
		{
			var textBox = new TextBox
			{
				Text = initContent,
				HorizontalContentAlignment = HorizontalAlignment.Right,
				VerticalContentAlignment = VerticalAlignment.Center
			};
			AddAtPositionInGrid(textBox, x, y);
			return textBox;
		}

		private void AddAtPositionInGrid(UIElement element, int x, int y)
		{
			Grid.Children.Add(element);
			Grid.SetColumn(element, x);
			Grid.SetRow(element, y);
		}
	}
}
