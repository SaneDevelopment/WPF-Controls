// -----------------------------------------------------------------------
// <copyright file="NullableTimeSpanToStringConverter.cs" company="Sane Development">
//
// Sane Development WPF Controls Library.
//
// The BSD 3-Clause License.
//
// Copyright (c) Sane Development.
// All rights reserved.
//
// See LICENSE file for full license information.
//
// </copyright>
// -----------------------------------------------------------------------

namespace SaneDevelopment.WPF.Controls
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using SaneDevelopment.WPF.Controls.Properties;

    /// <summary>
    /// Converter from nullable <see cref="TimeSpan"/> to <c>string</c> and vice-versa.
    /// Also provides a rule in order to check the validity of user input.
    /// </summary>
    public class NullableTimeSpanToStringConverter : ValidationRule, IValueConverter
    {
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

            if (TimeSpan.TryParse(s, out TimeSpan tm))
            {
                return tm;
            }

            return DependencyProperty.UnsetValue;
        }

        #endregion

        /// <summary>
        /// Performs validation checks on a value.
        /// </summary>
        /// <param name="converter">Value converter to use.</param>
        /// <param name="value">Value to convert.</param>
        /// <param name="cultureInfo">The culture to use in the converter.</param>
        /// <returns>A <see cref="ValidationResult"/> object.</returns>
        internal static ValidationResult Validate(IValueConverter converter, object value, CultureInfo cultureInfo)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return new ValidationResult(true, null);
            }

            return new ValidationResult(
                converter.ConvertBack(value, typeof(TimeSpan?), null, cultureInfo) != DependencyProperty.UnsetValue,
                LocalizationResource.TimeSpanValidationRuleMsg);
        }
    }
}
