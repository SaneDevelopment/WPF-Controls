// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyPropertyUtil.cs" company="Sane Development">
//
//   Sane Development WPF Controls Library
//
//   The BSD 3-Clause License
//
//   Copyright (c) 2011-2019, Sane Development
//   All rights reserved.
//
//   Redistribution and use in source and binary forms, with or without modification,
//   are permitted provided that the following conditions are met:
//
//   - Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//   - Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//   - Neither the name of the Sane Development nor the names of its contributors
//     may be used to endorse or promote products derived from this software
//     without specific prior written permission.
//
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//   AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
//   THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//   ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS
//   BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
//   OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
//   OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
//   WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//   EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Класс предоставляет набо вспомогательный функций для работы со свойствами зависимости
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Util")]
    public static class DependencyPropertyUtil
    {
        internal static double ExtractDouble(object value, double defaultValue)
        {
            if (value == null || !(value is double))
            {
                return defaultValue;
            }
            var newValue = (double)value;
            if (double.IsNaN(newValue) || double.IsInfinity(newValue))
            {
                newValue = defaultValue;
            }
            return newValue;
        }

        internal static double ConvertToDouble(object value, double defaultValue)
        {
            double newValue = defaultValue;
            if (value != null)
            {
                try
                {
                    newValue = Convert.ToDouble(value, CultureInfo.CurrentCulture);
                }
                catch (FormatException)
                { }
                catch (InvalidCastException)
                { }
                catch (OverflowException)
                { }
            }
            return newValue;
        }

        /// <summary>
        /// Является ли заданный объект корректным значением типа <see cref="double"/>
        /// </summary>
        /// <param name="value">Проверяемый объект</param>
        /// <returns><c>true</c>, если <paramref name="value"/> может быть приведено к числу с плавающей точкой
        /// и является допустимым значением (т.е. не NaN и не бесконечность)</returns>
        public static bool IsValidDoubleValue(object value)
        {
            Contract.Ensures(value != null || !Contract.Result<bool>());

            if (value == null)
                return false;

            double num;
            try
            {
                num = (double)value;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            return (!double.IsNaN(num) && !double.IsInfinity(num));
        }

        /// <summary>
        /// Является ли заданный объект корректным значением типа <see cref="DateTime"/>
        /// </summary>
        /// <param name="value">Проверяемый объект</param>
        /// <returns>Является ли <paramref name="value"/> объектом типа <see cref="DateTime"/></returns>
        public static bool IsValidDateTimeValue(object value)
        {
            return value is DateTime;
        }

        /// <summary>
        /// Является ли заданный объект корректным значением типа <see cref="bool"/>
        /// </summary>
        /// <param name="value">Проверяемый объект</param>
        /// <returns>Является ли <paramref name="value"/> объектом типа <see cref="bool"/></returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool")]
        public static bool IsValidBoolValue(object value)
        {
            return value is bool;
        }

        /// <summary>
        /// Является ли заданный объект корректным значением типа <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="value">Проверяемый объект</param>
        /// <returns>Является ли <paramref name="value"/> объектом типа <see cref="TimeSpan"/></returns>
        public static bool IsValidTimeSpanValue(object value)
        {
            return value is TimeSpan;
        }

        /// <summary>
        /// Является ли заданный объект корректным значением типа <paramref name="targetType"/>
        /// </summary>
        /// <param name="targetType">Проверяемый тип</param>
        /// <param name="value">Проверяемый объект</param>
        /// <returns><c>true</c>, если <paramref name="value"/> является корректным значением типа <paramref name="targetType"/></returns>
        public static bool IsValidValue(Type targetType, object value)
        {
            // TODO сделать возможность динамически добавлять обработчики для новых типов
            if (typeof(double).IsAssignableFrom(targetType))
            {
                return IsValidDoubleValue(value);
            }
            if (typeof(DateTime).IsAssignableFrom(targetType))
            {
                return IsValidDateTimeValue(value);
            }
            if (typeof(TimeSpan).IsAssignableFrom(targetType))
            {
                return IsValidTimeSpanValue(value);
            }
            return false;
        }

        /// <summary>
        /// Является ли заданный объект корректным значением смещения типа <paramref name="targetType"/>
        /// </summary>
        /// <param name="targetType">Проверяемый тип</param>
        /// <param name="value">Проверяемый объект</param>
        /// <returns><c>true</c>, если <paramref name="value"/> является корректным значением смещения типа <paramref name="targetType"/></returns>
        public static bool IsValidChange(Type targetType, object value)
        {
            Contract.Ensures(value != null || !Contract.Result<bool>());

            // TODO сделать возможность динамически добавлять обработчики для новых типов
            if (value == null)
                return false;

            if (!IsValidValue(targetType, value))
                return false;

            if (typeof(double).IsAssignableFrom(targetType))
            {
                var val = (double)value;
                return val >= 0.0;
            }
            if (typeof(TimeSpan).IsAssignableFrom(targetType))
            {
                var val = (TimeSpan)value;
                return val >= TimeSpan.Zero;
            }
            return false;
        }

        /// <summary>
        /// Является ли объект <paramref name="value"/> корректным значением типа <see cref="Orientation"/>
        /// </summary>
        /// <param name="value">Проверяемый объект</param>
        /// <returns>Является ли объект <paramref name="value"/> корректным значением типа <see cref="Orientation"/></returns>
        public static bool IsValidOrientation(object value)
        {
            if (!(value is Orientation))
                return false;

            var orientation = (Orientation)value;
            return (orientation == Orientation.Horizontal) || (orientation == Orientation.Vertical);
        }

        /// <summary>
        /// Является ли объект <paramref name="value"/> корректным значением для точности отображения чисел с плавающей точкой
        /// </summary>
        /// <param name="value">Проверяемый объект</param>
        /// <returns>Является ли объект <paramref name="value"/> корректным значением для точности отображения чисел с плавающей точкой</returns>
        public static bool IsValidAutoToolTipPrecision(object value)
        {
            if (!(value is int))
            {
                return false;
            }
            var intVal = (int) value;
            return intVal >= 0 && intVal <= 99;
        }

        /// <summary>
        /// Является ли объект <paramref name="value"/> корректным значением типа <see cref="AutoToolTipPlacement"/>
        /// </summary>
        /// <param name="value">Проверяемый объект</param>
        /// <returns>Является ли объект <paramref name="value"/> корректным значением типа <see cref="AutoToolTipPlacement"/></returns>
        public static bool IsValidAutoToolTipPlacement(object value)
        {
            if (!(value is AutoToolTipPlacement))
            {
                return false;
            }
            var placement = (AutoToolTipPlacement)value;
            return placement == AutoToolTipPlacement.None ||
                   placement == AutoToolTipPlacement.TopLeft ||
                   placement == AutoToolTipPlacement.BottomRight;
        }

        /// <summary>
        /// Функция выполняет коррекцию начального значения интервала для заданного объекта.
        /// Коррекция выполняется согласно принятым для интервальных контролов правилам, а именно,
        /// что начальное значение не должно меньше минимума и больше конечного значения или максимума
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <typeparam name="TInterval">Тип интервала</typeparam>
        /// <param name="ranged">Корректируемый объект</param>
        /// <param name="value">Проверяемое значение</param>
        /// <returns><paramref name="value"/>, либо скорректированное значение</returns>
        public static object CoerceRangeStartValue<T, TInterval>(IRanged<T, TInterval> ranged, T value)
        {
            if (ranged == null) return value;

            double? coercedValue = null;
            double doubleValue = ranged.ValueToDouble(value);

            // порядок проверок важен: сначала проверяем размер интервала,
            // т.к. его "подгонка" под минимальное значение может "испортить" более важные ограничения.
            double endValue = ranged.ValueToDouble(ranged.EndValue);
            double minRangeValue = ranged.IntervalToDouble(ranged.MinRangeValue);
            double newRangeValue = endValue - doubleValue;
            bool isSingleValue = ranged.IsSingleValue;
            if (!isSingleValue && DoubleUtil.LessThan(newRangeValue, minRangeValue))
            {
                coercedValue = doubleValue = endValue - minRangeValue;
            }

            double minimum = ranged.ValueToDouble(ranged.Minimum);
            if (doubleValue < minimum)
            {
                coercedValue = doubleValue = minimum;
            }

            if (!isSingleValue && doubleValue > endValue)
            {
                coercedValue = endValue;
            }

            if (coercedValue.HasValue)
            {
                return ranged.DoubleToValue(coercedValue.Value);
            }
            return value;
        }

        /// <summary>
        /// Функция выполняет коррекцию конечного значения интервала для заданного объекта.
        /// Коррекция выполняется согласно принятым для интервальных контролов правилам, а именно,
        /// что конечное значение не должно меньше начального значение или минимума и больше максимума
        /// </summary>
        /// <typeparam name="T">Тип значения</typeparam>
        /// <typeparam name="TInterval">Тип интервала</typeparam>
        /// <param name="ranged">Корректируемый объект</param>
        /// <param name="value">Проверяемое значение</param>
        /// <returns><paramref name="value"/>, либо скорректированное значение</returns>
        public static object CoerceRangeEndValue<T, TInterval>(IRanged<T, TInterval> ranged, T value)
        {
            if (ranged == null) return value;

            double? coercedValue = null;
            double doubleValue = ranged.ValueToDouble(value);

            // порядок проверок важен: сначала проверяем размер интервала,
            // т.к. его "подгонка" под минимальное значение может "испортить" более важные ограничения.
            double startValue = ranged.ValueToDouble(ranged.StartValue);
            double minRangeValue = ranged.IntervalToDouble(ranged.MinRangeValue);
            double newRangeValue = doubleValue - startValue;
            bool isSingleValue = ranged.IsSingleValue;
            if (!isSingleValue && DoubleUtil.LessThan(newRangeValue, minRangeValue))
            {
                coercedValue = doubleValue = minRangeValue + startValue;
            }

            double maximum = ranged.ValueToDouble(ranged.Maximum);
            if (doubleValue > maximum)
            {
                coercedValue = doubleValue = maximum;
            }

            if (!isSingleValue && doubleValue < startValue)
            {
                coercedValue = startValue;
            }

            if (coercedValue.HasValue)
            {
                return ranged.DoubleToValue(coercedValue.Value);
            }
            return value;
        }
    }
}
