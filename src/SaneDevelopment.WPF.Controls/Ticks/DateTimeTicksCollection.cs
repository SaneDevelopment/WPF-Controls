// -----------------------------------------------------------------------
// <copyright file="DateTimeTicksCollection.cs" company="Sane Development">
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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Collection of ticks of <see cref="DateTime"/> type.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Do similar to Microsoft code.")]
    public sealed class DateTimeTicksCollection : ITicksCollection<DateTime>
    {
#pragma warning disable SA1308 // Variable names should not be prefixed

        private readonly IList<DateTime> m_Ticks;

#pragma warning restore SA1308 // Variable names should not be prefixed

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeTicksCollection"/> class
        /// with the specified collection of <see cref="DateTime"/> values.
        /// </summary>
        /// <param name="ticks">The collection of <see cref="DateTime"/> values that make up the <see cref="DateTimeTicksCollection"/>.</param>
        public DateTimeTicksCollection(IList<DateTime> ticks)
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
        public DateTime this[int index]
        {
            get { return this.m_Ticks[index]; }
        }
    }
}
