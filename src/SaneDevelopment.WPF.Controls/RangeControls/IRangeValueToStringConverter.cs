// -----------------------------------------------------------------------
// <copyright file="IRangeValueToStringConverter.cs" company="Sane Development">
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
    /// <summary>
    /// Describes operation of conversion some range slider value to string.
    ///
    /// Objects that implement this interface supposed to use for conversion values of <see cref="SimpleRangeSlider{T, TInterval}"/>
    /// into their string representation for showing in UI to user
    /// (e.g. in tooltips etc.)
    /// </summary>
    /// <typeparam name="T">Range slider value type.</typeparam>
    public interface IRangeValueToStringConverter<in T>
    {
        /// <summary>
        /// Converts <paramref name="value"/> to <c>string</c>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="thumbType">Type of the thumb, which value is <paramref name="value"/>.</param>
        /// <param name="parameter">Additional parameter for convertion.</param>
        /// <returns>String representation of <paramref name="value"/>.</returns>
        string Convert(T value, RangeThumbType thumbType, object parameter);
    }
}
