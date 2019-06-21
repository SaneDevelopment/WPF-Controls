// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomBar.cs" company="Sane Development">
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Зум-панель.
    /// Представляет собой элемент управления с содержимым, позволяющий наглядно масштабировать свое содержимое,
    /// отрезая от него поля настраиваемой ширины. Таким образом реализуется свого рода элемент предварительного просмотра.
    /// </summary>
    [TemplatePart(Name = "PART_ContentContainer", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_RangeSlider", Type = typeof(SimpleNumericRangeSlider))]
    [TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_ShiftLeftButton", Type = typeof(ButtonBase))]
    [TemplatePart(Name = "PART_ShiftRightButton", Type = typeof(ButtonBase))]
    [Description("Sane Zoombar")]
    public class ZoomBar : ContentControl, IRanged<double, double>
    {
        private const string c_ShiftLeftCommandName = "ShiftLeftCommand";
        private const string c_ShiftRightCommandName = "ShiftRightCommand";

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ZoomBar()
        {
            InitializeCommands();

            // Listen to MouseLeftButtonDown event to determine if slide should move focus to itself
            EventManager.RegisterClassHandler(typeof(ZoomBar),
                Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);

            Debug.Assert(DefaultMinimum < DefaultMaximum, "DefaultMinimum <= DefaultMaximum");

            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomBar),
                new FrameworkPropertyMetadata(typeof(ZoomBar)));
        }

        #region Default constants

        /// <summary>
        /// Минимальное значение, задаваемое по умолчанию
        /// </summary>
        public const double DefaultMinimum = 0.0;

        /// <summary>
        /// Максимальное значение, задаваемое по умолчанию
        /// </summary>
        public const double DefaultMaximum = 100.0;

        /// <summary>
        /// Значение сдвига, задаваемое по умолчанию
        /// </summary>
        public const double DefaultShiftValue = 5.0;

        /// <summary>
        /// Минимально возможное значение сдвига
        /// </summary>
        public const double MinShiftValue = 1.0;

        /// <summary>
        /// Начальное значение области выделения, задаваемое по умолчанию
        /// </summary>
        public const double DefaultSelectionStart = DefaultMinimum;

        /// <summary>
        /// Конечно значение области выделения, задаваемое по умолчанию
        /// </summary>
        public const double DefaultSelectionEnd = DefaultMaximum;

        private const double c_DefaultSelectionRange = DefaultSelectionEnd - DefaultSelectionStart;

        /// <summary>
        /// Толщина границы области выделения, задаваемое по умолчанию
        /// </summary>
        public const double DefaultSelectionBorderThickness = 1.0;

        /// <summary>
        /// Прозрачность заливки области за пределами области выделения (т.н. "область невыделения" или "отбрасываемая область"), задаваемое по умолчанию
        /// </summary>
        public const double DefaultNotSelectedOpacity = 0.7;

        /// <summary>
        /// Прозрачность границы области выделения, задаваемое по умолчанию
        /// </summary>
        public const double DefaultSelectionBorderOpacity = 0.5;

        /// <summary>
        /// Default value of start and end thumbs size (width or height depending on <see cref="Orientation"/>)
        /// </summary>
        public const double DefaultThumbSize = 10.0;

        #endregion

        /// <summary>
        /// Содержимое элемента управления
        /// </summary>
        protected FrameworkElement ContentContainer { get; set; }

        /// <summary>
        /// Слайдер, на основе которого строится область выделения
        /// </summary>
        protected SimpleNumericRangeSlider RangeSlider { get; set; }

        /// <summary>
        /// Начальный ползунок
        /// </summary>
        protected Thumb StartThumb { get; set; }

        /// <summary>
        /// Конечный ползунок
        /// </summary>
        protected Thumb EndThumb { get; set; }

        /// <summary>
        /// Кнопка сдвига области выделения влево (вниз)
        /// </summary>
        protected ButtonBase ShiftLeftButton { get; set; }

        /// <summary>
        /// Кнопка сдвига области выделения вправо (вверх)
        /// </summary>
        protected ButtonBase ShiftRightButton { get; set; }

        private void RefreshContentIndentValues()
        {
            if (this.ContentContainer == null)
                return;

            var generalTransform = this.ContentContainer.TransformToAncestor(this);
            if (generalTransform == null)
                return;

            Point relativePoint = generalTransform.Transform(new Point(0, 0));
            double leftIndent = this.Orientation == Orientation.Horizontal ?
                                    relativePoint.X :
                                    relativePoint.Y;
            double rightIndent = this.Orientation == Orientation.Horizontal ?
                                     this.ActualWidth - leftIndent - this.ContentContainer.ActualWidth :
                                     this.ActualHeight - leftIndent - this.ContentContainer.ActualHeight;
            this.LeftContentIndent = leftIndent;
            this.RightContentIndent = rightIndent;
        }

        private void ResizeContentContainerMargin()
        {
            if (this.ContentContainer == null)
            {
                return;
            }

            // делаем справа и слева от краев области содержимого отступ, равный ширине ползунков,
            // чтобы в случае, когда ползунок сдвигался до упора он не загораживал бы собой области содержимого
            double left = 0.0, top = 0.0, right = 0.0, bottom = 0.0;
            if (this.StartThumb != null)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    left = this.StartThumb.ActualWidth + this.StartThumb.Margin.Left + this.StartThumb.Margin.Right;
                }
                else
                {
                    bottom = this.StartThumb.ActualHeight + this.StartThumb.Margin.Bottom + this.StartThumb.Margin.Top;
                }
            }
            if (this.EndThumb != null)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    right = this.EndThumb.ActualWidth + this.EndThumb.Margin.Left + this.EndThumb.Margin.Right;
                }
                else
                {
                    top = this.EndThumb.ActualHeight + this.EndThumb.Margin.Bottom + this.EndThumb.Margin.Top;
                }
            }

            this.ContentContainer.Margin = new Thickness(left, top, right, bottom);
        }

        #region Dependency properties

        #region Orientation Property

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.Orientation"/>
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender),
                DependencyPropertyUtil.IsValidOrientation);

        /// <summary>
        /// Ориентация контрола
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public Orientation Orientation
        {
            get
            {
                var res = GetValue(OrientationProperty);
                Contract.Assume(res != null);
                return (Orientation) res;
            }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region Minimum

        /// <summary>
        /// Минимальное значение
        /// </summary>
        [Category("Common")]
        public double Minimum
        {
            get
            {
                var res = GetValue(MinimumProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
        }

// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey MinimumPropertyKey =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "Minimum",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultMinimum));

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.Minimum"/>
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = MinimumPropertyKey.DependencyProperty;

        #endregion

        #region Maximum

        /// <summary>
        /// Максимальное значение
        /// </summary>
        [Category("Common")]
        public double Maximum
        {
            get
            {
                var res = GetValue(MaximumProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
        }

// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey MaximumPropertyKey =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "Maximum",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultMaximum));

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.Maximum"/>
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = MaximumPropertyKey.DependencyProperty;

        #endregion

        #region ShiftValue

        /// <summary>
        /// Величина сдвига
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double ShiftValue
        {
            get
            {
                var res = GetValue(ShiftValueProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            set { SetValue(ShiftValueProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.ShiftValue"/>
        /// </summary>
        public static readonly DependencyProperty ShiftValueProperty =
            DependencyProperty.Register(
                "ShiftValue",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultShiftValue,
                    OnShiftValueChanged,
                    CoerceShiftValue));

        private static void OnShiftValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        { }

        private static object CoerceShiftValue(DependencyObject element, object value)
        {
            Contract.Requires(element is ZoomBar);

            double newValue =
                DependencyPropertyUtil.ExtractDouble(
                    value,
                    ShiftValueProperty.DefaultMetadata == null
                        ? 0.0
                        : (double) (ShiftValueProperty.DefaultMetadata.DefaultValue ?? DefaultShiftValue));

            var zoombar = element as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (zoombar != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                if (newValue < MinShiftValue)
                {
                    newValue = MinShiftValue;
                }
                else if (newValue > zoombar.Maximum)
                {
                    newValue = zoombar.Maximum;
                }
            }

            return newValue;
        }

        #endregion

        #region SelectionStart

        /// <summary>
        /// Начало области выделения
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double SelectionStart
        {
            get
            {
                var res = GetValue(SelectionStartProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            set { SetValue(SelectionStartProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionStart"/>
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.Register(
                "SelectionStart",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultSelectionStart,
                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectionStartChanged,
                    CoerceSelectionStart));

        private static void OnSelectionStartChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Contract.Requires(obj is ZoomBar);
            Contract.Requires(args.OldValue is double);
            Contract.Requires(args.NewValue is double);

            var zoombar = obj as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (zoombar != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                zoombar.OnSelectionStartChanged((double)args.OldValue, (double)args.NewValue);
            }
        }

        private static object CoerceSelectionStart(DependencyObject d, object value)
        {
            Contract.Requires(d is ZoomBar);
            Contract.Requires(value != null);

            var base2 = d as ZoomBar;
            Contract.Assert(base2 != null);
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (base2 != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                var doubleVal = (double)value;
                value = DependencyPropertyUtil.CoerceRangeStartValue(base2, doubleVal);
            }
            return value;
        }

        /// <summary>
        /// Обработчик изменения значения свойства <see cref="ZoomBar.SelectionStart"/>
        /// </summary>
        /// <param name="oldValue">Предыдущее значение</param>
        /// <param name="newValue">Новое значение</param>
        protected virtual void OnSelectionStartChanged(double oldValue, double newValue)
        {
            var newEventArgs = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = SelectionStartChangedEvent
            };
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region SelectionEnd

        /// <summary>
        /// Конец области выделения
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double SelectionEnd
        {
            get
            {
                var res = GetValue(SelectionEndProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            set { SetValue(SelectionEndProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionEnd"/>
        /// </summary>
        public static readonly DependencyProperty SelectionEndProperty =
            DependencyProperty.Register(
                "SelectionEnd",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultSelectionEnd,
                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectionEndChanged,
                    CoerceSelectionEnd));

        private static void OnSelectionEndChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Contract.Requires(obj is ZoomBar);
            Contract.Requires(args.OldValue is double);
            Contract.Requires(args.NewValue is double);

            var zoombar = obj as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (zoombar == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

            zoombar.OnSelectionEndChanged((double)args.OldValue, (double)args.NewValue);
        }

        private static object CoerceSelectionEnd(DependencyObject d, object value)
        {
            Contract.Requires(d is ZoomBar);
            Contract.Requires(value != null);

            var base2 = d as ZoomBar;
            Contract.Assert(base2 != null);
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (base2 != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                var doubleVal = (double)value;
                value = DependencyPropertyUtil.CoerceRangeEndValue(base2, doubleVal);
            }
            return value;
        }

        /// <summary>
        /// Обработчик изменения значения свойства <see cref="ZoomBar.SelectionEnd"/>
        /// </summary>
        /// <param name="oldValue">Предыдущее значение</param>
        /// <param name="newValue">Новое значение</param>
        protected virtual void OnSelectionEndChanged(double oldValue, double newValue)
        {
            var newEventArgs = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = SelectionEndChangedEvent
            };
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region MinSelectionRange

        /// <summary>
        /// Минимально допустимое значение ширины области выделения
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double MinSelectionRange
        {
            get
            {
                var res = GetValue(MinSelectionRangeProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            set { SetValue(MinSelectionRangeProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.MinSelectionRange"/>
        /// </summary>
        public static readonly DependencyProperty MinSelectionRangeProperty =
            DependencyProperty.Register(
                "MinSelectionRange",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    null,
                    CoerceMinSelectionRangeValue));

        private static object CoerceMinSelectionRangeValue(DependencyObject element, object value)
        {
            Contract.Requires(element is ZoomBar);

            var cntrl = element as ZoomBar;
            Debug.Assert(cntrl != null, "cntrl != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (cntrl != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                double newValue =
                    DependencyPropertyUtil.ExtractDouble(
                        value,
                        MinSelectionRangeProperty.DefaultMetadata == null
                            ? 0.0
                            : (double) (MinSelectionRangeProperty.DefaultMetadata.DefaultValue ?? 0.0));

                if (DoubleUtil.LessThan(newValue, 0.0))
                {
                    newValue = 0.0;
                }
                else
                {
                    double range = cntrl.Maximum - cntrl.Minimum;
                    if (DoubleUtil.GreaterThan(newValue, range))
                    {
                        newValue = range;
                    }
                }

                value = newValue;
            }

            return value;
        }

        #endregion

        #region SelectionRange

        /// <summary>
        /// Текущая величина (ширина/высота) области выделения
        /// </summary>
        [Category("Common")]
        public double SelectionRange
        {
            get
            {
                var res = GetValue(SelectionRangeProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            private set { SetValue(SelectionRangePropertyKey, value); }
        }

// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey SelectionRangePropertyKey =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "SelectionRange",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(c_DefaultSelectionRange,
                    OnSelectionRangeChanged));

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionRange"/>
        /// </summary>
        public static readonly DependencyProperty SelectionRangeProperty = SelectionRangePropertyKey.DependencyProperty;

        private static void OnSelectionRangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Contract.Requires(obj is ZoomBar);
            Contract.Requires(args.OldValue is double);
            Contract.Requires(args.NewValue is double);

            var zoombar = obj as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (zoombar != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                zoombar.OnSelectionRangeChanged((double)args.OldValue, (double)args.NewValue);
            }
        }

        /// <summary>
        /// Обработчик изменения значения свойства <see cref="ZoomBar.SelectionRange"/>
        /// </summary>
        /// <param name="oldValue">Предыдущее значение</param>
        /// <param name="newValue">Новое значение</param>
        protected virtual void OnSelectionRangeChanged(double oldValue, double newValue)
        {
            var newEventArgs = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = SelectionRangeChangedEvent
            };
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region LeftContentIndent

        /// <summary>
        /// Отступ слева/снизу содержимого контрола, относительно крайней левой/нижней точки контрола.
        /// Показывает текущее положение содержимого относительно самого контрола.
        /// </summary>
        [Category("Layout")]
        public double LeftContentIndent
        {
            get
            {
                var res = GetValue(LeftContentIndentProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            private set { SetValue(LeftContentIndentPropertyKey, value); }
        }

// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey LeftContentIndentPropertyKey =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "LeftContentIndent",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.LeftContentIndent"/>
        /// </summary>
        public static readonly DependencyProperty LeftContentIndentProperty = LeftContentIndentPropertyKey.DependencyProperty;

        #endregion

        #region RightContentIndent

        /// <summary>
        /// Отступ справа/сверху содержимого контрола, относительно крайней правой/верхней точки контрола.
        /// Показывает текущее положение содержимого относительно самого контрола.
        /// </summary>
        [Category("Layout")]
        public double RightContentIndent
        {
            get
            {
                var res = GetValue(RightContentIndentProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            private set { SetValue(RightContentIndentPropertyKey, value); }
        }

// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey RightContentIndentPropertyKey =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "RightContentIndent",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.RightContentIndent"/>
        /// </summary>
        public static readonly DependencyProperty RightContentIndentProperty = RightContentIndentPropertyKey.DependencyProperty;

        #endregion

        #region IsSelectionDragging

        /// <summary>
        /// Находится ли контрол в состоянии изменения значения области выделения путем перетаскивания любого из ползунков.
        /// Позволяет узнать о том, что пользователь находится в процессе выбора величины области выделения,
        /// захватив и перемещая в данный момент один из ползунков.
        /// </summary>
        [Category("Common")]
        public bool IsSelectionDragging
        {
            get
            {
                var res = GetValue(IsSelectionDraggingProperty);
                Contract.Assume(res != null);
                return (bool) res;
            }
            private set { SetValue(IsSelectionDraggingPropertyKey, value); }
        }

// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey IsSelectionDraggingPropertyKey =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "IsSelectionDragging",
                typeof(bool),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(false,
                    OnIsSelectionDraggingChanged));

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.IsSelectionDragging"/>
        /// </summary>
        public static readonly DependencyProperty IsSelectionDraggingProperty = IsSelectionDraggingPropertyKey.DependencyProperty;

        private static void OnIsSelectionDraggingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Contract.Requires(obj is ZoomBar);
            Contract.Requires(args.OldValue is bool);
            Contract.Requires(args.NewValue is bool);

            var zoombar = obj as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (zoombar != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                Contract.Assert(args.OldValue is bool);
                Contract.Assert(args.NewValue is bool);
                zoombar.OnIsSelectionDraggingChanged((bool)args.OldValue, (bool)args.NewValue);
            }
        }

        /// <summary>
        /// Обработчик изменения значения свойства <see cref="ZoomBar.IsSelectionDragging"/>
        /// </summary>
        /// <param name="oldValue">Предыдущее значение</param>
        /// <param name="newValue">Новое значение</param>
        protected virtual void OnIsSelectionDraggingChanged(bool oldValue, bool newValue)
        {
            var newEventArgs = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue)
            {
                RoutedEvent = IsSelectionDraggingChangedEvent
            };
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region IsRaiseSelectionChangedWhileDragging Property

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.IsRaiseSelectionChangedWhileDragging"/>
        /// </summary>
        public static readonly DependencyProperty IsRaiseSelectionChangedWhileDraggingProperty =
            DependencyProperty.Register(
                "IsRaiseSelectionChangedWhileDragging",
                typeof(bool),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender),
                DependencyPropertyUtil.IsValidBoolValue);

        /// <summary>
        /// Генерировать событие <see cref="ZoomBar.SelectionChanged"/>,
        /// когда элемент находится в состоянии изменения значения области выделения путем перетаскивания любого из ползунков,
        /// иными словами, когда <see cref="ZoomBar.IsSelectionDragging"/> <c>== true</c>.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsRaiseSelectionChangedWhileDragging
        {
            get
            {
                var res = GetValue(IsRaiseSelectionChangedWhileDraggingProperty);
                Contract.Assume(res != null);
                return (bool) res;
            }
            set { SetValue(IsRaiseSelectionChangedWhileDraggingProperty, value); }
        }

        #endregion


        #region AutoToolTipValueConverter Property

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.AutoToolTipValueConverter"/>
        /// </summary>
        public static readonly DependencyProperty AutoToolTipValueConverterProperty =
            DependencyProperty.Register(
                "AutoToolTipValueConverter",
                typeof(IRangeValueToStringConverter<double>),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Конвертер значений в их строковые представления для отображения во всплывающих подсказках.
        /// Если задано это значение, то <see cref="ZoomBar.AutoToolTipFormat"/> игнорируется.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public IRangeValueToStringConverter<double> AutoToolTipValueConverter
        {
            get { return (IRangeValueToStringConverter<double>)GetValue(AutoToolTipValueConverterProperty); }
            set { SetValue(AutoToolTipValueConverterProperty, value); }
        }

        #endregion

        #region AutoToolTipValueConverterParameter Property

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.AutoToolTipValueConverterParameter"/>
        /// </summary>
        public static readonly DependencyProperty AutoToolTipValueConverterParameterProperty =
            DependencyProperty.Register(
                "AutoToolTipValueConverterParameter",
                typeof(object),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Параметр конвертера числовых значений в их строковые представления
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public object AutoToolTipValueConverterParameter
        {
            get { return GetValue(AutoToolTipValueConverterParameterProperty); }
            set { SetValue(AutoToolTipValueConverterParameterProperty, value); }
        }

        #endregion

        #region AutoToolTipFormat Property

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.AutoToolTipFormat"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty AutoToolTipFormatProperty
            = DependencyProperty.Register("AutoToolTipFormat", typeof(string), typeof(ZoomBar),
                                          new FrameworkPropertyMetadata());

        /// <summary>
        /// Формат отображения числа во всплывающей подсказке.
        /// Используется только, если не задан <see cref="ZoomBar.AutoToolTipValueConverter"/>.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public string AutoToolTipFormat
        {
            get
            {
                return (string)GetValue(AutoToolTipFormatProperty);
            }
            set
            {
                SetValue(AutoToolTipFormatProperty, value);
            }
        }

        #endregion

        #region AutoToolTipPrecision Property

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.AutoToolTipPrecision"/>
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register(
            "AutoToolTipPrecision",
            typeof(int),
            typeof(ZoomBar),
            new FrameworkPropertyMetadata(4),
            DependencyPropertyUtil.IsValidAutoToolTipPrecision);

        /// <summary>
        /// Точность отображения чисел с плавающей точкой во всплывающей подсказке, используемой по умолчанию.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public int AutoToolTipPrecision
        {
            get
            {
                var res = GetValue(AutoToolTipPrecisionProperty);
                Contract.Assume(res != null);
                return (int)res;
            }
            set
            {
                SetValue(AutoToolTipPrecisionProperty, value);
            }
        }

        #endregion

        #region AutoToolTipPlacement Property

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.AutoToolTipPlacement"/>
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPlacementProperty
            = DependencyProperty.Register("AutoToolTipPlacement", typeof(AutoToolTipPlacement), typeof(ZoomBar),
                                          new FrameworkPropertyMetadata(AutoToolTipPlacement.BottomRight),
                                          DependencyPropertyUtil.IsValidAutoToolTipPlacement);

        /// <summary>
        /// Относительное метоположение отображения всплывающей подсказки
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get
            {
                var res = GetValue(AutoToolTipPlacementProperty);
                Contract.Assume(res != null);
                return (AutoToolTipPlacement)res;
            }
            set
            {
                SetValue(AutoToolTipPlacementProperty, value);
            }
        }

        #endregion


        #region Visual style properties

        #region ThumbSize

        /// <summary>
        /// Size (width or height) of start and end thumbs
        /// </summary>
        [Bindable(true), Category("Layout")]
        public double ThumbSize
        {
            get
            {
                var res = GetValue(ThumbSizeProperty);
                Contract.Assume(res != null);
                return (double)res;
            }
            set { SetValue(ThumbSizeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.ThumbSize"/>
        /// </summary>
        public static readonly DependencyProperty ThumbSizeProperty =
            DependencyProperty.Register(
                "ThumbSize",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultThumbSize,
                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.AffectsArrange,
                    null,
                    CoerceThumbSizeValue));

        private static object CoerceThumbSizeValue(DependencyObject element, object value)
        {
            Contract.Requires(element is ZoomBar);

            var cntrl = element as ZoomBar;
            Debug.Assert(cntrl != null, "cntrl != null");
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (cntrl != null)
            // ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                double newValue =
                    DependencyPropertyUtil.ExtractDouble(
                        value,
                        ThumbSizeProperty.DefaultMetadata == null
                            ? 0.0
                            : (double)(ThumbSizeProperty.DefaultMetadata.DefaultValue ?? 0.0));

                if (DoubleUtil.LessThan(newValue, 0.0))
                {
                    newValue = 0.0;
                }

                value = newValue;
            }

            return value;
        }

        #endregion

        #region NotSelectedBackground Property

        /// <summary>
        /// Фон невыделенной области содержимого
        /// </summary>
        [Category("Brush")]
        public Brush NotSelectedBackground
        {
            get { return (Brush)GetValue(NotSelectedBackgroundProperty); }
            set { SetValue(NotSelectedBackgroundProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.NotSelectedBackground"/>
        /// </summary>
        public static readonly DependencyProperty NotSelectedBackgroundProperty =
            DependencyProperty.Register(
                "NotSelectedBackground",
                typeof(Brush),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(Brushes.White));

        #endregion

        #region NotSelectedOpacity Property

        /// <summary>
        /// Прозрачность фона невыделенной области содержимого
        /// </summary>
        public double NotSelectedOpacity
        {
            get
            {
                var res = GetValue(NotSelectedOpacityProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            set { SetValue(NotSelectedOpacityProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.NotSelectedOpacity"/>
        /// </summary>
        public static readonly DependencyProperty NotSelectedOpacityProperty =
            DependencyProperty.Register(
                "NotSelectedOpacity",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultNotSelectedOpacity));

        #endregion

        #region SelectionBorderBackground Property

        /// <summary>
        /// Фон границы области выделения
        /// </summary>
        [Category("Brush")]
        public Brush SelectionBorderBackground
        {
            get { return (Brush)GetValue(SelectionBorderBackgroundProperty); }
            set { SetValue(SelectionBorderBackgroundProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionBorderBackground"/>
        /// </summary>
        public static readonly DependencyProperty SelectionBorderBackgroundProperty =
            DependencyProperty.Register(
                "SelectionBorderBackground",
                typeof(Brush),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(Brushes.Red));

        #endregion

        #region SelectionBorderOpacity Property

        /// <summary>
        /// Прозрачность границы области выделения
        /// </summary>
        public double SelectionBorderOpacity
        {
            get
            {
                var res = GetValue(SelectionBorderOpacityProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            set { SetValue(SelectionBorderOpacityProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionBorderOpacity"/>
        /// </summary>
        public static readonly DependencyProperty SelectionBorderOpacityProperty =
            DependencyProperty.Register(
                "SelectionBorderOpacity",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultSelectionBorderOpacity));

        #endregion

        #region SelectionBorderThickness Property

        /// <summary>
        /// Толщина границы области выделения
        /// </summary>
        public double SelectionBorderThickness
        {
            get
            {
                var res = GetValue(SelectionBorderThicknessProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            set { SetValue(SelectionBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionBorderThickness"/>
        /// </summary>
        public static readonly DependencyProperty SelectionBorderThicknessProperty =
            DependencyProperty.Register(
                "SelectionBorderThickness",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultSelectionBorderThickness));
        
        #endregion

        #endregion Visual style properties

        #endregion Dependency properties

        #region Public Events

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.IsSelectionDraggingChanged"/>
        /// </summary>
        public static readonly RoutedEvent IsSelectionDraggingChangedEvent =
            EventManager.RegisterRoutedEvent(
                "IsSelectionDraggingChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ZoomBar));

        /// <summary>
        /// Событие изменения значения свойства <see cref="ZoomBar.IsSelectionDragging"/>
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsSelectionDraggingChanged
        {
            add { AddHandler(IsSelectionDraggingChangedEvent, value); }
            remove { RemoveHandler(IsSelectionDraggingChangedEvent, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionStartChanged"/>
        /// </summary>
        public static readonly RoutedEvent SelectionStartChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectionStartChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ZoomBar));

        /// <summary>
        /// Событие изменения значения свойства <see cref="ZoomBar.SelectionStart"/>
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SelectionStartChanged
        {
            add { AddHandler(SelectionStartChangedEvent, value); }
            remove { RemoveHandler(SelectionStartChangedEvent, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionEndChanged"/>
        /// </summary>
        public static readonly RoutedEvent SelectionEndChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectionEndChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ZoomBar));

        /// <summary>
        /// Событие изменения значения свойства <see cref="ZoomBar.SelectionEnd"/>
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SelectionEndChanged
        {
            add { AddHandler(SelectionEndChangedEvent, value); }
            remove { RemoveHandler(SelectionEndChangedEvent, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionRangeChanged"/>
        /// </summary>
        public static readonly RoutedEvent SelectionRangeChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectionRangeChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ZoomBar));

        /// <summary>
        /// Событие изменения значения свойства <see cref="ZoomBar.SelectionRange"/>
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SelectionRangeChanged
        {
            add { AddHandler(SelectionRangeChangedEvent, value); }
            remove { RemoveHandler(SelectionRangeChangedEvent, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionChanged"/>
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectionChanged",
                RoutingStrategy.Bubble,
                typeof(EventHandler<SelectionDragCompletedEventArgs>),
                typeof(ZoomBar));

        /// <summary>
        /// Событие изменения любого из свойств <see cref="ZoomBar.SelectionStart"/>, <see cref="ZoomBar.SelectionEnd"/>
        /// или <see cref="ZoomBar.SelectionRange"/>
        /// </summary>
        public event EventHandler<SelectionDragCompletedEventArgs> SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ZoomBar.SelectionDragCompleted"/>
        /// </summary>
        public static readonly RoutedEvent SelectionDragCompletedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectionDragCompleted",
                RoutingStrategy.Bubble,
                typeof(EventHandler<SelectionDragCompletedEventArgs>),
                typeof(ZoomBar));

        /// <summary>
        /// Событие завершения перетаскивания одного из ползунков области выделения
        /// </summary>
        public event EventHandler<SelectionDragCompletedEventArgs> SelectionDragCompleted
        {
            add { AddHandler(SelectionDragCompletedEvent, value); }
            remove { RemoveHandler(SelectionDragCompletedEvent, value); }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Команда сдвига области выделения влево/вниз
        /// </summary>
        public static RoutedCommand ShiftLeftCommand { get; private set; }

        /// <summary>
        /// Команда сдвига области выделения вправо/вверх
        /// </summary>
        public static RoutedCommand ShiftRightCommand { get; private set; }

        private static void InitializeCommands()
        {
            ShiftLeftCommand = new RoutedCommand(c_ShiftLeftCommandName, typeof(ZoomBar));
            ShiftRightCommand = new RoutedCommand(c_ShiftRightCommandName, typeof(ZoomBar));

            CommandHelpers.RegisterCommandHandler(
                typeof(ZoomBar),
                ShiftLeftCommand,
                OnShiftLeftCommand,
                ShiftLeftCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                typeof(ZoomBar),
                ShiftRightCommand,
                OnShiftRightCommand,
                ShiftRightCommand_CanExecute,
                null);
        }

        private static void OnShiftLeftCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var zoombar = sender as ZoomBar;
            if (zoombar == null)
            {
                return;
            }

            double shiftValue = DependencyPropertyUtil.ConvertToDouble(e.Parameter, zoombar.ShiftValue);
            zoombar.OnShiftLeft(shiftValue);
        }

        private static void ShiftLeftCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var zoombar = sender as ZoomBar;
            if (zoombar == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = zoombar.SelectionStart > zoombar.Minimum;
        }

        private static void OnShiftRightCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var zoombar = sender as ZoomBar;
            if (zoombar == null)
            {
                return;
            }

            double shiftValue = DependencyPropertyUtil.ConvertToDouble(e.Parameter, zoombar.ShiftValue);
            zoombar.OnShiftRight(shiftValue);
        }

        private static void ShiftRightCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var zoombar = sender as ZoomBar;
            if (zoombar == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = zoombar.SelectionEnd < zoombar.Maximum;
        }

        /// <summary>
        /// Обработчик сдвига области выделения влево/вниз
        /// </summary>
        /// <param name="shiftValue">Величина сдвига</param>
        protected virtual void OnShiftLeft(double shiftValue)
        {
            DoShift(-shiftValue);
        }

        /// <summary>
        /// Обработчик сдвига области выделения вправо/вверх
        /// </summary>
        /// <param name="shiftValue">Величина сдвига</param>
        protected virtual void OnShiftRight(double shiftValue)
        {
            DoShift(shiftValue);
        }

        /// <summary>
        /// Метод выполняет сдвиг области выделения на заданную величину, если это возможно.
        /// Направление сдвига задается знаком величины сдвига <paramref name="shiftValue"/>.
        /// </summary>
        /// <param name="shiftValue">Величина сдвига</param>
        public void DoShift(double shiftValue)
        {
            if (DoubleUtil.LessThanOrClose(Math.Abs(shiftValue), double.Epsilon) || RangeSlider == null)
                return;

            RangeSlider.MoveRangeToNextTick(Math.Abs(shiftValue), shiftValue < 0.0);
        }

        #endregion Commands

        /// <summary>
        /// Обработчик назначения шаблона данному контролу
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // TODO заменить все GetTemplateChild на FindName согласно рекомендациям
            // в http://social.msdn.microsoft.com/forums/en-US/wpf/thread/eb509920-e2c3-43d7-987b-6f67cbd544c9

            RangeSlider = GetTemplateChild("PART_RangeSlider") as SimpleNumericRangeSlider;
            if (RangeSlider != null)
            {
                RangeSlider.ApplyTemplate();

                var binding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(SelectionStartProperty),
                        Mode = BindingMode.TwoWay
                    };
                RangeSlider.SetBinding(SimpleNumericRangeSlider.StartValueProperty, binding);

                binding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(SelectionEndProperty),
                        Mode = BindingMode.TwoWay
                    };
                RangeSlider.SetBinding(SimpleNumericRangeSlider.EndValueProperty, binding);

                binding = new Binding {Source = this, Path = new PropertyPath(MinSelectionRangeProperty)};
                RangeSlider.SetBinding(SimpleNumericRangeSlider.MinRangeValueProperty, binding);

                // инициализируем интервал здесь, т.к. он устанавливается только при вызове RangeSlider_RangeValueChanged
                SelectionRange = RangeSlider.RangeValue;

                RangeSlider.RangeValueChanged += this.RangeSlider_RangeValueChanged;
                RangeSlider.ValueChanged += this.RangeSlider_ValueChanged;
                RangeSlider.IsRangeDraggingChanged += this.RangeSlider_IsRangeDraggingChanged;
                RangeSlider.RangeDragCompleted += this.RangeSlider_RangeDragCompleted;

                var track = RangeSlider.Track;
                if (track != null)
                {
                    StartThumb = track.StartThumb;
                    if (StartThumb != null)
                    {
                        StartThumb.SizeChanged += this.Thumb_SizeChanged;

                        StartThumb.Focusable = false;
                    }

                    EndThumb = track.EndThumb;
                    if (EndThumb != null)
                    {
                        EndThumb.SizeChanged += this.Thumb_SizeChanged;

                        EndThumb.Focusable = false;
                    }
                }
            }

            ContentContainer = GetTemplateChild("PART_ContentContainer") as FrameworkElement;
            if (ContentContainer != null)
            {
                //ContentContainer.ApplyTemplate(); // непонятно, надо или нет...

                ContentContainer.SizeChanged += this.Content_SizeChanged;
            }

            ShiftLeftButton = GetTemplateChild("PART_ShiftLeftButton") as ButtonBase;
            if (ShiftLeftButton != null)
            {
                ShiftLeftButton.Focusable = false;
                ShiftLeftButton.Command = ShiftLeftCommand;
            }
            ShiftRightButton = GetTemplateChild("PART_ShiftRightButton") as ButtonBase;
            if (ShiftRightButton != null)
            {
                ShiftRightButton.Focusable = false;
                ShiftRightButton.Command = ShiftRightCommand;
            }
        }

        /// <summary>
        /// Обработчик изменения шаблона данного контрола
        /// </summary>
        /// <param name="oldTemplate">Старый шаблон</param>
        /// <param name="newTemplate">Новый шаблон</param>
        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            if (oldTemplate != null)
            {
                if (ContentContainer != null)
                {
                    ContentContainer.SizeChanged -= this.Content_SizeChanged;

                    ContentContainer = null;
                }

                if (RangeSlider != null)
                {
                    BindingOperations.ClearAllBindings(RangeSlider);

                    RangeSlider.RangeValueChanged -= this.RangeSlider_RangeValueChanged;
                    RangeSlider.ValueChanged -= this.RangeSlider_ValueChanged;
                    RangeSlider.IsRangeDraggingChanged -= this.RangeSlider_IsRangeDraggingChanged;
                    RangeSlider.RangeDragCompleted -= this.RangeSlider_RangeDragCompleted;

                    RangeSlider = null;
                }

                if (StartThumb != null)
                {
                    StartThumb.SizeChanged -= this.Thumb_SizeChanged;

                    StartThumb = null;
                }

                if (EndThumb != null)
                {
                    EndThumb.SizeChanged -= this.Thumb_SizeChanged;

                    EndThumb = null;
                }

                ShiftLeftButton = null;
                ShiftRightButton = null;
            }

            base.OnTemplateChanged(oldTemplate, newTemplate);
        }

        /// <summary>
        /// This is a class handler for MouseLeftButtonDown event.
        /// The purpose of this handle is to move input focus to ZoomBar when user pressed
        /// mouse left button on any part of slider that is not focusable.
        /// </summary>
        /// <param name="sender"><see cref="ZoomBar"/></param>
        /// <param name="e">Data for event</param>
        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Contract.Requires(sender is ZoomBar);

            var zoombar = sender as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (zoombar == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

            // When someone click on a part in the ZoomBar and it's not focusable
            // ZoomBar needs to take the focus in order to process keyboard correctly
            if (!zoombar.IsKeyboardFocusWithin)
            {
                e.Handled = zoombar.Focus() || e.Handled;
            }
        }

        private void RangeSlider_RangeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SelectionRange = e.NewValue;
        }

        private void RangeSlider_ValueChanged(object sender, RangeDragCompletedEventArgs<double> e)
        {
            var newEventArgs = new SelectionDragCompletedEventArgs(e.OldStartValue, e.OldEndValue, e.NewStartValue, e.NewEndValue)
            {
                RoutedEvent = SelectionChangedEvent
            };
            RaiseEvent(newEventArgs);
        }

        private void RangeSlider_IsRangeDraggingChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            IsSelectionDragging = e.NewValue;
        }

        private void RangeSlider_RangeDragCompleted(object sender, RangeDragCompletedEventArgs<double> e)
        {
            var newEventArgs = new SelectionDragCompletedEventArgs(e.OldStartValue, e.OldEndValue, e.NewStartValue, e.NewEndValue)
            {
                RoutedEvent = SelectionDragCompletedEvent
            };
            RaiseEvent(newEventArgs);
        }

        private void Content_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
                return;
            RefreshContentIndentValues();
        }

        private void Thumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeContentContainerMargin();
            RefreshContentIndentValues();
        }

        #region IRanged<double, double>

        /// <summary>
        /// Для данного объекта реализуется логика единого значения, когда начало интевала совпадает с его концом,
        /// а величина самого интервала, соответственно, равна нулю.
        /// Всегда возвращает <c>false</c>.
        /// </summary>
        public bool IsSingleValue
        {
            get { return false; }
        }

        /// <summary>
        /// Метод преобразования числа в значение
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Всегда <paramref name="value"/></returns>
        public double DoubleToValue(double value)
        {
            return value;
        }

        /// <summary>
        /// Метод преобразования значения в число
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Всегда <paramref name="value"/></returns>
        public double ValueToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Метод преобразования числа в интервал
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Всегда <paramref name="value"/></returns>
        public double DoubleToInterval(double value)
        {
            return value;
        }

        /// <summary>
        /// Метод преобразования интервала в число
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Всегда <paramref name="value"/></returns>
        public double IntervalToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Значение начала интервала
        /// </summary>
        public double StartValue
        {
            get { return SelectionStart; }
        }

        /// <summary>
        /// Значение конца интервала
        /// </summary>
        public double EndValue
        {
            get { return SelectionEnd; }
        }

        /// <summary>
        /// Минимально допустимая величина интервала
        /// </summary>
        public double MinRangeValue
        {
            get { return MinSelectionRange; }
        }

        #endregion // IRanged<double, double>
    }

}