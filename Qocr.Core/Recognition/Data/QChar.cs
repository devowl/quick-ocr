namespace Qocr.Core.Recognition.Data
{
    /// <summary>
    /// Информация о распознаваемом символе.
    /// </summary>
    public class QChar
    {
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