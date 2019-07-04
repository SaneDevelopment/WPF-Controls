// -----------------------------------------------------------------------
// <copyright file="SelectionDragCompletedEventArgs.cs" company="Sane Development">
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
    using System.Windows;

    /// <summary>
    /// Contains state information and event data associated with a double-range control drag completed routed event.
    /// </summary>
    public class SelectionDragCompletedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionDragCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="oldSelectionStart">Old selection start value.</param>
        /// <param name="oldSelectionEnd">Old selection end value.</param>
        /// <param name="newSelectionStart">New selection start value.</param>
        /// <param name="newSelectionEnd">New selection end value.</param>
        public SelectionDragCompletedEventArgs(
            double oldSelectionStart,
            double oldSelectionEnd,
            double newSelectionStart,
            double newSelectionEnd)
        {
            this.OldSelectionStart = oldSelectionStart;
            this.OldSelectionEnd = oldSelectionEnd;
            this.NewSelectionStart = newSelectionStart;
            this.NewSelectionEnd = newSelectionEnd;
        }

        /// <summary>
        /// Gets old selection start value.
        /// </summary>
        /// <value>Old selection start value.</value>
        public double OldSelectionStart { get; private set; }

        /// <summary>
        /// Gets old selection end value.
        /// </summary>
        /// <value>Old selection end value.</value>
        public double OldSelectionEnd { get; private set; }

        /// <summary>
        /// Gets old value of selection range.
        /// </summary>
        /// <value>Old value of selection range.</value>
        public double OldSelectionRange
        {
            get { return this.OldSelectionEnd - this.OldSelectionStart; }
        }

        /// <summary>
        /// Gets new selection start value.
        /// </summary>
        /// <value>New selection start value.</value>
        public double NewSelectionStart { get; private set; }

        /// <summary>
        /// Gets new selection end value.
        /// </summary>
        /// <value>New selection end value.</value>
        public double NewSelectionEnd { get; private set; }

        /// <summary>
        /// Gets new value of selection range.
        /// </summary>
        /// <value>New value of selection range.</value>
        public double NewSelectionRange
        {
            get { return this.NewSelectionEnd - this.NewSelectionStart; }
        }
    }
}
