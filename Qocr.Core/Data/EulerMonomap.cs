using System.Linq;

using Qocr.Core.Utils;

namespace Qocr.Core.Data
{
    /// <summary>
    /// Эйлеровая характеристика для изображения.
    /// </summary>
    internal class EulerMonomap
    {
        private const string PropertyPrefix = "S";

        /// <summary>
        /// Создание экземпляра класса <see cref="EulerMonomap"/>.
        /// </summary>
        public EulerMonomap(params int[] characteristics)
        {
            ClassUtils.FillClassProperiesValues(this, PropertyPrefix, characteristics);
        }

        public EulerMonomap(string characteristics, char splitter = ',')
            : this(characteristics.Split(splitter).Select(int.Parse).ToArray())
        {
        }

        /// <summary>
        /// <para>[ ] [ ]</para> 
        /// <para>[ ] [ ]</para> 
        /// </summary>
        public int S0 { get; private set; }

        /// <summary>
        /// <para>[ ] [X]</para> 
        /// <para>[ ] [ ]</para> 
        /// </summary>
        public int S1 { get; private set; }

        /// <summary>
        /// <para>[ ] [ ]</para> 
        /// <para>[ ] [X]</para> 
        /// </summary>
        public int S2 { get; private set; }

        /// <summary>
        /// <para>[X] [ ]</para> 
        /// <para>[ ] [ ]</para> 
        /// </summary>
        public int S3 { get; private set; }

        /// <summary>
        /// <para>[ ] [ ]</para> 
        /// <para>[X] [ ]</para> 
        /// </summary>
        public int S4 { get; private set; }

        /// <summary>
        /// <para>[X] [X]</para> 
        /// <para>[ ] [ ]</para> 
        /// </summary>
        public int S5 { get; private set; }

        /// <summary>
        /// <para>[ ] [X]</para> 
        /// <para>[ ] [X]</para> 
        /// </summary>
        public int S6 { get; private set; }

        /// <summary>
        /// <para>[ ] [ ]</para> 
        /// <para>[X] [X]</para> 
        /// </summary>
        public int S7 { get; private set; }

        /// <summary>
        /// <para>[X] [ ]</para> 
        /// <para>[X] [ ]</para> 
        /// </summary>
        public int S8 { get; private set; }

        /// <summary>
        /// <para>[ ] [X]</para> 
        /// <para>[X] [ ]</para> 
        /// </summary>
        public int S9 { get; private set; }

        /// <summary>
        /// <para>[X] [ ]</para> 
        /// <para>[ ] [X]</para> 
        /// </summary>
        public int S10 { get; private set; }

        /// <summary>
        /// <para>[X] [X]</para> 
        /// <para>[ ] [X]</para> 
        /// </summary>
        public int S11 { get; private set; }

        /// <summary>
        /// <para>[X] [ ]</para> 
        /// <para>[X] [X]</para> 
        /// </summary>
        public int S12 { get; private set; }

        /// <summary>
        /// <para>[ ] [X]</para> 
        /// <para>[X] [X]</para> 
        /// </summary>
        public int S13 { get; private set; }

        /// <summary>
        /// <para>[X] [X]</para> 
        /// <para>[X] [ ]</para> 
        /// </summary>
        public int S14 { get; private set; }

        /// <summary>
        /// <para>[X] [X]</para> 
        /// <para>[X] [X]</para> 
        /// </summary>
        public int S15 { get; private set; }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var properties = GetType().GetProperties().Where(property => !property.CanWrite).ToList();
            return properties.Aggregate(0, (current, t) => current ^ (int)t.GetValue(this, null));
        }
    }
}