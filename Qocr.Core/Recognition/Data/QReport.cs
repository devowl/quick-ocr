using System;
using System.Collections.Generic;
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
        public QReport(IList<QAnalyzedSymbol> symbols)
        {
            Symbols = new ReadOnlyCollection<QAnalyzedSymbol>(symbols);
        }

        public ReadOnlyCollection<QAnalyzedSymbol> Symbols { get; private set; }

        /// <summary>
        /// Получить "сырой" тест 
        /// </summary>
        /// <returns></returns>
        public string RawText()
        {
            // TODO Векторный спуск, Оценить пробелы всё тут
            // TODO Возможно метод получение текста части исходного изображения
            throw new NotImplementedException();
        }
    }
}