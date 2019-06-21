// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyPropertyUtil.cs" company="Sane Development">
//
//   Sane Development WPF Controls Library
//
//   The BSD 3-Clause License
//
//   Copyright (c) Sane Development
//   All rights reserved.
//
//   Redistribution and use in source and binary forms, with or without modification,
//   are permitted provided that the following conditions are met:
//
//   - Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//   - Redistributions in binary form must reproduce the above copyright notice,
//     this list of conditions and the following disclaimer in the documentation
//     and/or other materials provided with the distribution.
//   - Neither the name of the Sane Development nor the names of its contributors
//     may be used to endorse or promote products derived from this software
//     without specific prior written permission.
//
//   THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//   AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
//   THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//   ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS
//   BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
//   OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//   PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
//   OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
//   WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//   ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
//   EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Provides bunch of usefull handy methods for work with dependency properties
    /// of controls in this library.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Util")]
    public static class DependencyPropertyUtil
    {
        /// <summary>
        /// Minimum value of the <c>AutoToolTipPrecision</c> property
        /// </summary>
        public static int MinimumAutoToolTipPrecision
        {
            get { return 0; }
        }

        /// <summary>
        /// Maximum value of the <c>AutoToolTipPrecision</c> property
        /// </summary>
        public static int MaximumAutoToolTipPrecision
        {
            get { return 99; }
        }

        internal static double ExtractDouble(object value, double defaultValue)
        {
            if (value == null || !(value is double))
            {
                return defaultValue;
            }
            var newValue = (double)value;
            if (double.IsNaN(newValue) || double.IsInfinity(newValue))
            {
                newValue = defaultValue;
            }
            return newValue;
        }

        internal static double ConvertToDouble(object value, double defaultValue)
        {
            double newValue = defaultValue;
            if (value != null)
            {
                try
                {
                    newValue = Convert.ToDouble(value, CultureInfo.CurrentCulture);
                }
                catch (FormatException)
                { }
                catch (InvalidCastException)
                { }
                catch (OverflowException)
                { }
            }
            return newValue;
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="System.Double"/> value.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns><c>true</c> if <paramref name="value"/> can be cast to <c>double</c> and is valid value (i.e. not Nan and not infinity).</returns>
        public static bool IsValidDoubleValue(object value)
        {
            Contract.Ensures(value != null || !Contract.Result<bool>());

            if (value == null)
                return false;

            double num;
            try
            {
                num = (double)value;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            return (!double.IsNaN(num) && !double.IsInfinity(num));
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="System.DateTime"/> value.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns><c>true</c> if <paramref name="value"/> is <see cref="DateTime"/>; otherwise, <c>false</c>.</returns>
        public static bool IsValidDateTimeValue(object value)
        {
            return value is DateTime;
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="System.Boolean"/> value.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns><c>true</c> if <paramref name="value"/> is <see cref="System.Boolean"/>; otherwise, <c>false</c>.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bool")]
        public static bool IsValidBoolValue(object value)
        {
            return value is bool;
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="System.TimeSpan"/> value.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns><c>true</c> if <paramref name="value"/> is <see cref="TimeSpan"/>; otherwise, <c>false</c>.</returns>
        public static bool IsValidTimeSpanValue(object value)
        {
            return value is TimeSpan;
        }

        /// <summary>
        /// Checks whether received value is valid value of type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">Target type</param>
        /// <param name="value">Value to check</param>
        /// <returns><c>true</c>, if <paramref name="value"/> is valid value of type <paramref name="targetType"/></returns>
        public static bool IsValidValue(Type targetType, object value)
        {
            if (typeof(double).IsAssignableFrom(targetType))
            {
                return IsValidDoubleValue(value);
            }
            if (typeof(DateTime).IsAssignableFrom(targetType))
            {
                return IsValidDateTimeValue(value);
            }
            if (typeof(TimeSpan).IsAssignableFrom(targetType))
            {
                return IsValidTimeSpanValue(value);
            }
            return false;
        }

        /// <summary>
        /// Checks whether received value is valid change (distance) value of type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">Target type</param>
        /// <param name="value">Value to check</param>
        /// <returns><c>true</c> if <paramref name="value"/> id valid change value of type <paramref name="targetType"/></returns>
        public static bool IsValidChange(Type targetType, object value)
        {
            Contract.Ensures(value != null || !Contract.Result<bool>());

            if (value == null)
                return false;

            if (!IsValidValue(targetType, value))
                return false;

            if (typeof(double).IsAssignableFrom(targetType))
            {
                var val = (double)value;
                return val >= 0.0;
            }
            if (typeof(TimeSpan).IsAssignableFrom(targetType))
            {
                var val = (TimeSpan)value;
                return val >= TimeSpan.Zero;
            }
            return false;
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="Orientation"/> value.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns><c>true</c> if <paramref name="value"/> is valid <see cref="Orientation"/>; otherwise, <c>false</c>.</returns>
        public static bool IsValidOrientation(object value)
        {
            if (!(value is Orientation))
                return false;

            var orientation = (Orientation)value;
            return (orientation == Orientation.Horizontal) || (orientation == Orientation.Vertical);
        }

        /// <summary>
        /// Checks whether received value is valid value of floating-point number precision for auto tooltip view.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns><c>true</c> if <paramref name="value"/> is valid value of floating-point number precision; otherwise, <c>false</c>.</returns>
        public static bool IsValidAutoToolTipPrecision(object value)
        {
            if (!(value is int))
            {
                return false;
            }
            var intVal = (int) value;
            return intVal >= MinimumAutoToolTipPrecision && intVal <= MaximumAutoToolTipPrecision;
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="AutoToolTipPlacement"/> value.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns><c>true</c> if <paramref name="value"/> is valid <see cref="AutoToolTipPlacement"/>; otherwise, <c>false</c>.</returns>
        public static bool IsValidAutoToolTipPlacement(object value)
        {
            if (!(value is AutoToolTipPlacement))
            {
                return false;
            }
            var placement = (AutoToolTipPlacement)value;
            return placement == AutoToolTipPlacement.None ||
                   placement == AutoToolTipPlacement.TopLeft ||
                   placement == AutoToolTipPlacement.BottomRight;
        }

        /// <summary>
        /// Method performs coercing of range start value.
        /// Used ranged controls rules: e.g. start value must be greater or equal to minimum value
        /// and must be less or equal to end value or maximum value.
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <typeparam name="TInterval">Interval type</typeparam>
        /// <param name="ranged">Ranged object</param>
        /// <param name="value">Value to coerce</param>
        /// <returns><paramref name="value"/>, or coerced value</returns>
        public static object CoerceRangeStartValue<T, TInterval>(IRanged<T, TInterval> ranged, T value)
        {
            if (ranged == null) return value;

            double? coercedValue = null;
            double doubleValue = ranged.ValueToDouble(value);

            // the order of validations below is important
            // at first, validate range (interval) value,
            // because its coercing to minimum value can damage other more important limitations.
            double endValue = ranged.ValueToDouble(ranged.EndValue);
            double minRangeValue = ranged.IntervalToDouble(ranged.MinRangeValue);
            double newRangeValue = endValue - doubleValue;
            bool isSingleValue = ranged.IsSingleValue;
            if (!isSingleValue && DoubleUtil.LessThan(newRangeValue, minRangeValue))
            {
                coercedValue = doubleValue = endValue - minRangeValue;
            }

            double minimum = ranged.ValueToDouble(ranged.Minimum);
            if (doubleValue < minimum)
            {
                coercedValue = doubleValue = minimum;
            }

            if (!isSingleValue && doubleValue > endValue)
            {
                coercedValue = endValue;
            }

            if (coercedValue.HasValue)
            {
                return ranged.DoubleToValue(coercedValue.Value);
            }
            return value;
        }

        /// <summary>
        /// Method performs coercing of range end value.
        /// Used ranged controls rules: e.g. end value must be greater or equal to start value or minimum value
        /// and must be less or equal to maximum value.
        /// </summary>
        /// <typeparam name="T">Value type</typeparam>
        /// <typeparam name="TInterval">Interval type</typeparam>
        /// <param name="ranged">Ranged object</param>
        /// <param name="value">Value to coerce</param>
        /// <returns><paramref name="value"/>, or coerced value</returns>
        public static object CoerceRangeEndValue<T, TInterval>(IRanged<T, TInterval> ranged, T value)
        {
            if (ranged == null) return value;

            double? coercedValue = null;
            double doubleValue = ranged.ValueToDouble(value);

            // the order of validations below is important
            // at first, validate range (interval) value,
            // because its coercing to minimum value can damage other more important limitations.
            double startValue = ranged.ValueToDouble(ranged.StartValue);
            double minRangeValue = ranged.IntervalToDouble(ranged.MinRangeValue);
            double newRangeValue = doubleValue - startValue;
            bool isSingleValue = ranged.IsSingleValue;
            if (!isSingleValue && DoubleUtil.LessThan(newRangeValue, minRangeValue))
            {
                coercedValue = doubleValue = minRangeValue + startValue;
            }

            double maximum = ranged.ValueToDouble(ranged.Maximum);
            if (doubleValue > maximum)
            {
                coercedValue = doubleValue = maximum;
            }

            if (!isSingleValue && doubleValue < startValue)
            {
                coercedValue = startValue;
            }

            if (coercedValue.HasValue)
            {
                return ranged.DoubleToValue(coercedValue.Value);
            }
            return value;
        }
    }
}
