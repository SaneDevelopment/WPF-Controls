// -----------------------------------------------------------------------
// <copyright file="DateTimeRangeTrack.cs" company="Sane Development">
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
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Track that uses <see cref="DateTime"/> as values type and <see cref="TimeSpan"/> as inteval type.
    /// </summary>
    public class DateTimeRangeTrack : RangeTrack<DateTime, TimeSpan>
    {
#pragma warning disable SA1308 // Variable names should not be prefixed
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter

        private static readonly DateTime s_DefaultMinimum = new DateTime(1900, 1, 1);
        private static readonly DateTime s_DefaultMaximum = new DateTime(2000, 1, 1);

#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning restore SA1308 // Variable names should not be prefixed

        static DateTimeRangeTrack()
        {
            Type thisType = typeof(DateTimeRangeTrack);

            MinimumProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(s_DefaultMinimum, FrameworkPropertyMetadataOptions.AffectsArrange));
            MaximumProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(s_DefaultMaximum, FrameworkPropertyMetadataOptions.AffectsArrange));
            StartValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(s_DefaultMinimum, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));
            EndValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(s_DefaultMaximum, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));
        }

        /// <summary>
        /// Gets default value for <see cref="RangeTrack{T,TInterval}.MinimumProperty"/>.
        /// </summary>
        /// <value>Default value for <see cref="RangeTrack{T,TInterval}.MinimumProperty"/>.</value>
        public static DateTime DefaultMinimum
        {
            get { return s_DefaultMinimum; }
        }

        /// <summary>
        /// Gets default value for <see cref="RangeTrack{T,TInterval}.MaximumProperty"/>.
        /// </summary>
        /// <value>Default value for <see cref="RangeTrack{T,TInterval}.MaximumProperty"/>.</value>
        public static DateTime DefaultMaximum
        {
            get { return s_DefaultMaximum; }
        }

        #region Control Parts

        /// <summary>
        /// Gets or sets button for decreasing interval.
        /// </summary>
        /// <value>Button for decreasing interval.</value>
        public override RepeatButton DecreaseRepeatButton
        {
            get
            {
                return base.DecreaseRepeatButton;
            }

            set
            {
                base.DecreaseRepeatButton = value;
            }
        }

        /// <summary>
        /// Gets or sets button for increasing interval.
        /// </summary>
        /// <value>Button for increasing interval.</value>
        public override RepeatButton IncreaseRepeatButton
        {
            get
            {
                return base.IncreaseRepeatButton;
            }

            set
            {
                base.IncreaseRepeatButton = value;
            }
        }

        /// <summary>
        /// Gets or sets start interval thumb.
        /// </summary>
        /// <value>Start interval thumb.</value>
        public override Thumb StartThumb
        {
            get
            {
                return base.StartThumb;
            }

            set
            {
                base.StartThumb = value;
            }
        }

        /// <summary>
        /// Gets or sets interval (range) thumb.
        /// </summary>
        /// <value>Interval (range) thumb.</value>
        public override Thumb RangeThumb
        {
            get
            {
                return base.RangeThumb;
            }

            set
            {
                base.RangeThumb = value;
            }
        }

        /// <summary>
        /// Gets or sets end interval thumb.
        /// </summary>
        /// <value>End interval thumb.</value>
        public override Thumb EndThumb
        {
            get
            {
                return base.EndThumb;
            }

            set
            {
                base.EndThumb = value;
            }
        }

        #endregion Control Parts

        /// <summary>
        /// Converts <c>double</c> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Date initialized to a <paramref name="value"/> number of ticks.</returns>
        protected override DateTime DoubleToValue(double value)
        {
            return (value > 10.0) ? new DateTime((long)value) : DateTime.MinValue;
        }

        /// <summary>
        /// Converts date to number.
        /// </summary>
        /// <param name="value">Date to convert.</param>
        /// <returns>The number of ticks that represent the date and time of <paramref name="value"/>.</returns>
        protected override double ValueToDouble(DateTime value)
        {
            return value.Ticks;
        }
    }
}
