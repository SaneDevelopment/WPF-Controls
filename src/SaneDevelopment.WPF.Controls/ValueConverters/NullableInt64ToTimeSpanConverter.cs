// -----------------------------------------------------------------------
// <copyright file="NullableInt64ToTimeSpanConverter.cs" company="Sane Development">
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

    /// <summary>
    /// Converter from <c>long?</c> to <see cref="TimeSpan"/>.
    /// </summary>
    public class NullableInt64ToTimeSpanConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts <c>long?</c> to <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Target type (ignores).</param>
        /// <param name="parameter">Conversion parameter: type of <paramref name="value"/>
        /// "D" - days,
        /// "H" - hours,
        /// "M" - minutes,
        /// "S" - seconds,
        /// "MS" - milliseconds,
        /// "T" - ticks.
        /// </param>
        /// <param name="culture">Culture.</param>
        /// <returns><see cref="TimeSpan"/> or <see cref="DependencyProperty.UnsetValue"/>,
        /// if <paramref name="value"/> has incorrect value.</returns>
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
        /// Converts <see cref="TimeSpan"/> to <c>long</c>.
        /// </summary>
        /// <param name="value">Time span.</param>
        /// <param name="targetType">Target type (ignores).</param>
        /// <param name="parameter">Conversion parameter (ignores).</param>
        /// <param name="culture">Culture.</param>
        /// <returns><see cref="TimeSpan.Ticks"/>.</returns>
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
}
