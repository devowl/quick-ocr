using System.Diagnostics;

namespace Qocr.Core.Recognition.Data
{
    /// <summary>
    /// Информация о распознаваемом символе.
    /// </summary>
    [DebuggerDisplay("{Char} @ {Probability}")]
    public class QChar
    {
        /// <summary>
        /// Неизвестный символ.
        /// </summary>
        public static QChar Unknown = new QChar('?', QState.Unknown);

        /// <summary>
        /// Создание экземпляра класса <see cref="QChar"/>.
        /// </summary>
        public QChar(char chr, QState state)
            : this(chr, state, 100)
        {
            
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="QChar"/>.
        /// </summary>
        public QChar(char chr, QState state, int probability)
        {
            State = state;
            Probability = probability;
            Char = chr;
        }

        /// <summary>
        /// Вероятность символа.
        /// </summary>
        public int Probability { get; private set; }

        /// <summary>
        /// Статус распознания.
        /// </summary>
        public QState State { get; private set; }

        /// <summary>
        /// Символ.
        /// </summary>
        public char Char { get; private set; }
    }
}