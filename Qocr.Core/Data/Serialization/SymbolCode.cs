using System.Diagnostics;
using System.Xml.Serialization;

using Qocr.Core.Data.Map2D;

namespace Qocr.Core.Data.Serialization
{
    /// <summary>
    /// Информация по каждому символу в <see cref="Symbol"/>.
    /// </summary>
    [DebuggerDisplay("{FontSize}-{EulerCode}")]
    public class SymbolCode
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="SymbolCode"/>.
        /// </summary>
        public SymbolCode(int fontSize, EulerMonomap2D eulerCode)
        {
            EulerCode = eulerCode;
            FontSize = fontSize;
        }
        
        /// <summary>
        /// Размер шрифта.
        /// </summary>
        public int FontSize { get; set; }

        /// <summary>
        /// Значение эйлеровой характеристики.
        /// </summary>
        public EulerMonomap2D EulerCode { get; set; }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return EulerCode.GetHashCode() ^ FontSize.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj?.GetHashCode() == GetHashCode();
        }
    }
}