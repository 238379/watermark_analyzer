using System.Drawing;

namespace Algorithms.common
{
    class DctImage
    {
        private byte[,,] YCbCr;
        public double[,] Data { get; set; }
        public int Height { get; }
        public int Width { get; }
        public bool Transformed { get; set; } = false;

        public DctImage(Bitmap image)
        {
            YCbCr = BitmapExtensions.Rgb2Ycbcr(image);
            Height = image.Height;
            Width = image.Width;
            Data = new double[Height, Width];
            image.RunOnEveryPixel((i, j) =>
            {
                Data[i, j] = YCbCr[i, j, 0];
            });
        }

        public DctImage(DctImage image)
        {
            Height = image.Height;
            Width = image.Width;
            YCbCr = new byte[Height, Width, 3];
            Data = new double[Height, Width];
            Transformed = image.Transformed;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Data[y, x] = image.Data[y, x];
                    YCbCr[y, x, 0] = image.YCbCr[y, x, 0];
                    YCbCr[y, x, 1] = image.YCbCr[y, x, 1];
                    YCbCr[y, x, 2] = image.YCbCr[y, x, 2];
                }
            }
        }

        public Bitmap ToBitmap()
        {
            var resultImage = new Bitmap(Width, Height);
            if (Transformed)
            {
                resultImage.RunOnEveryPixel((i, j) =>
                {
                    var value = (byte)Data[i, j];
                    resultImage.SetPixel(i, j, Color.FromArgb(value, value, value));
                });
            }
            else
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        YCbCr[y, x, 0] = (byte)Data[y, x];
                    }
                }
                resultImage.Ycbcr2Rgb(YCbCr);
            }
            return resultImage;
        }

        public void DCT()
        {
            if (!Transformed)
            {
                CosineTransform.DCT(Data);
                Transformed = true;
            }
        }

        public void IDCT()
        {
            if (Transformed)
            {
                CosineTransform.IDCT(Data);
                Transformed = false;
            }
        }
    }
}
