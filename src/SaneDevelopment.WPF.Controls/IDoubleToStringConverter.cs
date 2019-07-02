// -----------------------------------------------------------------------
// <copyright file="IDoubleToStringConverter.cs" company="Sane Development">
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
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Interface for converting <c>double</c> value to <c>string</c>.
    /// Classes which implements this interface uses for converting <c>double</c> tick values in <see cref="TickBar"/> control
    /// to <c>string</c> representations for showing in UI.
    /// </summary>
    public interface IDoubleToStringConverter
    {
        /// <summary>
        /// Convert <paramref name="value"/> to <c>string</c>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="parameter">Additional parameter for conversion.</param>
        /// <returns>String representation of <paramref name="value"/>.</returns>
        string Convert(double value, object parameter);
    }
}
