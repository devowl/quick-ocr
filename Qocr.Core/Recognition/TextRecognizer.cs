using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using Qocr.Core.Approximation;
using Qocr.Core.Data.Serialization;
using Qocr.Core.Interfaces;
using Qocr.Core.Properties;
using Qocr.Core.Recognition.Data;
using Qocr.Core.Recognition.Logic;

namespace Qocr.Core.Recognition
{
    /// <summary>
    /// Распознаватель текста.
    /// </summary>
    public class TextRecognizer
    {
        public Action<IMonomap> PrintTest;

        private readonly IApproximator _approximator;

        private readonly IAnalyzer _analyzer;

        private readonly IScanner _scanner;

        /// <summary>
        /// Создание экземпляра класса <see cref="TextRecognizer"/>.
        /// </summary>
        public TextRecognizer() : this(new LuminosityApproximator(), null, null, null)
        {
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="TextRecognizer"/>.
        /// </summary>
        public TextRecognizer(IApproximator approximator) : this(approximator, null, null, null)
        {
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="TextRecognizer"/>.
        /// </summary>
        public TextRecognizer(
            IApproximator approximator,
            EulerContainer container,
            IAnalyzer analyzer,
            IScanner scanner)
        {
            if (approximator == null)
            {
                throw new ArgumentNullException(nameof(approximator));
            }

            if (container == null)
            {
                using (var genEnRu = new MemoryStream(Resources.Gen))
                {
                    container = CompressionUtils.Decompress<EulerContainer>(genEnRu);
                }
            }

            _approximator = approximator;
            _analyzer = analyzer ?? new DefaultAnalyzer(container);
            _scanner = scanner ?? new DefaultScanner();
        }

        /// <summary>
        /// Распознать.
        /// </summary>
        /// <param name="monomap">Ссылка на исходный <see cref="Bitmap"/>.</param>
        /// <returns></returns>
        public QReport Recognize(Bitmap monomap)
        {
            var appBitmap = _approximator.Approximate(monomap);
            return Recognize(appBitmap);
        }

        /// <summary>
        /// Распознать.
        /// </summary>
        /// <param name="monomap">Ссылка на <see cref="IMonomap"/>.</param>
        /// <returns></returns>
        public QReport Recognize(IMonomap monomap)
        {
            // TODO Там надо анализировать как то раздробленные картинки
            // Получаем все фрагменты изображения
            IList<QSymbol> fragments = _scanner.GetFragments(monomap);
            List<QAnalyzedSymbol> qSymbols = new List<QAnalyzedSymbol>();

            foreach (var fragment in fragments)
            {
                var existingValue = qSymbols.FirstOrDefault(symbol => Equals(symbol.Euler, fragment.Euler));
                if (existingValue == null)
                {
                    qSymbols.Add(_analyzer.Analyze(fragment));
                }
                else
                {
                    var analyzedSymbol = new QAnalyzedSymbol(fragment, existingValue.Chars);
                    qSymbols.Add(analyzedSymbol);
                }
            }

            return new QReport(qSymbols);
        }
    }
}