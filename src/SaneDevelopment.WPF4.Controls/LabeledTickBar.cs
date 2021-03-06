﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LabeledTickBar.cs" company="Sane Development">
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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Интерфейс для преобразования числа с плавающей точкой в строку.
    /// Объекты, реализующие этот интерфейс, используются для преобразования числовых значений делений контрола <see cref="TickBar"/>
    /// в их строковые представления для отображения пользователю.
    /// </summary>
    public interface IDoubleToStringConverter
    {
        /// <summary>
        /// Функция преобразует значение <paramref name="value"/> в строку
        /// </summary>
        /// <param name="value">Число для преобразования</param>
        /// <param name="parameter">Дополнительный произвольный параметер для преобразования</param>
        /// <returns>Строковое представление числа</returns>
        string Convert(double value, object parameter);
    }

    /// <summary>
    /// Элемент управления, который рисует набор делений с текстовыми подписями
    /// </summary>
    public class LabeledTickBar : TickBar
    {
        #region EliminateOverlapping Property

        /// <summary>
        /// Свойство зависимости для <see cref="LabeledTickBar.EliminateOverlapping"/>
        /// </summary>
        public static readonly DependencyProperty EliminateOverlappingProperty =
            DependencyProperty.Register(
                "EliminateOverlapping",
                typeof(bool),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Избегать ли перекрытия (наложения) текстовых подписей делений путем сокрытия непомещающихся
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool EliminateOverlapping
        {
            get
            {
                var res = GetValue(EliminateOverlappingProperty);
                Contract.Assume(res is bool);
                return (bool) res;
            }
            set { SetValue(EliminateOverlappingProperty, value); }
        }

        #endregion

        #region StartLabelSpace Property

        /// <summary>
        /// Свойство зависимости для <see cref="LabeledTickBar.StartLabelSpace"/>
        /// </summary>
        public static readonly DependencyProperty StartLabelSpaceProperty =
            DependencyProperty.Register(
                "StartLabelSpace",
                typeof(double),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidSpace);

        /// <summary>
        /// Доступное для вывода надписей пространство за пределами этого элемента управления,
        /// расположенное в начале (слева/внизу)
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double StartLabelSpace
        {
            get
            {
                var res = GetValue(StartLabelSpaceProperty);
                Contract.Assume(res is double);
                return (double) res;
            }
            set { SetValue(StartLabelSpaceProperty, value); }
        }

        #endregion

        #region EndLabelSpace Property

        /// <summary>
        /// Свойство зависимости для <see cref="LabeledTickBar.EndLabelSpace"/>
        /// </summary>
        public static readonly DependencyProperty EndLabelSpaceProperty =
            DependencyProperty.Register(
                "EndLabelSpace",
                typeof(double),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidSpace);

        /// <summary>
        /// Доступное для вывода надписей пространство за пределами этого элемента управления,
        /// расположенное в конце (справа/вверху)
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double EndLabelSpace
        {
            get
            {
                var res = GetValue(EndLabelSpaceProperty);
                Contract.Assume(res is double);
                return (double) res;
            }
            set { SetValue(EndLabelSpaceProperty, value); }
        }

        #endregion

        #region ValueConverter Property

        /// <summary>
        /// Свойство зависимости для <see cref="LabeledTickBar.ValueConverter"/>
        /// </summary>
        public static readonly DependencyProperty ValueConverterProperty =
            DependencyProperty.Register(
                "ValueConverter",
                typeof(IDoubleToStringConverter),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Конвертер числовых значений делений в их строковые представления
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public IDoubleToStringConverter ValueConverter
        {
            get { return (IDoubleToStringConverter)GetValue(ValueConverterProperty); }
            set { SetValue(ValueConverterProperty, value); }
        }

        #endregion

        #region ValueConverterParameter Property

        /// <summary>
        /// Свойство зависимости для <see cref="LabeledTickBar.ValueConverterParameter"/>
        /// </summary>
        public static readonly DependencyProperty ValueConverterParameterProperty =
            DependencyProperty.Register(
                "ValueConverterParameter",
                typeof(object),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Параметр конвертера числовых значений делений в их строковые представления
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public object ValueConverterParameter
        {
            get { return GetValue(ValueConverterParameterProperty); }
            set { SetValue(ValueConverterParameterProperty, value); }
        }

        #endregion

        /// <summary>
        /// Рисует деления с подписями
        /// </summary>
        /// <param name="dc">Контекст рисования</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "Based on Microsoft realisation")]
        protected override void OnRender(DrawingContext dc)
        {
            bool eliminateOverlapping = EliminateOverlapping;
            double controlActualWidth = ActualWidth, controlActualHeight = ActualHeight;
            Contract.Assert(ActualWidth >= 0);
            var size = new Size(controlActualWidth, controlActualHeight);
            double min = Minimum, max = Maximum;
            double range = max - min;
            double logicalToPhysical = 1.0;
            double textPointShiftDirection = 0.0d;
            var startPoint = new Point(0d, 0d);
            var endPoint = new Point(0d, 0d);

            // Take Thumb size in to account
            double halfReservedSpace = ReservedSpace * 0.5;

            switch (Placement)
            {
                case TickBarPlacement.Top:
                    if (DoubleUtil.GreaterThanOrClose(ReservedSpace, size.Width))
                    {
                        return;
                    }
                    size.Width -= ReservedSpace;
                    textPointShiftDirection = -1.0d;
                    startPoint = new Point(halfReservedSpace, size.Height);
                    endPoint = new Point(halfReservedSpace + size.Width, size.Height);
                    logicalToPhysical = size.Width / range;
                    break;

                case TickBarPlacement.Bottom:
                    if (DoubleUtil.GreaterThanOrClose(ReservedSpace, size.Width))
                    {
                        return;
                    }
                    size.Width -= ReservedSpace;
                    textPointShiftDirection = 0.0d;
                    startPoint = new Point(halfReservedSpace, 0d);
                    endPoint = new Point(halfReservedSpace + size.Width, 0d);
                    logicalToPhysical = size.Width / range;
                    break;

                case TickBarPlacement.Left:
                    if (DoubleUtil.GreaterThanOrClose(ReservedSpace, size.Height))
                    {
                        return;
                    }
                    size.Height -= ReservedSpace;
                    textPointShiftDirection = 1.0d;
                    startPoint = new Point(size.Width, size.Height + halfReservedSpace);
                    endPoint = new Point(size.Width, halfReservedSpace);
                    logicalToPhysical = size.Height / range * -1;
                    break;

                case TickBarPlacement.Right:
                    if (DoubleUtil.GreaterThanOrClose(ReservedSpace, size.Height))
                    {
                        return;
                    }
                    size.Height -= ReservedSpace;
                    textPointShiftDirection = 0.0d;
                    startPoint = new Point(0d, size.Height + halfReservedSpace);
                    endPoint = new Point(0d, halfReservedSpace);
                    logicalToPhysical = size.Height / range * -1;
                    break;
            }

            // Invert direciton of the ticks
            bool isDirectionReversed = IsDirectionReversed;
            if (isDirectionReversed)
            {
                logicalToPhysical *= -1;

                // swap startPoint & endPoint
                Point pt = startPoint;
                startPoint = endPoint;
                endPoint = pt;
            }

            Brush textBrush = Fill;
            var pen = new Pen(textBrush, 1.0d);
            DoubleCollection ticks = Ticks;
            const double pointRadius = 1.0d;
            IDoubleToStringConverter valueConverter = ValueConverter;
            object valueConverterParameter = ValueConverterParameter;
            var textTypeface = new Typeface("Verdana");

            double startSpace = StartLabelSpace, endSpace = EndLabelSpace;
            Contract.Assume(startSpace >= 0.0);
            Contract.Assume(endSpace >= 0.0);

            // Is it Vertical?
            if ((Placement == TickBarPlacement.Left) || (Placement == TickBarPlacement.Right))
            {
                double prevTextEdge = isDirectionReversed ? (0.0 - startSpace) : (controlActualHeight + startSpace);
                double maxTextEdge = isDirectionReversed ? (controlActualHeight + endSpace) : (0.0 - endSpace);

                // Reduce tick interval if it is more than would be visible on the screen
                double interval = TickFrequency;
                if (interval > 0.0)
                {
                    double minInterval = range / size.Height;
                    if (interval < minInterval)
                    {
                        interval = minInterval;
                    }
                }

                Action<double, double> drawer = (tick, y) =>
                {
                    var formattedText = GetFormattedText(tick, textBrush, textTypeface, valueConverter, valueConverterParameter);
                    var textPoint = new Point(startPoint.X - formattedText.Width / 2,
                        y - (textPointShiftDirection * formattedText.Height));
                    double newTextEdge = y + (isDirectionReversed ? 1 : -1) * formattedText.Width / 2;

                    bool drawThisText = true;
                    if (eliminateOverlapping)
                    {
                        if (isDirectionReversed)
                        {
                            drawThisText = DoubleUtil.GreaterThanOrClose(y - formattedText.Width / 2, prevTextEdge) &&
                                DoubleUtil.LessThanOrClose(y + formattedText.Width / 2, maxTextEdge);
                        }
                        else
                        {
                            drawThisText = DoubleUtil.LessThanOrClose(y + formattedText.Width / 2, prevTextEdge) &&
                                DoubleUtil.GreaterThanOrClose(y - formattedText.Width / 2, maxTextEdge);
                        }
                    }

                    if (drawThisText)
                    {
                        var rotateTransform = new RotateTransform(270,
                            textPoint.X + formattedText.Width / 2,
                            textPoint.Y + textPointShiftDirection * formattedText.Height);
                        dc.DrawEllipse(textBrush, pen, new Point(startPoint.X, y), pointRadius, pointRadius);
                        dc.PushTransform(rotateTransform);
                        dc.DrawText(formattedText, textPoint);
                        dc.Pop();
                        prevTextEdge = newTextEdge;
                    }
                };
                // Draw Min tick
                drawer(min, startPoint.Y);

                // Draw ticks using specified Ticks collection
                if ((ticks != null) && (ticks.Count > 0))
                {
                    foreach (double tick in ticks)
                    {
                        if (DoubleUtil.LessThanOrClose(tick, min) || DoubleUtil.GreaterThanOrClose(tick, max))
                        {
                            continue;
                        }

                        double adjustedTick = tick - min;
                        double y = adjustedTick * logicalToPhysical + startPoint.Y;

                        drawer(tick, y);
                    }
                }
                // Draw ticks using specified TickFrequency
                else if (interval > 0.0)
                {
                    for (double i = interval; i < range; i += interval)
                    {
                        double y = i * logicalToPhysical + startPoint.Y;

                        drawer(i + min, y);
                    }
                }

                // Draw Max tick
                drawer(max, endPoint.Y);
            }
            else  // Placement == Top || Placement == Bottom
            {
                double prevTextEdge = isDirectionReversed ? (controlActualWidth + startSpace) : (0.0 - startSpace);
                double maxTextEdge = isDirectionReversed ? (0.0 - endSpace) : (controlActualWidth + endSpace);

                // Reduce tick interval if it is more than would be visible on the screen
                double interval = TickFrequency;
                if (interval > 0.0)
                {
                    double minInterval = range / size.Width;
                    if (interval < minInterval)
                    {
                        interval = minInterval;
                    }
                }

                Action<double, double> drawer = (tick, x) =>
                {
                    var formattedText = GetFormattedText(tick, textBrush, textTypeface, valueConverter, valueConverterParameter);
                    var textPoint = new Point(x - (formattedText.Width / 2),
                        startPoint.Y + textPointShiftDirection * formattedText.Height);
                    double newTextEdge = textPoint.X + (isDirectionReversed ? 0.0 : formattedText.Width);

                    bool drawThisText = true;
                    if (eliminateOverlapping)
                    {
                        if (isDirectionReversed)
                        {
                            drawThisText = DoubleUtil.LessThanOrClose(textPoint.X + formattedText.Width, prevTextEdge) &&
                                DoubleUtil.GreaterThanOrClose(textPoint.X, maxTextEdge);
                        }
                        else
                        {
                            drawThisText = DoubleUtil.GreaterThanOrClose(textPoint.X, prevTextEdge) &&
                                DoubleUtil.LessThanOrClose(textPoint.X + formattedText.Width, maxTextEdge);
                        }
                    }

                    if (drawThisText)
                    {
                        dc.DrawEllipse(textBrush, pen, new Point(x, startPoint.Y), pointRadius, pointRadius);
                        dc.DrawText(formattedText, textPoint);
                        prevTextEdge = newTextEdge;
                    }
                };
                // Draw Min tick
                drawer(min, startPoint.X);

                // Draw ticks using specified Ticks collection
                if ((ticks != null) && (ticks.Count > 0))
                {
                    foreach (double tick in ticks)
                    {
                        if (DoubleUtil.LessThanOrClose(tick, min) || DoubleUtil.GreaterThanOrClose(tick, max))
                        {
                            continue;
                        }
                        double adjustedTick = tick - min;
                        double x = adjustedTick * logicalToPhysical + startPoint.X;

                        drawer(tick, x);
                    }
                }
                // Draw ticks using specified TickFrequency
                else if (interval > 0.0)
                {
                    for (double i = interval; i < range; i += interval)
                    {
                        double x = i * logicalToPhysical + startPoint.X;

                        drawer(i + min, x);
                    }
                }

                // Draw Max tick
                drawer(max, endPoint.X);
            }
        }

        private static FormattedText GetFormattedText(
            double value,
            Brush textBrush,
            Typeface textTypeface,
            IDoubleToStringConverter valueConverter,
            object valueConverterParameter)
        {
            string text = (valueConverter == null) ?
                value.ToString(CultureInfo.CurrentCulture.NumberFormat) :
                valueConverter.Convert(value, valueConverterParameter);
            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
                textTypeface,
                8,
                textBrush);
            return formattedText;
        }

        internal static bool IsValidSpace(object value)
        {
            return DependencyPropertyUtil.IsValidChange(typeof(double), value);
        }

        //[ContractInvariantMethod]
        //private void ObjectInvariant()
        //{
        //}
    }
}
