using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qocr.Core.Utils
{
    /// <summary>
    /// Набор методов для работы с классами.
    /// </summary>
    public static class ClassUtils
    {
        /// <summary>
        /// Заполнить значения свойств класса с шагом нумерации 1.
        /// </summary>
        /// <param name="instance">Ссылка на экземпляр.</param>
        /// <param name="prefix">Префикс свойства.</param>
        /// <param name="values">Список задаваемых значений.</param>
        public static void FillClassProperiesValues(object instance, string prefix, params object[] values)
        {
            var type = instance.GetType();
            for (int i = 0; i < values.Length; i++)
            {
                var value = values[i];
                var property = type.GetProperty($"{prefix}{i}");

                var t = type.GetProperties();
                property.SetValue(instance, value, null);
            }
        }
    }
}
