// -----------------------------------------------------------------------
// <copyright file="DateTimeCollection.cs" company="Sane Development">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Markup;
    using SaneDevelopment.WPF.Controls.Properties;

    /// <summary>
    /// Represents a collection of <see cref="DateTime"/> values.
    /// </summary>
    [ValueSerializer(typeof(DateTimeCollectionValueSerializer))]
    [TypeConverter(typeof(DateTimeCollectionConverter))]
    public sealed class DateTimeCollection
        : Freezable, IFormattable, IList, IList<DateTime>
    {
        #region Private fields
#pragma warning disable SA1308 // Variable names should not be prefixed
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter
        private static readonly Lazy<DateTimeCollection> s_Empty = new Lazy<DateTimeCollection>(() =>
        {
            var doubles = new DateTimeCollection();
            doubles.Freeze();
            return doubles;
        });

        private List<DateTime> m_Collection;
        private uint m_Version;

#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning restore SA1308 // Variable names should not be prefixed
        #endregion Private fields

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeCollection"/> class.
        /// </summary>
        public DateTimeCollection()
        {
            this.m_Collection = new List<DateTime>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeCollection"/> class with the specified collection of <see cref="DateTime"/> values.
        /// </summary>
        /// <param name="collection">The collection of <see cref="DateTime"/> values that make up the <see cref="DateTimeCollection"/>.</param>
        public DateTimeCollection(IEnumerable<DateTime> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            Debug.Assert(!this.IsFrozen, "!this.IsFrozen");

            this.WritePreamble();
            if (collection is ICollection<DateTime> is3)
            {
                this.m_Collection = new List<DateTime>(is3);
            }
            else
            {
                if (collection is ICollection is2)
                {
                    this.m_Collection = new List<DateTime>(is2.OfType<DateTime>());
                }
                else
                {
                    this.m_Collection = new List<DateTime>();
                    foreach (DateTime num in collection)
                    {
                        this.m_Collection.Add(num);
                    }
                }
            }

            this.WritePostscript();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeCollection"/> class with the specified capacity,
        /// or the number of <see cref="DateTime"/> values the collection is initially capable of storing.
        /// </summary>
        /// <param name="capacity">The number of <see cref="DateTime"/> values that the collection is initially capable of storing.</param>
        public DateTimeCollection(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            this.m_Collection = new List<DateTime>(capacity);
        }

        /// <summary>
        /// Gets frozen empty <see cref="DateTimeCollection"/>.
        /// </summary>
        /// <value>Frozen empty <see cref="DateTimeCollection"/>.</value>
        public static DateTimeCollection Empty
        {
            get
            {
                return s_Empty.Value;
            }
        }

        /// <summary>
        /// Converts a <c>string</c> representation of a collection of dates into an equivalent <see cref="DateTimeCollection"/>.
        /// </summary>
        /// <param name="source">The <c>string</c> representation of the collection of dates.</param>
        /// <param name="itemFormat">Format string for <see cref="DateTime"/> item in collection.</param>
        /// <param name="separator">Separator (delimiter) for items in string.</param>
        /// <returns>Returns the equivalent <see cref="DateTimeCollection"/>.</returns>
        public static DateTimeCollection Parse(
            string source,
            string itemFormat = DateTimeCollectionConverter.DefaultFormatString,
            char separator = DateTimeCollectionConverter.StringItemsSeparator)
        {
            if (itemFormat == null)
            {
                throw new ArgumentNullException(nameof(itemFormat));
            }

            return DateTimeCollectionConverter.ParseCore(source, itemFormat, separator);
        }

        /// <summary>
        /// Creates a <c>string</c> representation of this collection.
        /// </summary>
        /// <param name="provider">Culture-specific formatting information.</param>
        /// <returns>Returns a <see cref="string"/> containing the values of this collection.</returns>
        public string ToString(IFormatProvider provider)
        {
            this.ReadPreamble();

            return DateTimeCollectionConverter.ConvertToStringCore(
                this.m_Collection,
                DateTimeCollectionConverter.DefaultFormatString,
                provider,
                DateTimeCollectionConverter.StringItemsSeparator);
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the collection.
        /// </summary>
        /// <returns>An <see cref="DateTimeCollection.Enumerator"/> that can iterate through the collection.</returns>
        public Enumerator GetEnumerator()
        {
            this.ReadPreamble();
            return new Enumerator(this);
        }

        #region IFormattable implementation

        /// <inheritdoc/>
        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            this.ReadPreamble();

            return DateTimeCollectionConverter.ConvertToStringCore(
                this.m_Collection,
                format,
                provider,
                DateTimeCollectionConverter.StringItemsSeparator);
        }

        #endregion IFormattable implementation

        #region IList implementation

#pragma warning disable SA1201 // Elements should appear in the correct order

        /// <inheritdoc/>
        bool IList.IsFixedSize
        {
            get
            {
                this.ReadPreamble();
                return this.IsFrozen;
            }
        }

#pragma warning restore SA1201 // Elements should appear in the correct order

        /// <inheritdoc/>
        bool IList.IsReadOnly
        {
            get
            {
                return ((ICollection<DateTime>)this).IsReadOnly;
            }
        }

        /// <inheritdoc/>
        object IList.this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return this[index];
            }

            set
            {
                if (this.IsFrozen)
                {
                    throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
                }

                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                if (value == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                if (!(value is DateTime))
                {
                    throw new ArgumentException(LocalizationResource.CollectionBadType, nameof(value));
                }

                this[index] = Cast(value);
            }
        }

        /// <inheritdoc/>
        int IList.Add(object value)
        {
            if (this.IsFrozen)
            {
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
            }

            if (value == null)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (!(value is DateTime))
            {
                throw new ArgumentException(LocalizationResource.CollectionBadType, nameof(value));
            }

            return this.AddHelper(Cast(value));
        }

        /// <inheritdoc/>
        bool IList.Contains(object value)
        {
            return (value is DateTime) && this.Contains((DateTime)value);
        }

        /// <inheritdoc/>
        int IList.IndexOf(object value)
        {
            if (value is DateTime)
            {
                return this.IndexOf((DateTime)value);
            }

            return -1;
        }

        /// <inheritdoc/>
        void IList.Insert(int index, object value)
        {
            if (this.IsFrozen)
            {
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
            }

            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (value == null)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (!(value is DateTime))
            {
                throw new ArgumentException(LocalizationResource.CollectionBadType, nameof(value));
            }

            this.Insert(index, Cast(value));
        }

        /// <inheritdoc/>
        void IList.Remove(object value)
        {
            if (value is DateTime)
            {
                _ = this.Remove((DateTime)value);
            }
        }

        /// <inheritdoc/>
        void IList.RemoveAt(int index)
        {
            if (this.IsFrozen)
            {
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
            }

            var oldCount = this.Count;

            this.RemoveAtWithoutFiringPublicEvents(index);
            this.WritePostscript();

            Debug.Assert(this.Count == oldCount - 1, "this.Count == oldCount - 1");
        }

        /// <summary>
        /// Removes the <see cref="System.Collections.IList"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            ((IList)this).RemoveAt(index);
        }

        #endregion IList implementation

        #region ICollection implementation

#pragma warning disable SA1201 // Elements should appear in the correct order

        /// <summary>
        /// Gets the number of elements actually contained in the collection.
        /// </summary>
        /// <value>The number of elements actually contained in the collection.</value>
        public int Count
        {
            get
            {
                this.ReadPreamble();
                return this.m_Collection.Count;
            }
        }

#pragma warning restore SA1201 // Elements should appear in the correct order

        /// <inheritdoc/>
        bool ICollection.IsSynchronized
        {
            get
            {
                this.ReadPreamble();
                return this.IsFrozen || this.Dispatcher != null;
            }
        }

        /// <inheritdoc/>
        object ICollection.SyncRoot
        {
            get
            {
                this.ReadPreamble();
                return this;
            }
        }

        /// <inheritdoc/>
        void ICollection.CopyTo(Array array, int index)
        {
            this.ReadPreamble();

            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if ((index < 0) || ((index + this.m_Collection.Count) > array.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException(LocalizationResource.CollectionBadRank);
            }

            int count = this.m_Collection.Count;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    Debug.Assert(array.Rank > 0, "array.Rank > 0");
                    Debug.Assert(index + i <= array.GetUpperBound(0), "index + i <= array.GetUpperBound(0)");
                    array.SetValue(this.m_Collection[i], index + i);
                }
            }
            catch (InvalidCastException exception)
            {
                throw new ArgumentException(LocalizationResource.CollectionBadDestArray, exception);
            }
        }

        #endregion ICollection implementation

        #region IList<DateTime> implementation

#pragma warning disable SA1201 // Elements should appear in the correct order

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns><see cref="DateTime"/>.</returns>
        public DateTime this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                this.ReadPreamble();
                return this.m_Collection[index];
            }

            set
            {
                if (this.IsFrozen)
                {
                    throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
                }

                if (index < 0 || index >= this.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                this.WritePreamble();
                this.m_Collection[index] = value;
                this.m_Version++;
                this.WritePostscript();
            }
        }

#pragma warning restore SA1201 // Elements should appear in the correct order

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="item">The item to locate in the collection.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire collection, if found; otherwise, –1.</returns>
        public int IndexOf(DateTime item)
        {
            this.ReadPreamble();
            return this.m_Collection.IndexOf(item);
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The <see cref="DateTime"/> to insert.</param>
        public void Insert(int index, DateTime item)
        {
            if (this.IsFrozen)
            {
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
            }

            if (index < 0 || index > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var oldCount = this.Count;

            this.WritePreamble();
            Debug.Assert(index <= this.m_Collection.Count, "index <= this.m_Collection.Count");
            this.m_Collection.Insert(index, item);
            this.m_Version++;
            this.WritePostscript();
            Debug.Assert(this.Count == oldCount + 1, "this.Count == oldCount + 1");
        }

        #endregion IList<DateTime> implementation

        #region ICollection<DateTime> implementation

#pragma warning disable SA1201 // Elements should appear in the correct order

        /// <inheritdoc/>
        bool ICollection<DateTime>.IsReadOnly
        {
            get
            {
                this.ReadPreamble();
                return this.IsFrozen;
            }
        }

#pragma warning restore SA1201 // Elements should appear in the correct order

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The <see cref="DateTime"/> to add to the collection.</param>
        public void Add(DateTime item)
        {
            if (this.IsFrozen)
            {
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
            }

            _ = this.AddHelper(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            if (this.IsFrozen)
            {
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
            }

            this.WritePreamble();
            this.m_Collection.Clear();
            this.m_Version++;
            this.WritePostscript();

            Debug.Assert(((ICollection)this).IsSynchronized || this.Count == 0, "((ICollection)this).IsSynchronized || this.Count == 0");
            Debug.Assert(this.Count == 0, "this.Count == 0");
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="item">The <see cref="DateTime"/> to locate in the collection.</param>
        /// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(DateTime item)
        {
            this.ReadPreamble();
            var res = this.m_Collection.Contains(item);

            Debug.Assert(!res || this.Count > 0, "!res || this.Count > 0");

            return res;
        }

        /// <summary>
        /// Copies the elements of the collection to an <see cref="System.Array"/>, starting at a particular <see cref="System.Array "/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="System.Array"/> that is the destination of the elements
        /// copied from collection. The <see cref="System.Array"/> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0
        /// or the number of elements in the source collection is greater than the available space from <paramref name="arrayIndex"/>
        /// to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(DateTime[] array, int arrayIndex)
        {
            this.ReadPreamble();
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if ((arrayIndex < 0) || ((arrayIndex + this.m_Collection.Count) > array.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            this.m_Collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific date from the collection.
        /// </summary>
        /// <param name="item">The date to remove from the collection.</param>
        /// <returns><c>true</c> if item was successfully removed from the collection; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if item is not found in the original collection.</returns>
        public bool Remove(DateTime item)
        {
            if (this.IsFrozen)
            {
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
            }

            int oldCount = this.Count;

            this.WritePreamble();
            int index = this.IndexOf(item);
            if (index >= 0)
            {
                this.m_Collection.RemoveAt(index);
                this.m_Version++;
                this.WritePostscript();
                Debug.Assert(this.Count == oldCount - 1, "this.Count == oldCount - 1");
                return true;
            }

            Debug.Assert(this.Count <= oldCount, "this.Count <= oldCount");
            return false;
        }

        #endregion ICollection<DateTime> implementation

        #region IEnumerable<DateTime> implementation

        /// <inheritdoc/>
        IEnumerator<DateTime> IEnumerable<DateTime>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable<DateTime> implementation

        #region IEnumerable implementation

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable implementation

        #region Override methods

        /// <summary>
        /// Calls base <see cref="Freezable.Clone"/> method.
        ///
        /// <remarks>Created to prevent type casting.</remarks>
        /// </summary>
        /// <returns><see cref="DateTimeCollection"/>.</returns>
        public new DateTimeCollection Clone()
        {
            return (DateTimeCollection)base.Clone();
        }

        /// <summary>
        /// Calls base <see cref="Freezable.CloneCurrentValue"/> method.
        ///
        /// <remarks>Created to prevent type casting.</remarks>
        /// </summary>
        /// <returns><see cref="DateTimeCollection"/>.</returns>
        public new DateTimeCollection CloneCurrentValue()
        {
            return (DateTimeCollection)base.CloneCurrentValue();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            this.ReadPreamble();

            return DateTimeCollectionConverter.ConvertToStringCore(
                this.m_Collection,
                DateTimeCollectionConverter.DefaultFormatString,
                null,
                DateTimeCollectionConverter.StringItemsSeparator);
        }

        /// <summary>
        /// Makes the instance a clone (deep copy) of the specified <see cref="System.Windows.Freezable"/>
        /// using base (non-animated) property values.
        /// </summary>
        /// <param name="sourceFreezable">The object to clone (<see cref="DateTimeCollection"/>).</param>
        protected override void CloneCore(Freezable sourceFreezable)
        {
            if (sourceFreezable == null)
            {
                throw new ArgumentNullException(nameof(sourceFreezable));
            }

            var doubles = (DateTimeCollection)sourceFreezable;
            base.CloneCore(sourceFreezable);

            Debug.Assert(doubles.m_Collection != null, "doubles.m_Collection != null");
            int count = doubles.m_Collection.Count;
            this.m_Collection = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                this.m_Collection.Add(doubles.m_Collection[i]);
            }
        }

        /// <summary>
        /// Makes the instance a modifiable clone (deep copy) of the specified <see cref="System.Windows.Freezable"/>
        /// using current property values.
        /// </summary>
        /// <param name="sourceFreezable">The <see cref="System.Windows.Freezable"/> to be cloned (<see cref="DateTimeCollection"/>).</param>
        protected override void CloneCurrentValueCore(Freezable sourceFreezable)
        {
            if (sourceFreezable == null)
            {
                throw new ArgumentNullException(nameof(sourceFreezable));
            }

            var doubles = (DateTimeCollection)sourceFreezable;
            base.CloneCurrentValueCore(sourceFreezable);

            Debug.Assert(doubles.m_Collection != null, "doubles.m_Collection != null");
            int count = doubles.m_Collection.Count;
            this.m_Collection = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                this.m_Collection.Add(doubles.m_Collection[i]);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="DateTimeCollection"/>.
        /// </summary>
        /// <returns>New empty instance of <see cref="DateTimeCollection"/>.</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new DateTimeCollection();
        }

        /// <summary>
        /// Makes the instance a frozen clone of the specified <see cref="System.Windows.Freezable"/>
        /// using base (non-animated) property values.
        /// </summary>
        /// <param name="sourceFreezable">The instance to copy.</param>
        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            if (sourceFreezable == null)
            {
                throw new ArgumentNullException(nameof(sourceFreezable));
            }

            var doubles = (DateTimeCollection)sourceFreezable;
            base.GetAsFrozenCore(sourceFreezable);

            Debug.Assert(doubles.m_Collection != null, "doubles.m_Collection != null");
            int count = doubles.m_Collection.Count;
            this.m_Collection = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                this.m_Collection.Add(doubles.m_Collection[i]);
            }
        }

        /// <summary>
        /// Makes the current instance a frozen clone of the specified <see cref="System.Windows.Freezable"/>.
        /// If the object has animated dependency properties, their current animated values are copied.
        /// </summary>
        /// <param name="sourceFreezable">The <see cref="System.Windows.Freezable"/> to copy and freeze.</param>
        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            if (sourceFreezable == null)
            {
                throw new ArgumentNullException(nameof(sourceFreezable));
            }

            var doubles = (DateTimeCollection)sourceFreezable;
            base.GetCurrentValueAsFrozenCore(sourceFreezable);

            Debug.Assert(doubles.m_Collection != null, "doubles.m_Collection != null");
            int count = doubles.m_Collection.Count;
            this.m_Collection = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                this.m_Collection.Add(doubles.m_Collection[i]);
            }
        }

        #endregion Override methods

        private static DateTime Cast(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(value is DateTime))
            {
                throw new ArgumentException(LocalizationResource.CollectionBadType, nameof(value));
            }

            return (DateTime)value;
        }

        private int AddHelper(DateTime value)
        {
            Debug.Assert(!this.IsFrozen, "!this.IsFrozen");

            var oldCount = this.Count;

            int num = this.AddWithoutFiringPublicEvents(value);
            this.WritePostscript();

            Debug.Assert(num == this.Count - 1, "num == this.Count - 1");
            Debug.Assert(this.Count == oldCount + 1, "this.Count == oldCount + 1");

            return num;
        }

        private int AddWithoutFiringPublicEvents(DateTime value)
        {
            Debug.Assert(!this.IsFrozen, "!this.IsFrozen");

            var oldCount = this.Count;

            this.WritePreamble();
            this.m_Collection.Add(value);
            int num = this.m_Collection.Count - 1;
            this.m_Version++;

            Debug.Assert(num == this.Count - 1, "num == this.Count - 1");
            Debug.Assert(this.Count == oldCount + 1, "this.Count == oldCount + 1");

            return num;
        }

        private void RemoveAtWithoutFiringPublicEvents(int index)
        {
            Debug.Assert(index >= 0, "index >= 0");
            Debug.Assert(index < this.Count, "index < this.Count");
            Debug.Assert(!this.IsFrozen, "!this.IsFrozen");

            var oldCollCount = this.m_Collection.Count;

            this.WritePreamble();
            Debug.Assert(index < this.m_Collection.Count, "index < this.m_Collection.Count");
            this.m_Collection.RemoveAt(index);
            this.m_Version++;

            Debug.Assert(this.m_Collection.Count == oldCollCount - 1, "this.m_Collection.Count == oldCollCount - 1");
        }

        /// <summary>
        /// Enumerates <see cref="DateTime"/> items in a <see cref="DateTimeCollection"/>.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<DateTime>
        {
#pragma warning disable SA1308 // Variable names should not be prefixed

            private readonly DateTimeCollection m_List;
            private readonly uint m_Version;

            private DateTime m_Current;
            private int m_Index;

#pragma warning restore SA1308 // Variable names should not be prefixed

            /// <summary>
            /// Initializes a new instance of the <see cref="Enumerator"/> struct.
            /// </summary>
            /// <param name="list">Source for enumerator.</param>
            internal Enumerator(DateTimeCollection list)
            {
                Debug.Assert(list != null, "list != null");

                this.m_List = list;
                this.m_Version = list.m_Version;
                this.m_Index = -1;
                this.m_Current = DateTime.MinValue;
            }

            /// <inheritdoc/>
            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            /// <summary>
            /// Gets the current element in the collection.
            /// </summary>
            /// <value>The current element in the collection.</value>
            public DateTime Current
            {
                get
                {
                    if (this.m_Index > -1)
                    {
                        return this.m_Current;
                    }

                    if (this.m_Index == -1)
                    {
                        throw new InvalidOperationException(LocalizationResource.EnumeratorNotStarted);
                    }

                    throw new InvalidOperationException(LocalizationResource.EnumeratorReachedEnd);
                }
            }

            /// <summary>
            /// Disposes resources.
            /// </summary>
            public void Dispose()
            {
            }

            /// <summary>
            /// Advances the enumerator to the next element in the collection.
            /// </summary>
            /// <returns><c>true</c> if the enumerator successfully advanced to the next element; otherwise, <c>false</c>.</returns>
            public bool MoveNext()
            {
                this.m_List.ReadPreamble();
                if (this.m_Version != this.m_List.m_Version)
                {
                    throw new InvalidOperationException(LocalizationResource.EnumeratorCollectionChanged);
                }

                Debug.Assert(this.m_List.m_Collection != null, "this.m_List.m_Collection != null");

                if ((this.m_Index > -2) && (this.m_Index < (this.m_List.m_Collection.Count - 1)))
                {
                    this.m_Current = this.m_List.m_Collection[++this.m_Index];
                    return true;
                }

                this.m_Index = -2;
                return false;
            }

            /// <summary>
            /// Resets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                this.m_List.ReadPreamble();
                if (this.m_Version != this.m_List.m_Version)
                {
                    throw new InvalidOperationException(LocalizationResource.EnumeratorCollectionChanged);
                }

                this.m_Index = -1;
            }
        }
    }
}