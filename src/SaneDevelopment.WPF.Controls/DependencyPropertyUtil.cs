// -----------------------------------------------------------------------
// <copyright file="DependencyPropertyUtil.cs" company="Sane Development">
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
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Provides bunch of usefull handy methods for work with dependency properties
    /// of controls in this library.
    /// </summary>
    public static class DependencyPropertyUtil
    {
        /// <summary>
        /// Gets minimum value of the <c>AutoToolTipPrecision</c> property.
        /// </summary>
        /// <value>Minimum value of the <c>AutoToolTipPrecision</c> property.</value>
        public static int MinimumAutoToolTipPrecision
        {
            get { return 0; }
        }

        /// <summary>
        /// Gets maximum value of the <c>AutoToolTipPrecision</c> property.
        /// </summary>
        /// <value>Maximum value of the <c>AutoToolTipPrecision</c> property.</value>
        public static int MaximumAutoToolTipPrecision
        {
            get { return 99; }
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="double"/> value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> can be cast to <c>double</c> and is valid value (i.e. not Nan and not infinity).</returns>
        public static bool IsValidDoubleValue(object value)
        {
            if (value == null)
            {
                return false;
            }

            double num;
            try
            {
                num = (double)value;
            }
            catch (InvalidCastException)
            {
                return false;
            }

            return !double.IsNaN(num) && !double.IsInfinity(num);
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="System.DateTime"/> value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is <see cref="DateTime"/>; otherwise, <c>false</c>.</returns>
        public static bool IsValidDateTimeValue(object value)
        {
            return value is DateTime;
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="bool"/> value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is <see cref="bool"/>; otherwise, <c>false</c>.</returns>
        public static bool IsValidBoolValue(object value)
        {
            return value is bool;
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="System.TimeSpan"/> value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is <see cref="TimeSpan"/>; otherwise, <c>false</c>.</returns>
        public static bool IsValidTimeSpanValue(object value)
        {
            return value is TimeSpan;
        }

        /// <summary>
        /// Checks whether received value is valid value of type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">Target type.</param>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c>, if <paramref name="value"/> is valid value of type <paramref name="targetType"/>.</returns>
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
        /// <param name="targetType">Target type.</param>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> id valid change value of type <paramref name="targetType"/>.</returns>
        public static bool IsValidChange(Type targetType, object value)
        {
            if (value == null)
            {
                return false;
            }

            if (!IsValidValue(targetType, value))
            {
                return false;
            }

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
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is valid <see cref="Orientation"/>;
        /// otherwise, <c>false</c>.</returns>
        public static bool IsValidOrientation(object value)
        {
            if (!(value is Orientation))
            {
                return false;
            }

            var orientation = (Orientation)value;
            return (orientation == Orientation.Horizontal) || (orientation == Orientation.Vertical);
        }

        /// <summary>
        /// Checks whether received value is valid value of floating-point number precision for auto tooltip view.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is valid value of floating-point number precision;
        /// otherwise, <c>false</c>.</returns>
        public static bool IsValidAutoToolTipPrecision(object value)
        {
            if (!(value is int))
            {
                return false;
            }

            var intVal = (int)value;
            return intVal >= MinimumAutoToolTipPrecision && intVal <= MaximumAutoToolTipPrecision;
        }

        /// <summary>
        /// Checks whether received value is valid <see cref="AutoToolTipPlacement"/> value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> is valid <see cref="AutoToolTipPlacement"/>;
        /// otherwise, <c>false</c>.</returns>
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
        /// <typeparam name="T">Value type.</typeparam>
        /// <typeparam name="TInterval">Interval type.</typeparam>
        /// <param name="ranged">Ranged object.</param>
        /// <param name="value">Value to coerce.</param>
        /// <returns><paramref name="value"/>, or coerced value.</returns>
        public static object CoerceRangeStartValue<T, TInterval>(IRanged<T, TInterval> ranged, T value)
        {
            if (ranged == null)
            {
                return value;
            }

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
        /// <typeparam name="T">Value type.</typeparam>
        /// <typeparam name="TInterval">Interval type.</typeparam>
        /// <param name="ranged">Ranged object.</param>
        /// <param name="value">Value to coerce.</param>
        /// <returns><paramref name="value"/>, or coerced value.</returns>
        public static object CoerceRangeEndValue<T, TInterval>(IRanged<T, TInterval> ranged, T value)
        {
            if (ranged == null)
            {
                return value;
            }

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

        /// <summary>
        /// Extract <see cref="double"/> value from <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Double value.</param>
        /// <param name="defaultValue">Value to return if <paramref name="value"/> is not a valid <see cref="double"/>.</param>
        /// <returns><see cref="double"/> value of <paramref name="value"/> or <paramref name="defaultValue"/>.</returns>
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

        /// <summary>
        /// Try to convert <paramref name="value"/> to <see cref="double"/> using <see cref="Convert.ToDouble(object,IFormatProvider)"/>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="defaultValue">Value to return if conversion failed.</param>
        /// <returns><paramref name="value"/> converted to <see cref="double"/> or <paramref name="defaultValue"/>.</returns>
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
                {
                }
                catch (InvalidCastException)
                {
                }
                catch (OverflowException)
                {
                }
            }

            return newValue;
        }
    }
}
