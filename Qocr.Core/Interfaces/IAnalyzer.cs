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
        /// <param name="fragment">Фрагмент изображение.</param>
        /// <returns>Распознанные символы.</returns>
        QAnalyzedSymbol Analyze(QSymbol fragment);
    }
}