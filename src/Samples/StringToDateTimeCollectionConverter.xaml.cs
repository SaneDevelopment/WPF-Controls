// -----------------------------------------------------------------------
// <copyright file="StringToDateTimeCollectionConverter.xaml.cs" company="Sane Development">
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
    /// Added for purposes to pass concrete <c>targetType</c> to <see cref="UniversalConverter.Convert"/>.
    /// </summary>
    public sealed class StringToDateTimeCollectionConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return new UniversalConverter().Convert(value ?? string.Empty, typeof(DateTimeCollection), parameter, culture);
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
