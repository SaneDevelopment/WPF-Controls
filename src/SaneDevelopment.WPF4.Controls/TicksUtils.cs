// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TicksUtils.cs" company="Sane Development">
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows.Media;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Интерфейс, который должны реализовывать все коллекции, чтобы иметь возможность использоваться в качестве набора меток в ползунках
    /// </summary>
    /// <typeparam name="T">Тип значений, хранимых в коллекции</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    [ContractClass(typeof(ITicksCollectionContract<>))]
    public interface ITicksCollection<out T>
    {
        /// <summary>
        /// Элемент коллекции по заданному индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <returns>Элемент коллекции по заданному индексу</returns>
        T this[int index] { get; }

        /// <summary>
        /// Число элементов в коллекции
        /// </summary>
        int Count { get; }
    }

    [ContractClassFor(typeof(ITicksCollection<>))]
// ReSharper disable InconsistentNaming
    abstract class ITicksCollectionContract<T> : ITicksCollection<T>
// ReSharper restore InconsistentNaming
    {
        public T this[int index]
        {
            get
            {
                Contract.Requires(index >= 0);
                Contract.Requires(index < this.Count);
                return default(T);
            }
        }

        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                return 0;
            }
        }
    }

    /// <summary>
    /// Реализация интерфейса <see cref="ITicksCollection{T}"/> для типа <see cref="double"/>
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public sealed class DoubleTicksCollection : ITicksCollection<double>
    {
        private readonly DoubleCollection m_Ticks;

        /// <summary>
        /// Инициализирует объект
        /// </summary>
        /// <param name="ticks">Коллекция значений</param>
        public DoubleTicksCollection(DoubleCollection ticks)
        {
            Contract.Requires<ArgumentNullException>(ticks != null);

            m_Ticks = ticks;
        }

        /// <summary>
        /// Элемент коллекции по заданному индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <returns>Элемент коллекции по заданному индексу</returns>
        public double this[int index]
        {
            get { return m_Ticks[index]; }
        }

        /// <summary>
        /// Число элементов в коллекции
        /// </summary>
        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() == m_Ticks.Count);
                return m_Ticks.Count;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"),
        SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(m_Ticks != null);
            Contract.Invariant(m_Ticks.Count == this.Count);
        }
    }

    /// <summary>
    /// Реализация интерфейса <see cref="ITicksCollection{T}"/> для типа <see cref="DateTime"/>
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public sealed class DateTimeTicksCollection : ITicksCollection<DateTime>
    {
        private readonly IList<DateTime> m_Ticks;

        /// <summary>
        /// Инициализирует объект
        /// </summary>
        /// <param name="ticks">Коллекция значений</param>
        public DateTimeTicksCollection(IList<DateTime> ticks)
        {
            Contract.Requires<ArgumentNullException>(ticks != null);

            m_Ticks = ticks;
        }

        /// <summary>
        /// Элемент коллекции по заданному индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <returns>Элемент коллекции по заданному индексу</returns>
        public DateTime this[int index]
        {
            get { return m_Ticks[index]; }
        }

        /// <summary>
        /// Число элементов в коллекции
        /// </summary>
        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() == m_Ticks.Count);
                return m_Ticks.Count;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"),
        SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(m_Ticks != null);
            Contract.Invariant(m_Ticks.Count == this.Count);
        }
    }
}
