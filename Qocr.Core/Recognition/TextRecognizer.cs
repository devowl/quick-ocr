using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qocr.Core.Approximation;
using Qocr.Core.Interfaces;

namespace Qocr.Core.Recognition
{
    /// <summary>
    /// Распознаватель текста.
    /// </summary>
    public class TextRecognizer : IDisposable
    {
        public TextRecognizer()
            : this(new OneBitApproximator())
        {
            
        }

        public TextRecognizer(IApproximator approximator)
        {
            
        }

        public StringBuilder Recognize(IMonomap monomap)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            
        }
    }
}
