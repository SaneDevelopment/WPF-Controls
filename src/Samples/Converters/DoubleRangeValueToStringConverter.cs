// -----------------------------------------------------------------------
// <copyright file="DoubleRangeValueToStringConverter.cs" company="Sane Development">
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

    /// <summary>
    /// Convert double ranged value to string.
    /// </summary>
    public sealed class DoubleRangeValueToStringConverter : IRangeValueToStringConverter<double>
    {
#pragma warning disable SA1308 // Variable names should not be prefixed

        /// <summary>
        /// Suppose we use ZoomBar as a slider for date interval [01-01-2000..31-12-2000].
        /// </summary>
        private static DateTime s_DateFrom = new DateTime(2000, 1, 1);

        /// <summary>
        /// Suppose we use ZoomBar as a slider for date interval [01-01-2000..31-12-2000].
        /// </summary>
        private static DateTime s_DateTo = new DateTime(2000, 12, 31);

#pragma warning restore SA1308 // Variable names should not be prefixed

        /// <inheritdoc/>
        public string Convert(double value, RangeThumbType thumbType, object parameter)
        {
            var zb = parameter as IRanged<double, double>;
            if (zb == null)
            {
                throw new ArgumentOutOfRangeException(nameof(parameter));
            }

            string res = value.ToString("F4", CultureInfo.CurrentCulture);
            if (thumbType == RangeThumbType.StartThumb || thumbType == RangeThumbType.RangeThumb)
            {
                double min = s_DateFrom.Ticks, max = s_DateTo.Ticks;

                double scale = (max - min) / (zb.Maximum - zb.Minimum);
                double newStart = (value * scale) + min;
                var newStartAsDateTime = new DateTime((long)newStart);

                res += " [" + newStartAsDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "]";
            }

            return res;
        }
    }
}
