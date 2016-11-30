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
        /// <inheritdoc/>
        int IComparer<EulerMonomap2D>.Compare(EulerMonomap2D sourceMap, EulerMonomap2D right)
        {
            // TODO 1. Надо по иному считать, учитывая размер.
            // TODO 2. В Gen.bin НЕ ВСЕ буквы
            var diffResult =
                Math.Abs(sourceMap.S0 - right.S0) +
                Math.Abs(sourceMap.S1 - right.S1) +
                Math.Abs(sourceMap.S2 - right.S2) +
                Math.Abs(sourceMap.S3 - right.S3) +
                Math.Abs(sourceMap.S4 - right.S4) +
                Math.Abs(sourceMap.S5 - right.S5) +
                Math.Abs(sourceMap.S6 - right.S6) +
                Math.Abs(sourceMap.S7 - right.S7) +
                Math.Abs(sourceMap.S8 - right.S8) +
                Math.Abs(sourceMap.S9 - right.S9) +
                Math.Abs(sourceMap.S10 - right.S10) +
                Math.Abs(sourceMap.S11 - right.S11) +
                Math.Abs(sourceMap.S12 - right.S12) +
                Math.Abs(sourceMap.S13 - right.S13) +
                Math.Abs(sourceMap.S14 - right.S14);

            return diffResult;
        }
    }
}
