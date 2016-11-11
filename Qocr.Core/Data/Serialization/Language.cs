using System.Collections.Generic;
using System.Drawing;

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
            FontFamilyNames = new List<string>();
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
        /// Список использованных шрифтов.
        /// </summary>
        public List<string> FontFamilyNames { get; set; }

        /// <summary>
        /// Название локализации.
        /// </summary>
        /// <remarks>RU-ru, EN-en, и т.д.</remarks>
        public string LocalizationName { get; set; }

        /// <summary>
        /// Минимальный размер шрифта.
        /// </summary>
        public int MinFontSize { get; set; }

        /// <summary>
        /// Максимальный размер шрифта.
        /// </summary>
        public int MaxFontSize { get; set; }
    }
}