// -----------------------------------------------------------------------
// <copyright file="SolidColorBrushToColorConverter.cs" company="Sane Development">
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
    using System.Windows.Media;

    /// <summary>
    /// Converts from <see cref="SolidColorBrush"/> to <see cref="Color"/>.
    /// </summary>
    public class SolidColorBrushToColorConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts from <see cref="SolidColorBrush"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Target type (ignores).</param>
        /// <param name="parameter">Conversion parameter (ignores).</param>
        /// <param name="culture">Culture (ignores).</param>
        /// <returns><see cref="Color"/> or <see cref="DependencyProperty.UnsetValue"/>,
        /// if <paramref name="value"/> has incorrect value.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (!(value is SolidColorBrush brush))
            {
                return DependencyProperty.UnsetValue;
            }

            return brush.Color;
        }

        /// <summary>
        /// Converts <see cref="Color"/> to <see cref="SolidColorBrush"/>.
        /// </summary>
        /// <remarks>This method every time constructs new <see cref="SolidColorBrush"/>.</remarks>
        /// <param name="value">Color.</param>
        /// <param name="targetType">Target type (ignores).</param>
        /// <param name="parameter">Conversion parameter (ignores).</param>
        /// <param name="culture">Culture (ignores).</param>
        /// <returns><see cref="SolidColorBrush"/> or <see cref="DependencyProperty.UnsetValue"/>.</returns>
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
