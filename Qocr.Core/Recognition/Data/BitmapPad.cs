using System;
using System.Drawing;

using Qocr.Core.Interfaces;

namespace Qocr.Core.Recognition.Data
{
    /// <summary>
    /// Черновик изображения.
    /// </summary>
    internal class BitmapPad : IMonomap
    {
        private readonly int _padWidth;

        private readonly int _padHeight;

        private readonly Point _padCenterPoint;

        private bool[,] _padScape;

        private Point _sourceImageStartPoint = Point.Empty;

        private int _maxY;

        private int _maxX;

        private int _minX;

        private int _minY;

        /// <summary>
        /// Создание экземпляра класса <see cref="BitmapPad"/>.
        /// </summary>
        public BitmapPad(int padWidth, int padHeight)
        {
            _padWidth = padWidth;
            _padHeight = padHeight;

            _padCenterPoint = new Point(padWidth / 2, padHeight / 2);
            ClearPad();
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="BitmapPad"/>.
        /// </summary>
        public BitmapPad() : this(101, 101)
        {
        }

        /// <inheritdoc/>
        public int Width => IsPadClear ? 0 : Math.Abs(_maxX) + Math.Abs(_minX) + 1;

        /// <inheritdoc/>
        public int Height => IsPadClear ? 0 : Math.Abs(_maxY) + Math.Abs(_minY) + 1;

        /// <summary>
        /// Левая верхняя точка изображения.
        /// </summary>
        public Point TopLeftPoint
        {
            get
            {
                if (IsPadClear)
                {
                    return Point.Empty;
                }

                var offsetX = _sourceImageStartPoint.X + _minX;
                var offsetY = _sourceImageStartPoint.X + _minY;
                return new Point(offsetX, offsetY);
            }
        }

        /// <summary>
        /// Правая нижняя точка изображения.
        /// </summary>
        public Point RightBottomPoint
        {
            get
            {
                if (IsPadClear)
                {
                    return Point.Empty;
                }

                var offsetX = _sourceImageStartPoint.X + _maxX;
                var offsetY = _sourceImageStartPoint.X + _maxY;
                return new Point(offsetX, offsetY);
            }
        }

        private bool IsPadClear => _sourceImageStartPoint == Point.Empty;

        /// <inheritdoc/>
        public bool this[int x, int y] => GetValue(x, y);

        /// <summary>
        /// Нарисовать точку в черновике.
        /// </summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        public void SetPoint(int x, int y)
        {
            if (IsPadClear)
            {
                _sourceImageStartPoint = new Point(x, y);
            }

            var offsetX = x - _sourceImageStartPoint.X;
            var offsetY = y - _sourceImageStartPoint.Y;

            if (offsetX > _maxX)
            {
                _maxX = offsetX;
            }
            else if (offsetX < _minX)
            {
                _minX = offsetX;
            }

            if (offsetY > _maxY)
            {
                _maxY = offsetY;
            }
            else if (offsetY < _minY)
            {
                _minY = offsetY;
            }

            var padX = _padCenterPoint.X + offsetX;
            var padY = _padCenterPoint.Y + offsetY;
            _padScape[padX, padY] = true;
        }

        /// <summary>
        /// Очистить черновик.
        /// </summary>
        public void ClearPad()
        {
            _padScape = new bool[_padWidth, _padHeight];
            _sourceImageStartPoint = Point.Empty;
            _maxY = _maxX = _minX = _minY = 0;
        }

        private bool GetValue(int x, int y)
        {
            var padX = x + _padCenterPoint.X - Math.Abs(_minX);
            var padY = y + _padCenterPoint.Y - Math.Abs(_minY);
            return _padScape[padX, padY];
        }
    }
}