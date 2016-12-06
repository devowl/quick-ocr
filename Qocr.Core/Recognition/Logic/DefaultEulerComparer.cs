using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Qocr.Core.Data.Map2D;

namespace Qocr.Core.Recognition.Logic
{
    /// <summary>
    /// Default equality comparer для <see cref="EulerMonomap2D"/>.
    /// </summary>
    public class DefaultEulerComparer : IComparer<EulerMonomap2D>
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="DefaultEulerComparer"/>.
        /// </summary>
        public DefaultEulerComparer()
        {
        }

        /// <inheritdoc/>
        int IComparer<EulerMonomap2D>.Compare(EulerMonomap2D sourceMap, EulerMonomap2D right)
        {
            // TODO 1. Надо по иному считать, учитывая размер.
            // TODO 2. В Gen.bin НЕ ВСЕ буквы
            /*
            var diffResult =
                DiffCompute(sourceMap.S0, right.S0) +
                DiffCompute(sourceMap.S1, right.S1) +
                DiffCompute(sourceMap.S2, right.S2) +
                DiffCompute(sourceMap.S3, right.S3) +
                DiffCompute(sourceMap.S4, right.S4) +
                DiffCompute(sourceMap.S5, right.S5) +
                DiffCompute(sourceMap.S6, right.S6) +
                DiffCompute(sourceMap.S7, right.S7) +
                DiffCompute(sourceMap.S8, right.S8) +
                DiffCompute(sourceMap.S9, right.S9) +
                DiffCompute(sourceMap.S10, right.S10) +
                DiffCompute(sourceMap.S11, right.S11) +
                DiffCompute(sourceMap.S12, right.S12) +
                DiffCompute(sourceMap.S13, right.S13) +
                DiffCompute(sourceMap.S14, right.S14);
            */
            var diffResult = new [] 
            {
                DiffCompute(sourceMap.S0, right.S0),
                DiffCompute(sourceMap.S1, right.S1),
                DiffCompute(sourceMap.S2, right.S2),
                DiffCompute(sourceMap.S3, right.S3),
                DiffCompute(sourceMap.S4, right.S4),
                DiffCompute(sourceMap.S5, right.S5),
                DiffCompute(sourceMap.S6, right.S6),
                DiffCompute(sourceMap.S7, right.S7),
                DiffCompute(sourceMap.S8, right.S8),
                DiffCompute(sourceMap.S9, right.S9),
                DiffCompute(sourceMap.S10, right.S10),
                DiffCompute(sourceMap.S11, right.S11),
                DiffCompute(sourceMap.S12, right.S12),
                DiffCompute(sourceMap.S13, right.S13),
                DiffCompute(sourceMap.S14, right.S14),
            };

            return diffResult.Length - diffResult.Where(diff => diff == 0).Count();
        }

        private int DiffCompute(int s1, int s2)
        {
            return Math.Abs(s1 - s2);
        }
    }
}
