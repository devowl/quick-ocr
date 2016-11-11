using System;
using System.Drawing;

namespace Qocr.Core.Data
{
    /// <summary>
    /// Реализация IMonomap для работы с <see cref="Bitmap"/>.
    /// </summary>
    public class Monomap : MonomapBase
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
        public override int Width => _image.Width;

        /// <inheritdoc/>
        public override int Height => _image.Height;

        /// <inheritdoc/>
        public override bool this[int x, int y] => _image.GetPixel(x, y) == _blackColor;
    }
}