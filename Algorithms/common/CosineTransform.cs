using System;
using System.Collections.Generic;
using System.Text;

namespace Algorithms.common
{
    public static class CosineTransform
    {
        private const double SQRT_2 = 1.414213562373095048801688;

        public static void DCT(double[] data)
        {
            double[] result = new double[data.Length];
            double c = Math.PI / (2.0 * data.Length);
            double scale = Math.Sqrt(2.0 / data.Length);

            for (int k = 0; k < data.Length; k++)
            {
                double sum = 0;
                for (int n = 0; n < data.Length; n++)
                {
                    sum += data[n] * Math.Cos((2.0 * n + 1.0) * k * c);
                }
                result[k] = scale * sum;
            }

            data[0] = result[0] / SQRT_2;
            for (int i = 1; i < data.Length; i++)
            {
                data[i] = result[i];
            }
        }

        public static void IDCT(double[] data)
        {
            double[] result = new double[data.Length];
            double c = Math.PI / (2.0 * data.Length);
            double scale = Math.Sqrt(2.0 / data.Length);

            for (int k = 0; k < data.Length; k++)
            {
                double sum = data[0] / SQRT_2;
                for (int n = 1; n < data.Length; n++)
                {
                    sum += data[n] * Math.Cos((2 * k + 1) * n * c);
                }

                result[k] = scale * sum;
            }

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = result[i];
            }
        }

        public static void DCT(double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            double[] row = new double[cols];
            double[] col = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < row.Length; j++)
                {
                    row[j] = data[i, j];
                }

                DCT(row);

                for (int j = 0; j < row.Length; j++)
                {
                    data[i, j] = row[j];
                }
            }

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < col.Length; i++)
                {
                    col[i] = data[i, j];
                }

                DCT(col);

                for (int i = 0; i < col.Length; i++)
                {
                    data[i, j] = col[i];
                }
            }
        }

        public static void IDCT(double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            double[] row = new double[cols];
            double[] col = new double[rows];

            for (int j = 0; j < cols; j++)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    col[i] = data[i, j];
                }

                IDCT(col);

                for (int i = 0; i < col.Length; i++)
                {
                    data[i, j] = col[i];
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < row.Length; j++)
                {
                    row[j] = data[i, j];
                }

                IDCT(row);

                for (int j = 0; j < row.Length; j++)
                {
                    data[i, j] = row[j];
                }
            }
        }
    }
}