// -----------------------------------------------------------------------
// <copyright file="DoubleToDateTimeStringConverter.cs" company="Sane Development">
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
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using SaneDevelopment.WPF.Controls.Properties;

    /// <summary>
    /// Converter from <c>double</c> to <c>string</c> representation of <see cref="DateTime"/>,
    /// where <c>double</c> value represents <see cref="System.DateTime.Ticks"/> value.
    /// </summary>
    public class DoubleToDateTimeStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts <see cref="System.DateTime.Ticks"/> to <c>string</c> representation of <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">Date time ticks.</param>
        /// <param name="targetType">Target type (ignores).</param>
        /// <param name="parameter">Convertion parameter: date time format.</param>
        /// <param name="culture">Culture.</param>
        /// <returns>String representation of date, or <c>null</c>, if <paramref name="value"/> is not <c>double</c>.</returns>
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

            var ticks = (long)dbl;
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

            Debug.Assert(ticks <= 0x2bca2875f4373fffL, "ticks <= 0x2bca2875f4373fffL"); // DateTime.MaxValue.Ticks
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
        /// Converts <c>string</c> to <see cref="DateTime.Ticks"/> as <c>double</c>.
        /// </summary>
        /// <param name="value">Source string.</param>
        /// <param name="targetType">Target type (ignores).</param>
        /// <param name="parameter">Convertion parameter (ignores).</param>
        /// <param name="culture">Culture.</param>
        /// <returns><see cref="DateTime"/>,
        /// or <c>null</c>, if <paramref name="value"/> is empty (or whitespace),
        /// or <see cref="DependencyProperty.UnsetValue"/>, if <paramref name="value"/> contains incorrect string.</returns>
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

            if (DateTime.TryParse(s, out DateTime dt))
            {
                return (double)dt.Ticks;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
