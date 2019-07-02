// -----------------------------------------------------------------------
// <copyright file="ThicknessMultiConverter.cs" company="Sane Development">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converter from array of numbers to <see cref="Thickness"/> and vice-versa.
    /// </summary>
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1704:IdentifiersShouldBeSpelledCorrectly",
        MessageId = "Multi",
        Justification = "Do similar to IMultiValueConverter")]
    public sealed class ThicknessMultiConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Converts source values to a value for the binding target of <see cref="Thickness"/> type.
        /// The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="System.Windows.Data.MultiBinding"/> produces.
        /// Each of values must be convertible to <see cref="double"/> using <see cref="System.Convert.ToDouble(object,IFormatProvider)"/>.</param>
        /// <param name="targetType">The type of the binding target property (ignores).</param>
        /// <param name="parameter">The converter parameter to use (ignores).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value as <see cref="Thickness"/>(<paramref name="values"/>[0], <paramref name="values"/>[1], <paramref name="values"/>[2], <paramref name="values"/>[3]).
        /// If received array <paramref name="values"/> contains not enough values (its length is less then 4),
        /// then lacking values adopt as zeroes (0.0).
        /// If length of received array is greater then 4, redundant items ignored.
        ///
        /// A return value of <see cref="System.Windows.DependencyProperty.UnsetValue"/> indicates
        /// that the binding does not transfer the correct value for conversion.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null)
            {
                return default(Thickness);
            }

            int len = values.Length;

            double[] convertedValues = { 0.0, 0.0, 0.0, 0.0 }; // fill with default values
            for (int i = 0; i < 4; i++)
            {
                int expectedLen = i + 1;
                if (len >= expectedLen)
                {
                    try
                    {
                        // replace default value by converted value
                        convertedValues[i] = System.Convert.ToDouble(values[i], culture);
                    }
                    catch (FormatException)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                    catch (InvalidCastException)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                    catch (OverflowException)
                    {
                        return DependencyProperty.UnsetValue;
                    }
                }
            }

            return new Thickness(convertedValues[0], convertedValues[1], convertedValues[2], convertedValues[3]);
        }

        /// <summary>
        /// Converts a binding target value of type <see cref="Thickness"/> to the source binding values of type <c>double[]</c>.
        /// </summary>
        /// <param name="value">The value of type <see cref="Thickness"/> that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to (ignores).</param>
        /// <param name="parameter">The converter parameter to use (ignores).</param>
        /// <param name="culture">The culture to use in the converter (ignores).</param>
        /// <returns>An array of sides lengths of <see cref="Thickness"/>
        /// that have been converted from the target value back to the source values.
        /// This is array of 4 elements: left, top, right and bottom size length.
        ///
        /// A return value of [<see cref="System.Windows.DependencyProperty.UnsetValue"/>] indicates
        /// that the binding does not transfer the correct value for conversion (i.e. <c>null</c> or not <see cref="Thickness"/>).</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is Thickness))
            {
                return new[] { DependencyProperty.UnsetValue };
            }

            var thickness = (Thickness)value;
            return new object[] { thickness.Left, thickness.Top, thickness.Right, thickness.Bottom };
        }

        #endregion
    }
}
