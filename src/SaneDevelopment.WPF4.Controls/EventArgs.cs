// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventArgs.cs" company="Sane Development">
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

using System.Windows;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Contains state information and event data associated with a range thumb drag completed routed event.
    /// </summary>
    /// <typeparam name="T">Range values type</typeparam>
    public class RangeDragCompletedEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="oldStartValue">Old range start value</param>
        /// <param name="oldEndValue">Old range end value</param>
        /// <param name="newStartValue">New range start value</param>
        /// <param name="newEndValue">New range end value</param>
        public RangeDragCompletedEventArgs(T oldStartValue, T oldEndValue,
            T newStartValue, T newEndValue)
        {
            OldStartValue = oldStartValue;
            OldEndValue = oldEndValue;
            NewStartValue = newStartValue;
            NewEndValue = newEndValue;
        }

        /// <summary>
        /// Old range start value
        /// </summary>
        public T OldStartValue { get; private set; }
        
        /// <summary>
        /// Old range end value
        /// </summary>
        public T OldEndValue { get; private set; }

        /// <summary>
        /// New range start value
        /// </summary>
        public T NewStartValue { get; private set; }
        
        /// <summary>
        /// New range end value
        /// </summary>
        public T NewEndValue { get; private set; }
    }

    /// <summary>
    /// Contains state information and event data associated with a double-range control drag completed routed event.
    /// </summary>
    public class SelectionDragCompletedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="oldSelectionStart">Old selection start value</param>
        /// <param name="oldSelectionEnd">Old selection end value</param>
        /// <param name="newSelectionStart">New selection start value</param>
        /// <param name="newSelectionEnd">New selection end value</param>
        public SelectionDragCompletedEventArgs(double oldSelectionStart, double oldSelectionEnd,
            double newSelectionStart, double newSelectionEnd)
        {
            OldSelectionStart = oldSelectionStart;
            OldSelectionEnd = oldSelectionEnd;
            NewSelectionStart = newSelectionStart;
            NewSelectionEnd = newSelectionEnd;
        }

        /// <summary>
        /// Old selection start value
        /// </summary>
        public double OldSelectionStart { get; private set; }

        /// <summary>
        /// Old selection end value
        /// </summary>
        public double OldSelectionEnd { get; private set; }
        
        /// <summary>
        /// Old value of selection range
        /// </summary>
        public double OldSelectionRange { get { return OldSelectionEnd - OldSelectionStart; } }

        /// <summary>
        /// New selection start value
        /// </summary>
        public double NewSelectionStart { get; private set; }
        
        /// <summary>
        /// New selection end value
        /// </summary>
        public double NewSelectionEnd { get; private set; }
        
        /// <summary>
        /// New value of selection range
        /// </summary>
        public double NewSelectionRange { get { return NewSelectionEnd - NewSelectionStart; } }
    }

}
