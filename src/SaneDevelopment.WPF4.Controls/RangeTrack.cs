// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RangeTrack.cs" company="Sane Development">
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
    /// Тип ползунка
    /// </summary>
    public enum RangeThumbType
    {
        /// <summary>
        /// Не задан
        /// </summary>
        None = 0,
        /// <summary>
        /// Начальный ползунок
        /// </summary>
        StartThumb,
        /// <summary>
        /// Ползунок интервала
        /// </summary>
        RangeThumb,
        /// <summary>
        /// Конечный ползунок
        /// </summary>
        EndThumb
    }

    /// <summary>
    /// Представляет собой примитив контрола, который управляет позиционированием трех ползунков <see cref="Thumb"/>
    /// и двух кнопок <see cref="RepeatButton"/>, которые используются для установки значений
    /// <see cref="RangeTrack{T, TInterval}.StartValue"/> и <see cref="RangeTrack{T, TInterval}.EndValue"/>.
    /// </summary>
    public abstract class RangeTrack<T, TInterval> : FrameworkElement
    {
        private RepeatButton m_DecreaseButton, m_IncreaseButton;
        private Thumb m_StartThumb, m_RangeThumb, m_EndThumb;

        private double m_Density = double.NaN; // плотность

        private Visual[] m_VisualChildren;

        private const int c_MaxVisualChildrenCount = 5; // максимальное число подчиненных визуальных элементов

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

        #region Dependency properties

        /// <summary>
        /// Свойство зависимости для <see cref="RangeTrack{T, TInterval}.Maximum"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty MaximumProperty = RangeBaseControl<T, TInterval>.MaximumProperty.AddOwner(
// ReSharper restore StaticFieldInGenericType
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Свойство зависимости для <see cref="RangeTrack{T, TInterval}.Minimum"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty MinimumProperty = RangeBaseControl<T, TInterval>.MinimumProperty.AddOwner(
// ReSharper restore StaticFieldInGenericType
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.AffectsArrange));

        #region Orientation

        /// <summary>
        /// Ориентация контрола
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
        /// Свойство зависимости для <see cref="RangeTrack{T, TInterval}.Orientation"/>
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

        #endregion

        /// <summary>
        /// Свойство зависимости для <see cref="RangeTrack{T, TInterval}.StartValue"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty StartValueProperty = RangeBaseControl<T, TInterval>.StartValueProperty.AddOwner(
// ReSharper restore StaticFieldInGenericType
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));

        /// <summary>
        /// Свойство зависимости для <see cref="RangeTrack{T, TInterval}.EndValue"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty EndValueProperty = RangeBaseControl<T, TInterval>.EndValueProperty.AddOwner(
// ReSharper restore StaticFieldInGenericType
            typeof(RangeTrack<T, TInterval>),
            new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsArrange));

        #endregion

        /// <summary>
        /// Выполняет расположение контрола <see cref="RangeTrack{T, TInterval}" />.
        /// </summary>
        /// <param name="finalSize">Область, доступная для <see cref="RangeTrack{T, TInterval}" />.</param>
        /// <returns>Размер <see cref="Size"/>, который будет использоваться для содержимого <see cref="RangeTrack{T, TInterval}" />.</returns>
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
        /// Вычисляет размер, необходимый для <see cref="RangeTrack{T, TInterval}" /> и его компонентов.
        /// </summary>
        /// <param name="availableSize">Размер доступного пространства.</param>
        /// <returns>Размер <see cref="Size"/>, который требуется для <see cref="RangeTrack{T, TInterval}" />.</returns>
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
        /// Метод выполняет непосредственную обработку <see cref="FrameworkElement.OnApplyTemplate"/>
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
        /// Обработчик назначения шаблона данному контролу
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DoApplyTemplate();
        }

        /// <summary>
        /// Число дочерних элементов управления от 0 до 5.
        /// </summary>
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
        /// Возвращает дочерний элемент контрола по его индексу.
        /// </summary>
        /// <param name="index">Индекс дочернего элемента</param>
        /// <exception cref="ArgumentOutOfRangeException">Заданный индекс больше чем <see cref="RangeTrack{T, TInterval}.VisualChildrenCount" /> минус один (1).</exception>
        /// <returns>Возвращает дочерний визуальный элемент контрола по заданному индексу.
        /// Индекс должен быть между нулем (0) и значением <see cref="RangeTrack{T, TInterval}.VisualChildrenCount" /> минус один (1).</returns>
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
        /// Назначить связь между заданным элементом и свойством родительского шаблона
        /// </summary>
        /// <param name="element">Элемент</param>
        /// <param name="target">Целевое свойство</param>
        /// <param name="source">Свойство родительского шаблона</param>
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
        /// Назначить связь со свойством родительского шаблона
        /// </summary>
        /// <param name="target">Целевое свойство</param>
        /// <param name="source">Свойство родительского шаблона</param>
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

        private void ComputeLengths(Size arrangeSize, bool isVertical,
            out double decreaseButtonLength,
            out double startThumbLength, out double rangeThumbLength, out double endThumbLength,
            out double increaseButtonLength)
        {
            Contract.Requires(!arrangeSize.IsEmpty);

            double minimum = ValueToDouble(this.Minimum), maximum = ValueToDouble(this.Maximum);
            double interval = Math.Max(0.0, maximum - minimum); // длина доступного интервала значений
            double decreaseAreaInterval = Math.Min(interval, ValueToDouble(this.StartValue) - minimum); // длина интервала от минимума до текущего значения (левая область)
            double increaseAreaInterval = Math.Min(interval, maximum - ValueToDouble(this.EndValue)); // длина интервала от текущего значения до максимума (правая область)

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

        /// <summary>
        /// Вычисляет изменение <see cref="RangeTrack{T, TInterval}.StartValue" /> или <see cref="RangeTrack{T, TInterval}.EndValue" />
        /// когда сдвигается один из ползунков.
        /// </summary>
        /// <param name="horizontal">Горизонтальный сдвиг ползунка</param>
        /// <param name="vertical">Вертикальный сдвиг ползунка</param>
        /// <returns>Величина смещения, соответствющая расположению ползунка.</returns>
        public virtual double ValueFromDistance(double horizontal, double vertical)
        {
            return (this.Orientation == Orientation.Horizontal) ?
                horizontal * this.Density :
                -1.0 * vertical * this.Density;
        }

        #region Control Parts

        /// <summary>
        /// Кнопка сдвига интервала в меньшую сторону
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
        /// Кнопка сдвига интервала в большую сторону
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
        /// Ползунок начала интервала
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
        /// Ползунок интервала
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
        /// Ползунок конца интервала
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

        /// <summary>
        /// Минимально возможная величина значения
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
        /// Максимально возможная величина значения
        /// </summary>
        public T Maximum
        {
            get
            {
                var res = this.GetValue(MaximumProperty);
                Contract.Assume(res != null);
                return (T) res;
            }
            set { this.SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// Значение начала интервала
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
        /// Значение конца интервала
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
    /// Трэк, использующий числа с плавающей точкой в качестве значений и интервала
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
        /// Метод преобразования числа в значение
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Всегда <paramref name="value"/></returns>
        protected override double DoubleToValue(double value)
        {
            return value;
        }
        /// <summary>
        /// Метод преобразования значения в число
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Всегда <paramref name="value"/></returns>
        protected override double ValueToDouble(double value)
        {
            return value;
        }

// ReSharper disable RedundantOverridenMember
        /// <summary>
        /// Кнопка сдвига интервала в меньшую сторону
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
        /// Кнопка сдвига интервала в большую сторону
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
        /// Ползунок начала интервала
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
        /// Ползунок интервала
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
        /// Ползунок конца интервала
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
    }

    /// <summary>
    /// Трэк, использующий даты <see cref="DateTime"/> в качестве значений и <see cref="TimeSpan"/> в качестве интервала
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
        /// Метод преобразования числа в дату
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Дата, сформированная на основе <paramref name="value"/> тактов</returns>
        protected override DateTime DoubleToValue(double value)
        {
            return (value > 10.0) ? new DateTime((long)value) : DateTime.MinValue;
        }
        /// <summary>
        /// Метод преобразования даты в число
        /// </summary>
        /// <param name="value">Дата</param>
        /// <returns>Число тактов даты</returns>
        protected override double ValueToDouble(DateTime value)
        {
            return value.Ticks;
        }

// ReSharper disable RedundantOverridenMember
        /// <summary>
        /// Кнопка сдвига интервала в меньшую сторону
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
        /// Кнопка сдвига интервала в большую сторону
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
        /// Ползунок начала интервала
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
        /// Ползунок интервала
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
        /// Ползунок конца интервала
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
    }
}

