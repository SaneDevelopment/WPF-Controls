// -----------------------------------------------------------------------
// <copyright file="NegateDoubleConverter.cs" company="Sane Development">
//
// Sane Development WPF Controls Library Samples.
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

namespace SaneDevelopment.WPF.Controls.Samples
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Convert double value to negate value.
    /// </summary>
    public sealed class NegateDoubleConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value == null)
            {
                return Binding.DoNothing;
            }

            return -1.0 * System.Convert.ToDouble(value, CultureInfo.CurrentCulture);
        }

        /// <inheritdoc/>
        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }
    }
}
