using System.Collections.Generic;

namespace Qocr.Core.Data.Serialization
{
    /// <summary>
    /// Контейнер для всех эйлеровых наборов.
    /// </summary>
    public class EulerContainer
    {
        /// <summary>
        /// Создание экземпляра класса <see cref="EulerContainer"/>.
        /// </summary>
        public EulerContainer()
        {
            Languages = new List<Language>();
        }

        /// <summary>
        /// Список языковых наборов.
        /// </summary>
        public List<Language> Languages { get; set; }

        /// <summary>
        /// Набор спецсимволов.
        /// </summary>
        public SpecialChars SpecialChars { get; set; }
    }
}