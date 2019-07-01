// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ZoomBar.cs" company="Sane Development">
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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SaneDevelopment.WPF.Controls
{
    /// <summary>
    /// Zoom bar is a content control, that allows to scale ("zoom") its content
    /// by cutting edges of adjustable length.
    /// Can be used as a sort of preview control.
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
        #region Private fields

        private const string c_ShiftLeftCommandName = "ShiftLeftCommand";
        private const string c_ShiftRightCommandName = "ShiftRightCommand";

        #endregion Private fields

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
        /// Default value for <see cref="MinimumProperty"/>
        /// </summary>
        public const double DefaultMinimum = 0.0;

        /// <summary>
        /// Default value for <see cref="MaximumProperty"/>.
        /// </summary>
        public const double DefaultMaximum = 100.0;

        /// <summary>
        /// Default value for <see cref="ShiftValueProperty"/>.
        /// </summary>
        public const double DefaultShiftValue = 5.0;

        /// <summary>
        /// Minimum value for <see cref="ShiftValueProperty"/>.
        /// </summary>
        public const double MinShiftValue = 1.0;

        /// <summary>
        /// Default value for <see cref="SelectionStartProperty"/>.
        /// </summary>
        public const double DefaultSelectionStart = DefaultMinimum;

        /// <summary>
        /// Default value for <see cref="SelectionEndProperty"/>.
        /// </summary>
        public const double DefaultSelectionEnd = DefaultMaximum;

        private const double c_DefaultSelectionRange = DefaultSelectionEnd - DefaultSelectionStart;

        /// <summary>
        /// Default value for <see cref="SelectionBorderThicknessProperty"/>.
        /// </summary>
        public const double DefaultSelectionBorderThickness = 1.0;

        /// <summary>
        /// Default value for <see cref="NotSelectedOpacityProperty"/>.
        /// </summary>
        public const double DefaultNotSelectedOpacity = 0.7;

        /// <summary>
        /// Default value for <see cref="SelectionBorderOpacityProperty"/>.
        /// </summary>
        public const double DefaultSelectionBorderOpacity = 0.5;

        /// <summary>
        /// Default value of start and end thumbs size (width or height depending on <see cref="Orientation"/>)
        /// </summary>
        public const double DefaultThumbSize = 10.0;

        #endregion

        /// <summary>
        /// Gets or sets container for content of a control
        /// </summary>
        protected FrameworkElement ContentContainer { get; set; }

        /// <summary>
        /// Gets or sets slider that provides selection area.
        /// </summary>
        protected SimpleNumericRangeSlider RangeSlider { get; set; }

        /// <summary>
        /// Gets or sets start thumb.
        /// </summary>
        protected Thumb StartThumb { get; set; }

        /// <summary>
        /// Gets or sets end thumb.
        /// </summary>
        protected Thumb EndThumb { get; set; }

        /// <summary>
        /// Gets or sets shift left (down) button.
        /// </summary>
        protected ButtonBase ShiftLeftButton { get; set; }

        /// <summary>
        /// Gets or sets shift right (up) button.
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

            // make the indent to the right and to the left of the content container
            // equals to the thumbs sizes.
            // So when the slider moves close to the area end, it not overlap content.
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
        /// Dependency property for <see cref="ZoomBar.Orientation"/>
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender),
                DependencyPropertyUtil.IsValidOrientation);

        /// <summary>
        /// Control orientation
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public Orientation Orientation
        {
            get
            {
                var res = GetValue(OrientationProperty);
                Debug.Assert(res != null);
                return (Orientation) res;
            }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region Minimum

        /// <summary>
        /// Gets the minimum value
        /// </summary>
        [Category("Common")]
        public double Minimum
        {
            get
            {
                var res = GetValue(MinimumProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="ZoomBar.Minimum"/>
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = MinimumPropertyKey.DependencyProperty;

        #endregion

        #region Maximum

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        [Category("Common")]
        public double Maximum
        {
            get
            {
                var res = GetValue(MaximumProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="ZoomBar.Maximum"/>
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = MaximumPropertyKey.DependencyProperty;

        #endregion

        #region ShiftValue

        /// <summary>
        /// Gets or sets the shift value.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double ShiftValue
        {
            get
            {
                var res = GetValue(ShiftValueProperty);
                Debug.Assert(res != null);
                return (double) res;
            }
            set { SetValue(ShiftValueProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.ShiftValue"/>
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
            Debug.Assert(element is ZoomBar);

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
        /// Gets or sets the selection start value.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double SelectionStart
        {
            get
            {
                var res = GetValue(SelectionStartProperty);
                Debug.Assert(res != null);
                return (double) res;
            }
            set { SetValue(SelectionStartProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionStart"/>
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
            Debug.Assert(obj is ZoomBar);
            Debug.Assert(args.OldValue is double);
            Debug.Assert(args.NewValue is double);

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
            Debug.Assert(d is ZoomBar);
            Debug.Assert(value != null);

            var base2 = d as ZoomBar;
            Debug.Assert(base2 != null);
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
        /// A handler implementation that the property system will call whenever the effective value of the <see cref="SelectionStart"/> changes.
        /// </summary>
        /// <param name="oldValue">The value of the <see cref="SelectionStart"/> before the change.</param>
        /// <param name="newValue">The value of the <see cref="SelectionStart"/> after the change.</param>
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
        /// Gets or sets the selection end value.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double SelectionEnd
        {
            get
            {
                var res = GetValue(SelectionEndProperty);
                Debug.Assert(res != null);
                return (double) res;
            }
            set { SetValue(SelectionEndProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionEnd"/>
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
            Debug.Assert(obj is ZoomBar);
            Debug.Assert(args.OldValue is double);
            Debug.Assert(args.NewValue is double);

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
            Debug.Assert(d is ZoomBar);
            Debug.Assert(value != null);

            var base2 = d as ZoomBar;
            Debug.Assert(base2 != null);
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
        /// A handler implementation that the property system will call whenever the effective value of the <see cref="SelectionEnd"/> changes.
        /// </summary>
        /// <param name="oldValue">The value of the <see cref="SelectionEnd"/> before the change.</param>
        /// <param name="newValue">The value of the <see cref="SelectionEnd"/> after the change.</param>
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
        /// Gets or sets the minimum value of selection range.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double MinSelectionRange
        {
            get
            {
                var res = GetValue(MinSelectionRangeProperty);
                Debug.Assert(res != null);
                return (double) res;
            }
            set { SetValue(MinSelectionRangeProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.MinSelectionRange"/>
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
            Debug.Assert(element is ZoomBar);

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
        /// Gets current selection range value.
        /// </summary>
        [Category("Common")]
        public double SelectionRange
        {
            get
            {
                var res = GetValue(SelectionRangeProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="ZoomBar.SelectionRange"/>
        /// </summary>
        public static readonly DependencyProperty SelectionRangeProperty = SelectionRangePropertyKey.DependencyProperty;

        private static void OnSelectionRangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is ZoomBar);
            Debug.Assert(args.OldValue is double);
            Debug.Assert(args.NewValue is double);

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
        /// A handler implementation that the property system will call whenever the effective value of the <see cref="SelectionRange"/> changes.
        /// </summary>
        /// <param name="oldValue">The value of the <see cref="SelectionRange"/> before the change.</param>
        /// <param name="newValue">The value of the <see cref="SelectionRange"/> after the change.</param>
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
        /// Gets the left/bottom indent value of control content relative to left/bottom control edge.
        /// Indicates the current placement of content relative to the control in whole.
        /// </summary>
        [Category("Layout")]
        public double LeftContentIndent
        {
            get
            {
                var res = GetValue(LeftContentIndentProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="ZoomBar.LeftContentIndent"/>
        /// </summary>
        public static readonly DependencyProperty LeftContentIndentProperty = LeftContentIndentPropertyKey.DependencyProperty;

        #endregion

        #region RightContentIndent

        /// <summary>
        /// Gets the right/top indent value of control content relative to right/top control edge.
        /// Indicates the current placement of content relative to the control in whole.
        /// </summary>
        [Category("Layout")]
        public double RightContentIndent
        {
            get
            {
                var res = GetValue(RightContentIndentProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="ZoomBar.RightContentIndent"/>
        /// </summary>
        public static readonly DependencyProperty RightContentIndentProperty = RightContentIndentPropertyKey.DependencyProperty;

        #endregion

        #region IsSelectionDragging

        /// <summary>
        /// Gets the indicator whether control is in process of dragging (changing) the selection range
        /// via movement of any of thumbs.
        /// Indicates that user is in process of choosing the selection value by moving a thumbs.
        /// </summary>
        [Category("Common")]
        public bool IsSelectionDragging
        {
            get
            {
                var res = GetValue(IsSelectionDraggingProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="ZoomBar.IsSelectionDragging"/>
        /// </summary>
        public static readonly DependencyProperty IsSelectionDraggingProperty = IsSelectionDraggingPropertyKey.DependencyProperty;

        private static void OnIsSelectionDraggingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is ZoomBar);
            Debug.Assert(args.OldValue is bool);
            Debug.Assert(args.NewValue is bool);

            var zoombar = obj as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (zoombar != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                Debug.Assert(args.OldValue is bool);
                Debug.Assert(args.NewValue is bool);
                zoombar.OnIsSelectionDraggingChanged((bool)args.OldValue, (bool)args.NewValue);
            }
        }

        /// <summary>
        /// A handler implementation that the property system will call whenever the effective value of the <see cref="IsSelectionDragging"/> changes.
        /// </summary>
        /// <param name="oldValue">The value of the <see cref="IsSelectionDragging"/> before the change.</param>
        /// <param name="newValue">The value of the <see cref="IsSelectionDragging"/> after the change.</param>
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
        /// Dependency property for <see cref="ZoomBar.IsRaiseSelectionChangedWhileDragging"/>
        /// </summary>
        public static readonly DependencyProperty IsRaiseSelectionChangedWhileDraggingProperty =
            DependencyProperty.Register(
                "IsRaiseSelectionChangedWhileDragging",
                typeof(bool),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender),
                DependencyPropertyUtil.IsValidBoolValue);

        /// <summary>
        /// Gets of sets indicator whether to raise <see cref="ZoomBar.SelectionChanged"/> event
        /// when control is in process of changing a selection value via dragging a thumb.
        /// In other word when <see cref="ZoomBar.IsSelectionDragging"/> is <c>true</c>.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsRaiseSelectionChangedWhileDragging
        {
            get
            {
                var res = GetValue(IsRaiseSelectionChangedWhileDraggingProperty);
                Debug.Assert(res != null);
                return (bool) res;
            }
            set { SetValue(IsRaiseSelectionChangedWhileDraggingProperty, value); }
        }

        #endregion


        #region AutoToolTipValueConverter Property

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.AutoToolTipValueConverter"/>
        /// </summary>
        public static readonly DependencyProperty AutoToolTipValueConverterProperty =
            DependencyProperty.Register(
                "AutoToolTipValueConverter",
                typeof(IRangeValueToStringConverter<double>),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the converter of selection values to their string representations for showing in autotooltips.
        /// If not <c>null</c> then <see cref="ZoomBar.AutoToolTipFormat"/> ignores.
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
        /// Dependency property for <see cref="ZoomBar.AutoToolTipValueConverterParameter"/>
        /// </summary>
        public static readonly DependencyProperty AutoToolTipValueConverterParameterProperty =
            DependencyProperty.Register(
                "AutoToolTipValueConverterParameter",
                typeof(object),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter for <see cref="AutoToolTipValueConverter"/>.
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
        /// Dependency property for <see cref="ZoomBar.AutoToolTipFormat"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")] public static readonly
            DependencyProperty AutoToolTipFormatProperty
                = DependencyProperty.Register(
                    "AutoToolTipFormat",
                    typeof (string),
                    typeof (ZoomBar),
                    new FrameworkPropertyMetadata());

        /// <summary>
        /// Gets or sets the format string for showing a values in the auto tooltips.
        /// Uses only if <see cref="ZoomBar.AutoToolTipValueConverter"/> is <c>null</c>.
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
        /// Dependency property for <see cref="ZoomBar.AutoToolTipPrecision"/>
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register(
            "AutoToolTipPrecision",
            typeof(int),
            typeof(ZoomBar),
            new FrameworkPropertyMetadata(4),
            DependencyPropertyUtil.IsValidAutoToolTipPrecision);

        /// <summary>
        /// Floating-point precision of numbers for showing in the auto tooltips in UI.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public int AutoToolTipPrecision
        {
            get
            {
                var res = GetValue(AutoToolTipPrecisionProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="ZoomBar.AutoToolTipPlacement"/>
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPlacementProperty
            = DependencyProperty.Register(
                "AutoToolTipPlacement",
                typeof (AutoToolTipPlacement),
                typeof (ZoomBar),
                new FrameworkPropertyMetadata(AutoToolTipPlacement.BottomRight),
                DependencyPropertyUtil.IsValidAutoToolTipPlacement);

        /// <summary>
        /// The placement where automatic <see cref="System.Windows.Controls.ToolTip"/> is positioned on the control.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get
            {
                var res = GetValue(AutoToolTipPlacementProperty);
                Debug.Assert(res != null);
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
                Debug.Assert(res != null);
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
            Debug.Assert(element is ZoomBar);

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
        /// Gets or sets the <see cref="Brush"/> for the "not selected" area,
        /// i.e. area of control located beyond currently selected range.
        /// </summary>
        [Category("Brush")]
        public Brush NotSelectedBackground
        {
            get { return (Brush)GetValue(NotSelectedBackgroundProperty); }
            set { SetValue(NotSelectedBackgroundProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.NotSelectedBackground"/>
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
        /// Gets or sets the opacity for the "not selected" area,
        /// i.e. area of control located beyond currently selected range.
        /// </summary>
        [Category("Appearance")]
        public double NotSelectedOpacity
        {
            get
            {
                var res = GetValue(NotSelectedOpacityProperty);
                Debug.Assert(res != null);
                return (double) res;
            }
            set { SetValue(NotSelectedOpacityProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.NotSelectedOpacity"/>
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
        /// Gets or sets the <see cref="Brush"/> for the background of border of selection area.
        /// </summary>
        [Category("Brush")]
        public Brush SelectionBorderBackground
        {
            get { return (Brush)GetValue(SelectionBorderBackgroundProperty); }
            set { SetValue(SelectionBorderBackgroundProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionBorderBackground"/>
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
        /// Gets or sets the opacity for the border of selection area.
        /// </summary>
        [Category("Appearance")]
        public double SelectionBorderOpacity
        {
            get
            {
                var res = GetValue(SelectionBorderOpacityProperty);
                Debug.Assert(res != null);
                return (double) res;
            }
            set { SetValue(SelectionBorderOpacityProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionBorderOpacity"/>
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
        /// Gets or sets the thickness for the border of selection area.
        /// </summary>
        public double SelectionBorderThickness
        {
            get
            {
                var res = GetValue(SelectionBorderThicknessProperty);
                Debug.Assert(res != null);
                return (double) res;
            }
            set { SetValue(SelectionBorderThicknessProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionBorderThickness"/>
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

        #region IsSelectionDraggingChanged

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.IsSelectionDraggingChanged"/>
        /// </summary>
        public static readonly RoutedEvent IsSelectionDraggingChangedEvent =
            EventManager.RegisterRoutedEvent(
                "IsSelectionDraggingChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when <see cref="ZoomBar.IsSelectionDragging"/> changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsSelectionDraggingChanged
        {
            add { AddHandler(IsSelectionDraggingChangedEvent, value); }
            remove { RemoveHandler(IsSelectionDraggingChangedEvent, value); }
        }

        #endregion IsSelectionDraggingChanged

        #region SelectionStartChanged

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionStartChanged"/>
        /// </summary>
        public static readonly RoutedEvent SelectionStartChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectionStartChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when <see cref="ZoomBar.SelectionStart"/> changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SelectionStartChanged
        {
            add { AddHandler(SelectionStartChangedEvent, value); }
            remove { RemoveHandler(SelectionStartChangedEvent, value); }
        }

        #endregion SelectionStartChanged

        #region SelectionEndChanged

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionEndChanged"/>
        /// </summary>
        public static readonly RoutedEvent SelectionEndChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectionEndChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when <see cref="ZoomBar.SelectionEnd"/> changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SelectionEndChanged
        {
            add { AddHandler(SelectionEndChangedEvent, value); }
            remove { RemoveHandler(SelectionEndChangedEvent, value); }
        }

        #endregion SelectionEndChanged

        #region SelectionRangeChanged

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionRangeChanged"/>
        /// </summary>
        public static readonly RoutedEvent SelectionRangeChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectionRangeChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when <see cref="ZoomBar.SelectionRange"/> changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SelectionRangeChanged
        {
            add { AddHandler(SelectionRangeChangedEvent, value); }
            remove { RemoveHandler(SelectionRangeChangedEvent, value); }
        }

        #endregion SelectionRangeChanged

        #region SelectionChanged

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionChanged"/>
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectionChanged",
                RoutingStrategy.Bubble,
                typeof(EventHandler<SelectionDragCompletedEventArgs>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when any of properties <see cref="ZoomBar.SelectionStart"/>, <see cref="ZoomBar.SelectionEnd"/>
        /// or <see cref="ZoomBar.SelectionRange"/> changed.
        /// </summary>
        public event EventHandler<SelectionDragCompletedEventArgs> SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        #endregion SelectionChanged

        #region SelectionDragCompleted

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionDragCompleted"/>
        /// </summary>
        public static readonly RoutedEvent SelectionDragCompletedEvent =
            EventManager.RegisterRoutedEvent(
                "SelectionDragCompleted",
                RoutingStrategy.Bubble,
                typeof(EventHandler<SelectionDragCompletedEventArgs>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when dragging of any of the thumbs completed.
        /// </summary>
        public event EventHandler<SelectionDragCompletedEventArgs> SelectionDragCompleted
        {
            add { AddHandler(SelectionDragCompletedEvent, value); }
            remove { RemoveHandler(SelectionDragCompletedEvent, value); }
        }

        #endregion SelectionDragCompleted

        #endregion Public Events

        #region Commands

        /// <summary>
        /// Routed command for decreasing (left/down) of selection range in whole.
        /// </summary>
        public static RoutedCommand ShiftLeftCommand { get; private set; }

        /// <summary>
        /// Routed command for increasing (right/up) of selection range in whole.
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
        /// Handler for <see cref="ShiftLeftCommand"/>.
        /// </summary>
        /// <param name="shiftValue">Shift value</param>
        protected virtual void OnShiftLeft(double shiftValue)
        {
            DoShift(-shiftValue);
        }

        /// <summary>
        /// Handler for <see cref="ShiftRightCommand"/>.
        /// </summary>
        /// <param name="shiftValue">Shift value</param>
        protected virtual void OnShiftRight(double shiftValue)
        {
            DoShift(shiftValue);
        }

        /// <summary>
        /// Performs shift (move) of the selection range on received value, if it possible.
        /// Direction of shift determines by the sign of <paramref name="shiftValue"/> -
        /// if it negative - shift left/down,
        /// if it positive - shift right/up.
        /// </summary>
        /// <param name="shiftValue">Shift value</param>
        public void DoShift(double shiftValue)
        {
            if (DoubleUtil.LessThanOrClose(Math.Abs(shiftValue), double.Epsilon) || RangeSlider == null)
                return;

            RangeSlider.MoveRangeToNextTick(Math.Abs(shiftValue), shiftValue < 0.0);
        }

        #endregion Commands

        /// <summary>
        /// Handler for <see cref="System.Windows.FrameworkElement.ApplyTemplate()"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            // TODO: replace all calls to GetTemplateChild by the FindName
            // as it recommended in http://social.msdn.microsoft.com/forums/en-US/wpf/thread/eb509920-e2c3-43d7-987b-6f67cbd544c9

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

                // initialize interval (range) here, because it set only inside RangeSlider_RangeValueChanged
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
                //ContentContainer.ApplyTemplate(); // don't know whether it necessary here, or not... need research!

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
        /// Called whenever the control's template changes.
        /// </summary>
        /// <param name="oldTemplate">The old template.</param>
        /// <param name="newTemplate">The new template.</param>
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
            Debug.Assert(sender is ZoomBar);

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
        /// Gets the indicator that this object behaves like a single value object, i.e. start value equals to end value,
        /// so interval (range) value equals to zero.
        /// </summary>
        public bool IsSingleValue
        {
            get { return false; }
        }

        /// <summary>
        /// Method for converting <c>double</c> to value type
        /// </summary>
        /// <param name="value">Value to convert from</param>
        /// <returns>Always <paramref name="value"/></returns>
        public double DoubleToValue(double value)
        {
            return value;
        }

        /// <summary>
        /// Method for converting value type to <c>double</c>
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Always <paramref name="value"/></returns>
        public double ValueToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Method for converting <c>double</c> to interval type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Always <paramref name="value"/></returns>
        public double DoubleToInterval(double value)
        {
            return value;
        }

        /// <summary>
        /// Method for converting interval value to <c>double</c>
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Always <paramref name="value"/></returns>
        public double IntervalToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Gets start interval value
        /// </summary>
        /// <returns><see cref="SelectionStart"/></returns>
        public double StartValue
        {
            get { return SelectionStart; }
        }

        /// <summary>
        /// Gets end interval value
        /// </summary>
        /// <returns><see cref="SelectionEnd"/></returns>
        public double EndValue
        {
            get { return SelectionEnd; }
        }

        /// <summary>
        /// Gets minimum available interval (range) value
        /// </summary>
        /// <returns><see cref="MinSelectionRange"/></returns>
        public double MinRangeValue
        {
            get { return MinSelectionRange; }
        }

        #endregion // IRanged<double, double>
    }

}