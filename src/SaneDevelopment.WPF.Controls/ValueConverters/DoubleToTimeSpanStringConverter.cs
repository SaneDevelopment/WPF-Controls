// -----------------------------------------------------------------------
// <copyright file="DoubleToTimeSpanStringConverter.cs" company="Sane Development">
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
    using System.Windows.Data;
    using SaneDevelopment.WPF.Controls.Properties;

    /// <summary>
    /// Converter from <c>double</c> to <c>string</c> representation of <see cref="TimeSpan"/>,
    /// where <c>double</c> value represents <see cref="System.TimeSpan.Ticks"/> value.
    /// </summary>
    public class DoubleToTimeSpanStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts <see cref="System.TimeSpan.Ticks"/> to <c>string</c> representation of <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">Time span ticks.</param>
        /// <param name="targetType">Target type (ignores).</param>
        /// <param name="parameter">Convertion parameter: time span format.</param>
        /// <param name="culture">Culture.</param>
        /// <returns>String representation of time span, or <c>null</c>, if <paramref name="value"/> is not <c>double</c>.</returns>
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
        /// Converts <c>string</c> to <see cref="TimeSpan.Ticks"/> as <c>double</c>.
        /// </summary>
        /// <param name="value">Source string.</param>
        /// <param name="targetType">Target type (ignores).</param>
        /// <param name="parameter">Convertion parameter (ignores).</param>
        /// <param name="culture">Culture.</param>
        /// <returns><see cref="TimeSpan"/>,
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

            TimeSpan tm;
            if (TimeSpan.TryParse(s, out tm))
            {
                return (double)tm.Ticks;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
