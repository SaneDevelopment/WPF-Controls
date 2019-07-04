// -----------------------------------------------------------------------
// <copyright file="VisualTreeAdapter.cs" company="Sane Development">
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
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Adapts a DependencyObject to provide methods required for generate
    /// a Linq To Tree API.
    /// See http://www.scottlogic.co.uk/blog/colin/2010/03/linq-to-visual-tree/.
    /// </summary>
    public class VisualTreeAdapter : ILinqTree<DependencyObject>
    {
#pragma warning disable SA1308 // Variable names should not be prefixed
        private readonly DependencyObject m_Item;
#pragma warning restore SA1308 // Variable names should not be prefixed

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualTreeAdapter"/> class.
        /// </summary>
        /// <param name="item"><see cref="DependencyObject"/>.</param>
        public VisualTreeAdapter(DependencyObject item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.m_Item = item;
        }

        /// <summary>
        /// Gets parent of current <see cref="DependencyObject"/>.
        /// </summary>
        /// <value>Parent of current <see cref="DependencyObject"/>.</value>
        public DependencyObject Parent
        {
            get
            {
                return VisualTreeHelper.GetParent(this.m_Item);
            }
        }

        /// <summary>
        /// Get children of current <see cref="DependencyObject"/>.
        /// </summary>
        /// <returns>children of current <see cref="DependencyObject"/>.</returns>
        public IEnumerable<DependencyObject> Children()
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(this.m_Item);
            for (int i = 0; i < childrenCount; i++)
            {
                yield return VisualTreeHelper.GetChild(this.m_Item, i);
            }
        }
    }
}
