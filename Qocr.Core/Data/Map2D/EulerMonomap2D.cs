using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Qocr.Core.Data.Attributes;
using Qocr.Core.Utils;

namespace Qocr.Core.Data.Map2D
{
    /// <summary>
    /// Эйлеровая характеристика для изображения.
    /// </summary>
    public class EulerMonomap2D
    {
        private const string PropertyPrefix = "S";

        /// <summary>
        /// Создание экземпляра класса <see cref="EulerMonomap2D"/>.
        /// </summary>
        public EulerMonomap2D(params int[] characteristics)
        {
            ClassUtils.FillClassProperiesValues(this, PropertyPrefix, characteristics);
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="EulerMonomap2D"/>.
        /// </summary>
        public EulerMonomap2D(string characteristics, char splitter = ',')
            : this(characteristics.Split(splitter).Select(int.Parse).ToArray())
        {
        }

        /// <summary>
        /// Создание экземпляра класса <see cref="EulerMonomap2D"/>.
        /// </summary>
        public EulerMonomap2D(IDictionary<string, int> squares)
        {
            var eulerProperties = GetEulerProperties()
                .ToDictionary(
                    property => property,
                    property =>
                        (EulerPathAttribute)property.GetCustomAttributes(typeof(EulerPathAttribute), true).First());

            foreach (var square in squares)
            {
                var targetProperty = eulerProperties.First(property => property.Value.Path == square.Key);
                targetProperty.Key.SetValue(this, squares[square.Key], null);
            }
        }

        /// <summary>
        /// <para>[ ] [X]</para> 
        /// <para>[ ] [ ]</para> 
        /// </summary>
        [EulerPath("0100")]
        public int S0 { get; private set; }

        /// <summary>
        /// <para>[ ] [ ]</para> 
        /// <para>[ ] [X]</para> 
        /// </summary>
        [EulerPath("0001")]
        public int S1 { get; private set; }

        /// <summary>
        /// <para>[X] [ ]</para> 
        /// <para>[ ] [ ]</para> 
        /// </summary>
        [EulerPath("1000")]
        public int S2 { get; private set; }

        /// <summary>
        /// <para>[ ] [ ]</para> 
        /// <para>[X] [ ]</para> 
        /// </summary>
        [EulerPath("0010")]
        public int S3 { get; private set; }

        /// <summary>
        /// <para>[X] [X]</para> 
        /// <para>[ ] [ ]</para> 
        /// </summary>
        [EulerPath("1100")]
        public int S4 { get; private set; }

        /// <summary>
        /// <para>[ ] [X]</para> 
        /// <para>[ ] [X]</para> 
        /// </summary>
        [EulerPath("0101")]
        public int S5 { get; private set; }

        /// <summary>
        /// <para>[ ] [ ]</para> 
        /// <para>[X] [X]</para> 
        /// </summary>
        [EulerPath("0011")]
        public int S6 { get; private set; }

        /// <summary>
        /// <para>[X] [ ]</para> 
        /// <para>[X] [ ]</para> 
        /// </summary>
        [EulerPath("1010")]
        public int S7 { get; private set; }

        /// <summary>
        /// <para>[ ] [X]</para> 
        /// <para>[X] [ ]</para> 
        /// </summary>
        [EulerPath("0110")]
        public int S8 { get; private set; }

        /// <summary>
        /// <para>[X] [ ]</para> 
        /// <para>[ ] [X]</para> 
        /// </summary>
        [EulerPath("1001")]
        public int S9 { get; private set; }

        /// <summary>
        /// <para>[X] [X]</para> 
        /// <para>[ ] [X]</para> 
        /// </summary>
        [EulerPath("1101")]
        public int S10 { get; private set; }

        /// <summary>
        /// <para>[X] [ ]</para> 
        /// <para>[X] [X]</para> 
        /// </summary>
        [EulerPath("1011")]
        public int S11 { get; private set; }

        /// <summary>
        /// <para>[ ] [X]</para> 
        /// <para>[X] [X]</para> 
        /// </summary>
        [EulerPath("0111")]
        public int S12 { get; private set; }

        /// <summary>
        /// <para>[X] [X]</para> 
        /// <para>[X] [ ]</para> 
        /// </summary>
        [EulerPath("1110")]
        public int S13 { get; private set; }

        /// <summary>
        /// <para>[X] [X]</para> 
        /// <para>[X] [X]</para> 
        /// </summary>
        [EulerPath("1111")]
        public int S14 { get; private set; }

        /// <inheritdoc/>
        public int SquareSize
        {
            get
            {
                return 2;
            }
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return GetEulerProperties().Aggregate(0, (current, t) => current ^ (int)t.GetValue(this, null));
        }

        private IList<PropertyInfo> GetEulerProperties()
        {
            return
                GetType()
                    .GetProperties()
                    .Where(property => !property.CanWrite && property.Name.StartsWith(PropertyPrefix))
                    .ToList();
        }
    }
}