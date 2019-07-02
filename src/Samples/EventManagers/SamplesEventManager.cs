// -----------------------------------------------------------------------
// <copyright file="SamplesEventManager.cs" company="Sane Development">
//
// Sane Development WPF Controls Library Samples.
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

namespace SaneDevelopment.WPF.Controls.Samples
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using SaneDevelopment.WPF.Controls.Interactivity;

    /// <summary>
    /// Events manager for in-XAML processing.
    /// </summary>
    public class SamplesEventManager : Freezable
    {
        /// <summary>
        /// Handles boolean property changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Paramater set.</param>
        public void OnBoolProperyValueChanged(object sender, ParameterizedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var initialEventArgs = e.InitialEventArgs as RoutedPropertyChangedEventArgs<bool>;
            Debug.Assert(initialEventArgs != null, "initialEventArgs != null");

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null, "txtLog != null");

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                $"{eventName}: {initialEventArgs.OldValue.ToString(CultureInfo.InvariantCulture)}->{initialEventArgs.NewValue.ToString(CultureInfo.InvariantCulture)}");
        }

        /// <summary>
        /// Handles double property changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Paramater set.</param>
        public void OnDoubleProperyValueChanged(object sender, ParameterizedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var initialEventArgs = e.InitialEventArgs as RoutedPropertyChangedEventArgs<double>;
            Debug.Assert(initialEventArgs != null, "initialEventArgs != null");

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null, "txtLog != null");

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                $"{eventName}: {initialEventArgs.OldValue.ToString(CultureInfo.InvariantCulture)}->{initialEventArgs.NewValue.ToString(CultureInfo.InvariantCulture)}");
        }

        /// <summary>
        /// Handles <see cref="DateTime"/> property changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Paramater set.</param>
        public void OnDateTimeProperyValueChanged(object sender, ParameterizedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var initialEventArgs = e.InitialEventArgs as RoutedPropertyChangedEventArgs<DateTime>;
            Debug.Assert(initialEventArgs != null, "initialEventArgs != null");

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null, "txtLog != null");

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                $"{eventName}: {initialEventArgs.OldValue.ToString("G", CultureInfo.InvariantCulture)}->{initialEventArgs.NewValue.ToString("G", CultureInfo.InvariantCulture)}");
        }

        /// <summary>
        /// Handles <see cref="TimeSpan"/> property changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Paramater set.</param>
        public void OnTimeSpanProperyValueChanged(object sender, ParameterizedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var initialEventArgs = e.InitialEventArgs as RoutedPropertyChangedEventArgs<TimeSpan>;
            Debug.Assert(initialEventArgs != null, "initialEventArgs != null");

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null, "txtLog != null");

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                $"{eventName}: {initialEventArgs.OldValue.ToString("G", CultureInfo.InvariantCulture)}->{initialEventArgs.NewValue.ToString("G", CultureInfo.InvariantCulture)}");
        }

        /// <summary>
        /// Handles double range drag completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Paramater set.</param>
        public void OnDoubleRangeDragCompleted(object sender, ParameterizedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var initialEventArgs = e.InitialEventArgs as RangeDragCompletedEventArgs<double>;
            Debug.Assert(initialEventArgs != null, "initialEventArgs != null");

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null, "txtLog != null");

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                $"{eventName}: [{initialEventArgs.OldStartValue.ToString("F4", CultureInfo.InvariantCulture)}..{initialEventArgs.OldEndValue.ToString("F4", CultureInfo.InvariantCulture)}] -> [{initialEventArgs.NewStartValue.ToString("F4", CultureInfo.InvariantCulture)}..{initialEventArgs.NewEndValue.ToString("F4", CultureInfo.InvariantCulture)}]");
        }

        /// <summary>
        /// Handles <see cref="DateTime"/> range drag completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Paramater set.</param>
        public void OnDateTimeRangeDragCompleted(object sender, ParameterizedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var initialEventArgs = e.InitialEventArgs as RangeDragCompletedEventArgs<DateTime>;
            Debug.Assert(initialEventArgs != null, "initialEventArgs != null");

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null, "txtLog != null");

            var eventName = (e.Parameter as string) ?? string.Empty;

            const string frmt = "d-M-yyyy H:m:s";
            AddLogText(
                txtLog,
                $"{eventName}: [{initialEventArgs.OldStartValue.ToString(frmt, CultureInfo.InvariantCulture)}..{initialEventArgs.OldEndValue.ToString(frmt, CultureInfo.InvariantCulture)}] -> [{initialEventArgs.NewStartValue.ToString(frmt, CultureInfo.InvariantCulture)}..{initialEventArgs.NewEndValue.ToString(frmt, CultureInfo.InvariantCulture)}]");
        }

        /// <summary>
        /// Handles selection drag completed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Paramater set.</param>
        public void OnSelectionDragCompleted(object sender, ParameterizedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var initialEventArgs = e.InitialEventArgs as SelectionDragCompletedEventArgs;
            Debug.Assert(initialEventArgs != null, "initialEventArgs != null");

            var txtLog = e.Parameter2 as TextBox;
            Debug.Assert(txtLog != null, "txtLog != null");

            var eventName = (e.Parameter as string) ?? string.Empty;

            AddLogText(
                txtLog,
                $"{eventName}: [{initialEventArgs.OldSelectionStart.ToString("F4", CultureInfo.InvariantCulture)}..{initialEventArgs.OldSelectionEnd.ToString("F4", CultureInfo.InvariantCulture)}] -> [{initialEventArgs.NewSelectionStart.ToString("F4", CultureInfo.InvariantCulture)}..{initialEventArgs.NewSelectionEnd.ToString("F4", CultureInfo.InvariantCulture)}]");
        }

        /// <summary>
        /// Handles AddNowToListBox button.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Paramater set.</param>
        public void AddNowToListBox(object sender, ParameterizedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // ListBox doesnt observe changes in DateTimeCollection (need something ObservableCollection<>)
            // so we need to reset ItemsSource every time we change it

            var listBox = e.Parameter as ListBox;
            Debug.Assert(listBox != null, "listBox != null");

            var collection = listBox.ItemsSource as DateTimeCollection;
            collection = collection == null ? new DateTimeCollection() : new DateTimeCollection(collection);

            collection.Add(DateTime.Now);

            listBox.ItemsSource = collection;
        }

        /// <summary>
        /// Handles RemoveItemsFromListBox button.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Paramater set.</param>
        public void RemoveItemsFromListBox(object sender, ParameterizedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // ListBox doesnt observe changes in DateTimeCollection (need something ObservableCollection<>)
            // so we need to reset ItemsSource every time we change it

            var listBox = e.Parameter as ListBox;
            Debug.Assert(listBox != null, "listBox != null");

            Debug.Assert(e.Parameter2 is DateTime, "e.Parameter2 is DateTime");
            var dateToRemove = (DateTime)e.Parameter2;

            var collection = listBox.ItemsSource as DateTimeCollection;
            collection = collection == null ? new DateTimeCollection() : new DateTimeCollection(collection);

            collection.Remove(dateToRemove);

            listBox.ItemsSource = collection;
        }


        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
        {
            return new SamplesEventManager();
        }

        private static void AddLogText(TextBox textBox, string text)
        {
            textBox.Text += text + Environment.NewLine;
            textBox.ScrollToEnd();
        }
    }
}
