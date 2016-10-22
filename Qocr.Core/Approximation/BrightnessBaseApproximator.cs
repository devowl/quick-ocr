using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Qocr.Core.Data;
using Qocr.Core.Interfaces;

namespace Qocr.Core.Approximation
{
    public class BrightnessBaseApproximator : IApproximator
    {
        public IMonomap Approximate(Bitmap image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            bool[,] imageCode = new bool[image.Width, image.Height];

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color c = image.GetPixel(i, j);
                    if (c.GetBrightness() > 127)
                    {
                        imageCode[i, j] = true;
                    }
                    else
                    {
                        imageCode[i, j] = false;
                    }
                }
            }

            return new BitMonomap(imageCode);
        }
    }
}
