﻿using System.Collections.Generic;
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
            ClassUtils.FillClassProperiesValues(this, PropertyPrefix, characteristics.Cast<object>().ToArray());
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
                .Where(property => property.GetCustomAttributes(typeof(EulerPathAttribute), false).Any())
                .ToDictionary(
                    property => property,
                    property =>
                        (EulerPathAttribute)property.GetCustomAttributes(typeof(EulerPathAttribute), false).FirstOrDefault());

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
            return S0 ^ S1 ^ S2 ^ S3 ^ S4 ^ S5 ^ S6 ^ S7 ^ S8 ^ S9 ^ S10 ^ S11 ^ S12 ^ S13 ^ S14;
        }

        public override bool Equals(object obj)
        {
            EulerMonomap2D objEuler = obj as EulerMonomap2D;
            if (objEuler == null)
            {
                return false;
            }

            return 
                objEuler.S0  == S0  && 
                objEuler.S1  == S1  &&
                objEuler.S2  == S2  &&
                objEuler.S3  == S3  &&
                objEuler.S4  == S4  &&
                objEuler.S5  == S5  &&
                objEuler.S6  == S6  &&
                objEuler.S7  == S7  &&
                objEuler.S8  == S8  &&
                objEuler.S9  == S9  &&
                objEuler.S10 == S10 &&
                objEuler.S11 == S11 &&
                objEuler.S12 == S12 &&
                objEuler.S13 == S13 &&
                objEuler.S14 == S14;

        }

        private IList<PropertyInfo> GetEulerProperties()
        {
            return
                GetType()
                    .GetProperties()
                    .Where(property => property.Name.StartsWith(PropertyPrefix))
                    .ToList();
        }

        public override string ToString()
        {
            return string.Join(",", S0, S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, S14);
        }                   
    }
}








