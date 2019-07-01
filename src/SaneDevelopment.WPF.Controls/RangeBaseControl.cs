// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RangeBaseControl.cs" company="Sane Development">
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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace SaneDevelopment.WPF.Controls
{
    #region Range interfaces

    /// <summary>
    /// Interface for polymorphous call of <see cref="FrameworkElement.OnApplyTemplate"/>,
    /// in contrast to hardcoded implementation of that method inside of CLR.
    /// </summary>
    public interface IRangeTrackTemplatedParent<T, TInterval>
    {
        /// <summary>
        /// Method should handle <see cref="FrameworkElement.OnApplyTemplate"/>
        /// </summary>
        /// <param name="templatedParent">Templated parent</param>
        /// <param name="track">Any range track</param>
        void OnApplyRangeTrackTemplate(DependencyObject templatedParent, RangeTrack<T, TInterval> track);
    }

    /// <summary>
    /// Describes classes of interval (ranged) objects
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    /// <typeparam name="TInterval">Interval (distance) type</typeparam>
    public interface IRanged<T, TInterval>
    {
        /// <summary>
        /// Gets minimum available value
        /// </summary>
        T Minimum { get; }

        /// <summary>
        /// Gets maximum available value
        /// </summary>
        T Maximum { get; }
        
        /// <summary>
        /// Gets start interval value
        /// </summary>
        T StartValue { get; }
        
        /// <summary>
        /// Gets end interval value
        /// </summary>
        T EndValue { get; }
        
        /// <summary>
        /// Gets minimum available interval (range) value
        /// </summary>
        TInterval MinRangeValue { get; }
        
        /// <summary>
        /// Gets the indicator that this object behaves like a single value object, i.e. start value equals to end value,
        /// so interval (range) value equals to zero.
        /// </summary>
        bool IsSingleValue { get; }

        /// <summary>
        /// Method for converting <c>double</c> to value type
        /// </summary>
        /// <param name="value">Value to convert from</param>
        /// <returns>Value of type <typeparamref name="T"/></returns>
        T DoubleToValue(double value);
        
        /// <summary>
        /// Method for converting value type to <c>double</c>
        /// </summary>
        /// <param name="value">Value of type <typeparamref name="T"/></param>
        /// <returns><c>double</c> value, converted from <paramref name="value"/></returns>
        double ValueToDouble(T value);

        /// <summary>
        /// Method for converting <c>double</c> to interval type
        /// </summary>
        /// <param name="value">Value to convert from</param>
        /// <returns>Value of type <typeparamref name="TInterval"/></returns>
        TInterval DoubleToInterval(double value);
        
        /// <summary>
        /// Method for converting interval value to <c>double</c>
        /// </summary>
        /// <param name="value">Value of type <typeparamref name="TInterval"/></param>
        /// <returns><c>double</c> value, converted from <paramref name="value"/></returns>
        double IntervalToDouble(TInterval value);
    }

    #endregion

    /// <summary>
    /// Control, that provides a pair of values inside some interval
    /// </summary>
    public abstract class RangeBaseControl<T, TInterval>
        : Control, IRangeTrackTemplatedParent<T, TInterval>, IRanged<T, TInterval>
    {
        private const bool c_DefaultMinRangeValueEnabled = true;
        private bool m_MinRangeValueEnabled = c_DefaultMinRangeValueEnabled;

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        protected RangeBaseControl()
        {
            IsRangeValueChanging = false;
        }

        #region Abstract methods

        /// <summary>
        /// Method for converting a number to value type
        /// </summary>
        /// <param name="value">Number to convert</param>
        /// <returns>Value of type <typeparamref name="T"/> - representation of <paramref name="value"/></returns>
        protected abstract T DoubleToValue(double value);
        
        /// <summary>
        /// Method for converting a value type to number
        /// </summary>
        /// <param name="value">Value of type <typeparamref name="T"/></param>
        /// <returns><c>double</c> representation of <paramref name="value"/></returns>
        protected abstract double ValueToDouble(T value);

        /// <summary>
        /// Method for converting a number to interval type
        /// </summary>
        /// <param name="value">Number to convert</param>
        /// <returns>Value of type <typeparamref name="TInterval"/> - representation of <paramref name="value"/></returns>
        protected abstract TInterval DoubleToInterval(double value);
        
        /// <summary>
        /// Method for converting an interval type to number
        /// </summary>
        /// <param name="value">Value of type <typeparamref name="TInterval"/></param>
        /// <returns><c>double</c> representation of <paramref name="value"/></returns>
        protected abstract double IntervalToDouble(TInterval value);

        /// <summary>
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>
        /// </summary>
        /// <param name="value">Value to coerce</param>
        /// <returns>Coerced (if needed) value</returns>
        protected abstract object CoerceMinRangeValue(object value);
        
        /// <summary>
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.Maximum"/>
        /// </summary>
        /// <param name="value">Value to coerce</param>
        /// <returns>Coerced (if needed) value</returns>
        protected abstract object CoerceMaximum(object value);

        /// <summary>
        /// Current interval (range) value
        /// </summary>
        protected abstract TInterval CurrentRangeValue { get; }

        #endregion Abstract methods

        #region IRanged<T, TInterval> implementation

        T IRanged<T, TInterval>.DoubleToValue(double value)
        {
            return DoubleToValue(value);
        }
        
        double IRanged<T, TInterval>.ValueToDouble(T value)
        {
            return ValueToDouble(value);
        }
        
        TInterval IRanged<T, TInterval>.DoubleToInterval(double value)
        {
            return DoubleToInterval(value);
        }
        
        double IRanged<T, TInterval>.IntervalToDouble(TInterval value)
        {
            return IntervalToDouble(value);
        }

        #endregion IRanged<T, TInterval>

        /// <summary>
        /// Indicator of changing the value of the whole interval (i.e. start and end values together), but not the one of the values.
        /// If it is ON (<c>true</c>), then method <see cref="RangeBaseControl{T, TInterval}.OnRangeValueChanged(TInterval,TInterval)"/> never invokes,
        /// and therefore event <see cref="RangeBaseControl{T, TInterval}.RangeValueChanged"/> never raises.
        /// Also, methods, that can change <see cref="RangeBaseControl{T, TInterval}.IsRangeValueChanging"/>
        /// must trace these changes and call <see cref="RangeBaseControl{T, TInterval}.OnRangeValueChanged(TInterval,TInterval)"/> in appropriate moment.
        /// </summary>
        protected bool IsRangeValueChanging { get; set; }

        /// <summary>
        /// Whether or not is enabled validation over min range value.
        /// By default it is <c>true</c>, but in derived classes it can be turned OFF.
        /// Also, it turns OFF when such validation can't be performed by objective reasons,
        /// e.g. when <see cref="RangeBaseControl{T, TInterval}.IsSingleValue"/> mode is ON.
        /// </summary>
        protected bool MinRangeValueEnabled
        {
            get { return m_MinRangeValueEnabled; }
            set
            {
                if (value != m_MinRangeValueEnabled)
                {
                    m_MinRangeValueEnabled = value;
                    CoerceValue(MinRangeValueProperty);
                }
            }
        }

        /// <summary>
        /// Default value of property <see cref="RangeBaseControl{T, TInterval}.MinRangeValueEnabled"/>,
        /// which will be set to when such validation become available.
        /// E.g. when <see cref="RangeBaseControl{T, TInterval}.IsSingleValue"/> mode comes to OFF (<c>false</c>).
        /// 
        /// In derived classes this property can be overridden to control mentioned mechanism,
        /// suitable for every control individually by default.
        /// </summary>
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
                RoutedEvent = ValueChangedEvent
            };
            RaiseEvent(e);
        }

        #region IRangeTrackTemplatedParent<T>

        /// <summary>
        /// Method handles <see cref="FrameworkElement.OnApplyTemplate"/>,
        /// notably bind some dependency properties with templated parent.
        /// </summary>
        /// <param name="templatedParent">Templated parent</param>
        /// <param name="track">Any range track</param>
        public virtual void OnApplyRangeTrackTemplate(DependencyObject templatedParent, RangeTrack<T, TInterval> track)
        {
            if (track == null)
                throw new ArgumentNullException("track");

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
        /// This object behaves like a single value object, i.e. start value equals to end value,
        /// so interval (range) value equals to zero.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsSingleValue
        {
            get
            {
                var res = GetValue(IsSingleValueProperty);
                Debug.Assert(res != null);
                return (bool) res;
            }
            set { SetValue(IsSingleValueProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.IsSingleValue"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty IsSingleValueProperty = DependencyProperty.Register(
// ReSharper restore StaticFieldInGenericType
            "IsSingleValue",
            typeof(bool),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(false, OnIsSingleValueChanged));

        private static void OnIsSingleValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is RangeBaseControl<T, TInterval>);
            Debug.Assert(args.NewValue != null);

            var element = obj as RangeBaseControl<T, TInterval>;
            Debug.Assert(element != null);
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (element == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

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
        /// Minimum available value
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public T Minimum
        {
            get
            {
                var res = this.GetValue(MinimumProperty);
                Debug.Assert(res != null);
                return (T)res;
            }
            set
            {
                this.SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.Minimum"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
// ReSharper restore StaticFieldInGenericType
            "Minimum",
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
        { }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d != null);
            Debug.Assert(e.OldValue != null);
            Debug.Assert(e.NewValue != null);

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
        /// Maximum available value
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public T Maximum
        {
            get
            {
                var res = this.GetValue(MaximumProperty);
                Debug.Assert(res != null);
                return (T)res;
            }
            set
            {
                this.SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.Maximum"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
// ReSharper restore StaticFieldInGenericType
            "Maximum",
            typeof(T),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(
                OnMaximumChanged,
                CoerceMaximum),
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
        { }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d != null);
            Debug.Assert(e.OldValue != null);
            Debug.Assert(e.NewValue != null);

            var element = (RangeBaseControl<T, TInterval>)d;
            element.CoerceValue(MinRangeValueProperty);
            element.CoerceValue(StartValueProperty);
            element.CoerceValue(EndValueProperty);
            element.OnMaximumChanged((T)e.OldValue, (T)e.NewValue);
        }

        #endregion

        #region StartValue

        /// <summary>
        /// Start interval value
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public T StartValue
        {
            get
            {
                var res = this.GetValue(StartValueProperty);
                Debug.Assert(res != null);
                return (T)res;
            }
            set
            {
                this.SetValue(StartValueProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.StartValue"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty StartValueProperty = DependencyProperty.Register(
// ReSharper restore StaticFieldInGenericType
            "StartValue",
            typeof(T),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(default(T),
                FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnStartValueChanged,
                CoerceStartValue),
            IsValidValue);

        private static object CoerceStartValue(DependencyObject d, object value)
        {
            Debug.Assert(d is RangeBaseControl<T, TInterval>);

            var base2 = d as RangeBaseControl<T, TInterval>;
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (base2 == null) return value;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

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
                RoutedEvent = StartValueChangedEvent
            };
            RaiseEvent(e);

            if (!IsRangeValueChanging)
            {
                OnValueChanged(oldValue, EndValue, newValue, EndValue);
            }
        }

        private static void OnStartValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d != null);
            Debug.Assert(e.OldValue != null);
            Debug.Assert(e.NewValue != null);

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
        /// End interval value
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public T EndValue
        {
            get
            {
                var res = this.GetValue(EndValueProperty);
                Debug.Assert(res != null);
                return (T)res;
            }
            set
            {
                this.SetValue(EndValueProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.EndValue"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty EndValueProperty = DependencyProperty.Register(
// ReSharper restore StaticFieldInGenericType
            "EndValue",
            typeof(T),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(default(T),
                FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnEndValueChanged,
                CoerceEndValue),
            IsValidValue);

        private static object CoerceEndValue(DependencyObject d, object value)
        {
            Debug.Assert(d is RangeBaseControl<T, TInterval>);

            var base2 = d as RangeBaseControl<T, TInterval>;
            Debug.Assert(base2 != null);
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (base2 == null) return value;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

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
                RoutedEvent = EndValueChangedEvent
            };
            RaiseEvent(e);

            if (!IsRangeValueChanging)
            {
                OnValueChanged(StartValue, oldValue, StartValue, newValue);
            }
        }

        private static void OnEndValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d != null);
            Debug.Assert(e.OldValue != null);
            Debug.Assert(e.NewValue != null);

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
        /// Minimum available interval (range) value
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public TInterval MinRangeValue
        {
            get
            {
                var res = GetValue(MinRangeValueProperty);
                Debug.Assert(res != null);
                return (TInterval) res;
            }
            set { SetValue(MinRangeValueProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty MinRangeValueProperty =
// ReSharper restore StaticFieldInGenericType
            DependencyProperty.Register(
                "MinRangeValue",
                typeof(TInterval),
                typeof(RangeBaseControl<T, TInterval>),
                new FrameworkPropertyMetadata(default(TInterval),
                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnMinRangeValueChanged,
                    CoerceMinRangeValue),
                IsValidIntervalValue);

        private static void OnMinRangeValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is RangeBaseControl<T, TInterval>);

            var element = obj as RangeBaseControl<T, TInterval>;
            Debug.Assert(element != null, "element != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (element == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

            element.CoerceValue(StartValueProperty);
            element.CoerceValue(EndValueProperty);
        }

        private static object CoerceMinRangeValue(DependencyObject element, object value)
        {
            Debug.Assert(element is RangeBaseControl<T, TInterval>);

            var cntrl = element as RangeBaseControl<T, TInterval>;
            Debug.Assert(cntrl != null);
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (cntrl != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                value = cntrl.CoerceMinRangeValue(value);
            }

            return value;
        }

        #endregion

        #region RangeValue

        /// <summary>
        /// Current interval (range) value
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public TInterval RangeValue
        {
            get
            {
                var res = GetValue(RangeValueProperty);
                Debug.Assert(res != null);
                return (TInterval) res;
            }
            private set { SetValue(RangeValuePropertyKey, value); }
        }

// ReSharper disable InconsistentNaming
// ReSharper disable StaticFieldInGenericType
        private static readonly DependencyPropertyKey RangeValuePropertyKey =
// ReSharper restore StaticFieldInGenericType
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "RangeValue",
                typeof(TInterval),
                typeof(RangeBaseControl<T, TInterval>),
                new FrameworkPropertyMetadata(OnRangeValueChanged));

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty RangeValueProperty = RangeValuePropertyKey.DependencyProperty;
// ReSharper restore StaticFieldInGenericType

        private static void OnRangeValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is RangeBaseControl<T, TInterval>);
            Debug.Assert(args.OldValue is TInterval);
            Debug.Assert(args.NewValue is TInterval);

            var element = obj as RangeBaseControl<T, TInterval>;
            Debug.Assert(element != null, "element != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (element == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

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
                RoutedEvent = RangeValueChangedEvent
            };
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region SmallChange

        /// <summary>
        /// Gets or sets a value to be added to or subtracted from the <see cref="StartValue"/> or <see cref="EndValue"/> of control.
        /// Uses when change by arrows keys.
        /// </summary>
        /// <returns>Value to add to or subtract from the <see cref="StartValue"/> or <see cref="EndValue"/> of the element.</returns>
        [Bindable(true), Category("Behavior")]
        public TInterval SmallChange
        {
            get
            {
                var res = this.GetValue(SmallChangeProperty);
                Debug.Assert(res != null);
                return (TInterval)res;
            }
            set
            {
                this.SetValue(SmallChangeProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.SmallChange"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
// ReSharper restore StaticFieldInGenericType
            "SmallChange",
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
        /// <returns>Value to add to or subtract from the <see cref="StartValue"/> or <see cref="EndValue"/> of the element.</returns>
        [Category("Behavior"), Bindable(true)]
        public TInterval LargeChange
        {
            get
            {
                var res = this.GetValue(LargeChangeProperty);
                Debug.Assert(res != null);
                return (TInterval)res;
            }
            set
            {
                this.SetValue(LargeChangeProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeBaseControl{T, TInterval}.LargeChange"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
// ReSharper restore StaticFieldInGenericType
            "LargeChange",
            typeof(TInterval),
            typeof(RangeBaseControl<T, TInterval>),
            new FrameworkPropertyMetadata(),
            IsValidChange);

        #endregion

        #endregion

        #region Public Events

        #region StartValueChanged

        /// <summary>
        /// Routed property event for <see cref="RangeBaseControl{T, TInterval}.StartValueChanged"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly RoutedEvent StartValueChangedEvent = EventManager.RegisterRoutedEvent(
// ReSharper restore StaticFieldInGenericType
            "StartValueChanged",
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
        /// Routed property event for <see cref="RangeBaseControl{T, TInterval}.EndValueChanged"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly RoutedEvent EndValueChangedEvent = EventManager.RegisterRoutedEvent(
// ReSharper restore StaticFieldInGenericType
            "EndValueChanged",
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
        /// Routed property event for <see cref="RangeBaseControl{T, TInterval}.RangeValueChanged"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly RoutedEvent RangeValueChangedEvent =
// ReSharper restore StaticFieldInGenericType
            EventManager.RegisterRoutedEvent(
                "RangeValueChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<TInterval>),
                typeof(RangeBaseControl<T, TInterval>));

        /// <summary>
        /// Occurs when <see cref="RangeBaseControl{T, TInterval}.RangeValue"/> changed
        /// </summary>
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<TInterval> RangeValueChanged
        {
            add { AddHandler(RangeValueChangedEvent, value); }
            remove { RemoveHandler(RangeValueChangedEvent, value); }
        }

        #endregion RangeValueChanged

        #region ValueChanged

        /// <summary>
        /// Routed property event for <see cref="RangeBaseControl{T, TInterval}.ValueChanged"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly RoutedEvent ValueChangedEvent =
// ReSharper restore StaticFieldInGenericType
            EventManager.RegisterRoutedEvent(
                "ValueChanged",
                RoutingStrategy.Bubble,
                typeof(EventHandler<RangeDragCompletedEventArgs<T>>),
                typeof(RangeBaseControl<T, TInterval>));

        /// <summary>
        /// Occurs when changed any of propeties: <see cref="RangeBaseControl{T, TInterval}.StartValue"/>, <see cref="RangeBaseControl{T, TInterval}.EndValue"/>
        /// or <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>
        /// </summary>
        public event EventHandler<RangeDragCompletedEventArgs<T>> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }

        #endregion ValueChanged

        #endregion

        internal static bool IsValidValue(object value)
        {
            bool res = DependencyPropertyUtil.IsValidValue(typeof(T), value);
            return res;
        }

        internal static bool IsValidIntervalValue(object value)
        {
            return DependencyPropertyUtil.IsValidValue(typeof(TInterval), value);
        }

        internal static bool IsValidChange(object value)
        {
            return DependencyPropertyUtil.IsValidChange(typeof(TInterval), value);
        }
    }
}
