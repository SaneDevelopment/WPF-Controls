// -----------------------------------------------------------------------
// <copyright file="ILinqTree.cs" company="Sane Development">
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

namespace SaneDevelopment.WPF.Controls.LinqToVisualTree
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an interface that must be implemented to generate the LinqToTree methods.
    /// See http://www.scottlogic.co.uk/blog/colin/2010/03/linq-to-visual-tree/.
    /// </summary>
    /// <typeparam name="T">Tree item type.</typeparam>
    public interface ILinqTree<out T>
    {
        /// <summary>
        /// Gets get parent of current item.
        /// </summary>
        /// <value>Parent of current item.</value>
        T Parent { get; }

        /// <summary>
        /// Get children of current item.
        /// </summary>
        /// <returns>children of current item.</returns>
        IEnumerable<T> Children();
    }
}
