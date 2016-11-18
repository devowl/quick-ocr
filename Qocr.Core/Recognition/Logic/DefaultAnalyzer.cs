﻿using System;
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
                var charCases = new[]
                {
                    language.LowcaseCharactors,
                    language.UppercaseCharactors
                };

                // Идём по всем буквам языка
                foreach (var charCase in charCases)
                {
                    foreach (var symbol in charCase)
                    {
                        var eulerStandarts = new[]
                        {
                            symbol.CodesBold,
                            symbol.CodesItalic,
                            symbol.CodesNormal,

                        };

                        foreach (var eulerHashset in eulerStandarts)
                        {
                            QChar @char;
                            resultData.Add(
                                TryFind(eulerHashset, currentEuler, symbol.Chr, monomap, out @char)
                                    ? @char
                                    : QChar.Unknown);
                        }
                    }
                }
            }

            return new QAnalyzedSymbol(monomap, resultData);
        }

        private bool TryFind(ICollection<EulerMonomap2D> eulerList, EulerMonomap2D charEuler, char chr, IMonomap image, out QChar @char)
        {
            @char = null;

            // Если данный символ присутствует в базе знаний
            if (eulerList.Contains(charEuler))
            {
                @char = new QChar(chr, QState.Ok);
                return true;
            }

            // TODO дорогое вычисление, пока тестовый вариант
            var eulerDiff = eulerList.ToDictionary(item => item.CompareTo(charEuler), item => item);
            var minEulerDiffValue = eulerDiff.Keys.Min();
            if (minEulerDiffValue < _roundingPrecents)
            {
                @char = new QChar(chr, QState.Assumptions);
                return true;
            }

            return false;
        }
    }
}