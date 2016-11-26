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

        private readonly int _roundingPrecents;

        /// <summary>
        /// Создание экземпляра класса <see cref="DefaultAnalyzer"/>.
        /// </summary>
        public DefaultAnalyzer(EulerContainer container)
            : this(container, RoundingPercents)
        {    
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="DefaultAnalyzer"/>.
        /// </summary>
        public DefaultAnalyzer(EulerContainer container, int roundingPrecents)
        {
            _container = container;
            _roundingPrecents = roundingPrecents;
        }

        /// <inheritdoc/>
        public QAnalyzedSymbol Analyze(IMonomap monomap)
        {
            var resultData = new List<QChar>();

            var currentEuler = EulerCharacteristicComputer.Compute2D(monomap);

            // Идём по всем языкам.
            foreach (var language in _container.Languages)
            {
                // Идём по всем буквам языка
                foreach (var symbol in language.Chars)
                {
                    QChar @char;
                    resultData.Add(
                        TryFind(symbol, currentEuler, monomap, out @char)
                            ? @char
                            : QChar.Unknown);
                }
                
            }

            return new QAnalyzedSymbol(monomap, resultData);
        }

        private bool TryFind(Symbol symbol, EulerMonomap2D charEuler, IMonomap image, out QChar @char)
        {
            @char = null;

            // Если данный символ присутствует в базе знаний
            if (symbol.Codes.Any(item => Equals(item.EulerCode, charEuler)))
            {
                @char = new QChar(symbol.Chr, QState.Ok);
                return true;
            }

            // TODO дорогое вычисление, пока тестовый вариант
            var eulerDiff = symbol.Codes.ToDictionary(item => item.EulerCode.CompareTo(charEuler), item => item);
            var minEulerDiffValue = eulerDiff.Keys.Min();
            if (minEulerDiffValue < _roundingPrecents)
            {
                @char = new QChar(symbol.Chr, QState.Assumptions);
                return true;
            }

            return false;
        }
    }
}