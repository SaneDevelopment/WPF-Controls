// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Sane Development">
//
//   Sane Development WPF Controls Library
//
//   The BSD 3-Clause License
//
//   Copyright (c) 2019, Sane Development
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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SaneDevelopment.WPF4.Controls.Interactivity;

namespace SaneDevelopment.WPF4.Controls.Samples
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }
    }

    public class SamplesEventManager : Freezable
    {
        private static void AddLogText(TextBox textBox, string text)
        {
            textBox.Text += text + Environment.NewLine;
            textBox.ScrollToEnd();
        }

        public void OnBoolProperyValueChanged(object sender, ParameterizedEventArgs e)
        {
            var initialEventArgs = e.InitialEventArgs as RoutedPropertyChangedEventArgs<bool>;
            Debug.Assert(initialEventArgs != null);

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null);

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                eventName + ": " +
                initialEventArgs.OldValue.ToString(CultureInfo.InvariantCulture) +
                "->" +
                initialEventArgs.NewValue.ToString(CultureInfo.InvariantCulture));
        }

        public void OnDoubleProperyValueChanged(object sender, ParameterizedEventArgs e)
        {
            var initialEventArgs = e.InitialEventArgs as RoutedPropertyChangedEventArgs<double>;
            Debug.Assert(initialEventArgs != null);

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null);

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                eventName + ": " +
                initialEventArgs.OldValue.ToString(CultureInfo.InvariantCulture) +
                "->" +
                initialEventArgs.NewValue.ToString(CultureInfo.InvariantCulture));
        }

        public void OnDateTimeProperyValueChanged(object sender, ParameterizedEventArgs e)
        {
            var initialEventArgs = e.InitialEventArgs as RoutedPropertyChangedEventArgs<DateTime>;
            Debug.Assert(initialEventArgs != null);

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null);

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                eventName + ": " +
                initialEventArgs.OldValue.ToString("G", CultureInfo.InvariantCulture) +
                "->" +
                initialEventArgs.NewValue.ToString("G", CultureInfo.InvariantCulture));
        }

        public void OnTimeSpanProperyValueChanged(object sender, ParameterizedEventArgs e)
        {
            var initialEventArgs = e.InitialEventArgs as RoutedPropertyChangedEventArgs<TimeSpan>;
            Debug.Assert(initialEventArgs != null);

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null);

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                eventName + ": " +
                initialEventArgs.OldValue.ToString("G", CultureInfo.InvariantCulture) +
                "->" +
                initialEventArgs.NewValue.ToString("G", CultureInfo.InvariantCulture));
        }

        public void OnDoubleRangeDragCompleted(object sender, ParameterizedEventArgs e)
        {
            var initialEventArgs = e.InitialEventArgs as RangeDragCompletedEventArgs<double>;
            Debug.Assert(initialEventArgs != null);

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null);

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                eventName + ": [" +
                initialEventArgs.OldStartValue.ToString("F4", CultureInfo.InvariantCulture) + ".." + initialEventArgs.OldEndValue.ToString("F4", CultureInfo.InvariantCulture) + 
                "] -> [" +
                initialEventArgs.NewStartValue.ToString("F4", CultureInfo.InvariantCulture) + ".." + initialEventArgs.NewEndValue.ToString("F4", CultureInfo.InvariantCulture) +
                "]");
        }

        public void OnDateTimeRangeDragCompleted(object sender, ParameterizedEventArgs e)
        {
            var initialEventArgs = e.InitialEventArgs as RangeDragCompletedEventArgs<DateTime>;
            Debug.Assert(initialEventArgs != null);

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null);

            var eventName = (e.Parameter as string) ?? string.Empty;

            const string frmt = "d-M-yyyy H:m:s";
            AddLogText(
                txtLog,
                eventName + ": [" +
                initialEventArgs.OldStartValue.ToString(frmt, CultureInfo.InvariantCulture) + ".." + initialEventArgs.OldEndValue.ToString(frmt, CultureInfo.InvariantCulture) +
                "] -> [" +
                initialEventArgs.NewStartValue.ToString(frmt, CultureInfo.InvariantCulture) + ".." + initialEventArgs.NewEndValue.ToString(frmt, CultureInfo.InvariantCulture) +
                "]");
        }

        public void OnSelectionDragCompleted(object sender, ParameterizedEventArgs e)
        {
            var initialEventArgs = e.InitialEventArgs as SelectionDragCompletedEventArgs;
            Debug.Assert(initialEventArgs != null);

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null);

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                eventName + ": [" +
                initialEventArgs.OldSelectionStart.ToString("F4", CultureInfo.InvariantCulture) + ".." + initialEventArgs.OldSelectionEnd.ToString("F4", CultureInfo.InvariantCulture) +
                "] -> [" +
                initialEventArgs.NewSelectionStart.ToString("F4", CultureInfo.InvariantCulture) + ".." + initialEventArgs.NewSelectionEnd.ToString("F4", CultureInfo.InvariantCulture) +
                "]");
        }

        public void AddNowToListBox(object sender, ParameterizedEventArgs e)
        {
            // ListBox doesnt observe changes in DateTimeCollection (need something ObservableCollection<>)
            // so we need to reset ItemsSource every time we change it

            var listBox = e.Parameter as ListBox;
            Debug.Assert(listBox != null);

            var collection = listBox.ItemsSource as DateTimeCollection;
            collection = collection == null ? new DateTimeCollection() : new DateTimeCollection(collection);

            collection.Add(DateTime.Now);

            listBox.ItemsSource = collection;
        }

        public void RemoveItemsFromListBox(object sender, ParameterizedEventArgs e)
        {
            // ListBox doesnt observe changes in DateTimeCollection (need something ObservableCollection<>)
            // so we need to reset ItemsSource every time we change it

            var listBox = e.Parameter as ListBox;
            Debug.Assert(listBox != null);

            Debug.Assert(e.Parameter2 is DateTime);
            var dateToRemove = (DateTime)e.Parameter2;

            var collection = listBox.ItemsSource as DateTimeCollection;
            collection = collection == null ? new DateTimeCollection() : new DateTimeCollection(collection);

            collection.Remove(dateToRemove);

            listBox.ItemsSource = collection;
        }


        protected override Freezable CreateInstanceCore()
        {
            return new SamplesEventManager();
        }
    }

    public sealed class NegateDoubleConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;

            return -1.0 * System.Convert.ToDouble(value);
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }
    }

    public sealed class DoubleRangeValueToStringConverter : IRangeValueToStringConverter<double>
    {
        /// <summary>
        /// Suppose we use ZoomBar as a slider for date interval [01-01-2000..31-12-2000]
        /// </summary>
        private static DateTime s_DateFrom = new DateTime(2000, 1, 1),
                                s_DateTo = new DateTime(2000, 12, 31);

        public string Convert(double value, RangeThumbType thumbType, object parameter)
        {
            var zb = parameter as ZoomBar;
            Debug.Assert(zb != null, "zb != null");

            string res = value.ToString("F4", CultureInfo.CurrentCulture);
            if (thumbType == RangeThumbType.StartThumb || thumbType == RangeThumbType.RangeThumb)
            {
                double min = s_DateFrom.Ticks, max = s_DateTo.Ticks;

                double scale = (max - min) / (zb.Maximum - zb.Minimum);
                double newStart = (value * scale) + min;
                var newStartAsDateTime = new DateTime((long)newStart);

                res += " [" + newStartAsDateTime.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture) + "]";
            }

            return res;
        }
    }

    public sealed class UniversalConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            if (targetType == null || value == null)
            {
                return null;
            }

            object res = DependencyProperty.UnsetValue;

            TypeConverter converter = TypeDescriptor.GetConverter(targetType);

            try
            {
                if (converter.CanConvertFrom(value.GetType()))
                {
                    res = converter.ConvertFrom(value);
                }
                else if (converter.CanConvertFrom(typeof(string)))
                {
                    res = converter.ConvertFrom(value.ToString());
                }
            }
            catch(FormatException)
            {}

            return res;
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }
    }

    /// <summary>
    /// Added for purposes to pass concrete <c>targetType</c> to <see cref="UniversalConverter.Convert"/>
    /// </summary>
    public sealed class StringToDateTimeCollectionConverter : IValueConverter
    {
        public object Convert(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        {
            return new UniversalConverter().Convert(value ?? string.Empty, typeof(DateTimeCollection), parameter, culture);
        }

        public object ConvertBack(object value,
                                  Type targetType,
                                  object parameter,
                                  CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }
    }
}
