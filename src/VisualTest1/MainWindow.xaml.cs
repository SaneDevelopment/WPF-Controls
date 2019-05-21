// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Sane Development">
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
using System.Globalization;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using SaneDevelopment.WPF4.Controls;

namespace VisualTest1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    ///
    /// Investigation test for:
    /// https://connect.microsoft.com/VisualStudio/feedback/details/483010/wpf-gridsplitter-randomly-jumps-when-resizing
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddLogText(TextBox textBox, string text)
        {
            textBox.Text += text + Environment.NewLine;
            textBox.ScrollToEnd();
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            Debug.Assert(btn != null);

            var txt = btn.Tag as TextBox;
            Debug.Assert(txt != null);

            txt.Clear();
            //txt.Text = string.Empty;
        }

        #region rs2

        private void rs2_StartValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized || !this.ChkHandleStartValueChanged3.IsChecked.GetValueOrDefault(false))
                return;

            AddLogText(this.TxtLog4,
                       "StartValueChanged changed: " +
                       e.OldValue.ToString(CultureInfo.InvariantCulture) +
                       "->" +
                       e.NewValue.ToString(CultureInfo.InvariantCulture));
        }

        private void rs2_EndValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized || !this.ChkHandleEndValueChanged3.IsChecked.GetValueOrDefault(false))
                return;

            AddLogText(this.TxtLog4,
                       "EndValueChanged changed: " +
                       e.OldValue.ToString(CultureInfo.InvariantCulture) +
                       "->" +
                       e.NewValue.ToString(CultureInfo.InvariantCulture));
        }

        private void rs2_RangeValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized || !this.ChkHandleRangeValueChanged3.IsChecked.GetValueOrDefault(false))
                return;

            AddLogText(this.TxtLog4,
                       "RangeValueChanged changed: " +
                       e.OldValue.ToString(CultureInfo.InvariantCulture) +
                       "->" +
                       e.NewValue.ToString(CultureInfo.InvariantCulture));
        }

        private void rs2_ValueChanged(object sender, RangeDragCompletedEventArgs<double> e)
        {
            if (!IsInitialized || !this.ChkHandleValueChanged3.IsChecked.GetValueOrDefault(false))
                return;

            AddLogText(this.TxtLog4,
                "ValueChanged: [" + e.OldStartValue.ToString("F4") + ".." + e.OldEndValue.ToString("F4") + "] -> [" +
                e.NewStartValue.ToString("F4") + ".." + e.NewEndValue.ToString("F4") + "]");
        }

        private void rs2_IsRangeDraggingChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!IsInitialized || !this.ChkHandleIsRangeDraggingChanged3.IsChecked.GetValueOrDefault(false))
                return;

            AddLogText(this.TxtLog4, "IsRangeDraggingChanged changed: " + e.OldValue.ToString() + "->" + e.NewValue.ToString());
        }

        private void rs2_RangeDragCompleted(object sender, RangeDragCompletedEventArgs<double> e)
        {
            if (!IsInitialized || !this.ChkHandleRangeDragCompleted3.IsChecked.GetValueOrDefault(false))
                return;

            AddLogText(this.TxtLog4, "RangeDragCompleted: [" + e.OldStartValue.ToString("F4") + ".." + e.OldEndValue.ToString("F4") + "] -> [" +
                e.NewStartValue.ToString("F4") + ".." + e.NewEndValue.ToString("F4") + "]");
        }

        #endregion

        #region rs3

        private void rs3_StartValueChanged(object sender, RoutedPropertyChangedEventArgs<DateTime> e)
        {
            if (!IsInitialized || !this.ChkHandleStartValueChanged4.IsChecked.GetValueOrDefault(false))
                return;

            AddLogText(this.TxtLog5,
                "StartValueChanged changed: " +
                e.OldValue.ToString("G", CultureInfo.InvariantCulture) +
                "->" +
                e.NewValue.ToString("G", CultureInfo.InvariantCulture));
        }

        private void rs3_EndValueChanged(object sender, RoutedPropertyChangedEventArgs<DateTime> e)
        {
            if (!IsInitialized || !this.ChkHandleEndValueChanged4.IsChecked.GetValueOrDefault(false))
                return;

            AddLogText(this.TxtLog5,
                "EndValueChanged changed: " +
                e.OldValue.ToString("G", CultureInfo.InvariantCulture) +
                "->" +
                e.NewValue.ToString("G", CultureInfo.InvariantCulture));
        }

        private void rs3_RangeValueChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            if (!IsInitialized || !this.ChkHandleRangeValueChanged4.IsChecked.GetValueOrDefault(false))
                return;

            AddLogText(this.TxtLog5,
                "RangeValueChanged changed: " +
                e.OldValue.ToString("G", CultureInfo.InvariantCulture) +
                "->" +
                e.NewValue.ToString("G", CultureInfo.InvariantCulture));
        }

        private void rs3_ValueChanged(object sender, RangeDragCompletedEventArgs<DateTime> e)
        {
            if (!IsInitialized || !this.ChkHandleValueChanged4.IsChecked.GetValueOrDefault(false))
                return;

            const string frmt = "d-M-yyyy H:m:s";
            AddLogText(this.TxtLog5, "ValueChanged: [" + e.OldStartValue.ToString(frmt) + ".." + e.OldEndValue.ToString(frmt) + "] -> [" +
                e.NewStartValue.ToString(frmt) + ".." + e.NewEndValue.ToString(frmt) + "]");
        }

        private void rs3_IsRangeDraggingChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!IsInitialized || !this.ChkHandleIsRangeDraggingChanged4.IsChecked.GetValueOrDefault(false))
                return;

            AddLogText(this.TxtLog5, "IsRangeDraggingChanged changed: " + e.OldValue.ToString() + "->" + e.NewValue.ToString());
        }

        private void rs3_RangeDragCompleted(object sender, RangeDragCompletedEventArgs<DateTime> e)
        {
            if (!IsInitialized || !this.ChkHandleRangeDragCompleted4.IsChecked.GetValueOrDefault(false))
                return;

            const string frmt = "d-M-yyyy H:m:s";
            AddLogText(this.TxtLog5, "RangeDragCompleted: [" + e.OldStartValue.ToString(frmt) + ".." + e.OldEndValue.ToString(frmt) + "] -> [" +
                e.NewStartValue.ToString(frmt) + ".." + e.NewEndValue.ToString(frmt) + "]");
        }

        #endregion

        #region zb2

        private void zb2_IsSelectionDraggingChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!IsInitialized || !this.ChkHandleIsSelectionDraggingChanged2.IsChecked.GetValueOrDefault(false))
                return;
            AddLogText(this.TxtLog3, "IsSelectionDragging changed: " + e.OldValue.ToString() + "->" + e.NewValue.ToString());
        }

        private void zb2_SelectionChanged(object sender, SelectionDragCompletedEventArgs e)
        {
            if (!IsInitialized || !this.ChkHandleSelectionChanged2.IsChecked.GetValueOrDefault(false))
                return;
            AddLogText(this.TxtLog3, "Selection changed: [" + e.OldSelectionStart.ToString("F4") + ".." + e.OldSelectionEnd.ToString("F4") + "] -> [" +
                e.NewSelectionStart.ToString("F4") + ".." + e.NewSelectionEnd.ToString("F4") + "]");
        }

        private void zb2_SelectionStartChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized || !this.ChkHandleSelectionStartChanged2.IsChecked.GetValueOrDefault(false))
                return;
            AddLogText(this.TxtLog3, "SelectionStart changed: " + e.OldValue.ToString("F4") + "->" + e.NewValue.ToString("F4"));
        }

        private void zb2_SelectionEndChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized || !this.ChkHandleSelectionEndChanged2.IsChecked.GetValueOrDefault(false))
                return;
            AddLogText(this.TxtLog3, "SelectionEnd changed: " + e.OldValue.ToString("F4") + "->" + e.NewValue.ToString("F4"));
        }

        private void zb2_SelectionRangeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!IsInitialized || !this.ChkHandleSelectionRangeChanged2.IsChecked.GetValueOrDefault(false))
                return;
            AddLogText(this.TxtLog3, "SelectionRange changed: " + e.OldValue.ToString("F4") + "->" + e.NewValue.ToString("F4"));
        }

        private void zb2_SelectionDragCompleted(object sender, SelectionDragCompletedEventArgs e)
        {
            if (!IsInitialized || !this.ChkHandleSelectionDragCompleted2.IsChecked.GetValueOrDefault(false))
                return;
            AddLogText(this.TxtLog3, "SelectionDragCompleted: [" + e.OldSelectionStart.ToString("F4") + ".." + e.OldSelectionEnd.ToString("F4") + "] -> [" +
                e.NewSelectionStart.ToString("F4") + ".." + e.NewSelectionEnd.ToString("F4") + "]");
        }

        #endregion

        public static DateTimeCollection HugeDateTimeCollection
        {
            get
            {
                return new DateTimeCollection(from y in Enumerable.Range(1, 9998) select new DateTime(y, 1, 1));
            }
        }
    }

    public sealed class MyBooleanToNullableConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            bool? res = null;
            if (value is bool)
            {
                res = (bool)value;
            }
            return res;
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            bool res = false;
            if (value is bool)
            {
                res = (bool)value;
            }
            return res;
        }
    }

    public sealed class StringToTickPlacementConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            var res = TickPlacement.None;
            if (value != null)
            {
                Enum.TryParse(value.ToString(), out res);
            }
            return res;
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            string res = null;
            if (value is TickPlacement)
            {
                res = ((TickPlacement)value).ToString();
            }
            return res;
        }
    }

    public sealed class DoubleRangeValueToStringConverter : IRangeValueToStringConverter<double>
    {
        public string Convert(double value, RangeThumbType thumbType, object parameter)
        {
            var zb = parameter as ZoomBar;
            Debug.Assert(zb != null, "zb != null");

            string res = value.ToString("F4", CultureInfo.CurrentCulture);
            if (thumbType == RangeThumbType.StartThumb || thumbType == RangeThumbType.RangeThumb)
            {
                double min = (new DateTime(2000, 1, 1)).Ticks,
                    max = (new DateTime(2000, 12, 31)).Ticks;
                double scale = (max - min) / (zb.Maximum - zb.Minimum);
                double newStart = (value * scale) + min;
                var newStartAsDateTime = new DateTime((long)newStart);

                res += " [" + newStartAsDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "]";
            }

            return res;
        }
    }

}