using System;

using Qocr.Core.Interfaces;
using Qocr.Core.Utils;

namespace Qocr.Core.Data
{
    /// <summary>
    /// Реализация <see cref="IElulerSquare"/>.
    /// </summary>
    internal class Square2D : IElulerSquare
    {
        private const string PropertyPrefix = "X";

        private const int SquareSideSize = 2;

        /// <summary>
        /// Создание экземпляра класса <see cref="Square2D"/>.
        /// </summary>
        public Square2D(params bool[] dots)
        {
            if (dots.Length != SquareSideSize * SquareSideSize)
            {
                throw new ArgumentException(nameof(dots));
            }

            ClassUtils.FillClassProperiesValues(this, PropertyPrefix, dots);
        }

        /// <summary>
        /// <para>[+] [ ]</para> 
        /// <para>[ ] [ ]</para> 
        /// </summary>
        public bool X1 { get; private set; }

        /// <summary>
        /// <para>[ ] [+]</para> 
        /// <para>[ ] [ ]</para> 
        /// </summary>
        public bool X2 { get; private set; }

        /// <summary>
        /// <para>[ ] [ ]</para> 
        /// <para>[ ] [+]</para> 
        /// </summary>
        public bool X3 { get; private set; }

        /// <summary>
        /// <para>[ ] [ ]</para> 
        /// <para>[+] [ ]</para> 
        /// </summary>
        public bool X4 { get; private set; }

        /// <inheritdoc/>
        public int SquareSize => SquareSideSize;

        /// <inheritdoc/>
        public bool IsSquareDetected(int topX, int topY, IMonomap monomap)
        {
            return 
                monomap[topX    , topY    ] == X1 && 
                monomap[topX + 1, topY    ] == X2 && 
                monomap[topX + 1, topY + 1] == X3 &&
                monomap[topX    , topY + 1] == X4;
        }
    }
}