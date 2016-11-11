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
        /// Создание экземпляра класса <see cref="SpecialChars"/>.
        /// </summary>
        public SpecialChars()
        {
            LowcaseCharactors = new List<Symbol>();
            UppercaseCharactors = new List<Symbol>();
        }

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