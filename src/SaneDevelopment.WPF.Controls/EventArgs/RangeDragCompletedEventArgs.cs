// -----------------------------------------------------------------------
// <copyright file="RangeDragCompletedEventArgs.cs" company="Sane Development">
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
    /// Contains state information and event data associated with a range thumb drag completed routed event.
    /// </summary>
    /// <typeparam name="T">Range values type.</typeparam>
    public class RangeDragCompletedEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RangeDragCompletedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="oldStartValue">Old range start value.</param>
        /// <param name="oldEndValue">Old range end value.</param>
        /// <param name="newStartValue">New range start value.</param>
        /// <param name="newEndValue">New range end value.</param>
        public RangeDragCompletedEventArgs(
            T oldStartValue,
            T oldEndValue,
            T newStartValue,
            T newEndValue)
        {
            this.OldStartValue = oldStartValue;
            this.OldEndValue = oldEndValue;
            this.NewStartValue = newStartValue;
            this.NewEndValue = newEndValue;
        }

        /// <summary>
        /// Gets old range start value.
        /// </summary>
        /// <value>Old range start value.</value>
        public T OldStartValue { get; private set; }

        /// <summary>
        /// Gets old range end value.
        /// </summary>
        /// <value>Old range end value.</value>
        public T OldEndValue { get; private set; }

        /// <summary>
        /// Gets new range start value.
        /// </summary>
        /// <value>New range start value.</value>
        public T NewStartValue { get; private set; }

        /// <summary>
        /// Gets new range end value.
        /// </summary>
        /// <value>New range end value.</value>
        public T NewEndValue { get; private set; }
    }
}
