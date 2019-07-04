// -----------------------------------------------------------------------
// <copyright file="IRanged.cs" company="Sane Development">
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
    /// Describes classes of interval (ranged) objects.
    /// </summary>
    /// <typeparam name="T">Value type.</typeparam>
    /// <typeparam name="TInterval">Interval (distance) type.</typeparam>
    public interface IRanged<T, TInterval>
    {
        /// <summary>
        /// Gets minimum available value.
        /// </summary>
        /// <value>Minimum available value.</value>
        T Minimum { get; }

        /// <summary>
        /// Gets maximum available value.
        /// </summary>
        /// <value>Maximum available value.</value>
        T Maximum { get; }

        /// <summary>
        /// Gets start interval value.
        /// </summary>
        /// <value>Start interval value.</value>
        T StartValue { get; }

        /// <summary>
        /// Gets end interval value.
        /// </summary>
        /// <value>End interval value.</value>
        T EndValue { get; }

        /// <summary>
        /// Gets minimum available interval (range) value.
        /// </summary>
        /// <value>Minimum available interval (range) value.</value>
        TInterval MinRangeValue { get; }

        /// <summary>
        /// Gets a value indicating whether gets the indicator that this object behaves like a single value object, i.e. start value equals to end value,
        /// so interval (range) value equals to zero.
        /// </summary>
        /// <value>A value indicating whether gets the indicator that this object behaves like a single value object.</value>
        bool IsSingleValue { get; }

        /// <summary>
        /// Method for converting <c>double</c> to value type.
        /// </summary>
        /// <param name="value">Value to convert from.</param>
        /// <returns>Value of type <typeparamref name="T"/>.</returns>
        T DoubleToValue(double value);

        /// <summary>
        /// Method for converting value type to <c>double</c>.
        /// </summary>
        /// <param name="value">Value of type <typeparamref name="T"/>.</param>
        /// <returns><c>double</c> value, converted from <paramref name="value"/>.</returns>
        double ValueToDouble(T value);

        /// <summary>
        /// Method for converting <c>double</c> to interval type.
        /// </summary>
        /// <param name="value">Value to convert from.</param>
        /// <returns>Value of type <typeparamref name="TInterval"/>.</returns>
        TInterval DoubleToInterval(double value);

        /// <summary>
        /// Method for converting interval value to <c>double</c>.
        /// </summary>
        /// <param name="value">Value of type <typeparamref name="TInterval"/>.</param>
        /// <returns><c>double</c> value, converted from <paramref name="value"/>.</returns>
        double IntervalToDouble(TInterval value);
    }
}
