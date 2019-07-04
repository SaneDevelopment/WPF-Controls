// -----------------------------------------------------------------------
// <copyright file="RangeBaseControl.cs" company="Sane Development">
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
    using System.Windows;
    using System.Windows.Controls;

#pragma warning disable CA1501 // Avoid excessive inheritance

    /// <summary>
    /// Control, that provides a pair of values inside some interval.
    /// </summary>
    /// <typeparam name="T">Type of values.</typeparam>
    /// <typeparam name="TInterval">Type of interval value.</typeparam>
    public abstract class RangeBaseControl<T, TInterval>
        : Control, IRangeTrackTemplatedParent<T, TInterval>, IRanged<T, TInterval>
    {
        #region Private fields

#pragma warning disable SA1310 // Field names should not contain underscore
#pragma warning disable SA1303 // Const field names should begin with upper-case letter
#pragma warning disable SA1308 // Variable names should not be prefixed

        private const bool c_DefaultMinRangeValueEnabled = true;
        private bool m_MinRangeValueEnabled = c_DefaultMinRangeValueEnabled;

#pragma warning restore SA1308 // Variable names should not be prefixed
#pragma warning restore SA1303 // Const field names should begin with upper-case letter
#pragma warning restore SA1310 // Field names should not contain underscore

        #endregion Private fields

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeBaseControl{T, TInterval}"/> class.
        /// </summary>
        protected RangeBaseControl()
        {
            this.IsRangeValueChanging = false;
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1202 // Elements should be ordered by access

        #region Abstract methods

        /// <summary>
        /// Gets current interval (range) value.
        /// </summary>
        /// <value>Current interval (range) value.</value>
        protected abstract TInterval CurrentRangeValue { get; }

        /// <summary>
        /// Method for converting a number to value type.
        /// </summary>
        /// <param name="value">Number to convert.</param>
        /// <returns>Value of type <typeparamref name="T"/> - representation of <paramref name="value"/>.</returns>
        protected abstract T DoubleToValue(double value);

        /// <summary>
        /// Method for converting a value type to number.
        /// </summary>
        /// <param name="value">Value of type <typeparamref name="T"/>.</param>
        /// <returns><c>double</c> representation of <paramref name="value"/>.</returns>
        protected abstract double ValueToDouble(T value);

        /// <summary>
        /// Method for converting a number to interval type.
        /// </summary>
        /// <param name="value">Number to convert.</param>
        /// <returns>Value of type <typeparamref name="TInterval"/> - representation of <paramref name="value"/>.</returns>
        protected abstract TInterval DoubleToInterval(double value);

        /// <summary>
        /// Method for converting an interval type to number.
        /// </summary>
        /// <param name="value">Value of type <typeparamref name="TInterval"/>.</param>
        /// <returns><c>double</c> representation of <paramref name="value"/>.</returns>
        protected abstract double IntervalToDouble(TInterval value);

        /// <summary>
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>.
        /// </summary>
        /// <param name="value">Value to coerce.</param>
        /// <returns>Coerced (if needed) value.</returns>
        protected abstract object CoerceMinRangeValue(object value);

        /// <summary>
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.Maximum"/>.
        /// </summary>
        /// <param name="value">Value to coerce.</param>
        /// <returns>Coerced (if needed) value.</returns>
        protected abstract object CoerceMaximum(object value);

        #endregion Abstract methods

        #region IRanged<T, TInterval> implementation

        /// <inheritdoc/>
        T IRanged<T, TInterval>.DoubleToValue(double value)
        {
            return this.DoubleToValue(value);
        }

        /// <inheritdoc/>
        double IRanged<T, TInterval>.ValueToDouble(T value)
        {
            return this.ValueToDouble(value);
        }

        /// <inheritdoc/>
        TInterval IRanged<T, TInterval>.DoubleToInterval(double value)
        {
            return this.DoubleToInterval(value);
        }

        /// <inheritdoc/>
        double IRanged<T, TInterval>.IntervalToDouble(TInterval value)
        {
            return this.IntervalToDouble(value);
        }

        #endregion IRanged<T, TInterval>

        /// <summary>
        /// Gets or sets a value indicating whether indicator of changing the value of the whole interval (i.e. start and end values together), but not the one of the values.
        /// If it is ON (<c>true</c>), then method <see cref="RangeBaseControl{T, TInterval}.OnRangeValueChanged(TInterval,TInterval)"/> never invokes,
        /// and therefore event <see cref="RangeBaseControl{T, TInterval}.RangeValueChanged"/> never raises.
        /// Also, methods, that can change <see cref="RangeBaseControl{T, TInterval}.IsRangeValueChanging"/>
        /// must trace these changes and call <see cref="RangeBaseControl{T, TInterval}.OnRangeValueChanged(TInterval,TInterval)"/> in appropriate moment.
        /// </summary>
        /// <value>A value indicating whether indicator of changing the value of the whole interval (i.e. start and end values together), but not the one of the values.</value>
        protected bool IsRangeValueChanging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not is enabled validation over min range value.
        /// By default it is <c>true</c>, but in derived classes it can be turned OFF.
        /// Also, it turns OFF when such validation can't be performed by objective reasons,
        /// e.g. when <see cref="RangeBaseControl{T, TInterval}.IsSingleValue"/> mode is ON.
        /// </summary>
        /// <value>A value indicating whether or not is enabled validation over min range value.</value>
        protected bool MinRangeValueEnabled
        {
            get
            {
                return this.m_MinRangeValueEnabled;
            }

            set
            {
                if (value != this.m_MinRangeValueEnabled)
                {
                    this.m_MinRangeValueEnabled = value;
                    this.CoerceValue(MinRangeValueProperty);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether default value of property <see cref="RangeBaseControl{T, TInterval}.MinRangeValueEnabled"/>,
        /// which will be set to when such validation become available.
        /// E.g. when <see cref="RangeBaseControl{T, TInterval}.IsSingleValue"/> mode comes to OFF (<c>false</c>).
        ///
        /// In derived classes this property can be overridden to control mentioned mechanism,
        /// suitable for every control individually by default.
        /// </summary>
        /// <value>A value indicating whether default value of property <see cref="RangeBaseControl{T, TInterval}.MinRangeValueEnabled"/>,
        /// which will be set to when such validation become available.</value>
        protected virtual bool DefaultMinRangeValueEnabled
        {
            get { return c_DefaultMinRangeValueEnabled; }
        }

        /// <summary>
        /// Calls whenever the effective value of any of following properties changes: <see cref="RangeBaseControl{T, TInterval}.StartValue"/>,
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/> or <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>.
        /// </summary>
        /// <param name="oldStartValue">The value of the <see cref="StartValue"/> property before the change reported by the relevant event or state change.</param>
        /// <param name="oldEndValue">The value of the <see cref="EndValue"/> property before the change reported by the relevant event or state change.</param>
        /// <param name="newStartValue">The value of the <see cref="StartValue"/> property after the change reported by the relevant event or state change.</param>
        /// <param name="newEndValue">The value of the <see cref="EndValue"/> property after the change reported by the relevant event or state change.</param>
        protected virtual void OnValueChanged(T oldStartValue, T oldEndValue, T newStartValue, T newEndValue)
        {
            var e = new RangeDragCompletedEventArgs<T>(oldStartValue, oldEndValue, newStartValue, newEndValue)
            {
                RoutedEvent = ValueChangedEvent,
            };
            this.RaiseEvent(e);
        }

        #region IRangeTrackTemplatedParent<T>

        /// <summary>
        /// Method handles <see cref="FrameworkElement.OnApplyTemplate"/>,
        /// notably bind some dependency properties with templated parent.
        /// </summary>
        /// <param name="templatedParent">Templated parent.</param>
        /// <param name="track">Any range track.</param>
        public virtual void OnApplyRangeTrackTemplate(DependencyObject templatedParent, RangeTrack<T, TInterval> track)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }

            var templatedParentControl = templatedParent as RangeBaseControl<T, TInterval>;
            if (templatedParentControl != null)
            {
                track.BindToTemplatedParent(RangeTrack<T, TInterval>.MinimumProperty, MinimumProperty);
                track.BindToTemplatedParent(RangeTrack<T, TInterval>.MaximumProperty, MaximumProperty);
                track.BindToTemplatedParent(RangeTrack<T, TInterval>.StartValueProperty, StartValueProperty);
                track.BindToTemplatedParent(RangeTrack<T, TInterval>.EndValueProperty, EndValueProperty);
            }
        }

        #endregion

        #region Dependency properties

        #region IsSingleValue Property

        /// <summary>
        /// Gets or sets a value indicating whether this object behaves like a single value object, i.e. start value equals to end value,
        /// so interval (range) value equals to zero.
        /// </summary>
        /// <value>A value indicating whether this object behaves like a single value object, i.e. start value equals to end value,
        /// so interval (range) value equals to zero.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public bool IsSingleValue
        {
            get
            {
                var res = this.GetValue(IsSingleValueProperty);
                Debug.Assert(res != null, "res != null");
                return (bool)res;
            }

            set
            {
                this.SetValue(IsSingleValueProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.IsSingleValue"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty IsSingleValueProperty = DependencyProperty.Register(
            nameof(IsSingleValue),
            typeof(bool),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(false, OnIsSingleValueChanged));

        private static void OnIsSingleValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is RangeBaseControl<T, TInterval>, "obj is RangeBaseControl<T, TInterval>");
            Debug.Assert(args.NewValue != null, "args.NewValue != null");

            var element = obj as RangeBaseControl<T, TInterval>;
            Debug.Assert(element != null, "element != null");
            if (element == null)
            {
                return;
            }

            var newValue = (bool)args.NewValue;
            if (newValue)
            {
                // if IsSingleValue mode is ON,
                // then validation over min range value turns OFF, because range value must be always zero from now.
                element.MinRangeValueEnabled = false;

                // if IsSingleValue mode is on (now thumbs must be on the same position),
                // then force to move end thumb to start thumb position.
                element.EndValue = element.StartValue;
                // forcibly set RangeValue to zero, because interval value won't be recalculated from now,
                // for purpose of hold it equal to zero.
                // here we just manualy raise event about interval zeroing.
                element.RangeValue = element.DoubleToInterval(0.0);
            }
            else
            {
                // if IsSingleValue mode is OFF,
                // then we turns ON the validation over min range value.
                // or may be not - it depends of default value of appropriate dependency property.
                element.MinRangeValueEnabled = element.DefaultMinRangeValueEnabled;
            }
        }

        #endregion

        #region Minimum

        /// <summary>
        /// Gets or sets minimum available value.
        /// </summary>
        /// <value>Minimum available value.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public T Minimum
        {
            get
            {
                var res = this.GetValue(MinimumProperty);
                Debug.Assert(res != null, "res != null");
                return (T)res;
            }

            set
            {
                this.SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.Minimum"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            nameof(Minimum),
            typeof(T),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(OnMinimumChanged),
            IsValidValue);

        /// <summary>
        /// Calls whenever the effective value of the <see cref="RangeBaseControl{T, TInterval}.Minimum"/> changes.
        /// </summary>
        /// <param name="oldMinimum">The value of the property before the change reported by the relevant event or state change.</param>
        /// <param name="newMinimum">The value of the property after the change reported by the relevant event or state change.</param>
        protected virtual void OnMinimumChanged(T oldMinimum, T newMinimum)
        {
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d != null, "d != null");
            Debug.Assert(e.OldValue != null, "e.OldValue != null");
            Debug.Assert(e.NewValue != null, "e.NewValue != null");

            var element = (RangeBaseControl<T, TInterval>)d;
            element.CoerceValue(MaximumProperty);
            element.CoerceValue(MinRangeValueProperty);
            element.CoerceValue(StartValueProperty);
            element.CoerceValue(EndValueProperty);
            element.OnMinimumChanged((T)e.OldValue, (T)e.NewValue);
        }

        #endregion

        #region Maximum

        /// <summary>
        /// Gets or sets maximum available value.
        /// </summary>
        /// <value>Maximum available value.</value>
        [Category("Behavior")]
        [Bindable(true)]
        public T Maximum
        {
            get
            {
                var res = this.GetValue(MaximumProperty);
                Debug.Assert(res != null, "res != null");
                return (T)res;
            }

            set
            {
                this.SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.Maximum"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            nameof(Maximum),
            typeof(T),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(OnMaximumChanged, CoerceMaximum),
            IsValidValue);

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            var base2 = d as RangeBaseControl<T, TInterval>;
            if (base2 != null)
            {
                value = base2.CoerceMaximum(value);
            }

            return value;
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="RangeBaseControl{T, TInterval}.Maximum"/> changes.
        /// </summary>
        /// <param name="oldMaximum">The value of the property before the change reported by the relevant event or state change.</param>
        /// <param name="newMaximum">The value of the property after the change reported by the relevant event or state change.</param>
        protected virtual void OnMaximumChanged(T oldMaximum, T newMaximum)
        {
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d != null, "d != null");
            Debug.Assert(e.OldValue != null, "e.OldValue != null");
            Debug.Assert(e.NewValue != null, "e.NewValue != null");

            var element = (RangeBaseControl<T, TInterval>)d;
            element.CoerceValue(MinRangeValueProperty);
            element.CoerceValue(StartValueProperty);
            element.CoerceValue(EndValueProperty);
            element.OnMaximumChanged((T)e.OldValue, (T)e.NewValue);
        }

        #endregion

        #region StartValue

        /// <summary>
        /// Gets or sets start interval value.
        /// </summary>
        /// <value>Start interval value.</value>
        [Category("Behavior")]
        [Bindable(true)]
        public T StartValue
        {
            get
            {
                var res = this.GetValue(StartValueProperty);
                Debug.Assert(res != null, "res != null");
                return (T)res;
            }

            set
            {
                this.SetValue(StartValueProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.StartValue"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty StartValueProperty = DependencyProperty.Register(
            nameof(StartValue),
            typeof(T),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(
                default(T),
                FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnStartValueChanged,
                CoerceStartValue),
            IsValidValue);

        private static object CoerceStartValue(DependencyObject d, object value)
        {
            Debug.Assert(d is RangeBaseControl<T, TInterval>, "d is RangeBaseControl<T, TInterval>");

            var base2 = d as RangeBaseControl<T, TInterval>;
            if (base2 == null)
            {
                return value;
            }

            if (value != null)
            {
                var doubleVal = (T)value;
                value = DependencyPropertyUtil.CoerceRangeStartValue(base2, doubleVal);
            }

            return value;
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="RangeBaseControl{T, TInterval}.StartValue"/> changes.
        /// </summary>
        /// <param name="oldValue">The value of the property before the change reported by the relevant event or state change.</param>
        /// <param name="newValue">The value of the property after the change reported by the relevant event or state change.</param>
        protected virtual void OnStartValueChanged(T oldValue, T newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<T>(oldValue, newValue)
            {
                RoutedEvent = StartValueChangedEvent,
            };
            this.RaiseEvent(e);

            if (!this.IsRangeValueChanging)
            {
                this.OnValueChanged(oldValue, this.EndValue, newValue, this.EndValue);
            }
        }

        private static void OnStartValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d != null, "d != null");
            Debug.Assert(e.OldValue != null, "e.OldValue != null");
            Debug.Assert(e.NewValue != null, "e.NewValue != null");

            var element = (RangeBaseControl<T, TInterval>)d;
            if (element.IsSingleValue)
            {
                element.EndValue = element.StartValue;
            }
            else
            {
                element.RangeValue = element.CurrentRangeValue;
            }

            element.OnStartValueChanged((T)e.OldValue, (T)e.NewValue);
        }

        #endregion

        #region EndValue

        /// <summary>
        /// Gets or sets end interval value.
        /// </summary>
        /// <value>End interval value.</value>
        [Category("Behavior")]
        [Bindable(true)]
        public T EndValue
        {
            get
            {
                var res = this.GetValue(EndValueProperty);
                Debug.Assert(res != null, "res != null");
                return (T)res;
            }

            set
            {
                this.SetValue(EndValueProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.EndValue"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty EndValueProperty = DependencyProperty.Register(
            nameof(EndValue),
            typeof(T),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(
                default(T),
                FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnEndValueChanged,
                CoerceEndValue),
            IsValidValue);

        private static object CoerceEndValue(DependencyObject d, object value)
        {
            Debug.Assert(d is RangeBaseControl<T, TInterval>, "d is RangeBaseControl<T, TInterval>");

            var base2 = d as RangeBaseControl<T, TInterval>;
            Debug.Assert(base2 != null, "base2 != null");
            if (base2 == null)
            {
                return value;
            }

            if (value != null)
            {
                var doubleVal = (T)value;
                value = DependencyPropertyUtil.CoerceRangeEndValue(base2, doubleVal);
            }

            return value;
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="RangeBaseControl{T, TInterval}.EndValue"/> changes.
        /// </summary>
        /// <param name="oldValue">The value of the property before the change reported by the relevant event or state change.</param>
        /// <param name="newValue">The value of the property after the change reported by the relevant event or state change.</param>
        protected virtual void OnEndValueChanged(T oldValue, T newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<T>(oldValue, newValue)
            {
                RoutedEvent = EndValueChangedEvent,
            };
            this.RaiseEvent(e);

            if (!this.IsRangeValueChanging)
            {
                this.OnValueChanged(this.StartValue, oldValue, this.StartValue, newValue);
            }
        }

        private static void OnEndValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d != null, "d != null");
            Debug.Assert(e.OldValue != null, "e.OldValue != null");
            Debug.Assert(e.NewValue != null, "e.NewValue != null");

            var element = (RangeBaseControl<T, TInterval>)d;
            if (element.IsSingleValue)
            {
                element.StartValue = element.EndValue;
            }
            else
            {
                element.RangeValue = element.CurrentRangeValue;
            }

            element.OnEndValueChanged((T)e.OldValue, (T)e.NewValue);
        }

        #endregion

        #region MinRangeValue

        /// <summary>
        /// Gets or sets minimum available interval (range) value.
        /// </summary>
        /// <value>Minimum available interval (range) value.</value>
        [Category("Behavior")]
        [Bindable(true)]
        public TInterval MinRangeValue
        {
            get
            {
                var res = this.GetValue(MinRangeValueProperty);
                Debug.Assert(res != null, "res != null");
                return (TInterval)res;
            }

            set
            {
                this.SetValue(MinRangeValueProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty MinRangeValueProperty =
            DependencyProperty.Register(
                nameof(MinRangeValue),
                typeof(TInterval),
                typeof(RangeBaseControl<T, TInterval>),
                new FrameworkPropertyMetadata(
                    default(TInterval),
                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnMinRangeValueChanged,
                    CoerceMinRangeValue),
                IsValidIntervalValue);

        private static void OnMinRangeValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is RangeBaseControl<T, TInterval>, "obj is RangeBaseControl<T, TInterval>");

            var element = obj as RangeBaseControl<T, TInterval>;
            Debug.Assert(element != null, "element != null");
            if (element == null)
            {
                return;
            }

            element.CoerceValue(StartValueProperty);
            element.CoerceValue(EndValueProperty);
        }

        private static object CoerceMinRangeValue(DependencyObject element, object value)
        {
            Debug.Assert(element is RangeBaseControl<T, TInterval>, "element is RangeBaseControl<T, TInterval>");

            var cntrl = element as RangeBaseControl<T, TInterval>;
            Debug.Assert(cntrl != null, "cntrl != null");
            if (cntrl != null)
            {
                value = cntrl.CoerceMinRangeValue(value);
            }

            return value;
        }

        #endregion

        #region RangeValue

        /// <summary>
        /// Gets current interval (range) value.
        /// </summary>
        /// <value>Current interval (range) value.</value>
        [Category("Behavior")]
        [Bindable(true)]
        public TInterval RangeValue
        {
            get
            {
                var res = this.GetValue(RangeValueProperty);
                Debug.Assert(res != null, "res != null");
                return (TInterval)res;
            }

            private set
            {
                this.SetValue(RangeValuePropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey RangeValuePropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(RangeValue),
                typeof(TInterval),
                typeof(RangeBaseControl<T, TInterval>),
                new FrameworkPropertyMetadata(OnRangeValueChanged));

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty RangeValueProperty = RangeValuePropertyKey.DependencyProperty;

        private static void OnRangeValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is RangeBaseControl<T, TInterval>, "obj is RangeBaseControl<T, TInterval>");
            Debug.Assert(args.OldValue is TInterval, "args.OldValue is TInterval");
            Debug.Assert(args.NewValue is TInterval, "args.NewValue is TInterval");

            var element = obj as RangeBaseControl<T, TInterval>;
            Debug.Assert(element != null, "element != null");
            if (element == null)
            {
                return;
            }

            if (!element.IsRangeValueChanging)
            {
                element.OnRangeValueChanged((TInterval)args.OldValue, (TInterval)args.NewValue);
            }
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="RangeBaseControl{T, TInterval}.RangeValue"/> changes.
        /// </summary>
        /// <param name="oldValue">The value of the property before the change reported by the relevant event or state change.</param>
        /// <param name="newValue">The value of the property after the change reported by the relevant event or state change.</param>
        protected virtual void OnRangeValueChanged(TInterval oldValue, TInterval newValue)
        {
            var newEventArgs = new RoutedPropertyChangedEventArgs<TInterval>(oldValue, newValue)
            {
                RoutedEvent = RangeValueChangedEvent,
            };
            this.RaiseEvent(newEventArgs);
        }

        #endregion

        #region SmallChange

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the <see cref="StartValue"/> or <see cref="EndValue"/> of control.
        /// Uses when change by arrows keys.
        /// </summary>
        /// <value>Value to add to or subtract from the <see cref="StartValue"/> or <see cref="EndValue"/> of the element.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public TInterval SmallChange
        {
            get
            {
                var res = this.GetValue(SmallChangeProperty);
                Debug.Assert(res != null, "res != null");
                return (TInterval)res;
            }

            set
            {
                this.SetValue(SmallChangeProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.SmallChange"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
            nameof(SmallChange),
            typeof(TInterval),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(),
            IsValidChange);

        #endregion

        #region LargeChange

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the <see cref="StartValue"/> or <see cref="EndValue"/> of control.
        /// Uses when change by Page Up or Page Down.
        /// </summary>
        /// <value>Value to add to or subtract from the <see cref="StartValue"/> or <see cref="EndValue"/> of the element.</value>
        [Category("Behavior")]
        [Bindable(true)]
        public TInterval LargeChange
        {
            get
            {
                var res = this.GetValue(LargeChangeProperty);
                Debug.Assert(res != null, "res != null");
                return (TInterval)res;
            }

            set
            {
                this.SetValue(LargeChangeProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.LargeChange"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
            nameof(LargeChange),
            typeof(TInterval),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(),
            IsValidChange);

        #endregion

        #endregion

        #region Public Events

        #region StartValueChanged

        /// <summary>
        /// Routed property event for <see cref="RangeBaseControl{T, TInterval}.StartValueChanged"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly RoutedEvent StartValueChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(StartValueChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<T>),
            typeof(RangeBaseControl<T, TInterval>));

        /// <summary>
        /// Occurs when <see cref="RangeBaseControl{T, TInterval}.StartValue"/> changed
        /// </summary>
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<T> StartValueChanged
        {
            add
            {
                this.AddHandler(StartValueChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(StartValueChangedEvent, value);
            }
        }

        #endregion StartValueChanged

        #region EndValueChanged

        /// <summary>
        /// Routed property event for <see cref="RangeBaseControl{T, TInterval}.EndValueChanged"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly RoutedEvent EndValueChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(EndValueChanged),
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<T>),
            typeof(RangeBaseControl<T, TInterval>));

        /// <summary>
        /// Occurs when <see cref="RangeBaseControl{T, TInterval}.EndValue"/> changed
        /// </summary>
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<T> EndValueChanged
        {
            add
            {
                this.AddHandler(EndValueChangedEvent, value);
            }

            remove
            {
                this.RemoveHandler(EndValueChangedEvent, value);
            }
        }

        #endregion EndValueChanged

        #region RangeValueChanged

        /// <summary>
        /// Routed property event for <see cref="RangeBaseControl{T, TInterval}.RangeValueChanged"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly RoutedEvent RangeValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(RangeValueChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<TInterval>),
                typeof(RangeBaseControl<T, TInterval>));

        /// <summary>
        /// Occurs when <see cref="RangeBaseControl{T, TInterval}.RangeValue"/> changed
        /// </summary>
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<TInterval> RangeValueChanged
        {
            add { this.AddHandler(RangeValueChangedEvent, value); }
            remove { this.RemoveHandler(RangeValueChangedEvent, value); }
        }

        #endregion RangeValueChanged

        #region ValueChanged

        /// <summary>
        /// Routed property event for <see cref="RangeBaseControl{T, TInterval}.ValueChanged"/>.
        /// </summary>
        // [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(ValueChanged),
                RoutingStrategy.Bubble,
                typeof(EventHandler<RangeDragCompletedEventArgs<T>>),
                typeof(RangeBaseControl<T, TInterval>));

        /// <summary>
        /// Occurs when changed any of propeties: <see cref="RangeBaseControl{T, TInterval}.StartValue"/>, <see cref="RangeBaseControl{T, TInterval}.EndValue"/>
        /// or <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>
        /// </summary>
        public event EventHandler<RangeDragCompletedEventArgs<T>> ValueChanged
        {
            add { this.AddHandler(ValueChangedEvent, value); }
            remove { this.RemoveHandler(ValueChangedEvent, value); }
        }

        #endregion ValueChanged

        #endregion

        /// <summary>
        /// Checks whether received value is valid value of type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c>, if <paramref name="value"/> is valid value of type <typeparamref name="T"/>.</returns>
        internal static bool IsValidValue(object value)
        {
            bool res = DependencyPropertyUtil.IsValidValue(typeof(T), value);
            return res;
        }

        /// <summary>
        /// Checks whether received value is valid value of type <typeparamref name="TInterval"/>.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c>, if <paramref name="value"/> is valid value of type <typeparamref name="TInterval"/>.</returns>
        internal static bool IsValidIntervalValue(object value)
        {
            return DependencyPropertyUtil.IsValidValue(typeof(TInterval), value);
        }

        /// <summary>
        /// Checks whether received value is valid change (distance) value of type <typeparamref name="TInterval"/>.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns><c>true</c> if <paramref name="value"/> id valid change value of type <typeparamref name="TInterval"/>.</returns>
        internal static bool IsValidChange(object value)
        {
            return DependencyPropertyUtil.IsValidChange(typeof(TInterval), value);
        }
#pragma warning restore SA1202 // Elements should be ordered by access
#pragma warning restore SA1201 // Elements should appear in the correct order
    }

#pragma warning restore CA1501 // Avoid excessive inheritance
}
