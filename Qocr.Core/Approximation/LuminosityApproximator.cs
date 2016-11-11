using System;
using System.Drawing;

using Qocr.Core.Data;
using Qocr.Core.Interfaces;

namespace Qocr.Core.Approximation
{
    /// <summary>
    /// Аппроксимация на основе значения яркости цвета.
    /// </summary>
    public class LuminosityApproximator : IApproximator
    {
        /// <summary>
        /// Максимальное значение яркости.
        /// </summary>
        private const int Scale = 240;

        /// <summary>
        /// Создание экземпляра класса <see cref="LuminosityApproximator"/>.
        /// </summary>
        public LuminosityApproximator()
            : this(130)
        {
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="LuminosityApproximator"/>.
        /// </summary>
        public LuminosityApproximator(int brightness)
        {
            if (brightness < 0 || brightness > 240)
            {
                throw new ArgumentOutOfRangeException($"{nameof(brightness)} MinValue: 0 MaxValue: 240");
            }

            Brightness = brightness;
        }

        /// <summary>
        /// Значение яркости, которое является черным значением.
        /// </summary>
        public int Brightness { get; }

        /// <inheritdoc/>
        public IMonomap Approximate(Bitmap bitmap)
        {
            bool[,] booleanBitmap = new bool[bitmap.Width, bitmap.Height];
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);

                    var brightness = pixel.GetBrightness();
                    brightness = brightness * Scale;

                    booleanBitmap[x, y] = brightness < Brightness;
                }
            }

            return new BitMonomap(booleanBitmap);
        }
    }
}