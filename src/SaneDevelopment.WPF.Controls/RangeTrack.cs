// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RangeTrack.cs" company="Sane Development">
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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace SaneDevelopment.WPF.Controls
{
    /// <summary>
    /// Range thumb type
    /// </summary>
    public enum RangeThumbType
    {
        /// <summary>
        /// No set
        /// </summary>
        None = 0,
        /// <summary>
        /// Start thumb
        /// </summary>
        StartThumb,
        /// <summary>
        /// Interval (range) thumb
        /// </summary>
        RangeThumb,
        /// <summary>
        /// End thumb
        /// </summary>
        EndThumb
    }

    /// <summary>
    /// Control primitive, that manages positions of three <see cref="Thumb"/>s and two <see cref="RepeatButton"/>s,
    /// which uses for changing of <see cref="RangeTrack{T, TInterval}.StartValue"/> and <see cref="RangeTrack{T, TInterval}.EndValue"/>.
    /// </summary>
    public abstract class RangeTrack<T, TInterval> : FrameworkElement
    {
        #region Private fields

        private RepeatButton m_DecreaseButton, m_IncreaseButton;
        private Thumb m_StartThumb, m_RangeThumb, m_EndThumb;

        private double m_Density = double.NaN;

        private Visual[] m_VisualChildren;

        private const int c_MaxVisualChildrenCount = 5; // maximum count of visual children elements

        #endregion Private fields

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
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

        /// <summary>
        /// Method should convert <c>double</c> to value type <typeparamref name="T"/>
        /// </summary>
        /// <param name="value">Number to convert</param>
        /// <returns>Number, converted to <typeparamref name="T"/></returns>
        protected abstract T DoubleToValue(double value);
        
        /// <summary>
        /// Method should convert value of type <typeparamref name="T"/> to <c>double</c>
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Number representation of <paramref name="value"/></returns>
        protected abstract double ValueToDouble(T value);

        /// <summary>
        /// Calculates the value change of <see cref="RangeTrack{T, TInterval}.StartValue" /> or <see cref="RangeTrack{T, TInterval}.EndValue" />
        /// from the distance that the mouse (or thumb) has moved.
        /// </summary>
        /// <param name="horizontalChange">The horizontal distance that the mouse or thumb has moved</param>
        /// <param name="verticalChange">The vertical distance that the mouse or thumb has moved</param>
        /// <returns>Change of start value or end value corresponding to distance that the mouse or thumb has moved.</returns>
        public virtual double ValueFromDistance(double horizontalChange, double verticalChange)
        {
            return (this.Orientation == Orientation.Horizontal)
                       ? horizontalChange * this.Density
                       : -1.0 * verticalChange * this.Density;
        }

        #region Dependency properties

        #region Minimum

        /// <summary>
        /// Minimum available value
        /// </summary>
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
        /// Dependency property for <see cref="RangeTrack{T, TInterval}.Minimum"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        // ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty MinimumProperty = RangeBaseControl<T, TInterval>.MinimumProperty.AddOwner(
            // ReSharper restore StaticFieldInGenericType
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion Minimum

        #region Maximum

        /// <summary>
        /// Maximum available value
        /// </summary>
        public T Maximum
        {
            get
            {
                var res = this.GetValue(MaximumProperty);
                Contract.Assume(res != null);
                return (T)res;
            }
            set { this.SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeTrack{T, TInterval}.Maximum"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        // ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty MaximumProperty = RangeBaseControl<T, TInterval>.MaximumProperty.AddOwner(
            // ReSharper restore StaticFieldInGenericType
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion Maximum

        #region StartValue

        /// <summary>
        /// Start value of interval (range)
        /// </summary>
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
        /// Dependency property for <see cref="RangeTrack{T, TInterval}.StartValue"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        // ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty StartValueProperty = RangeBaseControl<T, TInterval>.StartValueProperty.AddOwner(
            // ReSharper restore StaticFieldInGenericType
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion StartValue

        #region EndValue

        /// <summary>
        /// End value of interval (range)
        /// </summary>
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
        /// Dependency property for <see cref="RangeTrack{T, TInterval}.EndValue"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        // ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty EndValueProperty = RangeBaseControl<T, TInterval>.EndValueProperty.AddOwner(
            // ReSharper restore StaticFieldInGenericType
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion EndValue

        #region Orientation

        /// <summary>
        /// Control orientation
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                var res = this.GetValue(OrientationProperty);
                Contract.Assume(res != null);
                return (Orientation)res;
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="RangeTrack{T, TInterval}.Orientation"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
// ReSharper restore StaticFieldInGenericType
            "Orientation",
            typeof(Orientation),
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure),
            DependencyPropertyUtil.IsValidOrientation);

        #endregion Orientation

        #endregion

        /// <summary>
        /// Positions child elements and determines a size for a <see cref="RangeTrack{T, TInterval}"/>.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double decreaseButtonLength, startThumbLength, rangeThumbLength, endThumbLength, increaseButtonLength;
            Contract.Assert(finalSize.Width >= 0);
            bool isVertical = this.Orientation == Orientation.Vertical;

            this.ComputeLengths(finalSize, isVertical,
                out decreaseButtonLength,
                out startThumbLength, out rangeThumbLength, out endThumbLength,
                out increaseButtonLength);

            var location = new Point();
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

        private static Size MeasureThumb(Thumb thumb, bool isVertical, Size availableSize, Size desiredSize)
        {
            Contract.Requires(!desiredSize.IsEmpty);
            Contract.Ensures(!Contract.Result<Size>().IsEmpty);

            if (thumb != null)
            {
                thumb.Measure(availableSize);
                if (!thumb.DesiredSize.IsEmpty)
                {
                    Contract.Assert(thumb.DesiredSize.Height >= 0);
                    Contract.Assert(thumb.DesiredSize.Width >= 0);
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
                        Contract.Assert(desiredSize.Height >= 0);
                        desiredSize.Height = Math.Max(desiredSize.Height, thumb.DesiredSize.Height);
                    }
                }
            }
            return desiredSize;
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

            DoApplyTemplate();
        }

        /// <summary>
        /// Gets the number of visual child elements within this element.
        /// </summary>
        /// <returns>The number of visual child elements for this element (0 to 5).</returns>
        protected override int VisualChildrenCount
        {
            get
            {
                if (this.m_VisualChildren == null)
                {
                    return 0;
                }
                int i = 0;
                for (; i < m_VisualChildren.Length; i++)
                {
                    if (m_VisualChildren[i] == null)
                        return i;
                }
                return i;
            }
        }

        /// <summary>
        /// Overrides <see cref="System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>,
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
                throw new ArgumentOutOfRangeException("index");
            }
            return this.m_VisualChildren[index];
        }

        /// <summary>
        /// Attaches a binding to <paramref name="element"/>, based on the <see cref="FrameworkElement.TemplatedParent"/> as the binding source.
        /// </summary>
        /// <param name="element">Element</param>
        /// <param name="target">Identifies the property where the binding should be established.</param>
        /// <param name="source">A property path that describes a path to single dependency property.</param>
        public void BindChildToTemplatedParent(FrameworkElement element, DependencyProperty target, DependencyProperty source)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (source == null)
                throw new ArgumentNullException("source");

            if (element != null)
            {
                var binding = new Binding
                {
                    Source = this.TemplatedParent,
                    Path = new PropertyPath(source)
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
                throw new ArgumentNullException("target");
            if (source == null)
                throw new ArgumentNullException("source");

            var binding = new Binding
            {
                RelativeSource = RelativeSource.TemplatedParent,
                Path = new PropertyPath(source)
            };
            this.SetBinding(target, binding);
        }

        private void UpdateComponent(Control oldValue, Control newValue)
        {
            Contract.Ensures(this.m_VisualChildren == null || this.m_VisualChildren.Length == c_MaxVisualChildrenCount);

            if (Equals(oldValue, newValue))
                return;

            if (this.m_VisualChildren == null)
            {
                this.m_VisualChildren = new Visual[c_MaxVisualChildrenCount];
            }
            if (oldValue != null)
            {
                this.RemoveVisualChild(oldValue);
            }
            Contract.Assume(this.m_VisualChildren != null && this.m_VisualChildren.Length == c_MaxVisualChildrenCount);
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
            Contract.Assume(index < this.m_VisualChildren.Length);
            this.m_VisualChildren[index] = newValue;
            this.AddVisualChild(newValue);
            this.InvalidateMeasure();
            this.InvalidateArrange();
            Contract.Assume(this.m_VisualChildren != null && this.m_VisualChildren.Length == c_MaxVisualChildrenCount);
        }

        private static void CoerceLength(ref double componentLength, double trackLength)
        {
            Contract.Requires(trackLength >= 0);
            Contract.Ensures(componentLength >= 0.0 && componentLength <= trackLength);

            if (componentLength < 0.0)
            {
                componentLength = 0.0;
            }
            else if ((componentLength > trackLength) || double.IsNaN(componentLength))
            {
                componentLength = trackLength;
            }
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
            Contract.Requires(!arrangeSize.IsEmpty);

            double minimum = ValueToDouble(this.Minimum), maximum = ValueToDouble(this.Maximum);
            double interval = Math.Max(0.0, maximum - minimum); // the "length" of available interval of values
            double decreaseAreaInterval = Math.Min(interval, ValueToDouble(this.StartValue) - minimum); // interval "length" from minimum to start value ("left" area)
            double increaseAreaInterval = Math.Min(interval, maximum - ValueToDouble(this.EndValue)); // interval "length" from end value to maximum ("right" area)

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
            Contract.Assume(trackLength >= 0);
            this.Density = interval / trackLength;

            decreaseButtonLength = decreaseAreaInterval / this.Density;
            CoerceLength(ref decreaseButtonLength, trackLength);

            increaseButtonLength = increaseAreaInterval / this.Density;
            CoerceLength(ref increaseButtonLength, trackLength);

            rangeThumbLength = trackLength - (decreaseButtonLength + increaseButtonLength);
            CoerceLength(ref rangeThumbLength, trackLength);
        }

        #region Control Parts

        /// <summary>
        /// Button for decreasing interval
        /// </summary>
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
        /// Button for increasing interval
        /// </summary>
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
        /// Start interval thumb
        /// </summary>
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
        /// Interval (range) thumb
        /// </summary>
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
        /// End interval thumb
        /// </summary>
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

        private double Density
        {
            get
            {
                return this.m_Density;
            }
            set
            {
                this.m_Density = value;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.m_VisualChildren == null || this.m_VisualChildren.Length == c_MaxVisualChildrenCount);
        }
    }

    /// <summary>
    /// Track that uses <c>double</c> as values and inteval type
    /// </summary>
    public class NumericRangeTrack : RangeTrack<double, double>
    {
        static NumericRangeTrack()
        {
            Type thisType = typeof(NumericRangeTrack);

            MinimumProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));
            MaximumProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsArrange));
            StartValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));
            EndValueProperty.OverrideMetadata(
                thisType,
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));
        }

        /// <summary>
        /// Converts <c>double</c> to value type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Always <paramref name="value"/></returns>
        protected override double DoubleToValue(double value)
        {
            return value;
        }
        
        /// <summary>
        /// Convert received value to <c>double</c>
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Always <paramref name="value"/></returns>
        protected override double ValueToDouble(double value)
        {
            return value;
        }

        #region Control Parts
        // ReSharper disable RedundantOverridenMember

        /// <summary>
        /// Button for decreasing interval
        /// </summary>
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
        /// Button for increasing interval
        /// </summary>
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
        /// Start interval thumb
        /// </summary>
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
        /// Interval (range) thumb
        /// </summary>
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
        /// End interval thumb
        /// </summary>
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

        // ReSharper restore RedundantOverridenMember
        #endregion Control Parts
    }

    /// <summary>
    /// Track that uses <see cref="DateTime"/> as values type and <see cref="TimeSpan"/> as inteval type
    /// </summary>
    public class DateTimeRangeTrack : RangeTrack<DateTime, TimeSpan>
    {
        static DateTimeRangeTrack()
        {
            Type thisType = typeof(DateTimeRangeTrack);

            DateTime from = new DateTime(1900, 1, 1), to = new DateTime(2000, 1, 1);

            MinimumProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(from, FrameworkPropertyMetadataOptions.AffectsArrange));
            MaximumProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(to, FrameworkPropertyMetadataOptions.AffectsArrange));
            StartValueProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(from, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));
            EndValueProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(to, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));
        }

        /// <summary>
        /// Converts <c>double</c> to <see cref="DateTime"/>
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Date initialized to a <paramref name="value"/> number of ticks.</returns>
        protected override DateTime DoubleToValue(double value)
        {
            return (value > 10.0) ? new DateTime((long)value) : DateTime.MinValue;
        }
        
        /// <summary>
        /// Converts date to number.
        /// </summary>
        /// <param name="value">Date to convert</param>
        /// <returns>The number of ticks that represent the date and time of <paramref name="value"/>.</returns>
        protected override double ValueToDouble(DateTime value)
        {
            return value.Ticks;
        }

        #region Control Parts
        // ReSharper disable RedundantOverridenMember

        /// <summary>
        /// Button for decreasing interval
        /// </summary>
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
        /// Button for increasing interval
        /// </summary>
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
        /// Start interval thumb
        /// </summary>
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
        /// Interval (range) thumb
        /// </summary>
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
        /// End interval thumb
        /// </summary>
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

        // ReSharper restore RedundantOverridenMember
        #endregion Control Parts
    }
}

