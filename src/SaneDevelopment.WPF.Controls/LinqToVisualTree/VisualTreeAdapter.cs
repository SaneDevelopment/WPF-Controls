// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualTreeAdapter.cs" company="Sane Development">
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

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SaneDevelopment.WPF.Controls.LinqToVisualTree
{
    /// <summary>
    /// Adapts a DependencyObject to provide methods required for generate
    /// a Linq To Tree API.
    /// See http://www.scottlogic.co.uk/blog/colin/2010/03/linq-to-visual-tree/
    /// </summary>
    public class VisualTreeAdapter : ILinqTree<DependencyObject>
    {
        private readonly DependencyObject m_Item;

        /// <summary>
        /// Constructs new object
        /// </summary>
        /// <param name="item"><see cref="DependencyObject"/></param>
        public VisualTreeAdapter(DependencyObject item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            this.m_Item = item;
        }

        /// <summary>
        /// Get children of current <see cref="DependencyObject"/>
        /// </summary>
        /// <returns>children of current <see cref="DependencyObject"/></returns>
        public IEnumerable<DependencyObject> Children()
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(this.m_Item);
            for (int i = 0; i < childrenCount; i++)
            {
                yield return VisualTreeHelper.GetChild(this.m_Item, i);
            }
        }

        /// <summary>
        /// Get parent of current <see cref="DependencyObject"/>
        /// </summary>
        public DependencyObject Parent
        {
            get
            {
                return VisualTreeHelper.GetParent(this.m_Item);
            }
        }
    }
}
