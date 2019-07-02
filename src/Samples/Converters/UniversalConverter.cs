// -----------------------------------------------------------------------
// <copyright file="UniversalConverter.cs" company="Sane Development">
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
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Universal converter for any type using <see cref="TypeConverter"/>.
    /// </summary>
    public sealed class UniversalConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (targetType == null || value == null)
            {
                return null;
            }

            object res = DependencyProperty.UnsetValue;

            TypeConverter converter = TypeDescriptor.GetConverter(targetType);

            try
            {
                if (converter.CanConvertFrom(value.GetType()))
                {
                    res = converter.ConvertFrom(value);
                }
                else if (converter.CanConvertFrom(typeof(string)))
                {
                    res = converter.ConvertFrom(value.ToString());
                }
            }
            catch (FormatException)
            {
            }

            return res;
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
