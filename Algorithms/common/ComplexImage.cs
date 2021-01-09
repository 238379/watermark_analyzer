using Algorithms.common;
using System;
using System.Drawing;
using System.Numerics;

namespace Algorithms
{
    public class ComplexImage : ICloneable
    {

        public int Width { get; }
        public int Height { get; }
        public int OriginalWidth { get; }
        public int OriginalHeight { get; }
        public bool FourierTransformed { get; private set; } = false;
        public bool CosineTransformed { get; private set; } = false;
        public Complex[,] Data { get; set; }
        public double[,] DCTData { get; set; }
        public byte[,,] YCbCr
        {
            get => _YCbCr;
            set
            {
                if (value.GetLength(0) != Width || value.GetLength(1) != Height)
                {
                    throw new ArgumentException($"Can't set YCbCr {value.GetLength(0)}x{value.GetLength(1)}. Should be {Width}x{Height}.");
                }
                _YCbCr = value;
            }
        }
        private byte[,,] _YCbCr;


        protected ComplexImage(int width, int height, int originalWidth, int originalHeight)
        {
            Width = width;
            Height = height;
            OriginalWidth = originalWidth;
            OriginalHeight = originalHeight;
            Data = new Complex[height, width];
            DCTData = new double[height, width];
            FourierTransformed = false;
        }

        public ComplexImage(ComplexImage complexImage)
        {
            Width = complexImage.Width;
            Height = complexImage.Height;
            OriginalWidth = complexImage.OriginalWidth;
            OriginalHeight = complexImage.OriginalHeight;
            Data = new Complex[Height, Width];
            DCTData = new double[Height, Width];
            for (int y = 0; y < Width; y++)
            {
                for (int x = 0; x < Height; x++)
                {
                    Data[x, y] = new Complex(complexImage.Data[x, y].Real, complexImage.Data[x, y].Imaginary);
                    DCTData[x, y] = complexImage.DCTData[x, y];
                }
            }
            YCbCr = new byte[Width, Height, 3];
            for (int y = 0; y < Width; y++)
            {
                for (int x = 0; x < Height; x++)
                {
                    YCbCr[y, x, 0] = complexImage.YCbCr[y, x, 0];
                    YCbCr[y, x, 1] = complexImage.YCbCr[y, x, 1];
                    YCbCr[y, x, 2] = complexImage.YCbCr[y, x, 2];
                }
            }
        }

        public object Clone()
        {
            ComplexImage dstImage = new ComplexImage(Width, Height, OriginalWidth, OriginalHeight);
            Complex[,] data = dstImage.Data;
            double[,] dctData = dstImage.DCTData;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    data[i, j] = this.Data[i, j];
                    dctData[i, j] = this.DCTData[i, j];
                }
            }

            dstImage.FourierTransformed = FourierTransformed;

            return dstImage;
        }

        public static ComplexImage FromBitmap(EffectiveBitmap image, int newSize = 0)
        {
            if (newSize == 0)
                newSize = Math.Max(image.Width, image.Height);
            newSize = (int)Math.Pow(2, Math.Ceiling(Math.Log2(newSize)));
            var resizedImage = image.Resize(newSize, newSize);
            ComplexImage complexImage = new ComplexImage(newSize, newSize, image.Width, image.Height)
            {
                YCbCr = resizedImage.Rgb2Ycbcr()
            };

            Complex[,] data = complexImage.Data;
            double[,] dctData = complexImage.DCTData;
            resizedImage.RunOnEveryPixel((i, j) =>
            {
                data[j, i] = new Complex(complexImage.YCbCr[i, j, 0] / 255.0, data[j, i].Imaginary);
                dctData[j, i] = complexImage.YCbCr[i, j, 0];
            });

            return complexImage;
        }

        public EffectiveBitmap ToEffectiveBitmap()
        {
            double scale = FourierTransformed && !CosineTransformed ? Math.Sqrt(Width * Height) : 1;
            // todo depth
            return EffectiveBitmap.Create(Width, Height, 4, (i, j) =>
            {
                var value = (byte)Math.Max(0, Math.Min(255, Data[j, i].Magnitude * scale * 255));
                if (FourierTransformed || CosineTransformed)
                {
                    if (CosineTransformed)
                    {
                        value = (byte)DCTData[j, i];
                    }
                    return new PixelInfo(value, value, value);
                }
                else
                {
                    YCbCr[i, j, 0] = value;
                    return PixelInfo.FromYCbCr(YCbCr[i, j, 0], YCbCr[i, j, 1], YCbCr[i, j, 2]);
                }
            });
        }

        public void ForwardFourierTransform()
        {
            if (!FourierTransformed)
            {
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (((x + y) & 0x1) != 0)
                        {
                            Data[y, x] *= -1.0;
                        }
                    }
                }

                FourierTransform.FFT2(Data, FourierTransform.Direction.Forward);
                FourierTransformed = true;
            }
        }

        public void BackwardFourierTransform()
        {
            if (FourierTransformed)
            {
                FourierTransform.FFT2(Data, FourierTransform.Direction.Backward);
                FourierTransformed = false;

                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (((x + y) & 0x1) != 0)
                        {
                            Data[y, x] *= -1.0;
                        }
                    }
                }
            }
        }

        public void DCT()
        {
            CosineTransform.DCT(DCTData);
            CosineTransformed = true;
        }

        public void IDCT()
        {
            CosineTransform.IDCT(DCTData);
            CosineTransformed = false;
        }
    }
}