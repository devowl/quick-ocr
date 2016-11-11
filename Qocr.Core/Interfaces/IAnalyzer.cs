using Qocr.Core.Recognition.Data;

namespace Qocr.Core.Interfaces
{
    /// <summary>
    /// Анализ <see cref="IMonomap"/> на присутствие на изображении символа.
    /// </summary>
    public interface IAnalyzer
    {
        /// <summary>
        /// Проанализировать изображение.
        /// </summary>
        /// <param name="monomap">Исходное изображение.</param>
        /// <returns>Распознанные символы.</returns>
        QAnalyzedSymbol Analyze(IMonomap monomap);
    }
}