// -----------------------------------------------------------------------
// <copyright file="DoubleTicksCollection.cs" company="Sane Development">
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
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Media;

    /// <summary>
    /// Collection of ticks of <see cref="double"/> type.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Do similar to Microsoft code.")]
    public sealed class DoubleTicksCollection : ITicksCollection<double>
    {
#pragma warning disable SA1308 // Variable names should not be prefixed

        private readonly DoubleCollection m_Ticks;

#pragma warning restore SA1308 // Variable names should not be prefixed

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleTicksCollection"/> class
        /// with the specified collection of <see cref="double"/> values.
        /// </summary>
        /// <param name="ticks">The collection of <see cref="double"/> values that make up the <see cref="DoubleTicksCollection"/>.</param>
        public DoubleTicksCollection(DoubleCollection ticks)
        {
            if (ticks == null)
            {
                throw new ArgumentNullException(nameof(ticks));
            }

            this.m_Ticks = ticks;
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        /// <value>The number of elements contained in the collection.</value>
        public int Count
        {
            get
            {
                return this.m_Ticks.Count;
            }
        }

        /// <summary>
        /// Gets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index.</returns>
        public double this[int index]
        {
            get { return this.m_Ticks[index]; }
        }
    }
}
