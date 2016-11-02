using System.Collections.Generic;

namespace Qocr.Core.Data.Serialization
{
    /// <summary>
    /// Язык.
    /// </summary>
    public class Language
    {
        public Language()
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

        /// <summary>
        /// Название локализации.
        /// </summary>
        /// <remarks>RU-ru, EN-en, и т.д.</remarks>
        public string LocalizationName { get; set; }
    }
}