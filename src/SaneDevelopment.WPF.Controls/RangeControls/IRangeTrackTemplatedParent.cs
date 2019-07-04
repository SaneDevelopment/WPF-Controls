// -----------------------------------------------------------------------
// <copyright file="IRangeTrackTemplatedParent.cs" company="Sane Development">
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
    /// Interface for polymorphous call of <see cref="FrameworkElement.OnApplyTemplate"/>,
    /// in contrast to hardcoded implementation of that method inside of CLR.
    /// </summary>
    /// <typeparam name="T">Type of value.</typeparam>
    /// <typeparam name="TInterval">Type of interval value.</typeparam>
    public interface IRangeTrackTemplatedParent<T, TInterval>
    {
        /// <summary>
        /// Method should handle <see cref="FrameworkElement.OnApplyTemplate"/>.
        /// </summary>
        /// <param name="templatedParent">Templated parent.</param>
        /// <param name="track">Any range track.</param>
        void OnApplyRangeTrackTemplate(DependencyObject templatedParent, RangeTrack<T, TInterval> track);
    }
}
