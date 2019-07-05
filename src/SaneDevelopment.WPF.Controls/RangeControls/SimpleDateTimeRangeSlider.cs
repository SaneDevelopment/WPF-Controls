// -----------------------------------------------------------------------
// <copyright file="SimpleDateTimeRangeSlider.cs" company="Sane Development">
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
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using SaneDevelopment.WPF.Controls.Properties;

#pragma warning disable CA1501 // Avoid excessive inheritance

    /// <summary>
    /// Range slider, that uses <see cref="DateTime"/> as type for values and <see cref="TimeSpan"/> for interval.
    /// </summary>
    [TemplatePart(Name = TrackPartName, Type = typeof(DateTimeRangeTrack))]
    [TemplatePart(Name = TrackBackgroundPartName, Type = typeof(FrameworkElement))]
    [Description("Simple Date&Time Range Slider")]
    public class SimpleDateTimeRangeSlider : SimpleRangeSlider<DateTime, TimeSpan>
    {
#pragma warning disable SA1202 // Elements should be ordered by access
#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable IDE1006 // Naming Styles

        /// <summary>
        /// Format string used when <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> is <c>null</c> or empty.
        /// </summary>
        public const string DefaultAutoToolTipFormat = "dd-MM-yyyy HH:mm:ss";

        #region Private fields

#pragma warning disable SA1308 // Variable names should not be prefixed
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter

        private static readonly TimeSpan s_DefaultTickFrequency = TimeSpan.FromDays(365);
        private static readonly TimeSpan s_DefaultLargeChange = TimeSpan.FromDays(365);

        private static readonly DateTime s_DefaultMinimum = new DateTime(1900, 1, 1);
        private static readonly DateTime s_DefaultMaximum = new DateTime(9999, 12, 31);

#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning restore SA1308 // Variable names should not be prefixed

        #endregion Private fields

        static SimpleDateTimeRangeSlider()
        {
            Type thisType = typeof(SimpleDateTimeRangeSlider);

            DateTime from = s_DefaultMinimum, to = s_DefaultMaximum;
            TimeSpan minRange = TimeSpan.Zero, tickFrequency = s_DefaultTickFrequency;
            TimeSpan smallChange = TimeSpan.FromDays(1.0), largeChange = s_DefaultLargeChange;

            // Register all PropertyTypeMetadata
            MinimumProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(from, FrameworkPropertyMetadataOptions.AffectsMeasure));
            MaximumProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(to, FrameworkPropertyMetadataOptions.AffectsMeasure));
            MinRangeValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(minRange, FrameworkPropertyMetadataOptions.AffectsMeasure));
            StartValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(from, FrameworkPropertyMetadataOptions.AffectsMeasure));
            EndValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(to, FrameworkPropertyMetadataOptions.AffectsMeasure));

            TickFrequencyProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(tickFrequency));
            TickLabelNumericFormatProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(null));
            TickLabelConverterProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(new DefaultDateTimeTickLabelToStringConverter()));
            TickLabelConverterParameterProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(null));

            SmallChangeProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(smallChange));
            LargeChangeProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(largeChange));

            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
        }

        #region Overrides

        /// <summary>
        /// Gets current interval (range) value.
        /// </summary>
        /// <value>Current interval (range) value.</value>
        protected override TimeSpan CurrentRangeValue
        {
            get
            {
                return this.EndValue - this.StartValue;
            }
        }

        /// <summary>
        /// Gets the collection of tick marks of <see cref="DateTime"/> type.
        /// </summary>
        /// <value>The collection of tick marks of <see cref="DateTime"/> type.</value>
        protected override ITicksCollection<DateTime> TypedTicksCollection
        {
            get
            {
                var ticks = this.Ticks;
                return ticks == null ? null : new DateTimeTicksCollection(this.Ticks);
            }
        }

        /// <summary>
        /// Converts number to date.
        /// </summary>
        /// <param name="value">NUmber to convert.</param>
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

        /// <summary>
        /// Converts number to interval.
        /// </summary>
        /// <param name="value">Number to convert.</param>
        /// <returns>Returns a <see cref="System.TimeSpan"/> that represents <paramref name="value"/> time,
        /// where the <paramref name="value"/> is in units of ticks.
        /// For very small <paramref name="value"/> returns <see cref="TimeSpan.Zero"/>.</returns>
        protected override TimeSpan DoubleToInterval(double value)
        {
            return (value > 10.0) ? TimeSpan.FromTicks((long)value) : TimeSpan.Zero;
        }

        /// <summary>
        /// Converts interval to number.
        /// </summary>
        /// <param name="value">Interval value as a <see cref="TimeSpan"/>.</param>
        /// <returns>The number of ticks that represent <paramref name="value"/>.</returns>
        protected override double IntervalToDouble(TimeSpan value)
        {
            return value.Ticks;
        }

        /// <summary>
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>.
        /// </summary>
        /// <param name="value">Value to coerce.</param>
        /// <returns>Coerced (if needed) value.</returns>
        protected override object CoerceMinRangeValue(object value)
        {
            TimeSpan newValue = TimeSpan.Zero;
            if (value is TimeSpan)
            {
                newValue = (TimeSpan)value;
            }

            if (!this.MinRangeValueEnabled || newValue < TimeSpan.Zero)
            {
                newValue = TimeSpan.Zero;
            }
            else
            {
                TimeSpan range = this.Maximum - this.Minimum;
                if (newValue > range)
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
            DateTime newValue = DateTime.MinValue;
            if (value is DateTime)
            {
                newValue = (DateTime)value;
            }

            DateTime minimum = this.Minimum;
            if (newValue < minimum)
            {
                newValue = minimum;
            }

            return newValue;
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
        /// then uses "dd-MM-yyyy HH:mm:ss" (<see cref="DefaultAutoToolTipFormat"/>).
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="thumbType">The type of thumb which value is <paramref name="value"/>.</param>
        /// <returns>String representation of <paramref name="value"/> for thumb, which type is <paramref name="thumbType"/>.</returns>
        protected override string GetAutoToolTipString(DateTime value, RangeThumbType thumbType)
        {
            string res;
            if (this.AutoToolTipValueConverter == null)
            {
                string frmt = this.AutoToolTipFormat;
                if (string.IsNullOrEmpty(frmt))
                {
                    frmt = DefaultAutoToolTipFormat;
                }

                try
                {
                    res = value.ToString(frmt, CultureInfo.CurrentCulture);
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
            bool res = this.IsSingleValue
                           ? this.StartValue < this.Maximum
                           : this.StartValue < this.EndValue;
            return res;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the increasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
        protected override bool IncreaseEndValueCommandCanExecute()
        {
            return this.EndValue < this.Maximum;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of start value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
        protected override bool DecreaseStartValueCommandCanExecute()
        {
            return this.Minimum < this.StartValue;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
        protected override bool DecreaseEndValueCommandCanExecute()
        {
            bool res = this.IsSingleValue
                           ? this.Minimum < this.EndValue
                           : this.StartValue < this.EndValue;
            return res;
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="RangeBaseControl{T, TInterval}.Minimum"/> changes.
        ///
        /// Overridden here to update the value in <see cref="SimpleDateTimeRangeSlider.MinimumAsDouble"/>.
        /// </summary>
        /// <param name="oldMinimum">The value of the property before the change reported by the relevant event or state change.</param>
        /// <param name="newMinimum">The value of the property after the change reported by the relevant event or state change.</param>
        protected override void OnMinimumChanged(DateTime oldMinimum, DateTime newMinimum)
        {
            this.MinimumAsDouble = this.ValueToDouble(this.Minimum);

            base.OnMinimumChanged(oldMinimum, newMinimum);
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="RangeBaseControl{T, TInterval}.Maximum"/> changes.
        ///
        /// Overridden here to update the value in <see cref="SimpleDateTimeRangeSlider.MaximumAsDouble"/>.
        /// </summary>
        /// <param name="oldMaximum">The value of the property before the change reported by the relevant event or state change.</param>
        /// <param name="newMaximum">The value of the property after the change reported by the relevant event or state change.</param>
        protected override void OnMaximumChanged(DateTime oldMaximum, DateTime newMaximum)
        {
            this.MaximumAsDouble = this.ValueToDouble(this.Maximum);

            base.OnMaximumChanged(oldMaximum, newMaximum);
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/> changes.
        ///
        /// Overridden here to update the value in <see cref="SimpleDateTimeRangeSlider.TickFrequencyAsDouble"/>.
        /// </summary>
        /// <param name="oldTickFrequency">Gets the value of the <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/> before the change.</param>
        /// <param name="newTickFrequency">Gets the value of the <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/> after the change.</param>
        protected override void OnTickFrequencyChanged(TimeSpan oldTickFrequency, TimeSpan newTickFrequency)
        {
            this.TickFrequencyAsDouble = this.IntervalToDouble(this.TickFrequency);

            base.OnTickFrequencyChanged(oldTickFrequency, newTickFrequency);
        }

        #endregion Overrides

        #region Dependency Properties

        #region MinimumAsDouble

        /// <summary>
        /// Gets the value of <see cref="RangeBaseControl{T, TInterval}.Minimum"/> interpreted as <c>double</c>.
        ///
        /// Can be used for binding with framework elements that works with <see cref="double"/> values,
        /// e.g. <see cref="Slider"/>.
        /// </summary>
        /// <value>The value of <see cref="RangeBaseControl{T, TInterval}.Minimum"/> interpreted as <c>double</c>.</value>
        [Category("Behavior")]
        [Bindable(true)]
        public double MinimumAsDouble
        {
            get
            {
                var res = this.GetValue(MinimumAsDoubleProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            private set
            {
                this.SetValue(MinimumAsDoublePropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey MinimumAsDoublePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(MinimumAsDouble),
                typeof(double),
                typeof(SimpleDateTimeRangeSlider),
                // the default value is necessary for readonly dependency property evaluated by changes from another property
                // without that the initial value of readonly property wont be initialized correctly
                new FrameworkPropertyMetadata((double)s_DefaultMinimum.Ticks));

        /// <summary>
        /// Dependency property for <see cref="SimpleDateTimeRangeSlider.MinimumAsDouble"/>.
        /// </summary>
        public static readonly DependencyProperty MinimumAsDoubleProperty = MinimumAsDoublePropertyKey.DependencyProperty;

        #endregion

        #region MaximumAsDouble

        /// <summary>
        /// Gets the value of <see cref="RangeBaseControl{T, TInterval}.Maximum"/> interpreted as <c>double</c>.
        ///
        /// Can be used for binding with framework elements that works with <see cref="double"/> values,
        /// e.g. <see cref="Slider"/>.
        /// </summary>
        /// <value>The value of <see cref="RangeBaseControl{T, TInterval}.Maximum"/> interpreted as <c>double</c>.</value>
        [Category("Behavior")]
        [Bindable(true)]
        public double MaximumAsDouble
        {
            get
            {
                var res = this.GetValue(MaximumAsDoubleProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            private set
            {
                this.SetValue(MaximumAsDoublePropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey MaximumAsDoublePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(MaximumAsDouble),
                typeof(double),
                typeof(SimpleDateTimeRangeSlider),
                // the default value is necessary for readonly dependency property evaluated by changes from another property
                // without that the initial value of readonly property wont be initialized correctly
                new FrameworkPropertyMetadata((double)s_DefaultMaximum.Ticks));

        /// <summary>
        /// Dependency property for <see cref="SimpleDateTimeRangeSlider.MaximumAsDouble"/>.
        /// </summary>
        public static readonly DependencyProperty MaximumAsDoubleProperty = MaximumAsDoublePropertyKey.DependencyProperty;

        #endregion

        #region Ticks

#pragma warning disable CA2227 // Collection properties should be read only

        /// <summary>
        /// Gets or sets collection of numeric tick marks.
        /// If <see cref="SimpleDateTimeRangeSlider.Ticks"/> is not <c>null</c>
        /// slider ignores <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>
        /// and draws only tick marks from this collection.
        /// </summary>
        /// <value>Collection of numeric tick marks.</value>
        [Bindable(true)]
        [Category("Appearance")]
        public DateTimeCollection Ticks
        {
            get
            {
                return (DateTimeCollection)this.GetValue(TicksProperty);
            }

            set
            {
                this.SetValue(TicksProperty, value);
            }
        }

#pragma warning restore CA2227 // Collection properties should be read only

        /// <summary>
        /// Dependency property for <see cref="SimpleDateTimeRangeSlider.Ticks"/>.
        /// </summary>
        public static readonly DependencyProperty TicksProperty
            = DependencyProperty.Register(
                nameof(Ticks),
                typeof(DateTimeCollection),
                typeof(SimpleDateTimeRangeSlider),
                // the default value is necessary for readonly dependency property evaluated by changes from another property
                // without that the initial value of readonly property wont be initialized correctly
                new FrameworkPropertyMetadata(new DateTimeCollection().GetAsFrozen(), OnTicksChanged));

        private static void OnTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d is SimpleDateTimeRangeSlider, "d is SimpleDateTimeRangeSlider");

            var element = (SimpleDateTimeRangeSlider)d;
            if (element.IsSnapToTickEnabled)
            {
                // if tick's positions change and IsSnapToTickEnabled is ON,
                // then we have to align current values (thumbs) to nearest ticks.
                // shortcoming of that way: thumbs positions can be changed, therefore interval (range) value can be changed too
                // (but not necessarily).
                element.AlignValuesToTicks();
            }

            element.TicksAsDouble = element.Ticks == null
                                        ? null
                                        : new DoubleCollection(element.Ticks.Select(tick => (double)tick.Ticks));
        }

        #endregion

        #region TicksAsDouble

        /// <summary>
        /// Gets the value of <see cref="SimpleDateTimeRangeSlider.Ticks"/> interpreted as <c>double</c>.
        ///
        /// Can be used for binding with framework elements that works with <see cref="double"/> values,
        /// e.g. <see cref="TickBar"/>.
        /// </summary>
        /// <value>The value of <see cref="SimpleDateTimeRangeSlider.Ticks"/> interpreted as <c>double</c>.</value>
        [Category("Appearance")]
        [Bindable(true)]
        public DoubleCollection TicksAsDouble
        {
            get { return (DoubleCollection)this.GetValue(TicksAsDoubleProperty); }
            private set { this.SetValue(TicksAsDoublePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey TicksAsDoublePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(TicksAsDouble),
                typeof(DoubleCollection),
                typeof(SimpleDateTimeRangeSlider),
                // the default value is necessary for readonly dependency property evaluated by changes from another property
                // without that the initial value of readonly property wont be initialized correctly
                new FrameworkPropertyMetadata(new DoubleCollection().GetAsFrozen()));

        /// <summary>
        /// Dependency property for <see cref="SimpleDateTimeRangeSlider.TicksAsDouble"/>.
        /// </summary>
        public static readonly DependencyProperty TicksAsDoubleProperty = TicksAsDoublePropertyKey.DependencyProperty;

        #endregion

        #region TickFrequencyAsDouble

        /// <summary>
        /// Gets the value of <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/> interpreted as collection of <c>double</c> numbers.
        ///
        /// Can be used for binding with framework elements that works with <see cref="double"/> values,
        /// e.g. <see cref="TickBar"/>.
        /// </summary>
        /// <value>The value of <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/> interpreted as collection of <c>double</c> numbers.</value>
        [Category("Appearance")]
        [Bindable(true)]
        public double TickFrequencyAsDouble
        {
            get
            {
                var res = this.GetValue(TickFrequencyAsDoubleProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            private set
            {
                this.SetValue(TickFrequencyAsDoublePropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey TickFrequencyAsDoublePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(TickFrequencyAsDouble),
                typeof(double),
                typeof(SimpleDateTimeRangeSlider),
                // the default value is necessary for readonly dependency property evaluated by changes from another property
                // without that the initial value of readonly property wont be initialized correctly
                new FrameworkPropertyMetadata((double)s_DefaultTickFrequency.Ticks));

        /// <summary>
        /// Dependency property for <see cref="SimpleDateTimeRangeSlider.TickFrequencyAsDouble"/>.
        /// </summary>
        public static readonly DependencyProperty TickFrequencyAsDoubleProperty = TickFrequencyAsDoublePropertyKey.DependencyProperty;

        #endregion

        #endregion

#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1202 // Elements should be ordered by access
    }

#pragma warning restore CA1501 // Avoid excessive inheritance
}
