using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qocr.Core.Approximation;
using Qocr.Core.Interfaces;

namespace Qocr.Core.Recognition
{
    public class TextRecognizer
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
            
        }
    }
}
