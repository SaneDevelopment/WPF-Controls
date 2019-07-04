// -----------------------------------------------------------------------
// <copyright file="RangeTrack.cs" company="Sane Development">
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
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;

#pragma warning disable CA1501 // Avoid excessive inheritance

    /// <summary>
    /// Control primitive, that manages positions of three <see cref="Thumb"/>s and two <see cref="RepeatButton"/>s,
    /// which uses for changing of <see cref="RangeTrack{T, TInterval}.StartValue"/> and <see cref="RangeTrack{T, TInterval}.EndValue"/>.
    /// </summary>
    /// <typeparam name="T">Type of values.</typeparam>
    /// <typeparam name="TInterval">Type of interval value.</typeparam>
    public abstract class RangeTrack<T, TInterval> : FrameworkElement
    {
#pragma warning disable SA1201 // Elements should appear in the correct order

        #region Private fields
#pragma warning disable SA1308 // Variable names should not be prefixed
#pragma warning disable SA1303 // Const field names should begin with upper-case letter
#pragma warning disable SA1310 // Field names should not contain underscore

        private const int c_MaxVisualChildrenCount = 5; // maximum count of visual children elements

        private RepeatButton m_DecreaseButton;
        private RepeatButton m_IncreaseButton;
        private Thumb m_StartThumb;
        private Thumb m_RangeThumb;
        private Thumb m_EndThumb;

        private double m_Density = double.NaN;

        private Visual[] m_VisualChildren;

#pragma warning restore SA1310 // Field names should not contain underscore
#pragma warning restore SA1303 // Const field names should begin with upper-case letter
#pragma warning restore SA1308 // Variable names should not be prefixed
        #endregion Private fields

        static RangeTrack()
        {
            IsEnabledProperty.OverrideMetadata(
                typeof(RangeTrack<T, TInterval>),
                new UIPropertyMetadata((d, e) =>
                    {
                        if ((bool)e.NewValue)
                        {
                            Mouse.Synchronize();
                        }
                    }));
        }

        #region Control Parts

        /// <summary>
        /// Gets or sets button for decreasing interval.
        /// </summary>
        /// <value>Button for decreasing interval.</value>
        public virtual RepeatButton DecreaseRepeatButton
        {
            get
            {
                return this.m_DecreaseButton;
            }

            set
            {
                if (Equals(this.m_DecreaseButton, value))
                {
                    throw new NotSupportedException();
                }

                this.UpdateComponent(this.m_DecreaseButton, value);
                this.m_DecreaseButton = value;
                if (this.m_DecreaseButton != null)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Gets or sets button for increasing interval.
        /// </summary>
        /// <value>Button for increasing interval.</value>
        public virtual RepeatButton IncreaseRepeatButton
        {
            get
            {
                return this.m_IncreaseButton;
            }

            set
            {
                if (Equals(this.m_IncreaseButton, value))
                {
                    throw new NotSupportedException();
                }

                this.UpdateComponent(this.m_IncreaseButton, value);
                this.m_IncreaseButton = value;
                if (this.m_IncreaseButton != null)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Gets or sets start interval thumb.
        /// </summary>
        /// <value>Start interval thumb.</value>
        public virtual Thumb StartThumb
        {
            get
            {
                return this.m_StartThumb;
            }

            set
            {
                this.UpdateComponent(this.m_StartThumb, value);
                this.m_StartThumb = value;
            }
        }

        /// <summary>
        /// Gets or sets interval (range) thumb.
        /// </summary>
        /// <value>Interval (range) thumb.</value>
        public virtual Thumb RangeThumb
        {
            get
            {
                return this.m_RangeThumb;
            }

            set
            {
                this.UpdateComponent(this.m_RangeThumb, value);
                this.m_RangeThumb = value;
            }
        }

        /// <summary>
        /// Gets or sets end interval thumb.
        /// </summary>
        /// <value>End interval thumb.</value>
        public virtual Thumb EndThumb
        {
            get
            {
                return this.m_EndThumb;
            }

            set
            {
                this.UpdateComponent(this.m_EndThumb, value);
                this.m_EndThumb = value;
            }
        }

        #endregion

        #region Dependency properties

        #region Minimum

        /// <summary>
        /// Gets or sets minimum available value.
        /// </summary>
        /// <value>Minimum available value.</value>
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
        /// Dependency property for <see cref="RangeTrack{T, TInterval}.Minimum"/>.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = RangeBaseControl<T, TInterval>.MinimumProperty.AddOwner(
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion Minimum

        #region Maximum

        /// <summary>
        /// Gets or sets maximum available value.
        /// </summary>
        /// <value>Maximum available value.</value>
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
        /// Dependency property for <see cref="RangeTrack{T, TInterval}.Maximum"/>.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = RangeBaseControl<T, TInterval>.MaximumProperty.AddOwner(
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion Maximum

        #region StartValue

        /// <summary>
        /// Gets or sets start value of interval (range).
        /// </summary>
        /// <value>Start value of interval (range).</value>
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
        /// Dependency property for <see cref="RangeTrack{T, TInterval}.StartValue"/>.
        /// </summary>
        public static readonly DependencyProperty StartValueProperty = RangeBaseControl<T, TInterval>.StartValueProperty.AddOwner(
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion StartValue

        #region EndValue

        /// <summary>
        /// Gets or sets end value of interval (range).
        /// </summary>
        /// <value>End value of interval (range).</value>
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
        /// Dependency property for <see cref="RangeTrack{T, TInterval}.EndValue"/>.
        /// </summary>
        public static readonly DependencyProperty EndValueProperty = RangeBaseControl<T, TInterval>.EndValueProperty.AddOwner(
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion EndValue

        #region Orientation

        /// <summary>
        /// Gets or sets control orientation.
        /// </summary>
        /// <value>Control orientation.</value>
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

        /// <summary>
        /// Dependency property for <see cref="RangeTrack{T, TInterval}.Orientation"/>.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure),
            DependencyPropertyUtil.IsValidOrientation);

        #endregion Orientation

        #endregion

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <value>The number of visual child elements for this element (0 to 5).</value>
        protected override int VisualChildrenCount
        {
            get
            {
                if (this.m_VisualChildren == null)
                {
                    return 0;
                }

                int i = 0;
                for (; i < this.m_VisualChildren.Length; i++)
                {
                    if (this.m_VisualChildren[i] == null)
                    {
                        return i;
                    }
                }

                return i;
            }
        }


        /// <summary>
        /// Calculates the value change of <see cref="RangeTrack{T, TInterval}.StartValue" /> or <see cref="RangeTrack{T, TInterval}.EndValue" />
        /// from the distance that the mouse (or thumb) has moved.
        /// </summary>
        /// <param name="horizontalChange">The horizontal distance that the mouse or thumb has moved.</param>
        /// <param name="verticalChange">The vertical distance that the mouse or thumb has moved.</param>
        /// <returns>Change of start value or end value corresponding to distance that the mouse or thumb has moved.</returns>
        public virtual double ValueFromDistance(double horizontalChange, double verticalChange)
        {
            return (this.Orientation == Orientation.Horizontal)
                       ? horizontalChange * this.m_Density
                       : -1.0 * verticalChange * this.m_Density;
        }

        /// <summary>
        /// Method performs immediate processing of <see cref="FrameworkElement.OnApplyTemplate"/>.
        /// </summary>
        public void DoApplyTemplate()
        {
            var templatedParent = this.TemplatedParent as IRangeTrackTemplatedParent<T, TInterval>;
            if (templatedParent != null)
            {
                templatedParent.OnApplyRangeTrackTemplate(this.TemplatedParent, this);
            }
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="System.Windows.FrameworkElement.ApplyTemplate"/>().
        /// Invokes <see cref="DoApplyTemplate"/>().
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.DoApplyTemplate();
        }

        /// <summary>
        /// Attaches a binding to <paramref name="element"/>, based on the <see cref="FrameworkElement.TemplatedParent"/> as the binding source.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <param name="target">Identifies the property where the binding should be established.</param>
        /// <param name="source">A property path that describes a path to single dependency property.</param>
        public void BindChildToTemplatedParent(FrameworkElement element, DependencyProperty target, DependencyProperty source)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (element != null)
            {
                var binding = new Binding
                {
                    Source = this.TemplatedParent,
                    Path = new PropertyPath(source),
                };
                element.SetBinding(target, binding);
            }
        }

        /// <summary>
        /// Attaches a binding to this element, based on the templated parent relatively.
        /// </summary>
        /// <param name="target">Identifies the property where the binding should be established.</param>
        /// <param name="source">A property path that describes a path to single dependency property.</param>
        public void BindToTemplatedParent(DependencyProperty target, DependencyProperty source)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var binding = new Binding
            {
                RelativeSource = RelativeSource.TemplatedParent,
                Path = new PropertyPath(source),
            };
            this.SetBinding(target, binding);
        }


        /// <summary>
        /// Method should convert <c>double</c> to value type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">Number to convert.</param>
        /// <returns>Number, converted to <typeparamref name="T"/>.</returns>
        protected abstract T DoubleToValue(double value);

        /// <summary>
        /// Method should convert value of type <typeparamref name="T"/> to <c>double</c>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Number representation of <paramref name="value"/>.</returns>
        protected abstract double ValueToDouble(T value);


        /// <summary>
        /// Positions child elements and determines a size for a <see cref="RangeTrack{T, TInterval}"/>.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double decreaseButtonLength, startThumbLength, rangeThumbLength, endThumbLength, increaseButtonLength;
            Debug.Assert(finalSize.Width >= 0, "finalSize.Width >= 0");
            bool isVertical = this.Orientation == Orientation.Vertical;

            this.ComputeLengths(
                finalSize,
                isVertical,
                out decreaseButtonLength,
                out startThumbLength,
                out rangeThumbLength,
                out endThumbLength,
                out increaseButtonLength);

            var location = default(Point);
            Size size = finalSize;
            if (isVertical)
            {
                CoerceLength(ref decreaseButtonLength, finalSize.Height);
                CoerceLength(ref increaseButtonLength, finalSize.Height);
                CoerceLength(ref startThumbLength, finalSize.Height);
                CoerceLength(ref rangeThumbLength, finalSize.Height);
                CoerceLength(ref endThumbLength, finalSize.Height);

                // IncreaseRepeatButton
                location.Y = 0.0;
                size.Height = increaseButtonLength;
                if (this.IncreaseRepeatButton != null)
                {
                    this.IncreaseRepeatButton.Arrange(new Rect(location, size));
                }

                // StartThumb
                location.Y = increaseButtonLength;
                size.Height = endThumbLength;
                if (this.EndThumb != null)
                {
                    this.EndThumb.Arrange(new Rect(location, size));
                }

                // EndThumb
                location.Y = increaseButtonLength + endThumbLength + rangeThumbLength;
                size.Height = startThumbLength;
                if (this.StartThumb != null)
                {
                    this.StartThumb.Arrange(new Rect(location, size));
                }

                // DecreaseRepeatButton
                location.Y = increaseButtonLength + endThumbLength + rangeThumbLength + startThumbLength;
                size.Height = decreaseButtonLength;
                if (this.DecreaseRepeatButton != null)
                {
                    this.DecreaseRepeatButton.Arrange(new Rect(location, size));
                }

                // RangeThumb. Arranging in the last place as it done in PresentationFramework.dll.
                location.Y = increaseButtonLength + endThumbLength;
                size.Height = rangeThumbLength;
                if (this.RangeThumb != null)
                {
                    this.RangeThumb.Arrange(new Rect(location, size));
                }
            }
            else
            {
                CoerceLength(ref decreaseButtonLength, finalSize.Width);
                CoerceLength(ref increaseButtonLength, finalSize.Width);
                CoerceLength(ref startThumbLength, finalSize.Width);
                CoerceLength(ref rangeThumbLength, finalSize.Width);
                CoerceLength(ref endThumbLength, finalSize.Width);

                // DecreaseRepeatButton
                location.X = 0.0;
                size.Width = decreaseButtonLength;
                if (this.DecreaseRepeatButton != null)
                {
                    this.DecreaseRepeatButton.Arrange(new Rect(location, size));
                }

                // StartThumb
                location.X = decreaseButtonLength;
                size.Width = startThumbLength;
                if (this.StartThumb != null)
                {
                    this.StartThumb.Arrange(new Rect(location, size));
                }

                // EndThumb
                location.X = decreaseButtonLength + startThumbLength + rangeThumbLength;
                size.Width = endThumbLength;
                if (this.EndThumb != null)
                {
                    this.EndThumb.Arrange(new Rect(location, size));
                }

                // IncreaseRepeatButton
                location.X = decreaseButtonLength + startThumbLength + rangeThumbLength + endThumbLength;
                size.Width = increaseButtonLength;
                if (this.IncreaseRepeatButton != null)
                {
                    this.IncreaseRepeatButton.Arrange(new Rect(location, size));
                }

                // RangeThumb. Arranging in the last place as it done in PresentationFramework.dll.
                location.X = decreaseButtonLength + startThumbLength;
                size.Width = rangeThumbLength;
                if (this.RangeThumb != null)
                {
                    this.RangeThumb.Arrange(new Rect(location, size));
                }
            }

            return finalSize;
        }

        /// <summary>
        /// Measures the size in layout required for child elements and determines a size for the <see cref="RangeTrack{T, TInterval}"/>.
        /// </summary>
        /// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
        /// <returns>The size that this element determines it needs during layout, based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            bool isVertical = this.Orientation == Orientation.Vertical;
            var desiredSize = new Size(0.0, 0.0);
            desiredSize = MeasureThumb(this.StartThumb, isVertical, availableSize, desiredSize);
            MeasureThumb(this.RangeThumb, isVertical, availableSize, desiredSize);
            desiredSize = MeasureThumb(this.EndThumb, isVertical, availableSize, desiredSize);
            return desiredSize;
        }

        /// <summary>
        /// Overrides <see cref="Visual.GetVisualChild(int)"/>,
        /// and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The zero-based index of the requested child element in the collection.</param>
        /// <exception cref="ArgumentOutOfRangeException">The provided index is out of range [0..<see cref="RangeTrack{T, TInterval}.VisualChildrenCount"/>-1].</exception>
        /// <returns>The requested child element. If the provided index is out of range, an exception is thrown.</returns>
        protected override Visual GetVisualChild(int index)
        {
            if (this.m_VisualChildren == null ||
                index >= this.m_VisualChildren.Length ||
                index < 0 ||
                this.m_VisualChildren[index] == null)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return this.m_VisualChildren[index];
        }


        private static void CoerceLength(ref double componentLength, double trackLength)
        {
            Debug.Assert(trackLength >= 0, "trackLength >= 0");

            if (componentLength < 0.0)
            {
                componentLength = 0.0;
            }
            else if ((componentLength > trackLength) || double.IsNaN(componentLength))
            {
                componentLength = trackLength;
            }

            Debug.Assert(componentLength >= 0.0 && componentLength <= trackLength, "componentLength >= 0.0 && componentLength <= trackLength");
        }

        private static Size MeasureThumb(Thumb thumb, bool isVertical, Size availableSize, Size desiredSize)
        {
            Debug.Assert(!desiredSize.IsEmpty, "!desiredSize.IsEmpty");

            if (thumb != null)
            {
                thumb.Measure(availableSize);
                if (!thumb.DesiredSize.IsEmpty)
                {
                    Debug.Assert(thumb.DesiredSize.Height >= 0, "thumb.DesiredSize.Height >= 0");
                    Debug.Assert(thumb.DesiredSize.Width >= 0, "thumb.DesiredSize.Width >= 0");
                    if (isVertical)
                    {
                        // making desired width of vertical slider as a max of thumbs widthes
                        desiredSize.Width = Math.Max(desiredSize.Width, thumb.DesiredSize.Width);
                        // making desired height of vertical slider as a sum of thumbs hightes
                        desiredSize.Height += thumb.DesiredSize.Height;
                    }
                    else
                    {
                        // making desired width of horizontal slider as a sum of thumbs widthes
                        desiredSize.Width += thumb.DesiredSize.Width;
                        // making desired height of horizontal slider as a max of thumbs hightes
                        Debug.Assert(desiredSize.Height >= 0, "desiredSize.Height >= 0");
                        desiredSize.Height = Math.Max(desiredSize.Height, thumb.DesiredSize.Height);
                    }
                }
            }

            Debug.Assert(!desiredSize.IsEmpty, "!desiredSize.IsEmpty");
            return desiredSize;
        }

        private void UpdateComponent(Control oldValue, Control newValue)
        {
            if (Equals(oldValue, newValue))
            {
                return;
            }

            if (this.m_VisualChildren == null)
            {
                this.m_VisualChildren = new Visual[c_MaxVisualChildrenCount];
            }

            if (oldValue != null)
            {
                this.RemoveVisualChild(oldValue);
            }

            Debug.Assert(this.m_VisualChildren != null, "this.m_VisualChildren != null");
            Debug.Assert(this.m_VisualChildren.Length == c_MaxVisualChildrenCount, "this.m_VisualChildren.Length == c_MaxVisualChildrenCount");

            int index = 0;
            while (index < this.m_VisualChildren.Length)
            {
                if (this.m_VisualChildren[index] == null)
                {
                    break;
                }

                if (Equals(this.m_VisualChildren[index], oldValue))
                {
                    while (index < (this.m_VisualChildren.Length - 1) &&
                           this.m_VisualChildren[index + 1] != null)
                    {
                        this.m_VisualChildren[index] = this.m_VisualChildren[index + 1];
                        index++;
                    }
                }
                else
                {
                    index++;
                }
            }

            Debug.Assert(index < this.m_VisualChildren.Length, "index < this.m_VisualChildren.Length");
            this.m_VisualChildren[index] = newValue;
            this.AddVisualChild(newValue);
            this.InvalidateMeasure();
            this.InvalidateArrange();

            Debug.Assert(this.m_VisualChildren != null, "this.m_VisualChildren != null");
            Debug.Assert(this.m_VisualChildren.Length == c_MaxVisualChildrenCount, "this.m_VisualChildren.Length == c_MaxVisualChildrenCount");
        }

        private void ComputeLengths(
            Size arrangeSize,
            bool isVertical,
            out double decreaseButtonLength,
            out double startThumbLength,
            out double rangeThumbLength,
            out double endThumbLength,
            out double increaseButtonLength)
        {
            Debug.Assert(!arrangeSize.IsEmpty, "!arrangeSize.IsEmpty");

            double minimum = this.ValueToDouble(this.Minimum), maximum = this.ValueToDouble(this.Maximum);
            double interval = Math.Max(0.0, maximum - minimum); // the "length" of available interval of values
            double decreaseAreaInterval = Math.Min(interval, this.ValueToDouble(this.StartValue) - minimum); // interval "length" from minimum to start value ("left" area)
            double increaseAreaInterval = Math.Min(interval, maximum - this.ValueToDouble(this.EndValue)); // interval "length" from end value to maximum ("right" area)

            double height;
            if (isVertical)
            {
                height = arrangeSize.Height;
                startThumbLength = (this.StartThumb == null) ? 0.0 : this.StartThumb.DesiredSize.Height;
                endThumbLength = (this.EndThumb == null) ? 0.0 : this.EndThumb.DesiredSize.Height;
            }
            else
            {
                height = arrangeSize.Width;
                startThumbLength = (this.StartThumb == null) ? 0.0 : this.StartThumb.DesiredSize.Width;
                endThumbLength = (this.EndThumb == null) ? 0.0 : this.EndThumb.DesiredSize.Width;
            }

            CoerceLength(ref startThumbLength, height);
            CoerceLength(ref endThumbLength, height);

            double trackLength = height - (startThumbLength + endThumbLength);
            Debug.Assert(trackLength >= 0, "trackLength >= 0");
            this.m_Density = interval / trackLength;

            decreaseButtonLength = decreaseAreaInterval / this.m_Density;
            CoerceLength(ref decreaseButtonLength, trackLength);

            increaseButtonLength = increaseAreaInterval / this.m_Density;
            CoerceLength(ref increaseButtonLength, trackLength);

            rangeThumbLength = trackLength - (decreaseButtonLength + increaseButtonLength);
            CoerceLength(ref rangeThumbLength, trackLength);
        }

#pragma warning restore SA1201 // Elements should appear in the correct order
    }

#pragma warning restore CA1501 // Avoid excessive inheritance
}
