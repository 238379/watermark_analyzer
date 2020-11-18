using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Algorithms.common;

namespace Generators
{
	public class TextImageGenerator : Generator
	{
        public const string GENERATOR_NAME = "TEXT_IMAGE_GENERATOR";

        public const string TEXT_PARAM = "TEXT_PARAM";
        public const string WIDTH_PARAM = "WIDTH_PARAM";
        public const string HEIGHT_PARAM = "HEIGHT_PARAM";
        public const string FONT_PARAM = "FONT_PARAM";
        public const string TEXT_COLOR_PARAM = "TEXT_COLOR_PARAM";
        public const string BACKGROUND_COLOR_PARAM = "BACKGROUND_COLOR_PARAM";

        public TextImageGenerator(Dictionary<string, dynamic> parameters) : base(parameters)
		{
		}

		public override Bitmap Generate()
		{
            generatorParameters.TryGetValue(FONT_PARAM, out var font);
            generatorParameters.TryGetValue(TEXT_COLOR_PARAM, out var textColor);
            generatorParameters.TryGetValue(BACKGROUND_COLOR_PARAM, out var backgroundColor);

            return DrawText(generatorParameters[TEXT_PARAM], new Size(generatorParameters[WIDTH_PARAM], generatorParameters[HEIGHT_PARAM]), font, textColor, backgroundColor);
		}

        /// <summary>
        /// Creates an image containing the given text.
        /// NOTE: the image should be disposed after use.
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="fontOptional">Font to use, defaults to Control.DefaultFont</param>
        /// <param name="textColorOptional">Text color, defaults to Black</param>
        /// <param name="backColorOptional">Background color, defaults to white</param>
        /// <param name="minSizeOptional">Minimum image size, defaults the size required to display the text</param>
        /// <returns>The image containing the text, which should be disposed after use</returns>
        public Bitmap DrawText(string text, Size targetSize, Font fontOptional = null, Color? textColorOptional = null, Color? backColorOptional = null)
        {
            Font font = new Font("Arial", GetMaximumFontSizeFitInRectangle(text, new Font("Arial", 1), targetSize, false, 3));
            if (fontOptional != null)
                font = fontOptional;

            Color textColor = Color.Black;
            if (textColorOptional != null)
                textColor = (Color)textColorOptional;

            Color backColor = Color.White;
            if (backColorOptional != null)
                backColor = (Color)backColorOptional;

            //create a new image of the right size
            Bitmap retImg = new Bitmap(targetSize.Width, targetSize.Height);
            using (var drawing = Graphics.FromImage(retImg))
            {
                //paint the background
                drawing.Clear(backColor);

                //create a brush for the text
                using (Brush textBrush = new SolidBrush(textColor))
                {
                    drawing.DrawString(text, font, textBrush, 0, 0);
                    drawing.Save();
                }
            }
            return retImg;
        }

        private int GetMaximumFontSizeFitInRectangle(string text, Font font, Size rectanglef, bool isWarp, int MinumumFontSize = 6, int MaximumFontSize = 1000)
        {
            Font newFont;
            for (int newFontSize = MinumumFontSize; ; newFontSize++)
            {
                newFont = new Font(font.FontFamily, newFontSize, font.Style);

                List<string> ls = WarpText(text, newFont, rectanglef.Width);

                StringBuilder sb = new StringBuilder();
                if (isWarp)
                {
                    for (int i = 0; i < ls.Count; ++i)
                    {
                        sb.Append(ls[i] + Environment.NewLine);
                    }
                }
                else
                {
                    sb.Append(text);
                }

                Size size = MeasureDrawTextBitmapSize(sb.ToString(), newFont);
                if (size.Width > rectanglef.Width || size.Height > rectanglef.Height)
                {
                    return (newFontSize - 1);
                }
                if (newFontSize >= MaximumFontSize)
                {
                    return (newFontSize - 1);
                }
            }
        }

        private Size MeasureDrawTextBitmapSize(string text, Font font)
        {
            Bitmap bmp = new Bitmap(1, 1);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                SizeF size = g.MeasureString(text, font);
                return new Size((int)(Math.Ceiling(size.Width)), (int)(Math.Ceiling(size.Height)));
            }

        }

        private List<string> WarpText(string text, Font font, int lineWidthInPixels)
        {
            string[] originalLines = text.Split(new string[] { " " }, StringSplitOptions.None);

            List<string> wrappedLines = new List<string>();

            StringBuilder actualLine = new StringBuilder();
            double actualWidthInPixels = 0;

            foreach (string str in originalLines)
            {
                Size size = MeasureDrawTextBitmapSize(str, font);

                actualLine.Append(str + " ");
                actualWidthInPixels += size.Width;

                if (actualWidthInPixels > lineWidthInPixels)
                {
                    actualLine = actualLine.Remove(actualLine.ToString().Length - str.Length - 1, str.Length);
                    wrappedLines.Add(actualLine.ToString());
                    actualLine.Clear();
                    actualLine.Append(str + " ");
                    actualWidthInPixels = size.Width;
                }
            }

            if (actualLine.Length > 0)
            {
                wrappedLines.Add(actualLine.ToString());
            }

            return wrappedLines;
        }
    }
}
