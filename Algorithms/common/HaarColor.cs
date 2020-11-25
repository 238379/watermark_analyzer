using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Algorithms.common
{
	public class HaarColor
	{
		public double r { get; }
		public double g { get; }
		public double b { get; }

		public HaarColor(Color color)
		{
			this.r = ToDouble(color.R);
			this.g = ToDouble(color.G);
			this.b = ToDouble(color.B);
		}

		public HaarColor(double r, double g, double b)
		{
			this.r = r;
			this.g = g;
			this.b = b;
		}

		public int ToHex(double x)
		{
			double value = (255 * (x + 1) / 2);
			return (int)(value> 255 ? 255 : value < 0 ? 0 : value);
		}

		public double ToDouble(double x)
        {
			double value = (2 * x / 255) - 1;
			return (value > 1 ? 1 : value < -1 ? -1 : value);
		}

		public Color GetColor()
		{
			return Color.FromArgb(ToHex(r),ToHex(g), ToHex(b));
		}

	}
}
