using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using Qocr.Core.Interfaces;

namespace Qocr.Core.Data
{
    public class Monomap : IMonomap
    {
        private readonly Color _blackColor;
        private readonly Bitmap _image;

        public Monomap(Bitmap image, Color blackColor)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            _image = image;
            _blackColor = blackColor;
        }

        public Monomap(Bitmap image)
            : this(image, Color.FromArgb(0, 0, 0))
        {
            _image = image;
        }
        
        public bool this[int x, int y]
        {
            get
            {
                if (!IsPointInsideImage(x, y))
                {
                    throw new ArgumentOutOfRangeException(nameof(x)+nameof(y));
                }

                var pixel = _image.GetPixel(x, y);
                return pixel == _blackColor;
            }
        }

        public int Width
        {
            get { return _image.Width; }
        }

        public int Height
        {
            get { return _image.Height; }
        }

        private bool IsPointInsideImage(int x, int y)
        {
            return CheckBounds(x, _image.Width) && CheckBounds(y, _image.Height);
        }

        private static bool CheckBounds(int bound, int length)
        {
            return 0 <= bound && bound < length;
        }
    }
}