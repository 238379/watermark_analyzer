// AForge Image Processing Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright � Andrew Kirillov, 2005-2009
// andrew.kirillov@aforgenet.com
//
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Algorithms
{
    public class ComplexImage : ICloneable
    {
        // image complex data
        private Complex[,] data;
        // image dimension
        private int width;
        private int height;
        // current state of the image (transformed with Fourier ot not)
        private bool fourierTransformed = false;
        /// <summary>
        /// Image width.
        /// </summary>
        /// 
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Image height.
        /// </summary>
        /// 
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Status of the image - Fourier transformed or not.
        /// </summary>
        /// 
        public bool FourierTransformed
        {
            get { return fourierTransformed; }
        }

        /// <summary>
        /// Complex image's data.
        /// </summary>
        /// 
        /// <remarks>Return's 2D array of [<b>height</b>, <b>width</b>] size, which keeps image's
        /// complex data.</remarks>
        /// 
        public Complex[,] Data
        {
            get { return data; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexImage"/> class.
        /// </summary>
        /// 
        /// <param name="width">Image width.</param>
        /// <param name="height">Image height.</param>
        /// 
        /// <remarks>The constractor is protected, what makes it imposible to instantiate this
        /// class directly. To create an instance of this class <see cref="FromBitmap(Bitmap)"/> or
        /// <see cref="FromBitmap(BitmapData)"/> method should be used.</remarks>
        ///
        protected ComplexImage(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.data = new Complex[height, width];
            this.fourierTransformed = false;
        }

        /// <summary>
        /// Clone the complex image.
        /// </summary>
        /// 
        /// <returns>Returns copy of the complex image.</returns>
        /// 
        public object Clone()
        {
            // create new complex image
            ComplexImage dstImage = new ComplexImage(width, height);
            Complex[,] data = dstImage.data;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    data[i, j] = this.data[i, j];
                }
            }

            // clone mode as well
            dstImage.fourierTransformed = fourierTransformed;

            return dstImage;
        }

        /// <summary>
        /// Create complex image from grayscale bitmap.
        /// </summary>
        /// 
        /// <param name="image">Source grayscale bitmap (8 bpp indexed).</param>
        /// 
        /// <returns>Returns an instance of complex image.</returns>
        /// 
        /// <exception cref="UnsupportedImageFormatException">The source image has incorrect pixel format.</exception>
        /// <exception cref="InvalidImagePropertiesException">Image width and height should be power of 2.</exception>
        /// 
        public static ComplexImage FromBitmap(Bitmap image)
        {
            ComplexImage complexImage = new ComplexImage(image.Width, image.Height);
            Complex[,] data = complexImage.data;

            image.RunOnEveryPixel((i, j) =>
            {
                data[i, j] = new Complex(image.GetPixel(i, j).R / 255.0, data[i, j].Imaginary);
            });

            return complexImage;
        }

        /// <summary>
        /// Convert complex image to bitmap.
        /// </summary>
        /// 
        /// <returns>Returns grayscale bitmap.</returns>
        /// 
        public Bitmap ToBitmap()
        {
            // create new image
            Bitmap dstImage = new Bitmap(width, height);


            double scale = (fourierTransformed) ? Math.Sqrt(width * height) : 1;
            dstImage.RunOnEveryPixel((i, j) =>
            {
                var value = (byte)Math.Max(0, System.Math.Min(255, data[i, j].Magnitude * scale * 255));
                dstImage.SetPixel(i, j, Color.FromArgb(value, value, value));
            });

            return dstImage;
        }

        /// <summary>
        /// Applies forward fast Fourier transformation to the complex image.
        /// </summary>
        /// 
        public void ForwardFourierTransform()
        {
            if (!fourierTransformed)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (((x + y) & 0x1) != 0)
                        {
                            data[y, x] *= -1.0;
                        }
                    }
                }

                FourierTransform.FFT2(data, FourierTransform.Direction.Forward);
                fourierTransformed = true;
            }
        }

        /// <summary>
        /// Applies backward fast Fourier transformation to the complex image.
        /// </summary>
        /// 
        public void BackwardFourierTransform()
        {
            if (fourierTransformed)
            {
                FourierTransform.FFT2(data, FourierTransform.Direction.Backward);
                fourierTransformed = false;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (((x + y) & 0x1) != 0)
                        {
                            data[y, x] *= -1.0;
                        }
                    }
                }
            }
        }
    }
}