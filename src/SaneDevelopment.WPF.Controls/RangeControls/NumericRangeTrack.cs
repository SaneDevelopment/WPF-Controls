// -----------------------------------------------------------------------
// <copyright file="NumericRangeTrack.cs" company="Sane Development">
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
    /// Track that uses <c>double</c> as values and inteval type.
    /// </summary>
    public class NumericRangeTrack : RangeTrack<double, double>
    {
#pragma warning disable SA1308 // Variable names should not be prefixed
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning disable SA1303 // Const field names should begin with upper-case letter
#pragma warning disable SA1310 // Field names should not contain underscore

        private const double c_DefaultMinimum = 0.0;
        private const double c_DefaultMaximum = 1.0;

#pragma warning restore SA1310 // Field names should not contain underscore
#pragma warning restore SA1303 // Const field names should begin with upper-case letter
#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning restore SA1308 // Variable names should not be prefixed

        static NumericRangeTrack()
        {
            Type thisType = typeof(NumericRangeTrack);

            MinimumProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(c_DefaultMinimum, FrameworkPropertyMetadataOptions.AffectsArrange));
            MaximumProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(c_DefaultMaximum, FrameworkPropertyMetadataOptions.AffectsArrange));
            StartValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(c_DefaultMinimum, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));
            EndValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(c_DefaultMaximum, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));
        }

        /// <summary>
        /// Gets default value for <see cref="RangeTrack{T,TInterval}.MinimumProperty"/>.
        /// </summary>
        /// <value>Default value for <see cref="RangeTrack{T,TInterval}.MinimumProperty"/>.</value>
        public static double DefaultMinimum
        {
            get { return c_DefaultMinimum; }
        }

        /// <summary>
        /// Gets default value for <see cref="RangeTrack{T,TInterval}.MaximumProperty"/>.
        /// </summary>
        /// <value>Default value for <see cref="RangeTrack{T,TInterval}.MaximumProperty"/>.</value>
        public static double DefaultMaximum
        {
            get { return c_DefaultMaximum; }
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
        /// Converts <c>double</c> to value type.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Always <paramref name="value"/>.</returns>
        protected override double DoubleToValue(double value)
        {
            return value;
        }

        /// <summary>
        /// Convert received value to <c>double</c>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Always <paramref name="value"/>.</returns>
        protected override double ValueToDouble(double value)
        {
            return value;
        }
    }
}
