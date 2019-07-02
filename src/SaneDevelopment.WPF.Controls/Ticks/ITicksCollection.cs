// -----------------------------------------------------------------------
// <copyright file="ITicksCollection.cs" company="Sane Development">
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
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Defines methods to manipulate collections, that uses as ticks collections for tick bars inside sliders.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Do similar to Microsoft code.")]
    public interface ITicksCollection<out T>
    {
        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        /// <value>The number of elements contained in the collection.</value>
        int Count { get; }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        T this[int index] { get; }
    }
}
