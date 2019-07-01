// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LabeledTickBar.cs" company="Sane Development">
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
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using SaneDevelopment.WPF.Controls.Properties;

namespace SaneDevelopment.WPF.Controls
{
    /// <summary>
    /// Interface for converting <c>double</c> value to <c>string</c>.
    /// Classes which implements this interface uses for converting <c>double</c> tick values in <see cref="TickBar"/> control
    /// to <c>string</c> representations for showing in UI.
    /// </summary>
    public interface IDoubleToStringConverter
    {
        /// <summary>
        /// Convert <paramref name="value"/> to <c>string</c>
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="parameter">Additional parameter for conversion</param>
        /// <returns>String representation of <paramref name="value"/></returns>
        string Convert(double value, object parameter);
    }

    /// <summary>
    /// Represents a control that draws a set of tick marks with text labels for any slider control.
    /// </summary>
    public class LabeledTickBar : TickBar
    {
        private static readonly Lazy<Typeface> s_TextTypeface = new Lazy<Typeface>(() => new Typeface("Verdana"));

        #region EliminateOverlapping Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.EliminateOverlapping"/>
        /// </summary>
        public static readonly DependencyProperty EliminateOverlappingProperty =
            DependencyProperty.Register(
                "EliminateOverlapping",
                typeof(bool),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Whether eliminate overlapping of text labels by hiding overlapped.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool EliminateOverlapping
        {
            get
            {
                var res = GetValue(EliminateOverlappingProperty);
                Debug.Assert(res is bool);
                return (bool) res;
            }
            set { SetValue(EliminateOverlappingProperty, value); }
        }

        #endregion

        #region StartLabelSpace Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.StartLabelSpace"/>
        /// </summary>
        public static readonly DependencyProperty StartLabelSpaceProperty =
            DependencyProperty.Register(
                "StartLabelSpace",
                typeof(double),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidSpace);

        /// <summary>
        /// Space (in pixels) avaliable for drawing text labels beyond this control area at the "start" side (left/bottom)
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double StartLabelSpace
        {
            get
            {
                var res = GetValue(StartLabelSpaceProperty);
                Debug.Assert(res is double);
                return (double) res;
            }
            set { SetValue(StartLabelSpaceProperty, value); }
        }

        #endregion

        #region EndLabelSpace Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.EndLabelSpace"/>
        /// </summary>
        public static readonly DependencyProperty EndLabelSpaceProperty =
            DependencyProperty.Register(
                "EndLabelSpace",
                typeof(double),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidSpace);

        /// <summary>
        /// Space (in pixels) avaliable for drawing text labels beyond this control area at the "end" side (right/top)
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public double EndLabelSpace
        {
            get
            {
                var res = GetValue(EndLabelSpaceProperty);
                Debug.Assert(res is double);
                return (double) res;
            }
            set { SetValue(EndLabelSpaceProperty, value); }
        }

        #endregion

        #region ValueNumericFormat Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.ValueNumericFormat"/>
        /// </summary>
        public static readonly DependencyProperty ValueNumericFormatProperty =
            DependencyProperty.Register(
                "ValueNumericFormat",
                typeof(string),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Format string for conversion numeric labels to text.
        /// Empty string interprets as <c>null</c>.
        /// Uses only when <see cref="ValueConverter"/> is <c>null</c>.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public string ValueNumericFormat
        {
            get { return (string)GetValue(ValueNumericFormatProperty); }
            set { SetValue(ValueNumericFormatProperty, value); }
        }

        #endregion

        #region ValueConverter Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.ValueConverter"/>
        /// </summary>
        public static readonly DependencyProperty ValueConverterProperty =
            DependencyProperty.Register(
                "ValueConverter",
                typeof(IDoubleToStringConverter),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Converter of <c>double</c> ticks values to <c>string</c>
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
        /// Dependency property for <see cref="LabeledTickBar.ValueConverterParameter"/>
        /// </summary>
        public static readonly DependencyProperty ValueConverterParameterProperty =
            DependencyProperty.Register(
                "ValueConverterParameter",
                typeof(object),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Parameter for converter of <c>double</c> ticks values to <c>string</c>
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public object ValueConverterParameter
        {
            get { return GetValue(ValueConverterParameterProperty); }
            set { SetValue(ValueConverterParameterProperty, value); }
        }

        #endregion

        /// <summary>
        /// Draws the tick marks for some slider control.
        /// </summary>
        /// <param name="dc">The <see cref="System.Windows.Media.DrawingContext"/> that is used to draw the ticks.</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "Based on Microsoft realisation")]
        protected override void OnRender(DrawingContext dc)
        {
            bool eliminateOverlapping = EliminateOverlapping;
            double controlActualWidth = ActualWidth, controlActualHeight = ActualHeight;
            Debug.Assert(ActualWidth >= 0);

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
            string valueFormat = ValueNumericFormat;
            IDoubleToStringConverter valueConverter = ValueConverter;
            object valueConverterParameter = ValueConverterParameter;
            var textTypeface = s_TextTypeface.Value;
            var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            double startSpace = StartLabelSpace, endSpace = EndLabelSpace;
            Debug.Assert(startSpace >= 0.0);
            Debug.Assert(endSpace >= 0.0);

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
                        var formattedText = GetFormattedText(tick, textBrush, textTypeface, valueFormat, valueConverter,
                                                             valueConverterParameter, pixelsPerDip);
                        var textPoint = new Point(startPoint.X - formattedText.Width/2,
                                                  y - (textPointShiftDirection*formattedText.Height));
                        double newTextEdge = y + (isDirectionReversed ? 1 : -1)*formattedText.Width/2;

                        bool drawThisText = true;
                        if (eliminateOverlapping)
                        {
                            if (isDirectionReversed)
                            {
                                drawThisText = DoubleUtil.GreaterThanOrClose(y - formattedText.Width/2, prevTextEdge) &&
                                               DoubleUtil.LessThanOrClose(y + formattedText.Width/2, maxTextEdge);
                            }
                            else
                            {
                                drawThisText = DoubleUtil.LessThanOrClose(y + formattedText.Width/2, prevTextEdge) &&
                                               DoubleUtil.GreaterThanOrClose(y - formattedText.Width/2, maxTextEdge);
                            }
                        }

                        if (drawThisText)
                        {
                            var rotateTransform = new RotateTransform(270,
                                                                      textPoint.X + formattedText.Width/2,
                                                                      textPoint.Y +
                                                                      textPointShiftDirection*formattedText.Height);
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
                        var formattedText = GetFormattedText(tick, textBrush, textTypeface, valueFormat, valueConverter,
                                                             valueConverterParameter, pixelsPerDip);
                        var textPoint = new Point(x - (formattedText.Width/2),
                                                  startPoint.Y + textPointShiftDirection*formattedText.Height);
                        double newTextEdge = textPoint.X + (isDirectionReversed ? 0.0 : formattedText.Width);

                        bool drawThisText = true;
                        if (eliminateOverlapping)
                        {
                            if (isDirectionReversed)
                            {
                                drawThisText =
                                    DoubleUtil.LessThanOrClose(textPoint.X + formattedText.Width, prevTextEdge) &&
                                    DoubleUtil.GreaterThanOrClose(textPoint.X, maxTextEdge);
                            }
                            else
                            {
                                drawThisText =
                                    DoubleUtil.GreaterThanOrClose(textPoint.X, prevTextEdge) &&
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
            string valueFormat,
            IDoubleToStringConverter valueConverter,
            object valueConverterParameter,
            double pixelsPerDip)
        {
            string text;
            try
            {
                text = (valueConverter == null)
                                   ? (string.IsNullOrEmpty(valueFormat)
                                          ? value.ToString(CultureInfo.CurrentCulture.NumberFormat)
                                          : value.ToString(valueFormat, CultureInfo.CurrentCulture.NumberFormat))
                                   : valueConverter.Convert(value, valueConverterParameter);
            }
            catch (FormatException)
            {
                text = LocalizationResource.BadLabeledTickFormat;
            }

            var formattedText = new FormattedText(
                text,
                CultureInfo.CurrentCulture,
                CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight,
                textTypeface,
                8,
                textBrush,
                pixelsPerDip);

            return formattedText;
        }

        internal static bool IsValidSpace(object value)
        {
            return DependencyPropertyUtil.IsValidChange(typeof(double), value);
        }
    }
}
