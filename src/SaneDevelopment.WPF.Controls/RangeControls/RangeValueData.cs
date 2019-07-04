// -----------------------------------------------------------------------
// <copyright file="RangeValueData.cs" company="Sane Development">
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
    /// <summary>
    /// Uses to store the range value data while dragging a thumb.
    /// </summary>
    /// <typeparam name="T">Type of range values.</typeparam>
    internal struct RangeValueData<T>
    {
        /// <summary>
        /// Gets or sets a value indicating whether range is dragging now.
        /// </summary>
        /// <value>A value indicating whether range is dragging now.</value>
        internal bool IsRangeDragging { get; set; }

        /// <summary>
        /// Gets or sets value of range start.
        /// </summary>
        /// <value>Value of range start.</value>
        internal T RangeStart { get; set; }

        /// <summary>
        /// Gets or sets value of range end.
        /// </summary>
        /// <value>Value of range end.</value>
        internal T RangeEnd { get; set; }
    }
}
