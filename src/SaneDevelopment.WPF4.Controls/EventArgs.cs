// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventArgs.cs" company="Sane Development">
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

using System.Windows;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Аргументы события завершения перетаскивания центрального ползунка интервального контрола
    /// </summary>
    /// <typeparam name="T">Тип значений контрола</typeparam>
    public class RangeDragCompletedEventArgs<T> : RoutedEventArgs
    {
        /// <summary>
        /// Инициализирует новый объект
        /// </summary>
        /// <param name="oldStartValue">Старое значение начала интервала</param>
        /// <param name="oldEndValue">Старое значение конца интервала</param>
        /// <param name="newStartValue">Новое значение начала интервала</param>
        /// <param name="newEndValue">Новое значение конца интервала</param>
        public RangeDragCompletedEventArgs(T oldStartValue, T oldEndValue,
            T newStartValue, T newEndValue)
        {
            OldStartValue = oldStartValue;
            OldEndValue = oldEndValue;
            NewStartValue = newStartValue;
            NewEndValue = newEndValue;
        }

        /// <summary>
        /// Старое значение начала интервала
        /// </summary>
        public T OldStartValue { get; private set; }
        /// <summary>
        /// Старое значение конца интервала
        /// </summary>
        public T OldEndValue { get; private set; }

        /// <summary>
        /// Новое значение начала интервала
        /// </summary>
        public T NewStartValue { get; private set; }
        /// <summary>
        /// Новое значение конца интервала
        /// </summary>
        public T NewEndValue { get; private set; }
    }

    /// <summary>
    /// Аргументы события завершения перетаскивания области выделения интервального контрола
    /// </summary>
    public class SelectionDragCompletedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Инициализирует новый объект
        /// </summary>
        /// <param name="oldSelectionStart">Старое значение начала интервала</param>
        /// <param name="oldSelectionEnd">Старое значение конца интервала</param>
        /// <param name="newSelectionStart">Новое значение начала интервала</param>
        /// <param name="newSelectionEnd">Новое значение конца интервала</param>
        public SelectionDragCompletedEventArgs(double oldSelectionStart, double oldSelectionEnd,
            double newSelectionStart, double newSelectionEnd)
        {
            OldSelectionStart = oldSelectionStart;
            OldSelectionEnd = oldSelectionEnd;
            NewSelectionStart = newSelectionStart;
            NewSelectionEnd = newSelectionEnd;
        }

        /// <summary>
        /// Старое значение начала интервала
        /// </summary>
        public double OldSelectionStart { get; private set; }
        /// <summary>
        /// Старое значение конца интервала
        /// </summary>
        public double OldSelectionEnd { get; private set; }
        /// <summary>
        /// Старое значение величины области выделения
        /// </summary>
        public double OldSelectionRange { get { return OldSelectionEnd - OldSelectionStart; } }

        /// <summary>
        /// Новое значение начала интервала
        /// </summary>
        public double NewSelectionStart { get; private set; }
        /// <summary>
        /// Новое значение конца интервала
        /// </summary>
        public double NewSelectionEnd { get; private set; }
        /// <summary>
        /// Новое значение величины области выделения
        /// </summary>
        public double NewSelectionRange { get { return NewSelectionEnd - NewSelectionStart; } }
    }

}
