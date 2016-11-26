using System;
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

        private readonly EulerContainer _container;

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

            if (container != null)
            {
                _container = container;
            }
            else
            {
                using (var genEnRu = new MemoryStream(Resources.Gen))
                {
                    _container = CompressionUtils.Decompress<EulerContainer>(genEnRu);
                }
            }

            _approximator = approximator;
            _analyzer = analyzer ?? new DefaultAnalyzer(container);
            _scanner = scanner;
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
            var fragments = _scanner.GetFragments(monomap);

            var qSymbols = fragments.Select(fragment => _analyzer.Analyze(fragment.Monomap)).ToList();

            return new QReport(qSymbols);
        }

        private void PrintMeFast(IMonomap m)
        {
            if (PrintTest != null)
            {
                PrintTest(m);
            }
        }
    }
}