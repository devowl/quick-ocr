using System.Collections.Generic;
using System.Xml.Serialization;

namespace Qocr.Core.Data.Serialization
{
    /// <summary>
    /// Спецсимволы.
    /// </summary>
    public class SpecialChars
    {
        /// <summary>
        /// Список символов нижнего регистра.
        /// </summary>
        public List<Symbol> LowcaseCharactors { get; set; }
        
        /// <summary>
        /// Список символов верхнего регистра.
        /// </summary>
        public List<Symbol> UppercaseCharactors { get; set; }
    }
}