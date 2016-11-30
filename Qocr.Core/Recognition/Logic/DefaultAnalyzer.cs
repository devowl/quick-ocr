using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Qocr.Core.Data.Map2D;
using Qocr.Core.Data.Serialization;
using Qocr.Core.Interfaces;
using Qocr.Core.Recognition.Data;

namespace Qocr.Core.Recognition.Logic
{
    /// <summary>
    /// Анализатор изображения на наличие в нём символа, и распознание его.
    /// </summary>
    public class DefaultAnalyzer : IAnalyzer
    {
        /// <summary>
        /// Значение приближения значений, которое говорит о том что символ может являться данным изображением.
        /// </summary>
        private const int RoundingPercents = 5;

        private readonly EulerContainer _container;

        private readonly IComparer<EulerMonomap2D> _comparer;

        private readonly int _roundingPrecents;

        /// <summary>
        /// Создание экземпляра класса <see cref="DefaultAnalyzer"/>.
        /// </summary>
        public DefaultAnalyzer(EulerContainer container)
            : this(container, new DefaultEulerComparer(), RoundingPercents)
        {    
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="DefaultAnalyzer"/>.
        /// </summary>
        public DefaultAnalyzer(EulerContainer container, IComparer<EulerMonomap2D> comparer, int roundingPrecents)
        {
            _container = container;
            _comparer = comparer;
            _roundingPrecents = roundingPrecents;
        }

        /// <inheritdoc/>
        public QAnalyzedSymbol Analyze(QSymbol fragment)
        {
            // Результат анализа всех букв !!! 
            var resultData = new List<QChar>();

            var currentEuler = EulerCharacteristicComputer.Compute2D(fragment.Monomap);
            
            // Идём по всем языкам.
            foreach (var language in _container.Languages)
            {
                // Идём по всем буквам языка
                foreach (var symbol in language.Chars)
                {
                    QChar @char;
                    if (TryFind(symbol, currentEuler, out @char))
                    {
                        // Не стоит пропускать проверку всех символов, так как стоит найти 3 и з цифро-буквы, либо аналоги А ру. и а англ.
                        resultData.Add(@char);
                    }
                }
            }

            return new QAnalyzedSymbol(fragment, resultData);
        }

        private bool TryFind(Symbol symbol, EulerMonomap2D charEuler, out QChar @char)
        {
            @char = null;

            // Если данный символ присутствует в базе знаний
            if (symbol.Codes.Any(item => Equals(item.EulerCode, charEuler)))
            {
                @char = new QChar(symbol.Chr, QState.Ok);
                return true;
            }

            // TODO дорогое вычисление, пока тестовый вариант
            var minEulerDiffValue = symbol.Codes.Min(code => _comparer.Compare(charEuler, code.EulerCode));
            if (minEulerDiffValue < _roundingPrecents)
            {
                int probability = minEulerDiffValue;
                @char = new QChar(symbol.Chr, QState.Assumptions, probability);
                return true;
            }

            return false;
        }
    }
}