// -----------------------------------------------------------------------
// <copyright file="SimpleNumericRangeSlider.cs" company="Sane Development">
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Media;
    using SaneDevelopment.WPF.Controls.Properties;

#pragma warning disable CA1501 // Avoid excessive inheritance

    /// <summary>
    /// Range slider, that uses <see cref="double"/> as type for values and interval.
    /// </summary>
    [TemplatePart(Name = TrackPartName, Type = typeof(NumericRangeTrack))]
    [TemplatePart(Name = TrackBackgroundPartName, Type = typeof(FrameworkElement))]
    [Description("Simple Numeric Range Slider")]
    public class SimpleNumericRangeSlider : SimpleRangeSlider<double, double>
    {
#pragma warning disable SA1202 // Elements should be ordered by access
#pragma warning disable SA1201 // Elements should appear in the correct order

        #region Private fields

#pragma warning disable SA1308 // Variable names should not be prefixed
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning disable SA1303 // Const field names should begin with upper-case letter
#pragma warning disable SA1310 // Field names should not contain underscore

        private const double c_DefaultTickFrequency = 1.0;
        private const double c_DefaultSmallChange = 0.1;
        private const double c_DefaultLargeChange = 1.0;

        private const double c_DefaultMinimum = 0.0;
        private const double c_DefaultMaximum = 10.0;

#pragma warning restore SA1310 // Field names should not contain underscore
#pragma warning restore SA1303 // Const field names should begin with upper-case letter
#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning restore SA1308 // Variable names should not be prefixed

        #endregion Private fields

        static SimpleNumericRangeSlider()
        {
            Type thisType = typeof(SimpleNumericRangeSlider);

            // Register all PropertyTypeMetadata
            MinimumProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(c_DefaultMinimum, FrameworkPropertyMetadataOptions.AffectsMeasure));
            MaximumProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(c_DefaultMaximum, FrameworkPropertyMetadataOptions.AffectsMeasure));
            MinRangeValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
            StartValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(c_DefaultMinimum, FrameworkPropertyMetadataOptions.AffectsMeasure));
            EndValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(c_DefaultMaximum, FrameworkPropertyMetadataOptions.AffectsMeasure));

            TickFrequencyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(c_DefaultTickFrequency));
            TickLabelNumericFormatProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(null));
            TickLabelConverterProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(null));
            TickLabelConverterParameterProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(null));

            SmallChangeProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(c_DefaultSmallChange));
            LargeChangeProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(c_DefaultLargeChange));

            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
        }

        /// <summary>
        /// Format string used when <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> is <c>null</c> or empty.
        /// </summary>
        public const string DefaultAutoToolTipFormat = "N";

        #region override functions

        /// <summary>
        /// Converts number to value type.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Always <paramref name="value"/>.</returns>
        protected override double DoubleToValue(double value)
        {
            return value;
        }

        /// <summary>
        /// Converts value to number.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Always <paramref name="value"/>.</returns>
        protected override double ValueToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Converts number to interval type.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Always <paramref name="value"/>.</returns>
        protected override double DoubleToInterval(double value)
        {
            return value;
        }

        /// <summary>
        /// Converts interval value to number.
        /// </summary>
        /// <param name="value">Interval value.</param>
        /// <returns>Always <paramref name="value"/>.</returns>
        protected override double IntervalToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>.
        /// </summary>
        /// <param name="value">Value to coerce.</param>
        /// <returns>Coerced (if needed) value.</returns>
        protected override object CoerceMinRangeValue(object value)
        {
            var defaultMetadata = MinRangeValueProperty.DefaultMetadata;
            double newValue =
                DependencyPropertyUtil.ExtractDouble(
                    value,
                    defaultMetadata == null ? 0.0 : (double)(defaultMetadata.DefaultValue ?? 0.0));

            if (!this.MinRangeValueEnabled || DoubleUtil.LessThan(newValue, 0.0))
            {
                newValue = 0.0;
            }
            else
            {
                double range = this.Maximum - this.Minimum;
                if (DoubleUtil.GreaterThan(newValue, range))
                {
                    newValue = range;
                }
            }

            return newValue;
        }

        /// <summary>
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.Maximum"/>.
        /// </summary>
        /// <param name="value">Value to coerce.</param>
        /// <returns>Coerced (if needed) value.</returns>
        protected override object CoerceMaximum(object value)
        {
            var defaultMetadata = MaximumProperty.DefaultMetadata;
            double newValue =
                DependencyPropertyUtil.ExtractDouble(
                    value,
                    defaultMetadata == null ? 0.0 : (double)(defaultMetadata.DefaultValue ?? 0.0));

            double minimum = this.Minimum;
            if (newValue < minimum)
            {
                newValue = minimum;
            }

            return newValue;
        }

        /// <summary>
        /// Gets current interval (range) value.
        /// </summary>
        /// <value>Current interval (range) value.</value>
        protected override double CurrentRangeValue
        {
            get
            {
                return this.EndValue - this.StartValue;
            }
        }

        /// <summary>
        /// Gets the collection of tick marks of <see cref="double"/> type.
        /// </summary>
        /// <value>The collection of tick marks of <see cref="double"/> type.</value>
        protected override ITicksCollection<double> TypedTicksCollection
        {
            get
            {
                var ticks = this.Ticks;
                return ticks == null ? null : new DoubleTicksCollection(this.Ticks);
            }
        }

        /// <summary>
        /// Gets the string representation of <paramref name="value"/> for showing in tooltips.
        ///
        /// For conversion uses either <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>
        /// with parameter <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverterParameter"/>,
        /// or a format string from <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/>
        /// and numeric precision from <see cref="SimpleNumericRangeSlider.AutoToolTipPrecision"/>
        /// (if converter <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/> is <c>null</c>).
        /// If neither <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>
        /// nor <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> is set (not <c>null</c>),
        /// then uses gerenal nimeric format (<see cref="DefaultAutoToolTipFormat"/>).
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="thumbType">The type of thumb which value is <paramref name="value"/>.</param>
        /// <returns>String representation of <paramref name="value"/> for thumb, which type is <paramref name="thumbType"/>.</returns>
        protected override string GetAutoToolTipString(double value, RangeThumbType thumbType)
        {
            string res;
            if (this.AutoToolTipValueConverter == null)
            {
                string frmt = this.AutoToolTipFormat;
                if (string.IsNullOrEmpty(frmt))
                {
                    frmt = DefaultAutoToolTipFormat;
                }

                var format = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                Debug.Assert(this.AutoToolTipPrecision >= DependencyPropertyUtil.MinimumAutoToolTipPrecision, "this.AutoToolTipPrecision >= DependencyPropertyUtil.MinimumAutoToolTipPrecision");
                Debug.Assert(this.AutoToolTipPrecision <= DependencyPropertyUtil.MaximumAutoToolTipPrecision, "this.AutoToolTipPrecision <= DependencyPropertyUtil.MaximumAutoToolTipPrecision");
                format.NumberDecimalDigits = this.AutoToolTipPrecision;
                try
                {
                    res = value.ToString(frmt, format);
                }
                catch (FormatException)
                {
                    res = LocalizationResource.BadAutoToolTipFormat;
                }
            }
            else
            {
                res = this.AutoToolTipValueConverter.Convert(value, thumbType, this.AutoToolTipValueConverterParameter);
            }

            return res;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the increasing of start value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
        protected override bool IncreaseStartValueCommandCanExecute()
        {
            bool res = this.IsSingleValue ?
                DoubleUtil.LessThan(this.StartValue, this.Maximum) :
                DoubleUtil.LessThan(this.StartValue, this.EndValue);
            return res;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the increasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
        protected override bool IncreaseEndValueCommandCanExecute()
        {
            return DoubleUtil.LessThan(this.EndValue, this.Maximum);
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of start value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
        protected override bool DecreaseStartValueCommandCanExecute()
        {
            return DoubleUtil.LessThan(this.Minimum, this.StartValue);
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
        protected override bool DecreaseEndValueCommandCanExecute()
        {
            bool res = this.IsSingleValue ?
                DoubleUtil.LessThan(this.Minimum, this.EndValue) :
                DoubleUtil.LessThan(this.StartValue, this.EndValue);
            return res;
        }

        #endregion

        #region Dependency Properties

        #region AutoToolTipPrecision Property

        /// <summary>
        /// Dependency property for <see cref="SimpleNumericRangeSlider.AutoToolTipPrecision"/>.
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register(
            nameof(AutoToolTipPrecision),
            typeof(int),
            typeof(SimpleNumericRangeSlider),
            new FrameworkPropertyMetadata(0),
            DependencyPropertyUtil.IsValidAutoToolTipPrecision);

        /// <summary>
        /// Gets or sets floating-point precision of numbers for showing in the auto tooltips in UI.
        /// </summary>
        /// <value>Floating-point precision of numbers for showing in the auto tooltips in UI.</value>
        [Bindable(true)]
        [Category("Appearance")]
        public int AutoToolTipPrecision
        {
            get
            {
                var res = this.GetValue(AutoToolTipPrecisionProperty);
                Debug.Assert(res != null, "res != null");
                return (int)res;
            }

            set
            {
                this.SetValue(AutoToolTipPrecisionProperty, value);
            }
        }

        #endregion

        #region Ticks Property

        /// <summary>
        /// Dependency property for <see cref="SimpleNumericRangeSlider.Ticks"/>.
        /// </summary>
        public static readonly DependencyProperty TicksProperty
            = DependencyProperty.Register(
                nameof(Ticks),
                typeof(DoubleCollection),
                typeof(SimpleNumericRangeSlider),
                new FrameworkPropertyMetadata(new DoubleCollection().GetAsFrozen(), OnTicksChanged));

#pragma warning disable CA2227 // Collection properties should be read only

        /// <summary>
        /// Gets or sets collection of numeric tick marks.
        /// If <see cref="SimpleNumericRangeSlider.Ticks"/> is not <c>null</c>
        /// slider ignores <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>
        /// and draws only tick marks from this collection.
        /// </summary>
        /// <value>Collection of numeric tick marks.</value>
        [Bindable(true)]
        [Category("Appearance")]
        public DoubleCollection Ticks
        {
            get
            {
                return (DoubleCollection)this.GetValue(TicksProperty);
            }

            set
            {
                this.SetValue(TicksProperty, value);
            }
        }

#pragma warning restore CA2227 // Collection properties should be read only

        private static void OnTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d is SimpleNumericRangeSlider, "d is SimpleNumericRangeSlider");

            var element = (SimpleNumericRangeSlider)d;
            if (element.IsSnapToTickEnabled)
            {
                // if tick's positions change and IsSnapToTickEnabled is ON,
                // then we have to align current values (thumbs) to nearest ticks.
                // shortcoming of that way: thumbs positions can be changed, therefore interval (range) value can be changed too
                // (but not necessarily).
                element.AlignValuesToTicks();
            }
        }

        #endregion

        #endregion

#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1202 // Elements should be ordered by access
    }

#pragma warning restore CA1501 // Avoid excessive inheritance
}
