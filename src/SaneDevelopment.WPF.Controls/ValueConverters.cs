// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueConverters.cs" company="Sane Development">
//
//   Sane Development WPF Controls Library
//
//   The BSD 3-Clause License
//
//   Copyright (c) Sane Development
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
using System.Windows.Media;
using SaneDevelopment.WPF.Controls.Properties;

namespace SaneDevelopment.WPF.Controls
{
    #region Double converters

    /// <summary>
    /// Converter from <c>double</c> to <c>string</c> representation of <see cref="DateTime"/>,
    /// where <c>double</c> value represents <see cref="System.DateTime.Ticks"/> value
    /// </summary>
    public class DoubleToDateTimeStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts <see cref="System.DateTime.Ticks"/> to <c>string</c> representation of <see cref="DateTime"/>
        /// </summary>
        /// <param name="value">Date time ticks</param>
        /// <param name="targetType">Target type (ignores)</param>
        /// <param name="parameter">Convertion parameter: date time format</param>
        /// <param name="culture">Culture</param>
        /// <returns>String representation of date, or <c>null</c>, if <paramref name="value"/> is not <c>double</c></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string res = string.Empty;

            var dbl = value as double?;
            if (!dbl.HasValue)
            {
                return res;
            }

            var ticks = (long) dbl;
            if (ticks < DateTime.MinValue.Ticks)
            {
                return res;
            }
            if (ticks > DateTime.MaxValue.Ticks + 1)
            {
                // allow minimum excess over DateTime.MaxValue.Ticks because of loss of accuracy while casting from double
                return res;
            }

            if (ticks == DateTime.MaxValue.Ticks + 1)
            {
                ticks = DateTime.MaxValue.Ticks;
            }

            Contract.Assume(ticks <= 0x2bca2875f4373fffL); // DateTime.MaxValue.Ticks
            var date = new DateTime(ticks);

            if (parameter == null)
            {
                res = date.ToString(culture);
            }
            else
            {
                try
                {
                    res = date.ToString(parameter.ToString(), culture);
                }
                catch (FormatException)
                {
                    res = LocalizationResource.BadDateTimeToStringFormat;
                }
            }

            return res;
        }

        /// <summary>
        /// Converts <c>string</c> to <see cref="DateTime.Ticks"/> as <c>double</c>
        /// </summary>
        /// <param name="value">Source string</param>
        /// <param name="targetType">Target type (ignores)</param>
        /// <param name="parameter">Convertion parameter (ignores)</param>
        /// <param name="culture">Culture</param>
        /// <returns><see cref="DateTime"/>,
        /// or <c>null</c>, if <paramref name="value"/> is empty (or whitespace),
        /// or <see cref="DependencyProperty.UnsetValue"/>, if <paramref name="value"/> contains incorrect string</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            string s = value.ToString();
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            DateTime dt;
            if (DateTime.TryParse(s, out dt))
            {
                return (double)dt.Ticks;
            }

            return DependencyProperty.UnsetValue;
        }

        #endregion
    }

    /// <summary>
    /// Converter from <c>double</c> to <c>string</c> representation of <see cref="TimeSpan"/>,
    /// where <c>double</c> value represents <see cref="System.TimeSpan.Ticks"/> value
    /// </summary>
    public class DoubleToTimeSpanStringConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts <see cref="System.TimeSpan.Ticks"/> to <c>string</c> representation of <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="value">Time span ticks</param>
        /// <param name="targetType">Target type (ignores)</param>
        /// <param name="parameter">Convertion parameter: time span format</param>
        /// <param name="culture">Culture</param>
        /// <returns>String representation of time span, or <c>null</c>, if <paramref name="value"/> is not <c>double</c></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string res = string.Empty;

            var dbl = value as double?;
            if (!dbl.HasValue)
            {
                return res;
            }

            var ticks = (long)dbl.Value;

            // (long)(double)(System.TimeSpan.MaxValue.Ticks) == System.TimeSpan.MinValue.Ticks,
            // therefore we need to compare double values first
            // to decrease risk of loss of accuracy while casting from double

            if (DoubleUtil.AreClose(dbl.Value, TimeSpan.MaxValue.Ticks))
            {
                ticks = TimeSpan.MaxValue.Ticks;
            }
            else if (DoubleUtil.AreClose(dbl.Value, TimeSpan.MinValue.Ticks))
            {
                ticks = TimeSpan.MinValue.Ticks;
            }
            else
            {
                if (ticks < TimeSpan.MinValue.Ticks)
                {
                    return res;
                }
                if (ticks > TimeSpan.MaxValue.Ticks)
                {
                    return res;
                }
            }

            var timeSpan = new TimeSpan(ticks);

            if (parameter == null)
            {
                res = timeSpan.ToString();
            }
            else
            {
                try
                {
                    res = timeSpan.ToString(parameter.ToString(), culture);
                }
                catch (FormatException)
                {
                    res = LocalizationResource.BadTimeSpanToStringFormat;
                }
            }

            return res;
        }

        /// <summary>
        /// Converts <c>string</c> to <see cref="TimeSpan.Ticks"/> as <c>double</c>
        /// </summary>
        /// <param name="value">Source string</param>
        /// <param name="targetType">Target type (ignores)</param>
        /// <param name="parameter">Convertion parameter (ignores)</param>
        /// <param name="culture">Culture</param>
        /// <returns><see cref="TimeSpan"/>,
        /// or <c>null</c>, if <paramref name="value"/> is empty (or whitespace),
        /// or <see cref="DependencyProperty.UnsetValue"/>, if <paramref name="value"/> contains incorrect string</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            string s = value.ToString();
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            TimeSpan tm;
            if (TimeSpan.TryParse(s, out tm))
            {
                return (double)tm.Ticks;
            }

            return DependencyProperty.UnsetValue;
        }

        #endregion
    }

    #endregion

    #region DateTime converters

    /// <summary>
    /// Converter from nullable <see cref="DateTime"/> to <c>string</c> and vice-versa.
    /// Also provides a rule in order to check the validity of user input.
    /// </summary>
    public class NullableDateTimeToStringConverter : ValidationRule, IValueConverter
    {
        internal static ValidationResult Validate(IValueConverter converter, object value, CultureInfo cultureInfo)
        {
            Contract.Requires<ArgumentNullException>(converter != null);

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(true, null);
            }

            return new ValidationResult(
                converter.ConvertBack(value, typeof(DateTime?), null, cultureInfo) != DependencyProperty.UnsetValue,
                LocalizationResource.DateTimeValidationRuleMsg);
        }

        /// <summary>
        /// Performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <see cref="System.Windows.Controls.ValidationResult"/> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Validate(this, value, cultureInfo);
        }

        #region IValueConverter Members

        /// <summary>
        /// Convert a value of type <see cref="DateTime"/>? to <c>string</c>.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> produced by the binding source.</param>
        /// <param name="targetType"> The type of the binding target property (ignores).</param>
        /// <param name="parameter">The converter parameter to use: format string.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>String representation of date, or <c>null</c> if <paramref name="value"/> is not date.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string res = string.Empty;
            var dt = value as DateTime?;
            if (dt.HasValue)
            {
                if (parameter == null)
                {
                    res = dt.Value.ToString(culture);
                }
                else
                {
                    try
                    {
                        res = dt.Value.ToString(parameter.ToString(), culture);
                    }
                    catch (FormatException)
                    {
                        res = LocalizationResource.BadDateTimeToStringFormat;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Convert a <c>string</c> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The string that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to (ignores).</param>
        /// <param name="parameter">The converter parameter to use (ignores).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Instance of <see cref="DateTime"/>,
        /// or <c>null</c> if <paramref name="value"/> is <c>null</c>,
        /// or <see cref="DependencyProperty.UnsetValue"/> if <paramref name="value"/> has incorrect value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            string s = value.ToString();
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            DateTime dt;
            if (DateTime.TryParse(s, out dt))
            {
                return dt;
            }

            return DependencyProperty.UnsetValue;
        }

        #endregion
    }

    /// <summary>
    /// Converter from nullable <see cref="DateTime"/> to <c>double</c> and vice-versa.
    /// Also provides a rule in order to check the validity of user input.
    /// </summary>
    public class NullableDateTimeToDoubleConverter : ValidationRule, IValueConverter
    {
        /// <summary>
        /// Performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <see cref="System.Windows.Controls.ValidationResult"/> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return NullableDateTimeToStringConverter.Validate(this, value, cultureInfo);
        }

        #region IValueConverter Members

        /// <summary>
        /// Convert a value of type <see cref="DateTime"/>? to <c>double</c>.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> produced by the binding source.</param>
        /// <param name="targetType"> The type of the binding target property (ignores).</param>
        /// <param name="parameter">The converter parameter to use (ignores).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns><see cref="double"/> representation of date (number of ticks),
        /// or <c>null</c> if <paramref name="value"/> is not date.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            double? res = null;
            var dt = value as DateTime?;
            if (dt.HasValue)
            {
                res = dt.Value.Ticks;
            }
            return res;
        }

        /// <summary>
        /// Convert a number to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">The number that is produced by the binding target.
        /// Now supports only <see cref="double"/> numbers.</param>
        /// <param name="targetType">The type to convert to (ignores).</param>
        /// <param name="parameter">The converter parameter to use (ignores).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Instance of <see cref="DateTime"/>,
        /// or <c>null</c> if <paramref name="value"/> is <c>null</c> os is not <see cref="double"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is double))
            {
                return null;
            }

            var dbl = (double)value;
            var longTicks = (long) dbl;

            if (longTicks < DateTime.MinValue.Ticks)
            {
                return DateTime.MinValue;
            }

            if (longTicks > DateTime.MaxValue.Ticks)
            {
                return DateTime.MaxValue;
            }

            Contract.Assume(longTicks >= 0 && longTicks <= 0x2bca2875f4373fffL); // code contracts cant recognize DateTime.MaxValue.Ticks and DateTime.MinValue.Ticks below
            return new DateTime(longTicks);
        }

        #endregion
    }

    #endregion

    #region TimeSpan converters

    /// <summary>
    /// Converter from nullable <see cref="TimeSpan"/> to <c>string</c> and vice-versa.
    /// Also provides a rule in order to check the validity of user input.
    /// </summary>
    public class NullableTimeSpanToStringConverter : ValidationRule, IValueConverter
    {
        internal static ValidationResult Validate(IValueConverter converter, object value, CultureInfo cultureInfo)
        {
            Contract.Requires<ArgumentNullException>(converter != null);

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(true, null);
            }

            return new ValidationResult(
                converter.ConvertBack(value, typeof(TimeSpan?), null, cultureInfo) != DependencyProperty.UnsetValue,
                LocalizationResource.TimeSpanValidationRuleMsg);
        }

        /// <summary>
        /// Performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <see cref="System.Windows.Controls.ValidationResult"/> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return Validate(this, value, cultureInfo);
        }

        #region IValueConverter Members

        /// <summary>
        /// Convert a value of type <see cref="TimeSpan"/>? to <c>string</c>.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> produced by the binding source.</param>
        /// <param name="targetType"> The type of the binding target property (ignores).</param>
        /// <param name="parameter">The converter parameter to use: format string.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>String representation of timespan, or <c>null</c> if <paramref name="value"/> is not timespan.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string res = string.Empty;
            var dt = value as TimeSpan?;
            if (dt.HasValue)
            {
                if (parameter == null)
                {
                    res = dt.Value.ToString();
                }
                else
                {
                    try
                    {
                        res = dt.Value.ToString(parameter.ToString(), culture);
                    }
                    catch (FormatException)
                    {
                        res = LocalizationResource.BadTimeSpanToStringFormat;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Convert a <c>string</c> to <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The string that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to (ignores).</param>
        /// <param name="parameter">The converter parameter to use (ignores).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Instance of <see cref="TimeSpan"/>,
        /// or <c>null</c> if <paramref name="value"/> is <c>null</c>,
        /// or <see cref="DependencyProperty.UnsetValue"/> if <paramref name="value"/> has incorrect value.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            string s = value.ToString();
            if (string.IsNullOrWhiteSpace(s))
            {
                return null;
            }

            TimeSpan tm;
            if (TimeSpan.TryParse(s, out tm))
            {
                return tm;
            }

            return DependencyProperty.UnsetValue;
        }

        #endregion
    }

    /// <summary>
    /// Converter from nullable <see cref="TimeSpan"/> to <c>double</c> and vice-versa.
    /// Also provides a rule in order to check the validity of user input.
    /// </summary>
    public class NullableTimeSpanToDoubleConverter : ValidationRule, IValueConverter
    {
        /// <summary>
        /// Performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The culture to use in this rule.</param>
        /// <returns>A <see cref="System.Windows.Controls.ValidationResult"/> object.</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return NullableTimeSpanToStringConverter.Validate(this, value, cultureInfo);
        }

        #region IValueConverter Members

        /// <summary>
        /// Convert a value of type <see cref="TimeSpan"/>? to <c>double</c>.
        /// </summary>
        /// <param name="value">The <see cref="TimeSpan"/> produced by the binding source.</param>
        /// <param name="targetType"> The type of the binding target property (ignores).</param>
        /// <param name="parameter">The converter parameter to use (ignores).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns><see cref="double"/> representation of timespan (number of ticks),
        /// or <c>null</c> if <paramref name="value"/> is not timespan.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            double? res = null;
            var dt = value as TimeSpan?;
            if (dt.HasValue)
            {
                res = dt.Value.Ticks;
            }
            return res;
        }

        /// <summary>
        /// Convert a number to <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">The number that is produced by the binding target.
        /// Now supports only <see cref="double"/> numbers.</param>
        /// <param name="targetType">The type to convert to (ignores).</param>
        /// <param name="parameter">The converter parameter to use (ignores).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Instance of <see cref="DateTime"/>,
        /// or <c>null</c> if <paramref name="value"/> is <c>null</c> or is not <see cref="double"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is double))
            {
                return null;
            }

            var dbl = (double)value;

            return TimeSpan.FromTicks((long)dbl);
        }

        #endregion
    }

    /// <summary>
    /// Converter from <c>long?</c> to <see cref="TimeSpan"/>
    /// </summary>
    public class NullableInt64ToTimeSpanConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts <c>long?</c> to <see cref="TimeSpan"/>
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Target type (ignores)</param>
        /// <param name="parameter">Conversion parameter: type of <paramref name="value"/>
        /// "D" - days,
        /// "H" - hours,
        /// "M" - minutes,
        /// "S" - seconds,
        /// "MS" - milliseconds,
        /// "T" - ticks
        /// </param>
        /// <param name="culture">Culture</param>
        /// <returns><see cref="TimeSpan"/> or <see cref="DependencyProperty.UnsetValue"/>,
        /// if <paramref name="value"/> has incorrect value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var longValue = value as long?;
            if (!longValue.HasValue)
            {
                var intValue = value as int?;
                if (!intValue.HasValue)
                {
                    return DependencyProperty.UnsetValue;                    
                }
                longValue = intValue.Value;
            }

            string fromType = parameter == null ? "D" : parameter.ToString();

            switch (fromType)
            {
                case "D":
                    return TimeSpan.FromDays(longValue.Value);
                case "H":
                    return TimeSpan.FromHours(longValue.Value);
                case "M":
                    return TimeSpan.FromMinutes(longValue.Value);
                case "S":
                    return TimeSpan.FromSeconds(longValue.Value);
                case "MS":
                    return TimeSpan.FromMilliseconds(longValue.Value);
                case "T":
                    return TimeSpan.FromTicks(longValue.Value);
            }
            return DependencyProperty.UnsetValue;
        }

        /// <summary>
        /// Converts <see cref="TimeSpan"/> to <c>long</c>
        /// </summary>
        /// <param name="value">Time span</param>
        /// <param name="targetType">Target type (ignores)</param>
        /// <param name="parameter">Conversion parameter (ignores)</param>
        /// <param name="culture">Culture</param>
        /// <returns><see cref="TimeSpan.Ticks"/></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var ts = value as TimeSpan?;
            if (!ts.HasValue)
            {
                return DependencyProperty.UnsetValue;
            }

            return ts.Value.Ticks;
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// Converter from array of numbers to <see cref="Thickness"/> and vice-versa.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
    public sealed class ThicknessMultiConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Converts source values to a value for the binding target of <see cref="Thickness"/> type.
        /// The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="System.Windows.Data.MultiBinding"/> produces.
        /// Each of values must be convertible to <see cref="double"/> using <see cref="System.Convert.ToDouble(object,IFormatProvider)"/>.</param>
        /// <param name="targetType">The type of the binding target property (ignores).</param>
        /// <param name="parameter">The converter parameter to use (ignores).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value as <see cref="Thickness"/>(<paramref name="values"/>[0], <paramref name="values"/>[1], <paramref name="values"/>[2], <paramref name="values"/>[3]).
        /// If received array <paramref name="values"/> contains not enough values (its length is less then 4),
        /// then lacking values adopt as zeroes (0.0).
        /// If length of received array is greater then 4, redundant items ignored.
        /// 
        /// A return value of <see cref="System.Windows.DependencyProperty.UnsetValue"/> indicates
        /// that the binding does not transfer the correct value for conversion.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return new Thickness();
            }

            int len = values.Length;

            double[] convertedValues = {0.0, 0.0, 0.0, 0.0}; // fill with default values
            for (int i = 0; i < 4; i++)
            {
                int expectedLen = i + 1;
                if (len >= expectedLen)
                {
                    try
                    {
                        // replace default value by converted value
                        convertedValues[i] = System.Convert.ToDouble(values[i], culture);
                    }
                    catch (FormatException)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                    catch (InvalidCastException)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                    catch (OverflowException)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                }
            }

            return new Thickness(convertedValues[0], convertedValues[1], convertedValues[2], convertedValues[3]);
        }

        /// <summary>
        /// Converts a binding target value of type <see cref="Thickness"/> to the source binding values of type <c>double[]</c>.
        /// </summary>
        /// <param name="value">The value of type <see cref="Thickness"/> that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to (ignores).</param>
        /// <param name="parameter">The converter parameter to use (ignores).</param>
        /// <param name="culture">The culture to use in the converter (ignores).</param>
        /// <returns>An array of sides lengths of <see cref="Thickness"/>
        /// that have been converted from the target value back to the source values.
        /// This is array of 4 elements: left, top, right and bottom size length.
        /// 
        /// A return value of [<see cref="System.Windows.DependencyProperty.UnsetValue"/>] indicates
        /// that the binding does not transfer the correct value for conversion (i.e. <c>null</c> or not <see cref="Thickness"/>).</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Thickness))
            {
                return new[] {DependencyProperty.UnsetValue};
            }

            var thickness = (Thickness)value;
            return new object[] { thickness.Left, thickness.Top, thickness.Right, thickness.Bottom };
        }

        #endregion
    }

    /// <summary>
    /// Converts from <see cref="SolidColorBrush"/> to <see cref="Color"/>
    /// </summary>
    public class SolidColorBrushToColorConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts from <see cref="SolidColorBrush"/> to <see cref="Color"/>
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Target type (ignores)</param>
        /// <param name="parameter">Conversion parameter (ignores)</param>
        /// <param name="culture">Culture (ignores)</param>
        /// <returns><see cref="Color"/> or <see cref="DependencyProperty.UnsetValue"/>,
        /// if <paramref name="value"/> has incorrect value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var brush = value as SolidColorBrush;
            if (brush == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return brush.Color;
        }

        /// <summary>
        /// Converts <see cref="Color"/> to <see cref="SolidColorBrush"/>
        /// </summary>
        /// <remarks>This method every time constructs new <see cref="SolidColorBrush"/></remarks>
        /// <param name="value">Color</param>
        /// <param name="targetType">Target type (ignores)</param>
        /// <param name="parameter">Conversion parameter (ignores)</param>
        /// <param name="culture">Culture (ignores)</param>
        /// <returns><see cref="SolidColorBrush"/> or <see cref="DependencyProperty.UnsetValue"/></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var color = value as Color?;
            if (!color.HasValue)
            {
                return DependencyProperty.UnsetValue;
            }

            return new SolidColorBrush(color.Value);
        }

        #endregion
    }
}
