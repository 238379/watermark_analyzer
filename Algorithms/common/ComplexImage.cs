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
        public Complex[,] Data { get; set; }
        public byte[,,] YCbCr
        {
            get => _YCbCr;
            set
            {
                if (value.GetLength(0) != Height || value.GetLength(1) != Width)
                {
                    throw new ArgumentException($"Can't set YCbCr {value.GetLength(1)}x{value.GetLength(0)}. Should be {Width}x{Height}.");
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
            FourierTransformed = false;
        }

        public ComplexImage(ComplexImage complexImage)
        {
            Width = complexImage.Width;
            Height = complexImage.Height;
            OriginalWidth = complexImage.OriginalWidth;
            OriginalHeight = complexImage.OriginalHeight;
            Data = new Complex[Height, Width];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Data[y, x] = new Complex(complexImage.Data[y, x].Real, complexImage.Data[y, x].Imaginary);
                }
            }
            YCbCr = new byte[Height, Width, 3];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    YCbCr[y, x, 0] = complexImage.YCbCr[y, x, 0];
                    YCbCr[y, x, 0] = complexImage.YCbCr[y, x, 1];
                    YCbCr[y, x, 0] = complexImage.YCbCr[y, x, 2];
                }
            }
        }

        public object Clone()
        {
            ComplexImage dstImage = new ComplexImage(Width, Height, OriginalWidth, OriginalHeight);
            Complex[,] data = dstImage.Data;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    data[i, j] = this.Data[i, j];
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
            ComplexImage complexImage = new ComplexImage(newSize, newSize, image.Width, image.Height)
            {
                YCbCr = image.Resize(newSize, newSize).Rgb2Ycbcr()
            };

            Complex[,] data = complexImage.Data;

            image.RunOnEveryPixel((i, j) =>
            {
                data[i, j] = new Complex(complexImage.YCbCr[i, j, 0] / 255.0, data[i, j].Imaginary);
            });

            return complexImage;
        }

        public EffectiveBitmap ToEffectiveBitmap()
        {
            double scale = FourierTransformed ? Math.Sqrt(Width * Height) : 1;

            // todo depth
            return EffectiveBitmap.Create(Width, Height, 4, (i, j) => {
                var value = (byte)Math.Max(0, Math.Min(255, Data[i, j].Magnitude * scale * 255));
                if (FourierTransformed)
                {
                    return new PixelInfo(value, value, value);
                }
                else
                {
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
    }
}