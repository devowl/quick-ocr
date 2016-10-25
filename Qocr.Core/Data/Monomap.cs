using System;
using System.Drawing;

using Qocr.Core.Interfaces;

namespace Qocr.Core.Data
{
    /// <summary>
    /// Реализация <see cref="IMonomap"/> для работы с <see cref="Bitmap"/>.
    /// </summary>
    public class Monomap : IMonomap
    {
        private readonly Color _blackColor;

        private readonly Bitmap _image;

        /// <summary>
        /// Создание экземпляра класса <see cref="Monomap"/>.
        /// </summary>
        public Monomap(Bitmap image, Color blackColor)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            _image = image;
            _blackColor = blackColor;
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="Monomap"/>.
        /// </summary>
        public Monomap(Bitmap image) : this(image, Color.FromArgb(0, 0, 0))
        {
            _image = image;
        }

        /// <inheritdoc/>
        public int Width => _image.Width;

        /// <inheritdoc/>
        public int Height => _image.Height;

        /// <inheritdoc/>
        public bool this[int x, int y]
        {
            get
            {
                if (!IsPointInsideImage(x, y))
                {
                    throw new ArgumentOutOfRangeException(nameof(x) + nameof(y));
                }

                var pixel = _image.GetPixel(x, y);
                return pixel == _blackColor;
            }
        }

        private static bool CheckBounds(int bound, int length)
        {
            return 0 <= bound && bound < length;
        }

        private bool IsPointInsideImage(int x, int y)
        {
            return CheckBounds(x, _image.Width) && CheckBounds(y, _image.Height);
        }
    }
}