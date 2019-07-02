// -----------------------------------------------------------------------
// <copyright file="LabeledTickBar.cs" company="Sane Development">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using SaneDevelopment.WPF.Controls.Properties;

#pragma warning disable CA1501 // Avoid excessive inheritance

    /// <summary>
    /// Represents a control that draws a set of tick marks with text labels for any slider control.
    /// </summary>
    public class LabeledTickBar : TickBar
    {
        /// <summary>
        /// Label text font size.
        /// </summary>
        public const double TextFontSize = 8;

#pragma warning disable SA1308 // Variable names should not be prefixed
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter

        private static readonly Lazy<Typeface> s_TextTypeface = new Lazy<Typeface>(() => new Typeface("Verdana"));

#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning restore SA1308 // Variable names should not be prefixed

        /// <summary>
        /// Whether <paramref name="value"/> has valid space value.
        /// </summary>
        /// <param name="value">Value to validate.</param>
        /// <returns><c>true</c> if <paramref name="value"/> has valid space value;
        /// otherwise <c>false</c>.</returns>
        internal static bool IsValidSpace(object value)
        {
            return DependencyPropertyUtil.IsValidChange(typeof(double), value);
        }

#pragma warning disable SA1201 // Elements should appear in the correct order

        #region EliminateOverlapping Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.EliminateOverlapping"/>.
        /// </summary>
        public static readonly DependencyProperty EliminateOverlappingProperty =
            DependencyProperty.Register(
                nameof(EliminateOverlapping),
                typeof(bool),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets a value indicating whether eliminate overlapping of text labels by hiding overlapped.
        /// </summary>
        /// <value>A value indicating whether eliminate overlapping of text labels by hiding overlapped.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public bool EliminateOverlapping
        {
            get
            {
                var res = this.GetValue(EliminateOverlappingProperty);
                Debug.Assert(res is bool, "res is bool");
                return (bool)res;
            }

            set
            {
                this.SetValue(EliminateOverlappingProperty, value);
            }
        }

        #endregion

        #region StartLabelSpace Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.StartLabelSpace"/>.
        /// </summary>
        public static readonly DependencyProperty StartLabelSpaceProperty =
            DependencyProperty.Register(
                nameof(StartLabelSpace),
                typeof(double),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidSpace);

        /// <summary>
        /// Gets or sets space (in pixels) avaliable for drawing text labels beyond this control area at the "start" side (left/bottom).
        /// </summary>
        /// <value>Space (in pixels) avaliable for drawing text labels beyond this control area at the "start" side (left/bottom).</value>
        [Bindable(true)]
        [Category("Behavior")]
        public double StartLabelSpace
        {
            get
            {
                var res = this.GetValue(StartLabelSpaceProperty);
                Debug.Assert(res is double, "res is double");
                return (double)res;
            }

            set
            {
                this.SetValue(StartLabelSpaceProperty, value);
            }
        }

        #endregion

        #region EndLabelSpace Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.EndLabelSpace"/>.
        /// </summary>
        public static readonly DependencyProperty EndLabelSpaceProperty =
            DependencyProperty.Register(
                nameof(EndLabelSpace),
                typeof(double),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender),
                IsValidSpace);

        /// <summary>
        /// Gets or sets space (in pixels) avaliable for drawing text labels beyond this control area at the "end" side (right/top).
        /// </summary>
        /// <value>Space (in pixels) avaliable for drawing text labels beyond this control area at the "end" side (right/top).</value>
        [Bindable(true)]
        [Category("Behavior")]
        public double EndLabelSpace
        {
            get
            {
                var res = this.GetValue(EndLabelSpaceProperty);
                Debug.Assert(res is double, "res is double");
                return (double)res;
            }

            set
            {
                this.SetValue(EndLabelSpaceProperty, value);
            }
        }

        #endregion

        #region ValueNumericFormat Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.ValueNumericFormat"/>.
        /// </summary>
        public static readonly DependencyProperty ValueNumericFormatProperty =
            DependencyProperty.Register(
                nameof(ValueNumericFormat),
                typeof(string),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets format string for conversion numeric labels to text.
        /// Empty string interprets as <c>null</c>.
        /// Uses only when <see cref="ValueConverter"/> is <c>null</c>.
        /// </summary>
        /// <value>Format string for conversion numeric labels to text.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public string ValueNumericFormat
        {
            get { return (string)this.GetValue(ValueNumericFormatProperty); }
            set { this.SetValue(ValueNumericFormatProperty, value); }
        }

        #endregion

        #region ValueConverter Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.ValueConverter"/>.
        /// </summary>
        public static readonly DependencyProperty ValueConverterProperty =
            DependencyProperty.Register(
                nameof(ValueConverter),
                typeof(IDoubleToStringConverter),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets converter of <c>double</c> ticks values to <c>string</c>.
        /// </summary>
        /// <value>Converter of <c>double</c> ticks values to <c>string</c>.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public IDoubleToStringConverter ValueConverter
        {
            get { return (IDoubleToStringConverter)this.GetValue(ValueConverterProperty); }
            set { this.SetValue(ValueConverterProperty, value); }
        }

        #endregion

        #region ValueConverterParameter Property

        /// <summary>
        /// Dependency property for <see cref="LabeledTickBar.ValueConverterParameter"/>.
        /// </summary>
        public static readonly DependencyProperty ValueConverterParameterProperty =
            DependencyProperty.Register(
                nameof(ValueConverterParameter),
                typeof(object),
                typeof(LabeledTickBar),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Gets or sets parameter for converter of <c>double</c> ticks values to <c>string</c>.
        /// </summary>
        /// <value>Parameter for converter of <c>double</c> ticks values to <c>string</c>.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public object ValueConverterParameter
        {
            get { return this.GetValue(ValueConverterParameterProperty); }
            set { this.SetValue(ValueConverterParameterProperty, value); }
        }

        #endregion

#pragma warning restore SA1201 // Elements should appear in the correct order

        /// <summary>
        /// Draws the tick marks for some slider control.
        /// </summary>
        /// <param name="dc">The <see cref="DrawingContext"/> that is used to draw the ticks.</param>
        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1502:AvoidExcessiveComplexity",
            Justification = "Based on Microsoft realisation")]
        protected override void OnRender(DrawingContext dc)
        {
            if (dc == null)
            {
                throw new ArgumentNullException(nameof(dc));
            }

            bool eliminateOverlapping = this.EliminateOverlapping;
            double controlActualWidth = this.ActualWidth, controlActualHeight = this.ActualHeight;
            Debug.Assert(this.ActualWidth >= 0, "this.ActualWidth >= 0");

            var size = new Size(controlActualWidth, controlActualHeight);
            double min = this.Minimum, max = this.Maximum;
            double range = max - min;
            double logicalToPhysical = 1.0;
            double textPointShiftDirection = 0.0d;
            var startPoint = new Point(0d, 0d);
            var endPoint = new Point(0d, 0d);

            // Take Thumb size in to account
            double halfReservedSpace = this.ReservedSpace * 0.5;

            switch (this.Placement)
            {
                case TickBarPlacement.Top:
                    if (DoubleUtil.GreaterThanOrClose(this.ReservedSpace, size.Width))
                    {
                        return;
                    }

                    size.Width -= this.ReservedSpace;
                    textPointShiftDirection = -1.0d;
                    startPoint = new Point(halfReservedSpace, size.Height);
                    endPoint = new Point(halfReservedSpace + size.Width, size.Height);
                    logicalToPhysical = size.Width / range;
                    break;

                case TickBarPlacement.Bottom:
                    if (DoubleUtil.GreaterThanOrClose(this.ReservedSpace, size.Width))
                    {
                        return;
                    }

                    size.Width -= this.ReservedSpace;
                    textPointShiftDirection = 0.0d;
                    startPoint = new Point(halfReservedSpace, 0d);
                    endPoint = new Point(halfReservedSpace + size.Width, 0d);
                    logicalToPhysical = size.Width / range;
                    break;

                case TickBarPlacement.Left:
                    if (DoubleUtil.GreaterThanOrClose(this.ReservedSpace, size.Height))
                    {
                        return;
                    }

                    size.Height -= this.ReservedSpace;
                    textPointShiftDirection = 1.0d;
                    startPoint = new Point(size.Width, size.Height + halfReservedSpace);
                    endPoint = new Point(size.Width, halfReservedSpace);
                    logicalToPhysical = size.Height / range * -1;
                    break;

                case TickBarPlacement.Right:
                    if (DoubleUtil.GreaterThanOrClose(this.ReservedSpace, size.Height))
                    {
                        return;
                    }

                    size.Height -= this.ReservedSpace;
                    textPointShiftDirection = 0.0d;
                    startPoint = new Point(0d, size.Height + halfReservedSpace);
                    endPoint = new Point(0d, halfReservedSpace);
                    logicalToPhysical = size.Height / range * -1;
                    break;
            }

            // Invert direciton of the ticks
            bool isDirectionReversed = this.IsDirectionReversed;
            if (isDirectionReversed)
            {
                logicalToPhysical *= -1;

                // swap startPoint & endPoint
                Point pt = startPoint;
                startPoint = endPoint;
                endPoint = pt;
            }

            Brush textBrush = this.Fill;
            var pen = new Pen(textBrush, 1.0d);
            DoubleCollection ticks = this.Ticks;
            const double pointRadius = 1.0d;
            string valueFormat = this.ValueNumericFormat;
            IDoubleToStringConverter valueConverter = this.ValueConverter;
            object valueConverterParameter = this.ValueConverterParameter;
            var textTypeface = s_TextTypeface.Value;
            var pixelsPerDip = VisualTreeHelper.GetDpi(this).PixelsPerDip;

            double startSpace = this.StartLabelSpace, endSpace = this.EndLabelSpace;
            Debug.Assert(startSpace >= 0.0, "startSpace >= 0.0");
            Debug.Assert(endSpace >= 0.0, "endSpace >= 0.0");

            // Is it Vertical?
            if ((this.Placement == TickBarPlacement.Left) || (this.Placement == TickBarPlacement.Right))
            {
                double prevTextEdge = isDirectionReversed ? (0.0 - startSpace) : (controlActualHeight + startSpace);
                double maxTextEdge = isDirectionReversed ? (controlActualHeight + endSpace) : (0.0 - endSpace);

                // Reduce tick interval if it is more than would be visible on the screen
                double interval = this.TickFrequency;
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
                        var formattedText = GetFormattedText(
                            tick,
                            textBrush,
                            textTypeface,
                            valueFormat,
                            valueConverter,
                            valueConverterParameter,
                            pixelsPerDip);

                        var textPoint = new Point(
                            startPoint.X - (formattedText.Width / 2),
                            y - (textPointShiftDirection * formattedText.Height));

                        double newTextEdge = y + ((isDirectionReversed ? 1 : -1) * formattedText.Width / 2);

                        bool drawThisText = true;
                        if (eliminateOverlapping)
                        {
                            if (isDirectionReversed)
                            {
                                drawThisText = DoubleUtil.GreaterThanOrClose(y - (formattedText.Width / 2), prevTextEdge) &&
                                               DoubleUtil.LessThanOrClose(y + (formattedText.Width / 2), maxTextEdge);
                            }
                            else
                            {
                                drawThisText = DoubleUtil.LessThanOrClose(y + (formattedText.Width / 2), prevTextEdge) &&
                                               DoubleUtil.GreaterThanOrClose(y - (formattedText.Width / 2), maxTextEdge);
                            }
                        }

                        if (drawThisText)
                        {
                            var rotateTransform = new RotateTransform(
                                270,
                                textPoint.X + (formattedText.Width / 2),
                                textPoint.Y + (textPointShiftDirection * formattedText.Height));
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
                        double y = (adjustedTick * logicalToPhysical) + startPoint.Y;

                        drawer(tick, y);
                    }
                }

                // Draw ticks using specified TickFrequency
                else if (interval > 0.0)
                {
                    for (double i = interval; i < range; i += interval)
                    {
                        double y = (i * logicalToPhysical) + startPoint.Y;

                        drawer(i + min, y);
                    }
                }

                // Draw Max tick
                drawer(max, endPoint.Y);
            }
            else
            {
                // Placement == Top || Placement == Bottom

                double prevTextEdge = isDirectionReversed ? (controlActualWidth + startSpace) : (0.0 - startSpace);
                double maxTextEdge = isDirectionReversed ? (0.0 - endSpace) : (controlActualWidth + endSpace);

                // Reduce tick interval if it is more than would be visible on the screen
                double interval = this.TickFrequency;
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
                        var formattedText = GetFormattedText(
                            tick,
                            textBrush,
                            textTypeface,
                            valueFormat,
                            valueConverter,
                            valueConverterParameter,
                            pixelsPerDip);
                        var textPoint = new Point(
                            x - (formattedText.Width / 2),
                            startPoint.Y + (textPointShiftDirection * formattedText.Height));
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
                        double x = (adjustedTick * logicalToPhysical) + startPoint.X;

                        drawer(tick, x);
                    }
                }

                // Draw ticks using specified TickFrequency
                else if (interval > 0.0)
                {
                    for (double i = interval; i < range; i += interval)
                    {
                        double x = (i * logicalToPhysical) + startPoint.X;

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
                TextFontSize,
                textBrush,
                pixelsPerDip);

            return formattedText;
        }
    }

#pragma warning restore CA1501 // Avoid excessive inheritance
}
