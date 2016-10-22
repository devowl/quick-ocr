using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qocr.Core.Interfaces;

namespace Qocr.Core.Data
{
    public class BitMonomap : IMonomap
    {
        private readonly bool[,] _imageCode;

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

        public bool this[int x, int y] => _imageCode[x, y];

        public int Width => _imageCode.GetLength(0);

        public int Height => _imageCode.GetLength(1);
    }
}
