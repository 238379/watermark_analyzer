using FluentAssertions;
using System.Drawing;

namespace AlgorithmTest
{
	public class AlgorithmTests
	{
		protected void AssertBitmapsAreEqual(Bitmap bitmap1, Color[,] bitmap2)
		{
			bitmap1.Width.Should().Be(bitmap2.GetLength(1));
			bitmap1.Height.Should().Be(bitmap2.GetLength(0));
			for (int i = 0; i < bitmap1.Width; i++)
			{
				for (int j = 0; j < bitmap1.Height; j++)
				{
					bitmap1.GetPixel(i, j).Should().Be(bitmap2[i, j]);
				}
			}
		}
	}
}
