using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using Qocr.Core.Data.Map2D;

namespace Qocr.Core.Data.Serialization
{
    /// <summary>
    /// Сериализуемая информация о символе.
    /// </summary>
    public class Symbol
    {
        private List<EulerMonomap2D> _normalCodes;

        private List<EulerMonomap2D> _boldCodes;

        private List<EulerMonomap2D> _italicCodes;

        /// <summary>
        /// Код символа.
        /// </summary>
        public char Chr { get; set; }
        
        /// <summary>
        /// Строка из кодов для жирного шрифта.
        /// </summary>
        public string StringsCodesBold { get; set; }

        /// <summary>
        /// Строка из кодов для курсивного шрифта.
        /// </summary>
        public string StringsCodesItalic { get; set; }

        /// <summary>
        /// Строка из кодов для обычного шрифта.
        /// </summary>
        public string StringsCodesNormal { get; set; }

        /// <summary>
        /// Список эйлеровых характеристик для жирного шрифта.
        /// </summary>
        [XmlIgnore]
        public List<EulerMonomap2D> CodesBold => _boldCodes ?? (_boldCodes = GetData(StringsCodesBold));

        /// <summary>
        /// Список эйлеровых характеристик для обычного шрифта.
        /// </summary>
        [XmlIgnore]
        public List<EulerMonomap2D> CodesNormal => _normalCodes ?? (_normalCodes = GetData(StringsCodesNormal));

        /// <summary>
        /// Список эйлеровых характеристик для курсивного шрифта.
        /// </summary>
        [XmlIgnore]
        public List<EulerMonomap2D> CodesItalic => _italicCodes ?? (_italicCodes = GetData(StringsCodesItalic));

        private static List<EulerMonomap2D> GetData(string sourceString)
        {
            return sourceString.Split(';').Select(str => new EulerMonomap2D(str)).ToList();
        }
    }
}