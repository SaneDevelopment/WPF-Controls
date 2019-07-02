// -----------------------------------------------------------------------
// <copyright file="NullableTimeSpanToDoubleConverter.cs" company="Sane Development">
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
    using System.Windows.Controls;
    using System.Windows.Data;

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
}
