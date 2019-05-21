// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueConverters.cs" company="Sane Development">
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SaneDevelopment.WPF4.Controls
{
    #region DateTime converters

    /// <summary>
    /// Класс реализует конвертер nullable объектов типа <see cref="DateTime"/> в и из строк
    /// с возможностью валидации пользовательского ввода
    /// </summary>
    public class NullableDateTimeToStringConverter : ValidationRule, IValueConverter
    {
        internal static ValidationResult Validate(IValueConverter converter, object value, CultureInfo cultureInfo)
        {
            Contract.Requires<ArgumentNullException>(converter != null);

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(true, null);

            return new ValidationResult(
                converter.ConvertBack(value, typeof(DateTime?), null, cultureInfo) != DependencyProperty.UnsetValue,
                LocalizedStrings.DateTimeValidationRuleMsg);
        }

        /// <summary>
        /// Метод выполняет валидацию пользовательского ввода
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="cultureInfo">Используемая культура</param>
        /// <returns>Возвращает объект <see cref="ValidationResult"/> с данными о результате валидации</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Validate(this, value, cultureInfo);
        }

        #region IValueConverter Members

        /// <summary>
        /// Выполняет конвертацию объекта типа <see cref="DateTime"/>? в строку
        /// </summary>
        /// <param name="value">Дата</param>
        /// <param name="targetType">Целевой тип (игнорируется)</param>
        /// <param name="parameter">Параметр конвертации: формат</param>
        /// <param name="culture">Используемая культура</param>
        /// <returns>Строковое представление даты, либо <c>null</c>, если <paramref name="value"/> не является датой</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            string res = string.Empty;
            var dt = value as DateTime?;
            if (dt.HasValue)
            {
                res = parameter == null ?
                    dt.Value.ToString(culture) :
                    dt.Value.ToString(parameter.ToString(), culture);
            }
            return res;
        }

        /// <summary>
        /// Выполняет конвертацию строки в объект типа <see cref="DateTime"/>
        /// </summary>
        /// <param name="value">Исходная строка</param>
        /// <param name="targetType">Целевой тип (игнорируется)</param>
        /// <param name="parameter">Параметр конвертации (игнорируется)</param>
        /// <param name="culture">Используемая культура</param>
        /// <returns>Объект <see cref="DateTime"/>,
        /// либо <c>null</c>, если <paramref name="value"/> не заполнен,
        /// либо <see cref="DependencyProperty.UnsetValue"/>, если <paramref name="value"/> содержит некорректную строку</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string s = value.ToString();
            if (string.IsNullOrWhiteSpace(s))
                return null;

            DateTime dt;
            if (DateTime.TryParse(s, out dt))
                return dt;

            return DependencyProperty.UnsetValue;
        }

        #endregion
    }

    /// <summary>
    /// Класс реализует конвертер nullable объектов типа <see cref="DateTime"/> в и из чисел с плавающей точкой
    /// с возможностью валидации пользовательского ввода
    /// </summary>
    public class NullableDateTimeToDoubleConverter : ValidationRule, IValueConverter
    {
        /// <summary>
        /// Метод выполняет валидацию пользовательского ввода
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="cultureInfo">Используемая культура</param>
        /// <returns>Возвращает объект <see cref="ValidationResult"/> с данными о результате валидации</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return NullableDateTimeToStringConverter.Validate(this, value, cultureInfo);
        }

        #region IValueConverter Members

        /// <summary>
        /// Выполняет конвертацию объекта типа <see cref="DateTime"/>? в число
        /// </summary>
        /// <param name="value">Дата</param>
        /// <param name="targetType">Целевой тип (игнорируется)</param>
        /// <param name="parameter">Параметр конвертации (игнорируется)</param>
        /// <param name="culture">Используемая культура</param>
        /// <returns>Числовое представление даты (число тактов), либо <c>null</c>, если <paramref name="value"/> не является датой</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            double? res = null;
            var dt = value as DateTime?;
            if (dt.HasValue)
            {
                res = dt.Value.Ticks;
            }
            return res;
        }

        /// <summary>
        /// Выполняет конвертацию числа в объект типа <see cref="DateTime"/>
        /// </summary>
        /// <param name="value">Исходное число</param>
        /// <param name="targetType">Целевой тип (игнорируется)</param>
        /// <param name="parameter">Параметр конвертации (игнорируется)</param>
        /// <param name="culture">Используемая культура</param>
        /// <returns>Объект <see cref="DateTime"/>,
        /// либо <c>null</c>, если <paramref name="value"/> не задан</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is double))
                return null;

            var dbl = (double)value;
            var longTicks = (long) dbl;

            if (longTicks > 0x2bca2875f4373fffL || longTicks <= 0)
                return null;

            return new DateTime(longTicks);
        }

        #endregion
    }

    #endregion

    #region TimeSpan converters

    /// <summary>
    /// Класс реализует конвертер nullable объектов типа <see cref="TimeSpan"/> в и из строк
    /// с возможностью валидации пользовательского ввода
    /// </summary>
    public class NullableTimeSpanToStringConverter : ValidationRule, IValueConverter
    {
        internal static ValidationResult Validate(IValueConverter converter, object value, CultureInfo cultureInfo)
        {
            Contract.Requires<ArgumentNullException>(converter != null);

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return new ValidationResult(true, null);

            return new ValidationResult(
                converter.ConvertBack(value, typeof(TimeSpan?), null, cultureInfo) != DependencyProperty.UnsetValue,
                LocalizedStrings.TimeSpanValidationRuleMsg);
        }

        /// <summary>
        /// Метод выполняет валидацию пользовательского ввода
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="cultureInfo">Используемая культура</param>
        /// <returns>Возвращает объект <see cref="ValidationResult"/> с данными о результате валидации</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Validate(this, value, cultureInfo);
        }

        #region IValueConverter Members

        /// <summary>
        /// Выполняет конвертацию объекта типа <see cref="TimeSpan"/>? в строку
        /// </summary>
        /// <param name="value">Временной интервал</param>
        /// <param name="targetType">Целевой тип (игнорируется)</param>
        /// <param name="parameter">Параметр конвертации: формат</param>
        /// <param name="culture">Используемая культура</param>
        /// <returns>Строковое представление интервала, либо <c>null</c>, если <paramref name="value"/> не является временным интервалом</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            string res = string.Empty;
            var dt = value as TimeSpan?;
            if (dt.HasValue)
            {
                res = parameter == null ?
                    dt.Value.ToString() :
                    dt.Value.ToString(parameter.ToString(), culture);
            }
            return res;
        }

        /// <summary>
        /// Выполняет конвертацию строки в объект типа <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="value">Исходная строка</param>
        /// <param name="targetType">Целевой тип (игнорируется)</param>
        /// <param name="parameter">Параметр конвертации (игнорируется)</param>
        /// <param name="culture">Используемая культура</param>
        /// <returns>Объект <see cref="TimeSpan"/>,
        /// либо <c>null</c>, если <paramref name="value"/> не заполнен,
        /// либо <see cref="DependencyProperty.UnsetValue"/>, если <paramref name="value"/> содержит некорректную строку</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            string s = value.ToString();
            if (string.IsNullOrWhiteSpace(s))
                return null;

            TimeSpan tm;
            if (TimeSpan.TryParse(s, out tm))
                return tm;

            return DependencyProperty.UnsetValue;
        }

        #endregion
    }

    /// <summary>
    /// Класс реализует конвертер nullable объектов типа <see cref="TimeSpan"/> в и из чисел с плавающей точкой
    /// с возможностью валидации пользовательского ввода
    /// </summary>
    public class NullableTimeSpanToDoubleConverter : ValidationRule, IValueConverter
    {
        /// <summary>
        /// Метод выполняет валидацию пользовательского ввода
        /// </summary>
        /// <param name="value">Проверяемое значение</param>
        /// <param name="cultureInfo">Используемая культура</param>
        /// <returns>Возвращает объект <see cref="ValidationResult"/> с данными о результате валидации</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return NullableTimeSpanToStringConverter.Validate(this, value, cultureInfo);
        }

        #region IValueConverter Members

        /// <summary>
        /// Выполняет конвертацию объекта типа <see cref="TimeSpan"/>? в число
        /// </summary>
        /// <param name="value">Временной интервал</param>
        /// <param name="targetType">Целевой тип (игнорируется)</param>
        /// <param name="parameter">Параметр конвертации (игнорируется)</param>
        /// <param name="culture">Используемая культура</param>
        /// <returns>Строковое представление интервала, либо <c>null</c>, если <paramref name="value"/> не является временным интервалом</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            double? res = null;
            var dt = value as TimeSpan?;
            if (dt.HasValue)
            {
                res = dt.Value.Ticks;
            }
            return res;
        }

        /// <summary>
        /// Выполняет конвертацию числа в объект типа <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="value">Исходное число</param>
        /// <param name="targetType">Целевой тип (игнорируется)</param>
        /// <param name="parameter">Параметр конвертации (игнорируется)</param>
        /// <param name="culture">Используемая культура</param>
        /// <returns>Объект <see cref="TimeSpan"/>,
        /// либо <c>null</c>, если <paramref name="value"/> не задан</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is double))
                return null;

            var dbl = (double)value;

            return TimeSpan.FromTicks((long)dbl);
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// Конвертер между массивом чисел и объектом <see cref="Thickness"/>
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
    public sealed class ThicknessMultiConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Выполняет конвертацию из массива чисел в объект <see cref="Thickness"/>
        /// </summary>
        /// <param name="values">Массив объектов, которые можно преобразовать к <see cref="double"/></param>
        /// <param name="targetType">Целевой тип (игнорируется)</param>
        /// <param name="parameter">Параметр (игнорируется)</param>
        /// <param name="culture">Используемая культура</param>
        /// <returns>Возвращает объект <see cref="Thickness"/>(<paramref name="values"/>[0], <paramref name="values"/>[1], <paramref name="values"/>[2], <paramref name="values"/>[3]).
        /// Если массив <paramref name="values"/> содержит недостаточно элементов, то недостающие элементы принимаются равными нулю.
        /// Используются только первые 4 элемента массива, остальные игнорируются.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
                return new Thickness();

            int len = values.Length;

            double left = len >= 1 ? System.Convert.ToDouble(values[0], culture) : 0.0;
            double top = len >= 2 ? System.Convert.ToDouble(values[1], culture) : 0.0;
            double right = len >= 3 ? System.Convert.ToDouble(values[2], culture) : 0.0;
            double bottom = len >= 4 ? System.Convert.ToDouble(values[3], culture) : 0.0;

            return new Thickness(left, top, right, bottom);
        }

        /// <summary>
        /// Выполняет конвертацию объекта <see cref="Thickness"/> в массив чисел.
        /// </summary>
        /// <param name="value">Объект <see cref="Thickness"/></param>
        /// <param name="targetTypes">Целевые типы (игнорируется)</param>
        /// <param name="parameter">Параметр (игнорируется)</param>
        /// <param name="culture">Используемая культура</param>
        /// <returns><c>null</c>, если <paramref name="value"/> не является объектом <see cref="Thickness"/>,
        /// иначе массив из четырех чисел: левая, верхняя, правая, нижняя сторона прямоугольника</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Thickness))
                return null;

            var thickness = (Thickness)value;
            return new object[] { thickness.Left, thickness.Top, thickness.Right, thickness.Bottom };
        }

        #endregion
    }
}
