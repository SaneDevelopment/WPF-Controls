// -----------------------------------------------------------------------
// <copyright file="DefaultDateTimeTickLabelToStringConverter.cs" company="Sane Development">
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
    using System.Diagnostics;
    using System.Globalization;
    using SaneDevelopment.WPF.Controls.Properties;

    /// <summary>
    /// Class for converting <see cref="DateTime"/> value to <c>string</c> using its ticks value.
    ///
    /// Uses as default converter for <see cref="SimpleDateTimeRangeSlider"/>.
    /// </summary>
    public sealed class DefaultDateTimeTickLabelToStringConverter : IDoubleToStringConverter
    {
        /// <summary>
        /// Converts date's ticks value to the string representation of that date.
        /// </summary>
        /// <param name="value">Number of ticks.</param>
        /// <param name="parameter">If set, used as a format string in <see cref="DateTime.ToString(string,IFormatProvider)"/> method.</param>
        /// <returns>String representation of date
        /// or string indicates wrong <paramref name="value"/> or <paramref name="parameter"/> value
        /// (depends on version and culture settings).</returns>
        public string Convert(double value, object parameter)
        {
            var longTicks = (long)value;
            if (longTicks < DateTime.MinValue.Ticks || longTicks > (DateTime.MaxValue.Ticks + 1))
            {
                // allow minimum excess over DateTime.MaxValue.Ticks because of loss of accuracy while casting from double
                return LocalizationResource.BadDateTimeTicksValue;
            }

            if (longTicks == DateTime.MaxValue.Ticks + 1)
            {
                longTicks = DateTime.MaxValue.Ticks;
            }

            if (parameter != null && !(parameter is string))
            {
                return LocalizationResource.BadDateTimeTicksValue;
            }


            Debug.Assert(longTicks <= 0x2bca2875f4373fffL, "longTicks <= 0x2bca2875f4373fffL"); // DateTime.MaxValue.Ticks
            var dt = new DateTime(longTicks);

            if (!(parameter is string frmt))
            {
                return dt.ToString(CultureInfo.CurrentCulture);
            }

            try
            {
                return dt.ToString(frmt, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                return LocalizationResource.BadDateTimeTicksFormat;
            }
        }
    }
}
