using Algorithms.common;
using System;
using System.Drawing;

namespace Algorithms
{
	public class DctParameters : AlgorithmParameters
	{
		public readonly int Key;
		public readonly double Alpha;

		public DctParameters(Bitmap original, Bitmap watermark, Bitmap watermarked, int key, double alpha) : base(original, watermark, watermarked)
		{
			Key = key;
			Alpha = alpha;
		}
	}

	public class Dct : Algorithm
	{
		public const string ALGORITHM_NAME = "DCT";

		private readonly DctParameters parameters;

		public Dct(DctParameters parameters) : base()
		{
			this.parameters = parameters;
		}

		public Bitmap CleanWatermark(Bitmap watermarked)
		{
			throw new NotImplementedException();
		}

		public Bitmap ExtractWatermark(Bitmap watermarked)
		{
			throw new NotImplementedException();
		}

		private DctImage EmbedWatermark(DctImage image, DctImage watermark)
		{
			int[] v = new int[image.Width];
            double[] vAlpha = new double[image.Width];
            Random random = new Random(parameters.Key);
            for (int i = 0; i < image.Width; i++)
            {
                v[i] = random.Next(0, 2);
                vAlpha[i] = v[i] * parameters.Alpha;
            }

			DctImage watermarked = new DctImage(image);

            for (int y = 0; y < image.Height; y++)
            {
				for (int x = 0; x < image.Width; x++)
				{
					watermarked.Data[y, x] += vAlpha[x] * watermark.Data[y, x];
				}
			}
			watermarked.Transformed = true;
			return watermarked;
        }

        public override AlgorithmResult AddWatermark()
        {
			var cosineTransform = new DctImage(parameters.Original);
			cosineTransform.DCT();
			
			var cosineWatermarked = new DctImage(parameters.Watermark);
			cosineWatermarked.DCT();

			var watermarked = EmbedWatermark(cosineTransform, cosineWatermarked);

			watermarked.IDCT();

			return new AlgorithmResult(("Cosine transform (DCT)", cosineTransform.ToBitmap()), ("DCT + watermark", cosineWatermarked.ToBitmap()), ("Watermarked", watermarked.ToBitmap()));
		}

        public override AlgorithmResult RemoveWatermark()
        {
            throw new NotImplementedException();
        }
    }
}
