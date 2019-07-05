// -----------------------------------------------------------------------
// <copyright file="ZoomBar.cs" company="Sane Development">
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
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;

#pragma warning disable CA1501 // Avoid excessive inheritance

    /// <summary>
    /// Zoom bar is a content control, that allows to scale ("zoom") its content
    /// by cutting edges of adjustable length.
    /// Can be used as a sort of preview control.
    /// </summary>
    [TemplatePart(Name = ContentContainerPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = LeftThumbPartName, Type = typeof(Thumb))]
    [TemplatePart(Name = RangeSliderPartName, Type = typeof(SimpleNumericRangeSlider))]
    [TemplatePart(Name = RightThumbPartName, Type = typeof(Thumb))]
    [TemplatePart(Name = ShiftLeftButtonPartName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ShiftRightButtonPartName, Type = typeof(ButtonBase))]
    [Description("Sane Zoombar")]
    public class ZoomBar : ContentControl, IRanged<double, double>
    {
#pragma warning disable SA1202 // Elements should be ordered by access
#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable IDE1006 // Naming Styles

#pragma warning disable SA1303 // Const field names should begin with upper-case letter
#pragma warning disable SA1310 // Field names should not contain underscore

        /// <summary>
        /// Pre-defined name of the part track inside slider XAML.
        /// </summary>
        public const string ContentContainerPartName = "PART_ContentContainer";

        /// <summary>
        /// Pre-defined name of the part track inside slider XAML.
        /// </summary>
        public const string LeftThumbPartName = "PART_LeftThumb";

        /// <summary>
        /// Pre-defined name of the part track inside slider XAML.
        /// </summary>
        public const string RightThumbPartName = "PART_RightThumb";

        /// <summary>
        /// Pre-defined name of the part track inside slider XAML.
        /// </summary>
        public const string RangeSliderPartName = "PART_RangeSlider";

        /// <summary>
        /// Pre-defined name of the part track inside slider XAML.
        /// </summary>
        public const string ShiftLeftButtonPartName = "PART_ShiftLeftButton";

        /// <summary>
        /// Pre-defined name of the part track inside slider XAML.
        /// </summary>
        public const string ShiftRightButtonPartName = "PART_ShiftRightButton";

        #region Default constants

        /// <summary>
        /// Default value for <see cref="MinimumProperty"/>.
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
        /// Default value of start and end thumbs size (width or height depending on <see cref="Orientation"/>).
        /// </summary>
        public const double DefaultThumbSize = 10.0;

        #endregion

#pragma warning restore SA1310 // Field names should not contain underscore
#pragma warning restore SA1303 // Const field names should begin with upper-case letter

        static ZoomBar()
        {
            InitializeCommands();

            // Listen to MouseLeftButtonDown event to determine if slide should move focus to itself
            EventManager.RegisterClassHandler(
                typeof(ZoomBar),
                Mouse.MouseDownEvent,
                new MouseButtonEventHandler(OnMouseLeftButtonDown),
                true);

            Debug.Assert(DefaultMinimum < DefaultMaximum, "DefaultMinimum <= DefaultMaximum");

            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(typeof(ZoomBar)));
        }

        #region Parts

        /// <summary>
        /// Gets or sets container for content of a control.
        /// </summary>
        /// <value>Container for content of a control.</value>
        protected FrameworkElement ContentContainer { get; set; }

        /// <summary>
        /// Gets or sets slider that provides selection area.
        /// </summary>
        /// <value>Slider that provides selection area.</value>
        protected SimpleNumericRangeSlider RangeSlider { get; set; }

        /// <summary>
        /// Gets or sets start thumb.
        /// </summary>
        /// <value>Start thumb.</value>
        protected Thumb StartThumb { get; set; }

        /// <summary>
        /// Gets or sets end thumb.
        /// </summary>
        /// <value>End thumb.</value>
        protected Thumb EndThumb { get; set; }

        /// <summary>
        /// Gets or sets shift left (down) button.
        /// </summary>
        /// <value>Shift left (down) button.</value>
        protected ButtonBase ShiftLeftButton { get; set; }

        /// <summary>
        /// Gets or sets shift right (up) button.
        /// </summary>
        /// <value>Shift right (up) button.</value>
        protected ButtonBase ShiftRightButton { get; set; }

        #endregion Parts

        private void RefreshContentIndentValues()
        {
            if (this.ContentContainer == null)
            {
                return;
            }

            var generalTransform = this.ContentContainer.TransformToAncestor(this);
            if (generalTransform == null)
            {
                return;
            }

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
        /// Dependency property for <see cref="ZoomBar.Orientation"/>.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsRender),
                DependencyPropertyUtil.IsValidOrientation);

        /// <summary>
        /// Gets or sets control orientation.
        /// </summary>
        /// <value>Control orientation.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public Orientation Orientation
        {
            get
            {
                var res = this.GetValue(OrientationProperty);
                Debug.Assert(res != null, "res != null");
                return (Orientation)res;
            }

            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        #endregion

        #region Minimum

        /// <summary>
        /// Gets the minimum value.
        /// </summary>
        /// <value>The minimum value.</value>
        [Category("Common")]
        public double Minimum
        {
            get
            {
                var res = this.GetValue(MinimumProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }
        }

        private static readonly DependencyPropertyKey MinimumPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Minimum),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultMinimum));

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.Minimum"/>.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = MinimumPropertyKey.DependencyProperty;

        #endregion

        #region Maximum

        /// <summary>
        /// Gets the maximum value.
        /// </summary>
        /// <value>The maximum value.</value>
        [Category("Common")]
        public double Maximum
        {
            get
            {
                var res = this.GetValue(MaximumProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }
        }

        private static readonly DependencyPropertyKey MaximumPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(Maximum),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultMaximum));

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.Maximum"/>.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = MaximumPropertyKey.DependencyProperty;

        #endregion

        #region ShiftValue

        /// <summary>
        /// Gets or sets the shift value.
        /// </summary>
        /// <value>The shift value.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public double ShiftValue
        {
            get
            {
                var res = this.GetValue(ShiftValueProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            set
            {
                this.SetValue(ShiftValueProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.ShiftValue"/>.
        /// </summary>
        public static readonly DependencyProperty ShiftValueProperty =
            DependencyProperty.Register(
                nameof(ShiftValue),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultShiftValue, OnShiftValueChanged, CoerceShiftValue));

        private static void OnShiftValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        private static object CoerceShiftValue(DependencyObject element, object value)
        {
            Debug.Assert(element is ZoomBar, "element is ZoomBar");

            var defaultMetadata = ShiftValueProperty.DefaultMetadata;
            double newValue =
                DependencyPropertyUtil.ExtractDouble(
                    value,
                    defaultMetadata == null ? 0.0 : (double)(defaultMetadata.DefaultValue ?? DefaultShiftValue));

            var zoombar = element as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
            if (zoombar != null)
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
        /// <value>The selection start value.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public double SelectionStart
        {
            get
            {
                var res = this.GetValue(SelectionStartProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            set
            {
                this.SetValue(SelectionStartProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionStart"/>.
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.Register(
                nameof(SelectionStart),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(
                    DefaultSelectionStart,
                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectionStartChanged,
                    CoerceSelectionStart));

        private static void OnSelectionStartChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is ZoomBar, "obj is ZoomBar");
            Debug.Assert(args.OldValue is double, "args.OldValue is double");
            Debug.Assert(args.NewValue is double, "args.NewValue is double");

            var zoombar = obj as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
            if (zoombar != null)
            {
                zoombar.OnSelectionStartChanged((double)args.OldValue, (double)args.NewValue);
            }
        }

        private static object CoerceSelectionStart(DependencyObject d, object value)
        {
            Debug.Assert(d is ZoomBar, "d is ZoomBar");
            Debug.Assert(value != null, "value != null");

            var base2 = d as ZoomBar;
            Debug.Assert(base2 != null, "base2 != null");
            if (base2 != null)
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
                RoutedEvent = SelectionStartChangedEvent,
            };
            this.RaiseEvent(newEventArgs);
        }

        #endregion

        #region SelectionEnd

        /// <summary>
        /// Gets or sets the selection end value.
        /// </summary>
        /// <value>The selection end value.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public double SelectionEnd
        {
            get
            {
                var res = this.GetValue(SelectionEndProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            set
            {
                this.SetValue(SelectionEndProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionEnd"/>.
        /// </summary>
        public static readonly DependencyProperty SelectionEndProperty =
            DependencyProperty.Register(
                nameof(SelectionEnd),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(
                    DefaultSelectionEnd,
                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectionEndChanged,
                    CoerceSelectionEnd));

        private static void OnSelectionEndChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is ZoomBar, "obj is ZoomBar");
            Debug.Assert(args.OldValue is double, "args.OldValue is double");
            Debug.Assert(args.NewValue is double, "args.NewValue is double");

            var zoombar = obj as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
            if (zoombar == null)
            {
                return;
            }

            zoombar.OnSelectionEndChanged((double)args.OldValue, (double)args.NewValue);
        }

        private static object CoerceSelectionEnd(DependencyObject d, object value)
        {
            Debug.Assert(d is ZoomBar, "d is ZoomBar");
            Debug.Assert(value != null, "value != null");

            var base2 = d as ZoomBar;
            Debug.Assert(base2 != null, "base2 != null");
            if (base2 != null)
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
                RoutedEvent = SelectionEndChangedEvent,
            };
            this.RaiseEvent(newEventArgs);
        }

        #endregion

        #region MinSelectionRange

        /// <summary>
        /// Gets or sets the minimum value of selection range.
        /// </summary>
        /// <value>The minimum value of selection range.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public double MinSelectionRange
        {
            get
            {
                var res = this.GetValue(MinSelectionRangeProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            set
            {
                this.SetValue(MinSelectionRangeProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.MinSelectionRange"/>.
        /// </summary>
        public static readonly DependencyProperty MinSelectionRangeProperty =
            DependencyProperty.Register(
                nameof(MinSelectionRange),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(
                    0.0,
                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    null,
                    CoerceMinSelectionRangeValue));

        private static object CoerceMinSelectionRangeValue(DependencyObject element, object value)
        {
            Debug.Assert(element is ZoomBar, "element is ZoomBar");

            var cntrl = element as ZoomBar;
            Debug.Assert(cntrl != null, "cntrl != null");
            if (cntrl != null)
            {
                var defaultMetadata = MinSelectionRangeProperty.DefaultMetadata;
                double newValue =
                    DependencyPropertyUtil.ExtractDouble(
                        value,
                        defaultMetadata == null ? 0.0 : (double)(defaultMetadata.DefaultValue ?? 0.0));

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
        /// <value>Current selection range value.</value>
        [Category("Common")]
        public double SelectionRange
        {
            get
            {
                var res = this.GetValue(SelectionRangeProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            private set
            {
                this.SetValue(SelectionRangePropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey SelectionRangePropertyKey =
            DependencyProperty.RegisterReadOnly(
                "SelectionRange",
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(c_DefaultSelectionRange, OnSelectionRangeChanged));

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionRange"/>.
        /// </summary>
        public static readonly DependencyProperty SelectionRangeProperty = SelectionRangePropertyKey.DependencyProperty;

        private static void OnSelectionRangeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is ZoomBar, "obj is ZoomBar");
            Debug.Assert(args.OldValue is double, "args.OldValue is double");
            Debug.Assert(args.NewValue is double, "args.NewValue is double");

            var zoombar = obj as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
            if (zoombar != null)
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
                RoutedEvent = SelectionRangeChangedEvent,
            };
            this.RaiseEvent(newEventArgs);
        }

        #endregion

        #region LeftContentIndent

        /// <summary>
        /// Gets the left/bottom indent value of control content relative to left/bottom control edge.
        /// Indicates the current placement of content relative to the control in whole.
        /// </summary>
        /// <value>The left/bottom indent value of control content relative to left/bottom control edge.</value>
        [Category("Layout")]
        public double LeftContentIndent
        {
            get
            {
                var res = this.GetValue(LeftContentIndentProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            private set
            {
                this.SetValue(LeftContentIndentPropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey LeftContentIndentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(LeftContentIndent),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.LeftContentIndent"/>.
        /// </summary>
        public static readonly DependencyProperty LeftContentIndentProperty = LeftContentIndentPropertyKey.DependencyProperty;

        #endregion

        #region RightContentIndent

        /// <summary>
        /// Gets the right/top indent value of control content relative to right/top control edge.
        /// Indicates the current placement of content relative to the control in whole.
        /// </summary>
        /// <value>The right/top indent value of control content relative to right/top control edge.</value>
        [Category("Layout")]
        public double RightContentIndent
        {
            get
            {
                var res = this.GetValue(RightContentIndentProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            private set
            {
                this.SetValue(RightContentIndentPropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey RightContentIndentPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(RightContentIndent),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.RightContentIndent"/>.
        /// </summary>
        public static readonly DependencyProperty RightContentIndentProperty = RightContentIndentPropertyKey.DependencyProperty;

        #endregion

        #region IsSelectionDragging

        /// <summary>
        /// Gets a value indicating whether control is in process of dragging (changing) the selection range
        /// via movement of any of thumbs.
        /// Indicates that user is in process of choosing the selection value by moving a thumbs.
        /// </summary>
        /// <value>A value indicating whether control is in process of dragging (changing) the selection range
        /// via movement of any of thumbs.</value>
        [Category("Common")]
        public bool IsSelectionDragging
        {
            get
            {
                var res = this.GetValue(IsSelectionDraggingProperty);
                Debug.Assert(res != null, "res != null");
                return (bool)res;
            }

            private set
            {
                this.SetValue(IsSelectionDraggingPropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey IsSelectionDraggingPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsSelectionDragging),
                typeof(bool),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(false, OnIsSelectionDraggingChanged));

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.IsSelectionDragging"/>.
        /// </summary>
        public static readonly DependencyProperty IsSelectionDraggingProperty = IsSelectionDraggingPropertyKey.DependencyProperty;

        private static void OnIsSelectionDraggingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is ZoomBar, "obj is ZoomBar");
            Debug.Assert(args.OldValue is bool, "args.OldValue is bool");
            Debug.Assert(args.NewValue is bool, "args.NewValue is bool");

            var zoombar = obj as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
            if (zoombar != null)
            {
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
                RoutedEvent = IsSelectionDraggingChangedEvent,
            };
            this.RaiseEvent(newEventArgs);
        }

        #endregion

        #region IsRaiseSelectionChangedWhileDragging Property

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.IsRaiseSelectionChangedWhileDragging"/>.
        /// </summary>
        public static readonly DependencyProperty IsRaiseSelectionChangedWhileDraggingProperty =
            DependencyProperty.Register(
                nameof(IsRaiseSelectionChangedWhileDragging),
                typeof(bool),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender),
                DependencyPropertyUtil.IsValidBoolValue);

        /// <summary>
        /// Gets or sets a value indicating whether to raise <see cref="ZoomBar.SelectionChanged"/> event
        /// when control is in process of changing a selection value via dragging a thumb.
        /// In other word when <see cref="ZoomBar.IsSelectionDragging"/> is <c>true</c>.
        /// </summary>
        /// <value>A value indicating whether to raise <see cref="ZoomBar.SelectionChanged"/> event
        /// when control is in process of changing a selection value via dragging a thumb.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public bool IsRaiseSelectionChangedWhileDragging
        {
            get
            {
                var res = this.GetValue(IsRaiseSelectionChangedWhileDraggingProperty);
                Debug.Assert(res != null, "res != null");
                return (bool)res;
            }

            set
            {
                this.SetValue(IsRaiseSelectionChangedWhileDraggingProperty, value);
            }
        }

        #endregion


        #region AutoToolTipValueConverter Property

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.AutoToolTipValueConverter"/>.
        /// </summary>
        public static readonly DependencyProperty AutoToolTipValueConverterProperty =
            DependencyProperty.Register(
                nameof(AutoToolTipValueConverter),
                typeof(IRangeValueToStringConverter<double>),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the converter of selection values to their string representations for showing in autotooltips.
        /// If not <c>null</c> then <see cref="ZoomBar.AutoToolTipFormat"/> ignores.
        /// </summary>
        /// <value>The converter of selection values to their string representations for showing in autotooltips.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public IRangeValueToStringConverter<double> AutoToolTipValueConverter
        {
            get { return (IRangeValueToStringConverter<double>)this.GetValue(AutoToolTipValueConverterProperty); }
            set { this.SetValue(AutoToolTipValueConverterProperty, value); }
        }

        #endregion

        #region AutoToolTipValueConverterParameter Property

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.AutoToolTipValueConverterParameter"/>.
        /// </summary>
        public static readonly DependencyProperty AutoToolTipValueConverterParameterProperty =
            DependencyProperty.Register(
                nameof(AutoToolTipValueConverterParameter),
                typeof(object),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the parameter for <see cref="AutoToolTipValueConverter"/>.
        /// </summary>
        /// <value>The parameter for <see cref="AutoToolTipValueConverter"/>.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public object AutoToolTipValueConverterParameter
        {
            get { return this.GetValue(AutoToolTipValueConverterParameterProperty); }
            set { this.SetValue(AutoToolTipValueConverterParameterProperty, value); }
        }

        #endregion

        #region AutoToolTipFormat Property

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.AutoToolTipFormat"/>.
        /// </summary>
        public static readonly DependencyProperty AutoToolTipFormatProperty
                = DependencyProperty.Register(
                    nameof(AutoToolTipFormat),
                    typeof(string),
                    typeof(ZoomBar),
                    new FrameworkPropertyMetadata());

        /// <summary>
        /// Gets or sets the format string for showing a values in the auto tooltips.
        /// Uses only if <see cref="ZoomBar.AutoToolTipValueConverter"/> is <c>null</c>.
        /// </summary>
        /// <value>The format string for showing a values in the auto tooltips.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public string AutoToolTipFormat
        {
            get
            {
                return (string)this.GetValue(AutoToolTipFormatProperty);
            }

            set
            {
                this.SetValue(AutoToolTipFormatProperty, value);
            }
        }

        #endregion

        #region AutoToolTipPrecision Property

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.AutoToolTipPrecision"/>.
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register(
            nameof(AutoToolTipPrecision),
            typeof(int),
            typeof(ZoomBar),
            new FrameworkPropertyMetadata(4),
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

        #region AutoToolTipPlacement Property

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.AutoToolTipPlacement"/>.
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPlacementProperty
            = DependencyProperty.Register(
                nameof(AutoToolTipPlacement),
                typeof(AutoToolTipPlacement),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(AutoToolTipPlacement.BottomRight),
                DependencyPropertyUtil.IsValidAutoToolTipPlacement);

        /// <summary>
        /// Gets or sets the placement where automatic <see cref="System.Windows.Controls.ToolTip"/> is positioned on the control.
        /// </summary>
        /// <value>The placement where automatic <see cref="System.Windows.Controls.ToolTip"/> is positioned on the control.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get
            {
                var res = this.GetValue(AutoToolTipPlacementProperty);
                Debug.Assert(res != null, "res != null");
                return (AutoToolTipPlacement)res;
            }

            set
            {
                this.SetValue(AutoToolTipPlacementProperty, value);
            }
        }

        #endregion


        #region Visual style properties

        #region ThumbSize

        /// <summary>
        /// Gets or sets size (width or height) of start and end thumbs.
        /// </summary>
        /// <value>Size (width or height) of start and end thumbs.</value>
        [Bindable(true)]
        [Category("Layout")]
        public double ThumbSize
        {
            get
            {
                var res = this.GetValue(ThumbSizeProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            set
            {
                this.SetValue(ThumbSizeProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.ThumbSize"/>.
        /// </summary>
        public static readonly DependencyProperty ThumbSizeProperty =
            DependencyProperty.Register(
                nameof(ThumbSize),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(
                    DefaultThumbSize,
                    FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.AffectsArrange,
                    null,
                    CoerceThumbSizeValue));

        private static object CoerceThumbSizeValue(DependencyObject element, object value)
        {
            Debug.Assert(element is ZoomBar, "element is ZoomBar");

            var cntrl = element as ZoomBar;
            Debug.Assert(cntrl != null, "cntrl != null");
            if (cntrl != null)
            {
                var defaultMetadata = ThumbSizeProperty.DefaultMetadata;
                double newValue =
                    DependencyPropertyUtil.ExtractDouble(
                        value,
                        defaultMetadata == null ? 0.0 : (double)(defaultMetadata.DefaultValue ?? 0.0));

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
        /// <value>The <see cref="Brush"/> for the "not selected" area,
        /// i.e. area of control located beyond currently selected range.</value>
        [Category("Brush")]
        public Brush NotSelectedBackground
        {
            get { return (Brush)this.GetValue(NotSelectedBackgroundProperty); }
            set { this.SetValue(NotSelectedBackgroundProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.NotSelectedBackground"/>.
        /// </summary>
        public static readonly DependencyProperty NotSelectedBackgroundProperty =
            DependencyProperty.Register(
                nameof(NotSelectedBackground),
                typeof(Brush),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(Brushes.White));

        #endregion

        #region NotSelectedOpacity Property

        /// <summary>
        /// Gets or sets the opacity for the "not selected" area,
        /// i.e. area of control located beyond currently selected range.
        /// </summary>
        /// <value>The opacity for the "not selected" area,
        /// i.e. area of control located beyond currently selected range.</value>
        [Category("Appearance")]
        public double NotSelectedOpacity
        {
            get
            {
                var res = this.GetValue(NotSelectedOpacityProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            set
            {
                this.SetValue(NotSelectedOpacityProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.NotSelectedOpacity"/>.
        /// </summary>
        public static readonly DependencyProperty NotSelectedOpacityProperty =
            DependencyProperty.Register(
                nameof(NotSelectedOpacity),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultNotSelectedOpacity));

        #endregion

        #region SelectionBorderBackground Property

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> for the background of border of selection area.
        /// </summary>
        /// <value>The <see cref="Brush"/> for the background of border of selection area.</value>
        [Category("Brush")]
        public Brush SelectionBorderBackground
        {
            get { return (Brush)this.GetValue(SelectionBorderBackgroundProperty); }
            set { this.SetValue(SelectionBorderBackgroundProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionBorderBackground"/>.
        /// </summary>
        public static readonly DependencyProperty SelectionBorderBackgroundProperty =
            DependencyProperty.Register(
                nameof(SelectionBorderBackground),
                typeof(Brush),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(Brushes.Red));

        #endregion

        #region SelectionBorderOpacity Property

        /// <summary>
        /// Gets or sets the opacity for the border of selection area.
        /// </summary>
        /// <value>The opacity for the border of selection area.</value>
        [Category("Appearance")]
        public double SelectionBorderOpacity
        {
            get
            {
                var res = this.GetValue(SelectionBorderOpacityProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            set
            {
                this.SetValue(SelectionBorderOpacityProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionBorderOpacity"/>.
        /// </summary>
        public static readonly DependencyProperty SelectionBorderOpacityProperty =
            DependencyProperty.Register(
                nameof(SelectionBorderOpacity),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultSelectionBorderOpacity));

        #endregion

        #region SelectionBorderThickness Property

        /// <summary>
        /// Gets or sets the thickness for the border of selection area.
        /// </summary>
        /// <value>The thickness for the border of selection area.</value>
        public double SelectionBorderThickness
        {
            get
            {
                var res = this.GetValue(SelectionBorderThicknessProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            set
            {
                this.SetValue(SelectionBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionBorderThickness"/>.
        /// </summary>
        public static readonly DependencyProperty SelectionBorderThicknessProperty =
            DependencyProperty.Register(
                nameof(SelectionBorderThickness),
                typeof(double),
                typeof(ZoomBar),
                new FrameworkPropertyMetadata(DefaultSelectionBorderThickness));

        #endregion

        #endregion Visual style properties

        #endregion Dependency properties

        #region Public Events

        #region IsSelectionDraggingChanged

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.IsSelectionDraggingChanged"/>.
        /// </summary>
        public static readonly RoutedEvent IsSelectionDraggingChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(IsSelectionDraggingChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when <see cref="ZoomBar.IsSelectionDragging"/> changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsSelectionDraggingChanged
        {
            add { this.AddHandler(IsSelectionDraggingChangedEvent, value); }
            remove { this.RemoveHandler(IsSelectionDraggingChangedEvent, value); }
        }

        #endregion IsSelectionDraggingChanged

        #region SelectionStartChanged

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionStartChanged"/>.
        /// </summary>
        public static readonly RoutedEvent SelectionStartChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectionStartChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when <see cref="ZoomBar.SelectionStart"/> changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SelectionStartChanged
        {
            add { this.AddHandler(SelectionStartChangedEvent, value); }
            remove { this.RemoveHandler(SelectionStartChangedEvent, value); }
        }

        #endregion SelectionStartChanged

        #region SelectionEndChanged

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionEndChanged"/>.
        /// </summary>
        public static readonly RoutedEvent SelectionEndChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectionEndChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when <see cref="ZoomBar.SelectionEnd"/> changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SelectionEndChanged
        {
            add { this.AddHandler(SelectionEndChangedEvent, value); }
            remove { this.RemoveHandler(SelectionEndChangedEvent, value); }
        }

        #endregion SelectionEndChanged

        #region SelectionRangeChanged

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionRangeChanged"/>.
        /// </summary>
        public static readonly RoutedEvent SelectionRangeChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectionRangeChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when <see cref="ZoomBar.SelectionRange"/> changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> SelectionRangeChanged
        {
            add { this.AddHandler(SelectionRangeChangedEvent, value); }
            remove { this.RemoveHandler(SelectionRangeChangedEvent, value); }
        }

        #endregion SelectionRangeChanged

        #region SelectionChanged

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionChanged"/>.
        /// </summary>
        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectionChanged),
                RoutingStrategy.Bubble,
                typeof(EventHandler<SelectionDragCompletedEventArgs>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when any of properties <see cref="ZoomBar.SelectionStart"/>, <see cref="ZoomBar.SelectionEnd"/>
        /// or <see cref="ZoomBar.SelectionRange"/> changed.
        /// </summary>
        public event EventHandler<SelectionDragCompletedEventArgs> SelectionChanged
        {
            add { this.AddHandler(SelectionChangedEvent, value); }
            remove { this.RemoveHandler(SelectionChangedEvent, value); }
        }

        #endregion SelectionChanged

        #region SelectionDragCompleted

        /// <summary>
        /// Dependency property for <see cref="ZoomBar.SelectionDragCompleted"/>.
        /// </summary>
        public static readonly RoutedEvent SelectionDragCompletedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectionDragCompleted),
                RoutingStrategy.Bubble,
                typeof(EventHandler<SelectionDragCompletedEventArgs>),
                typeof(ZoomBar));

        /// <summary>
        /// Occurs when dragging of any of the thumbs completed.
        /// </summary>
        public event EventHandler<SelectionDragCompletedEventArgs> SelectionDragCompleted
        {
            add { this.AddHandler(SelectionDragCompletedEvent, value); }
            remove { this.RemoveHandler(SelectionDragCompletedEvent, value); }
        }

        #endregion SelectionDragCompleted

        #endregion Public Events

        #region Commands

        /// <summary>
        /// Gets routed command for decreasing (left/down) of selection range in whole.
        /// </summary>
        /// <value>Routed command for decreasing (left/down) of selection range in whole.</value>
        public static RoutedCommand ShiftLeftCommand { get; private set; }

        /// <summary>
        /// Gets routed command for increasing (right/up) of selection range in whole.
        /// </summary>
        /// <value>Routed command for increasing (right/up) of selection range in whole.</value>
        public static RoutedCommand ShiftRightCommand { get; private set; }


        private static void InitializeCommands()
        {
            ShiftLeftCommand = new RoutedCommand(nameof(ShiftLeftCommand), typeof(ZoomBar));
            ShiftRightCommand = new RoutedCommand(nameof(ShiftRightCommand), typeof(ZoomBar));

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
            if (!(sender is ZoomBar zoombar))
            {
                return;
            }

            double shiftValue = DependencyPropertyUtil.ConvertToDouble(e.Parameter, zoombar.ShiftValue);
            zoombar.OnShiftLeft(shiftValue);
        }

        private static void ShiftLeftCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!(sender is ZoomBar zoombar))
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = zoombar.SelectionStart > zoombar.Minimum;
        }

        private static void OnShiftRightCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(sender is ZoomBar zoombar))
            {
                return;
            }

            double shiftValue = DependencyPropertyUtil.ConvertToDouble(e.Parameter, zoombar.ShiftValue);
            zoombar.OnShiftRight(shiftValue);
        }

        private static void ShiftRightCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (!(sender is ZoomBar zoombar))
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = zoombar.SelectionEnd < zoombar.Maximum;
        }

        /// <summary>
        /// Handler for <see cref="ShiftLeftCommand"/>.
        /// </summary>
        /// <param name="shiftValue">Shift value.</param>
        protected virtual void OnShiftLeft(double shiftValue)
        {
            this.DoShift(-shiftValue);
        }

        /// <summary>
        /// Handler for <see cref="ShiftRightCommand"/>.
        /// </summary>
        /// <param name="shiftValue">Shift value.</param>
        protected virtual void OnShiftRight(double shiftValue)
        {
            this.DoShift(shiftValue);
        }

        /// <summary>
        /// Performs shift (move) of the selection range on received value, if it possible.
        /// Direction of shift determines by the sign of <paramref name="shiftValue"/> -
        /// if it negative - shift left/down,
        /// if it positive - shift right/up.
        /// </summary>
        /// <param name="shiftValue">Shift value.</param>
        public void DoShift(double shiftValue)
        {
            if (DoubleUtil.LessThanOrClose(Math.Abs(shiftValue), double.Epsilon) || this.RangeSlider == null)
            {
                return;
            }

            _ = this.RangeSlider.MoveRangeToNextTick(Math.Abs(shiftValue), shiftValue < 0.0);
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

            this.RangeSlider = this.GetTemplateChild(RangeSliderPartName) as SimpleNumericRangeSlider;
            if (this.RangeSlider != null)
            {
                _ = this.RangeSlider.ApplyTemplate();

                var binding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(SelectionStartProperty),
                        Mode = BindingMode.TwoWay,
                    };
                _ = this.RangeSlider.SetBinding(SimpleNumericRangeSlider.StartValueProperty, binding);

                binding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath(SelectionEndProperty),
                        Mode = BindingMode.TwoWay,
                    };
                _ = this.RangeSlider.SetBinding(SimpleNumericRangeSlider.EndValueProperty, binding);

                binding = new Binding { Source = this, Path = new PropertyPath(MinSelectionRangeProperty) };
                _ = this.RangeSlider.SetBinding(SimpleNumericRangeSlider.MinRangeValueProperty, binding);

                // initialize interval (range) here, because it set only inside RangeSlider_RangeValueChanged
                this.SelectionRange = this.RangeSlider.RangeValue;

                this.RangeSlider.RangeValueChanged += this.RangeSlider_RangeValueChanged;
                this.RangeSlider.ValueChanged += this.RangeSlider_ValueChanged;
                this.RangeSlider.IsRangeDraggingChanged += this.RangeSlider_IsRangeDraggingChanged;
                this.RangeSlider.RangeDragCompleted += this.RangeSlider_RangeDragCompleted;

                var track = this.RangeSlider.Track;
                if (track != null)
                {
                    this.StartThumb = track.StartThumb;
                    if (this.StartThumb != null)
                    {
                        this.StartThumb.SizeChanged += this.Thumb_SizeChanged;

                        this.StartThumb.Focusable = false;
                    }

                    this.EndThumb = track.EndThumb;
                    if (this.EndThumb != null)
                    {
                        this.EndThumb.SizeChanged += this.Thumb_SizeChanged;

                        this.EndThumb.Focusable = false;
                    }
                }
            }

            this.ContentContainer = this.GetTemplateChild(ContentContainerPartName) as FrameworkElement;
            if (this.ContentContainer != null)
            {
                // this.ContentContainer.ApplyTemplate(); // don't know whether it necessary here, or not... need research!

                this.ContentContainer.SizeChanged += this.Content_SizeChanged;
            }

            this.ShiftLeftButton = this.GetTemplateChild(ShiftLeftButtonPartName) as ButtonBase;
            if (this.ShiftLeftButton != null)
            {
                this.ShiftLeftButton.Focusable = false;
                this.ShiftLeftButton.Command = ShiftLeftCommand;
            }

            this.ShiftRightButton = this.GetTemplateChild(ShiftRightButtonPartName) as ButtonBase;
            if (this.ShiftRightButton != null)
            {
                this.ShiftRightButton.Focusable = false;
                this.ShiftRightButton.Command = ShiftRightCommand;
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
                if (this.ContentContainer != null)
                {
                    this.ContentContainer.SizeChanged -= this.Content_SizeChanged;

                    this.ContentContainer = null;
                }

                if (this.RangeSlider != null)
                {
                    BindingOperations.ClearAllBindings(this.RangeSlider);

                    this.RangeSlider.RangeValueChanged -= this.RangeSlider_RangeValueChanged;
                    this.RangeSlider.ValueChanged -= this.RangeSlider_ValueChanged;
                    this.RangeSlider.IsRangeDraggingChanged -= this.RangeSlider_IsRangeDraggingChanged;
                    this.RangeSlider.RangeDragCompleted -= this.RangeSlider_RangeDragCompleted;

                    this.RangeSlider = null;
                }

                if (this.StartThumb != null)
                {
                    this.StartThumb.SizeChanged -= this.Thumb_SizeChanged;

                    this.StartThumb = null;
                }

                if (this.EndThumb != null)
                {
                    this.EndThumb.SizeChanged -= this.Thumb_SizeChanged;

                    this.EndThumb = null;
                }

                this.ShiftLeftButton = null;
                this.ShiftRightButton = null;
            }

            base.OnTemplateChanged(oldTemplate, newTemplate);
        }

        /// <summary>
        /// This is a class handler for MouseLeftButtonDown event.
        /// The purpose of this handle is to move input focus to ZoomBar when user pressed
        /// mouse left button on any part of slider that is not focusable.
        /// </summary>
        /// <param name="sender"><see cref="ZoomBar"/>.</param>
        /// <param name="e">Data for event.</param>
        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.Assert(sender is ZoomBar, "sender is ZoomBar");

            var zoombar = sender as ZoomBar;
            Debug.Assert(zoombar != null, "zoombar != null");
            if (zoombar == null)
            {
                return;
            }

            // When someone click on a part in the ZoomBar and it's not focusable
            // ZoomBar needs to take the focus in order to process keyboard correctly
            if (!zoombar.IsKeyboardFocusWithin)
            {
                e.Handled = zoombar.Focus() || e.Handled;
            }
        }

        private void RangeSlider_RangeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.SelectionRange = e.NewValue;
        }

        private void RangeSlider_ValueChanged(object sender, RangeDragCompletedEventArgs<double> e)
        {
            var newEventArgs = new SelectionDragCompletedEventArgs(e.OldStartValue, e.OldEndValue, e.NewStartValue, e.NewEndValue)
            {
                RoutedEvent = SelectionChangedEvent,
            };
            this.RaiseEvent(newEventArgs);
        }

        private void RangeSlider_IsRangeDraggingChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            this.IsSelectionDragging = e.NewValue;
        }

        private void RangeSlider_RangeDragCompleted(object sender, RangeDragCompletedEventArgs<double> e)
        {
            var newEventArgs = new SelectionDragCompletedEventArgs(e.OldStartValue, e.OldEndValue, e.NewStartValue, e.NewEndValue)
            {
                RoutedEvent = SelectionDragCompletedEvent,
            };
            this.RaiseEvent(newEventArgs);
        }

        private void Content_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!e.WidthChanged)
            {
                return;
            }

            this.RefreshContentIndentValues();
        }

        private void Thumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.ResizeContentContainerMargin();
            this.RefreshContentIndentValues();
        }

        #region IRanged<double, double>

        /// <summary>
        /// Gets a value indicating whether this object behaves like a single value object, i.e. start value equals to end value,
        /// so interval (range) value equals to zero.
        /// </summary>
        /// <value>A value indicating whether this object behaves like a single value object, i.e. start value equals to end value,
        /// so interval (range) value equals to zero.</value>
        public bool IsSingleValue
        {
            get { return false; }
        }

        /// <summary>
        /// Method for converting <c>double</c> to value type.
        /// </summary>
        /// <param name="value">Value to convert from.</param>
        /// <returns>Always <paramref name="value"/>.</returns>
        public double DoubleToValue(double value)
        {
            return value;
        }

        /// <summary>
        /// Method for converting value type to <c>double</c>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Always <paramref name="value"/>.</returns>
        public double ValueToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Method for converting <c>double</c> to interval type.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Always <paramref name="value"/>.</returns>
        public double DoubleToInterval(double value)
        {
            return value;
        }

        /// <summary>
        /// Method for converting interval value to <c>double</c>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Always <paramref name="value"/>.</returns>
        public double IntervalToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Gets start interval value.
        /// </summary>
        /// <value><see cref="SelectionStart"/>.</value>
        public double StartValue
        {
            get { return this.SelectionStart; }
        }

        /// <summary>
        /// Gets end interval value.
        /// </summary>
        /// <value><see cref="SelectionEnd"/>.</value>
        public double EndValue
        {
            get { return this.SelectionEnd; }
        }

        /// <summary>
        /// Gets minimum available interval (range) value.
        /// </summary>
        /// <value><see cref="MinSelectionRange"/>.</value>
        public double MinRangeValue
        {
            get { return this.MinSelectionRange; }
        }

        #endregion // IRanged<double, double>

#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1202 // Elements should be ordered by access
    }

#pragma warning restore CA1501 // Avoid excessive inheritance
}
