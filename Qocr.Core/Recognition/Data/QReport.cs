using System;
using System.Collections.ObjectModel;

namespace Qocr.Core.Recognition.Data
{
    /// <summary>
    /// Результат распознавания.
    /// </summary>
    public class QReport
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="QReport"/>.
        /// </summary>
        public QReport()
        {
        }

        public ReadOnlyCollection<QAnalyzedSymbol> Symbols { get; private set; }

        /// <summary>
        /// Получить "сырой" тест 
        /// </summary>
        /// <returns></returns>
        public string RawText()
        {
            // Пробелы ???
            throw new NotImplementedException();
        }
    }
}