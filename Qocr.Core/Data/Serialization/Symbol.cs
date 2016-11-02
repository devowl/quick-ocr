using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Qocr.Core.Data.Map2D;

namespace Qocr.Core.Data.Serialization
{
    /// <summary>
    /// Сериализуемая информация о символе.
    /// </summary>
    public class Symbol
    {
        private const string Seporator = ";";

        /// <summary>
        /// Создание экземпляра класса <see cref="Symbol"/>.
        /// </summary>
        public Symbol()
        {
            CodesItalic = new HashSet<EulerMonomap2D>();
            CodesBold = new HashSet<EulerMonomap2D>();
            CodesNormal = new HashSet<EulerMonomap2D>();
        }

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
        public HashSet<EulerMonomap2D> CodesBold { get; private set; }

        /// <summary>
        /// Список эйлеровых характеристик для обычного шрифта.
        /// </summary>
        [XmlIgnore]
        public HashSet<EulerMonomap2D> CodesNormal { get; private set; }

        /// <summary>
        /// Список эйлеровых характеристик для курсивного шрифта.
        /// </summary>
        [XmlIgnore]
        public HashSet<EulerMonomap2D> CodesItalic { get; private set; }

        [OnSerializing]
        internal void OnSerializingMethod(StreamingContext context)
        {
            StringsCodesBold = string.Join(Seporator, CodesBold);
            StringsCodesItalic = string.Join(Seporator, CodesItalic);
            StringsCodesNormal = string.Join(Seporator, CodesNormal);
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            CodesItalic = GetData(StringsCodesItalic);
            CodesNormal = GetData(StringsCodesNormal);
            CodesBold = GetData(StringsCodesBold);
        }

        private static HashSet<EulerMonomap2D> GetData(string sourceString)
        {
            var splitter = new[]
            {
                Seporator
            };

            var result = new HashSet<EulerMonomap2D>();
            foreach (
                var eulerMonomap2D in
                    (sourceString ?? string.Empty).Split(splitter, StringSplitOptions.RemoveEmptyEntries)
                        .Select(str => new EulerMonomap2D(str)))
            {
                result.Add(eulerMonomap2D);
            }

            return result;
        }
    }
}