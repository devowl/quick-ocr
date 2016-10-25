using System;

using Qocr.Core.Interfaces;

namespace Qocr.Core.Data
{
    /// <summary>
    /// Реализация <see cref="IMonomap"/> для работы с бинарным массивом.
    /// </summary>
    public class BitMonomap : IMonomap
    {
        private readonly bool[,] _imageCode;

        /// <summary>
        /// Создание экземпляра класса <see cref="BitMonomap"/>.
        /// </summary>
        public BitMonomap(bool[,] imageCode)
        {
            if (imageCode == null)
            {
                throw new ArgumentNullException(nameof(imageCode));
            }

            if (imageCode.Length == 0)
            {
                throw new ArgumentException("Array empty", nameof(imageCode));
            }

            _imageCode = imageCode;
        }

        /// <inheritdoc/>
        public int Width => _imageCode.GetLength(0);

        /// <inheritdoc/>
        public int Height => _imageCode.GetLength(1);

        /// <inheritdoc/>
        public bool this[int x, int y] => _imageCode[x, y];
    }
}