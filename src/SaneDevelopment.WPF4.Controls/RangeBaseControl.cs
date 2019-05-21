// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RangeBaseControl.cs" company="Sane Development">
//
//   Sane Development WPF Controls Library
//
//   The BSD 3-Clause License
//
//   Copyright (c) 2011-2019, Sane Development
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
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;

namespace SaneDevelopment.WPF4.Controls
{
    #region Range interfaces

    /// <summary>
    /// Интерфейс для полиморфного вызова метода OnApplyTemplate,
    /// в отличие от жестко зашитой реализации этого метода в CLR
    /// </summary>
    public interface IRangeTrackTemplatedParent<T, TInterval>
    {
        /// <summary>
        /// Метод выполняет обработку <see cref="FrameworkElement.OnApplyTemplate"/>
        /// </summary>
        /// <param name="templatedParent">Шаблонный родитель</param>
        /// <param name="track">Интервальный трэк</param>
        void OnApplyRangeTrackTemplate(DependencyObject templatedParent, RangeTrack<T, TInterval> track);
    }

    /// <summary>
    /// Интерфейс описывает класс, реализующий логику интервального объекта,
    /// </summary>
    /// <typeparam name="T">Тип значений</typeparam>
    /// <typeparam name="TInterval">Тип интервала между значениями</typeparam>
    public interface IRanged<T, TInterval>
    {
        /// <summary>
        /// Минимально допустимое значение
        /// </summary>
        T Minimum { get; }
        /// <summary>
        /// Максимально допустимое значение
        /// </summary>
        T Maximum { get; }
        /// <summary>
        /// Значение начала интервала
        /// </summary>
        T StartValue { get; }
        /// <summary>
        /// Значение конца интервала
        /// </summary>
        T EndValue { get; }
        /// <summary>
        /// Минимально допустимая величина интервала
        /// </summary>
        TInterval MinRangeValue { get; }
        /// <summary>
        /// Для данного объекта реализуется логика единого значения, когда начало интевала совпадает с его концом,
        /// а величина самого интервала, соответственно, равна нулю.
        /// </summary>
        bool IsSingleValue { get; }

        /// <summary>
        /// Метод преобразования числа в значение
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Объект типа <typeparamref name="T"/></returns>
        T DoubleToValue(double value);
        /// <summary>
        /// Метод преобразования значения в число
        /// </summary>
        /// <param name="value">Объект типа <typeparamref name="T"/></param>
        /// <returns>Число</returns>
        double ValueToDouble(T value);

        /// <summary>
        /// Метод преобразования числа в интервал
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Объект типа <typeparamref name="TInterval"/></returns>
        TInterval DoubleToInterval(double value);
        /// <summary>
        /// Метод преобразования интервала в число
        /// </summary>
        /// <param name="value">Объект типа <typeparamref name="TInterval"/></param>
        /// <returns>Число</returns>
        double IntervalToDouble(TInterval value);
    }

    #endregion

    /// <summary>
    /// Представляет собой элемент, имеющий пару значений в заданном интервале.
    /// </summary>
    public abstract class RangeBaseControl<T, TInterval>
        : Control, IRangeTrackTemplatedParent<T, TInterval>, IRanged<T, TInterval>
    {
        private const bool c_DefaultMinRangeValueEnabled = true;
        private bool m_MinRangeValueEnabled = c_DefaultMinRangeValueEnabled;

        /// <summary>
        /// Инициализирует новый объект
        /// </summary>
        protected RangeBaseControl()
        {
            IsRangeValueChanging = false;
        }

        /// <summary>
        /// Метод преобразования числа в значение
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Объект типа <typeparamref name="T"/></returns>
        protected abstract T DoubleToValue(double value);
        /// <summary>
        /// Метод преобразования значения в число
        /// </summary>
        /// <param name="value">Объект типа <typeparamref name="T"/></param>
        /// <returns>Число</returns>
        protected abstract double ValueToDouble(T value);

        /// <summary>
        /// Метод преобразования числа в интервал
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Объект типа <typeparamref name="TInterval"/></returns>
        protected abstract TInterval DoubleToInterval(double value);
        /// <summary>
        /// Метод преобразования интервала в число
        /// </summary>
        /// <param name="value">Объект типа <typeparamref name="TInterval"/></param>
        /// <returns>Число</returns>
        protected abstract double IntervalToDouble(TInterval value);

        /// <summary>
        /// Метод выполняет корректировку устанавливаемого значения свойства <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>
        /// </summary>
        /// <param name="value">Устанавливаемое значение</param>
        /// <returns>Скорректированное (если необходимо) значение</returns>
        protected abstract object CoerceMinRangeValue(object value);
        /// <summary>
        /// Метод выполняет корректировку устанавливаемого значения свойства <see cref="RangeBaseControl{T, TInterval}.Maximum"/>
        /// </summary>
        /// <param name="value">Устанавливаемое значение</param>
        /// <returns>Скорректированное (если необходимо) значение</returns>
        protected abstract object CoerceMaximum(object value);

        /// <summary>
        /// Текущая величина интервала
        /// </summary>
        protected abstract TInterval CurrentRangeValue { get; }

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

        /// <summary>
        /// Признак того, что в данный момент происходит изменение всего интервала (т.е. значений начала и конца),
        /// а не отдельного значения.
        /// Если этот признак взведен (<c>true</c>), то метод <see cref="RangeBaseControl{T, TInterval}.OnRangeValueChanged(TInterval,TInterval)"/> не вызывается,
        /// и, как следствие, событие <see cref="RangeBaseControl{T, TInterval}.RangeValueChanged"/> не генерируется.
        /// При этом, функции, устанавливающие признак <see cref="RangeBaseControl{T, TInterval}.IsRangeValueChanging"/>,
        /// должны самостоятельно следить за его изменением и в нужный момент вызывать <see cref="RangeBaseControl{T, TInterval}.OnRangeValueChanged(TInterval,TInterval)"/>.
        /// </summary>
        protected bool IsRangeValueChanging { get; set; }

        /// <summary>
        /// Разрешено или нет в текущий момент выполнять проверку на минимальную величину интервала.
        /// По умолчанию <c>true</c>, но в наследниках это свойство можно отключать.
        /// Так же это свойство отключается, когда выполнять указанную проверку невозможно по объективным причинам,
        /// например, если включен режим единственного значения <see cref="RangeBaseControl{T, TInterval}.IsSingleValue"/>.
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
        /// Свойство возвращает значение свойства <see cref="RangeBaseControl{T, TInterval}.MinRangeValueEnabled"/>,
        /// которое оно получает, когда механизм выполнения проверки на минимальную величину интервала становится возможным в принципе,
        /// например, если отключается режим единственного значения <see cref="RangeBaseControl{T, TInterval}.IsSingleValue"/>.
        /// В наследниках это свойство можно перегружать, чтобы контролировать указанный механизм,
        /// заданный по умолчанию для того или иного элемента управления.
        /// </summary>
        protected virtual bool DefaultMinRangeValueEnabled
        {
            get { return c_DefaultMinRangeValueEnabled; }
        }

        /// <summary>
        /// Вызывается, когда меняется хотя бы одно из значений <see cref="RangeBaseControl{T, TInterval}.StartValue"/>,
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/> или <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>.
        /// </summary>
        /// <param name="oldStartValue">Старое значение начала интервала</param>
        /// <param name="oldEndValue">Старое значение конца интервала</param>
        /// <param name="newStartValue">Новое значение начала интервала</param>
        /// <param name="newEndValue">Новое значение конца интервала</param>
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
        /// Метод выполняет обработку <see cref="FrameworkElement.OnApplyTemplate"/>, а именно,
        /// связывает некоторые свойства зависимостей с шаблонным родителем.
        /// </summary>
        /// <param name="templatedParent">Шаблонный родитель</param>
        /// <param name="track">Интервальный трэк</param>
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
        /// Для данного объекта реализуется логика единого значения, когда начало интевала совпадает с его концом,
        /// а величина самого интервала, соответственно, равна нулю.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsSingleValue
        {
            get
            {
                var res = GetValue(IsSingleValueProperty);
                Contract.Assume(res != null);
                return (bool) res;
            }
            set { SetValue(IsSingleValueProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.IsSingleValue"/>
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
            Contract.Requires(obj is RangeBaseControl<T, TInterval>);
            Contract.Requires(args.NewValue != null);

            var element = obj as RangeBaseControl<T, TInterval>;
            Contract.Assert(element != null);
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (element == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

            var newValue = (bool)args.NewValue;
            if (newValue)
            {
                // если для слайдера включается режим единого (одного) значения,
                // то проверка на минимальный размер интервала отключается, т.к. теперь он заведомо должен быть равен нулю.
                element.MinRangeValueEnabled = false;

                // если флажок взводится (теперь ползунки могут быть только на одном значении),
                // то надо принудительно выравнять текущее положение правого ползунка до левого.
                element.EndValue = element.StartValue;
                // принудительно выставляем значению RangeValue ноль,
                // т.к. значение интервала из-за наличия флага IsSingleValue теперь пересчитываться не будет,
                // чтобы всегда держать значение, равным нулю.
                // Здесь мы просто принудительно выбрасываем событие об обнулении величины интервала.
                element.RangeValue = element.DoubleToInterval(0.0);
            }
            else
            {
                // если для слайдера отключается режим единого (одного) значения,
                // то проверка на минимальный размер интервала можно включить, а можно и не включать -
                // все зависит от значения этого признака по умолчанию в элементе управления.
                element.MinRangeValueEnabled = element.DefaultMinRangeValueEnabled;
            }
        }

        #endregion


        #region Minimum

        /// <summary>
        /// Минимально допустимое значение
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public T Minimum
        {
            get
            {
                var res = this.GetValue(MinimumProperty);
                Contract.Assume(res != null);
                return (T)res;
            }
            set
            {
                this.SetValue(MinimumProperty, value);
            }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.Minimum"/>
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
        /// Вызывается, когда меняется значение <see cref="RangeBaseControl{T, TInterval}.Minimum"/>
        /// </summary>
        /// <param name="oldMinimum">Старое значение</param>
        /// <param name="newMinimum">Новое значение</param>
        protected virtual void OnMinimumChanged(T oldMinimum, T newMinimum)
        { }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Contract.Requires(d != null);
            Contract.Requires(e.OldValue != null);
            Contract.Requires(e.NewValue != null);

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
        /// Максимально допустимое значение
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public T Maximum
        {
            get
            {
                var res = this.GetValue(MaximumProperty);
                Contract.Assume(res != null);
                return (T)res;
            }
            set
            {
                this.SetValue(MaximumProperty, value);
            }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.Maximum"/>
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
        /// Вызывается, когда меняется значение <see cref="RangeBaseControl{T, TInterval}.Maximum"/>
        /// </summary>
        /// <param name="oldMaximum">Старое значение</param>
        /// <param name="newMaximum">Новое значение</param>
        protected virtual void OnMaximumChanged(T oldMaximum, T newMaximum)
        { }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Contract.Requires(d != null);
            Contract.Requires(e.OldValue != null);
            Contract.Requires(e.NewValue != null);

            var element = (RangeBaseControl<T, TInterval>)d;
            element.CoerceValue(MinRangeValueProperty);
            element.CoerceValue(StartValueProperty);
            element.CoerceValue(EndValueProperty);
            element.OnMaximumChanged((T)e.OldValue, (T)e.NewValue);
        }

        #endregion

        #region StartValue

        /// <summary>
        /// Значение начала интервала
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public T StartValue
        {
            get
            {
                var res = this.GetValue(StartValueProperty);
                Contract.Assume(res != null);
                return (T)res;
            }
            set
            {
                this.SetValue(StartValueProperty, value);
            }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.StartValue"/>
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
            Contract.Requires(d is RangeBaseControl<T, TInterval>);

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
        /// Вызывается, когда меняется значение <see cref="RangeBaseControl{T, TInterval}.StartValue"/>
        /// </summary>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
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
            Contract.Requires(d != null);
            Contract.Requires(e.OldValue != null);
            Contract.Requires(e.NewValue != null);

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
        /// Значение конца интервала
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public T EndValue
        {
            get
            {
                var res = this.GetValue(EndValueProperty);
                Contract.Assume(res != null);
                return (T)res;
            }
            set
            {
                this.SetValue(EndValueProperty, value);
            }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.EndValue"/>
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
            Contract.Requires(d is RangeBaseControl<T, TInterval>);

            var base2 = d as RangeBaseControl<T, TInterval>;
            Contract.Assert(base2 != null);
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
        /// Вызывается, когда меняется значение <see cref="RangeBaseControl{T, TInterval}.EndValue"/>
        /// </summary>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
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
            Contract.Requires(d != null);
            Contract.Requires(e.OldValue != null);
            Contract.Requires(e.NewValue != null);

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
        /// Минимально допустимая величина интервала
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public TInterval MinRangeValue
        {
            get
            {
                var res = GetValue(MinRangeValueProperty);
                Contract.Assume(res != null);
                return (TInterval) res;
            }
            set { SetValue(MinRangeValueProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>
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
            Contract.Requires(obj is RangeBaseControl<T, TInterval>);

            var element = obj as RangeBaseControl<T, TInterval>;
            Contract.Assert(element != null, "element != null");
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
            Contract.Requires(element is RangeBaseControl<T, TInterval>);

            var cntrl = element as RangeBaseControl<T, TInterval>;
            Contract.Assert(cntrl != null);
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
        /// Текущая величина интервала
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public TInterval RangeValue
        {
            get
            {
                var res = GetValue(RangeValueProperty);
                Contract.Assume(res != null);
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
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty RangeValueProperty = RangeValuePropertyKey.DependencyProperty;
// ReSharper restore StaticFieldInGenericType

        private static void OnRangeValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Contract.Requires(obj is RangeBaseControl<T, TInterval>);
            Contract.Requires(args.OldValue is TInterval);
            Contract.Requires(args.NewValue is TInterval);

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
        /// Вызывается, когда меняется значение <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>
        /// </summary>
        /// <param name="oldValue">Старое значение</param>
        /// <param name="newValue">Новое значение</param>
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
        /// Величина маленького смещения значения или интервала. Используется при смещении при помощи стрелок.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public TInterval SmallChange
        {
            get
            {
                var res = this.GetValue(SmallChangeProperty);
                Contract.Assume(res != null);
                return (TInterval)res;
            }
            set
            {
                this.SetValue(SmallChangeProperty, value);
            }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.SmallChange"/>
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
        /// Величина большого смещения значения или интервала. Используется при смещении при помощи клавиш Page Up и Page Down.
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public TInterval LargeChange
        {
            get
            {
                var res = this.GetValue(LargeChangeProperty);
                Contract.Assume(res != null);
                return (TInterval)res;
            }
            set
            {
                this.SetValue(LargeChangeProperty, value);
            }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.LargeChange"/>
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

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.StartValueChanged"/>
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
        /// Событие изменения значения свойства <see cref="RangeBaseControl{T, TInterval}.StartValue"/>
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

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.EndValueChanged"/>
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
        /// Событие изменения значения свойства <see cref="RangeBaseControl{T, TInterval}.EndValue"/>
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

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.RangeValueChanged"/>
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
        /// Событие изменения значения свойства <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>
        /// </summary>
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<TInterval> RangeValueChanged
        {
            add { AddHandler(RangeValueChangedEvent, value); }
            remove { RemoveHandler(RangeValueChangedEvent, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="RangeBaseControl{T, TInterval}.ValueChanged"/>
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
        /// Событие изменения любого из свойств <see cref="RangeBaseControl{T, TInterval}.StartValue"/>, <see cref="RangeBaseControl{T, TInterval}.EndValue"/>
        /// или <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>
        /// </summary>
        public event EventHandler<RangeDragCompletedEventArgs<T>> ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }


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
