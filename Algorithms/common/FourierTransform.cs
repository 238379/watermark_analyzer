using Algorithms.common;
using System;
using System.Numerics;

namespace Algorithms
{
    public static class FourierTransform
    {

        private const int minLength = 2;
        private const int maxLength = 16384;
        private const int minBits = 1;
        private const int maxBits = 14;
        private static readonly int[][] reversedBits = new int[maxBits][];
        private static readonly Complex[,][] complexRotation = new Complex[maxBits, 2][];

        public enum Direction
        {
            Forward = 1,
            Backward = -1
        };

        public static void FFT(Complex[] data, Direction direction)
        {
            int n = data.Length;
            int m = (int)Math.Log2(n);

            ReorderData(data);

            int tn = 1, tm;

            for (int k = 1; k <= m; k++)
            {
                Complex[] rotation = FourierTransform.GetComplexRotation(k, direction);

                tm = tn;
                tn <<= 1;

                for (int i = 0; i < tm; i++)
                {
                    var t = rotation[i];

                    for (int even = i; even < n; even += tn)
                    {
                        int odd = even + tm;
                        var ce = data[even];
                        var cot = data[odd] * t;

                        data[even] += cot;
                        data[odd] = ce - cot;
                    }
                }
            }

            if (direction == Direction.Forward)
            {
                for (int i = 0; i < n; i++)
                {
                    data[i] /= (double)n;
                }
            }
        }

        public static void FFT2(Complex[,] data, Direction direction)
        {
            int k = data.GetLength(0);
            int n = data.GetLength(1);

            if (
                (!IsPowerOf2(k)) ||
                (!IsPowerOf2(n)) ||
                (k < minLength) || (k > maxLength) ||
                (n < minLength) || (n > maxLength)
                )
            {
                throw new ArgumentException("Incorrect data length.");
            }

            Complex[] row = new Complex[n];

            for (int i = 0; i < k; i++)
            {
                for (int j = 0; j < n; j++)
                    row[j] = data[i, j];
                FourierTransform.FFT(row, direction);
                for (int j = 0; j < n; j++)
                    data[i, j] = row[j];
            }

            Complex[] col = new Complex[k];

            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < k; i++)
                    col[i] = data[i, j];
                FourierTransform.FFT(col, direction);
                for (int i = 0; i < k; i++)
                    data[i, j] = col[i];
            }
        }

        private static int[] GetReversedBits(int numberOfBits)
        {
            if ((numberOfBits < minBits) || (numberOfBits > maxBits))
                throw new ArgumentOutOfRangeException();

            if (reversedBits[numberOfBits - 1] == null)
            {
                int n = (int)Math.Pow(2, numberOfBits);
                int[] rBits = new int[n];

                for (int i = 0; i < n; i++)
                {
                    int oldBits = i;
                    int newBits = 0;

                    for (int j = 0; j < numberOfBits; j++)
                    {
                        newBits = (newBits << 1) | (oldBits & 1);
                        oldBits >>= 1;
                    }
                    rBits[i] = newBits;
                }
                reversedBits[numberOfBits - 1] = rBits;
            }
            return reversedBits[numberOfBits - 1];
        }

        private static Complex[] GetComplexRotation(int numberOfBits, Direction direction)
        {
            int directionIndex = (direction == Direction.Forward) ? 0 : 1;

            if (complexRotation[numberOfBits - 1, directionIndex] == null)
            {
                int n = 1 << (numberOfBits - 1);
                double uR = 1.0;
                double uI = 0.0;
                double angle = System.Math.PI / n * (int)direction;
                double wR = System.Math.Cos(angle);
                double wI = System.Math.Sin(angle);
                double t;
                Complex[] rotation = new Complex[n];

                for (int i = 0; i < n; i++)
                {
                    rotation[i] = new Complex(uR, uI);
                    t = uR * wI + uI * wR;
                    uR = uR * wR - uI * wI;
                    uI = t;
                }

                complexRotation[numberOfBits - 1, directionIndex] = rotation;
            }
            return complexRotation[numberOfBits - 1, directionIndex];
        }

        private static bool IsPowerOf2(int x)
        {
            double sqrt = Math.Sqrt(x);
            return sqrt == (int)sqrt;
        }

        private static void ReorderData(Complex[] data)
        {
            int len = data.Length;

            if ((len < minLength) || (len > maxLength) || (!IsPowerOf2(len)))
                throw new ArgumentException("Incorrect data length.");

            int[] rBits = GetReversedBits((int)Math.Log2(len));

            for (int i = 0; i < len; i++)
            {
                int s = rBits[i];

                if (s > i)
                {
                    Complex t = data[i];
                    data[i] = data[s];
                    data[s] = t;
                }
            }
        }
    }
}