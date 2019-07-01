// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeCollection.cs" company="Sane Development">
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using SaneDevelopment.WPF.Controls.Properties;

namespace SaneDevelopment.WPF.Controls
{
    /// <summary>
    /// Provides serialization of <see cref="DateTimeCollection"/> from and to string
    /// </summary>
    public class DateTimeCollectionValueSerializer : ValueSerializer
    {
        /// <summary>
        /// Determines whether the specified <c>string</c> can be converted to an instance of the <see cref="DateTimeCollection"/>
        /// </summary>
        /// <param name="value">The string to evaluate for conversion.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns>Always <c>true</c></returns>
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        /// <summary>
        /// Determines whether the specified object can be converted into a <c>string</c>
        /// </summary>
        /// <param name="value">The object to evaluate for conversion.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns><c>true</c>, if <paramref name="value"/> is instance of <see cref="DateTimeCollection"/>; otherwise <c>false</c></returns>
        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return (value is DateTimeCollection);
        }

        /// <summary>
        /// Converts a <c>string</c> to an instance of the <see cref="DateTimeCollection"/>
        /// or raises <see cref="NotSupportedException"/> or <see cref="FormatException"/>, if conversion failed.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns><see cref="DateTimeCollection"/></returns>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            return DateTimeCollectionConverter.ParseImpl(
                value,
                DateTimeCollectionConverter.DefaultFormatString,
                DateTimeCollectionConverter.StringItemsSeparator);
        }

        /// <summary>
        /// Converts the specified object to a <c>string</c>
        /// </summary>
        /// <param name="value"><see cref="DateTimeCollection"/></param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns>A string representation of the specified object.</returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            var dates = value as DateTimeCollection;
            if (dates == null)
            {
                return base.ConvertToString(value, context);
            }

            return DateTimeCollectionConverter.ConvertToStringImpl(
                dates,
                DateTimeCollectionConverter.DefaultFormatString,
                CultureInfo.InvariantCulture,
                DateTimeCollectionConverter.StringItemsSeparator);
        }
    }

    /// <summary>
    /// Provides a unified way of converting <see cref="DateTimeCollection"/> to other types
    /// (only <c>string</c> for now)
    /// </summary>
    public sealed class DateTimeCollectionConverter : TypeConverter
    {
        /// <summary>
        /// Default format string while convert <see cref="DateTimeCollection"/> to <c>string</c>
        /// </summary>
        public const string DefaultFormatString = "d-M-yyyy H:m:s";

        /// <summary>
        /// Default items separator in string representation of <see cref="DateTimeCollection"/>
        /// </summary>
        public const char StringItemsSeparator = ',';


        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of <see cref="DateTimeCollection"/>,
        /// using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="System.Type"/> that represents the type you want to convert from.</param>
        /// <returns><c>true</c>, if <paramref name="sourceType"/> is <c>string</c>; otherwise, <c>false</c></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="System.Type"/> that represents the type you want to convert to.</param>
        /// <returns><c>true</c>, if <paramref name="destinationType"/> is <c>string</c>; otherwise, <c>false</c></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
        }

        /// <summary>
        /// Converts the given object to the <see cref="DateTimeCollection"/>, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <c>string</c> to convert.</param>
        /// <returns><see cref="DateTimeCollection"/></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw GetConvertFromException(null);
            }
            var source = value as string;
            if (source != null)
            {
                return ParseImpl(source, DefaultFormatString, StringItemsSeparator);
            }
            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="System.Globalization.CultureInfo"/>. If <c>null</c> is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="System.Type"/> to convert the value parameter to.</param>
        /// <returns>An <see cref="System.Object"/> that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var dates = value as DateTimeCollection;
            if ((dates != null) && destinationType == typeof(string))
            {
                return ConvertToStringImpl(dates, DefaultFormatString, culture, StringItemsSeparator);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        internal static string ConvertToStringImpl(
            IEnumerable<DateTime> dates,
            string itemFormat,
            IFormatProvider provider,
            char delimiter)
        {
            if (itemFormat == null)
                throw new ArgumentNullException(nameof(itemFormat));

            Debug.Assert(dates != null);

            var datesCopy = dates.ToList();

            if (datesCopy.Count == 0)
            {
                return string.Empty;
            }

            var format = "{0:" + itemFormat + "}";

            var builder = new StringBuilder();
            for (int i = 0; i < datesCopy.Count; i++)
            {
                builder.AppendFormat(provider, format, new object[] { datesCopy[i] });
                if (i != (datesCopy.Count - 1))
                {
                    builder.Append(delimiter);
                }
            }
            datesCopy.Clear();

            return builder.ToString();
        }

        internal static DateTimeCollection ParseImpl(string source, string itemFormat, char separator)
        {
            if (itemFormat == null)
                throw new ArgumentNullException(nameof(itemFormat));

            var res = new DateTimeCollection();
            if (!string.IsNullOrWhiteSpace(source))
            {
                foreach (var str in source.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var strToParse = str.Trim();
                    if (!string.IsNullOrEmpty(strToParse))
                    {
                        DateTime dt = DateTime.ParseExact(strToParse, itemFormat, CultureInfo.InvariantCulture);
                        res.Add(dt);
                    }
                }
            }
            return res;
        }
    }

    /// <summary>
    /// Represents a collection of <see cref="DateTime"/> values.
    /// </summary>
    [ValueSerializer(typeof(DateTimeCollectionValueSerializer))]
    [TypeConverter(typeof(DateTimeCollectionConverter))]
    public sealed class DateTimeCollection
        : Freezable, IFormattable, IList, IList<DateTime>
    {
        #region Private fields

        private List<DateTime> m_Collection;
        private uint m_Version;

        private static readonly Lazy<DateTimeCollection> s_Empty = new Lazy<DateTimeCollection>(() =>
            {
                var doubles = new DateTimeCollection();
                doubles.Freeze();
                return doubles;
            });

        #endregion Private fields

        /// <summary>
        /// Initializes a new instance of a <see cref="DateTimeCollection"/>
        /// </summary>
        public DateTimeCollection()
        {
            this.m_Collection = new List<DateTime>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeCollection"/> class with the specified collection of <see cref="DateTime"/> values.
        /// </summary>
        /// <param name="collection">The collection of <see cref="DateTime"/> values that make up the <see cref="DateTimeCollection"/>.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public DateTimeCollection(IEnumerable<DateTime> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            Debug.Assert(!this.IsFrozen);

            this.WritePreamble();
            var is3 = collection as ICollection<DateTime>;
            if (is3 != null)
            {
                this.m_Collection = new List<DateTime>(is3);
            }
            else
            {
                var is2 = collection as ICollection;
                if (is2 != null)
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
                throw new ArgumentOutOfRangeException(nameof(capacity));

            this.m_Collection = new List<DateTime>(capacity);
        }

        /// <summary>
        /// Frozen empty <see cref="DateTimeCollection"/>
        /// </summary>
        public static DateTimeCollection Empty
        {
            get
            {
                return s_Empty.Value;
            }
        }

        #region Override methods

        /// <summary>
        /// Makes the instance a clone (deep copy) of the specified <see cref="System.Windows.Freezable"/>
        /// using base (non-animated) property values.
        /// </summary>
        /// <param name="source">The object to clone (<see cref="DateTimeCollection"/>)</param>
        protected override void CloneCore(Freezable source)
        {
            Debug.Assert(source != null);

            var doubles = (DateTimeCollection)source;
            base.CloneCore(source);

            Debug.Assert(doubles.m_Collection != null);
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
        /// <param name="source">The <see cref="System.Windows.Freezable"/> to be cloned (<see cref="DateTimeCollection"/>)</param>
        protected override void CloneCurrentValueCore(Freezable source)
        {
            Debug.Assert(source != null);

            var doubles = (DateTimeCollection)source;
            base.CloneCurrentValueCore(source);

            Debug.Assert(doubles.m_Collection != null);
            int count = doubles.m_Collection.Count;
            this.m_Collection = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                this.m_Collection.Add(doubles.m_Collection[i]);
            }
        }

        /// <summary>
        /// Creates new instance of <see cref="DateTimeCollection"/>
        /// </summary>
        /// <returns>New empty instance of <see cref="DateTimeCollection"/></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new DateTimeCollection();
        }

        /// <summary>
        /// Makes the instance a frozen clone of the specified <see cref="System.Windows.Freezable"/>
        /// using base (non-animated) property values.
        /// </summary>
        /// <param name="source">The instance to copy.</param>
        protected override void GetAsFrozenCore(Freezable source)
        {
            Debug.Assert(source != null);

            var doubles = (DateTimeCollection)source;
            base.GetAsFrozenCore(source);

            Debug.Assert(doubles.m_Collection != null);
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
        /// <param name="source">The <see cref="System.Windows.Freezable"/> to copy and freeze.</param>
        protected override void GetCurrentValueAsFrozenCore(Freezable source)
        {
            var doubles = (DateTimeCollection)source;
            base.GetCurrentValueAsFrozenCore(source);

            Debug.Assert(doubles.m_Collection != null);
            int count = doubles.m_Collection.Count;
            this.m_Collection = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                this.m_Collection.Add(doubles.m_Collection[i]);
            }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            this.ReadPreamble();

            return DateTimeCollectionConverter.ConvertToStringImpl(
                this.m_Collection,
                DateTimeCollectionConverter.DefaultFormatString,
                null,
                DateTimeCollectionConverter.StringItemsSeparator);
        }

        #endregion Override methods

        #region IFormattable implementation

        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            if (format == null)
                throw new ArgumentNullException(nameof(format));

            this.ReadPreamble();

            return DateTimeCollectionConverter.ConvertToStringImpl(
                this.m_Collection,
                format,
                provider,
                DateTimeCollectionConverter.StringItemsSeparator);
        }

        #endregion IFormattable implementation

        #region IList implementation

        bool IList.IsFixedSize
        {
            get
            {
                this.ReadPreamble();
                return this.IsFrozen;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return ((ICollection<DateTime>)this).IsReadOnly;
            }
        }

        object IList.this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return this[index];
            }
            set
            {
                if (this.IsFrozen)
                    throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (value == null)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (!(value is DateTime))
                    throw new ArgumentException(LocalizationResource.CollectionBadType, nameof(value));

                this[index] = Cast(value);
            }
        }

        int IList.Add(object value)
        {
            if (this.IsFrozen)
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
            if (value == null)
                throw new ArgumentOutOfRangeException(nameof(value));
            if (!(value is DateTime))
                throw new ArgumentException(LocalizationResource.CollectionBadType, nameof(value));

            return this.AddHelper(Cast(value));
        }

        bool IList.Contains(object value)
        {
            return ((value is DateTime) && this.Contains((DateTime)value));
        }

        int IList.IndexOf(object value)
        {
            if (value is DateTime)
            {
                return this.IndexOf((DateTime)value);
            }
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            if (this.IsFrozen)
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
            if (index < 0 || index > this.Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (value == null)
                throw new ArgumentOutOfRangeException(nameof(value));
            if (!(value is DateTime))
                throw new ArgumentException(LocalizationResource.CollectionBadType, nameof(value));

            this.Insert(index, Cast(value));
        }

        void IList.Remove(object value)
        {
            if (value is DateTime)
            {
                this.Remove((DateTime)value);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "oldCount")]
        void IList.RemoveAt(int index)
        {
            if (this.IsFrozen)
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);

            var oldCount = this.Count;

            this.RemoveAtWithoutFiringPublicEvents(index);
            this.WritePostscript();

            Debug.Assert(this.Count == oldCount - 1);
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

        /// <summary>
        /// Gets the number of elements actually contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                this.ReadPreamble();
                return this.m_Collection.Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                this.ReadPreamble();
                return this.IsFrozen || this.Dispatcher != null;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                this.ReadPreamble();
                return this;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.ReadPreamble();
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if ((index < 0) || ((index + this.m_Collection.Count) > array.Length))
            {
                throw new ArgumentOutOfRangeException("index");
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
                    Debug.Assert(0 < array.Rank);
                    Debug.Assert(index + i <= array.GetUpperBound(0));
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

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns><see cref="DateTime"/></returns>
        public DateTime this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                this.ReadPreamble();
                return this.m_Collection[index];
            }
            set
            {
                if (this.IsFrozen)
                    throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
                if (index < 0 || index >= this.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                this.WritePreamble();
                this.m_Collection[index] = value;
                this.m_Version++;
                this.WritePostscript();
            }
        }

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
        /// <param name="item">The <see cref="DateTime"/> to insert</param>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "oldCount")]
        public void Insert(int index, DateTime item)
        {
            if (this.IsFrozen)
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);
            if (index < 0 || index > this.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

// ReSharper disable RedundantAssignment
            var oldCount = this.Count;
// ReSharper restore RedundantAssignment

            this.WritePreamble();
            Debug.Assert(index <= this.m_Collection.Count);
            this.m_Collection.Insert(index, item);
            this.m_Version++;
            this.WritePostscript();
            Debug.Assert(this.Count == oldCount + 1);
        }

        #endregion IList<DateTime> implementation

        #region ICollection<DateTime> implementation

        bool ICollection<DateTime>.IsReadOnly
        {
            get
            {
                this.ReadPreamble();
                return this.IsFrozen;
            }
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="item">The <see cref="DateTime"/> to add to the collection.</param>
        public void Add(DateTime item)
        {
            if (this.IsFrozen)
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);

            this.AddHelper(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            if (this.IsFrozen)
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);

            this.WritePreamble();
            this.m_Collection.Clear();
            this.m_Version++;
            this.WritePostscript();

            Debug.Assert(((ICollection)this).IsSynchronized || this.Count == 0);
            Debug.Assert(this.Count == 0);
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

            Debug.Assert(!res || this.Count > 0);

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
                throw new ArgumentNullException("array");
            }
            if ((arrayIndex < 0) || ((arrayIndex + this.m_Collection.Count) > array.Length))
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            this.m_Collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific date from the collection.
        /// </summary>
        /// <param name="item">The date to remove from the collection.</param>
        /// <returns><c>true</c> if item was successfully removed from the collection; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if item is not found in the original collection.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "oldCount")]
        public bool Remove(DateTime item)
        {
            if (this.IsFrozen)
                throw new NotSupportedException(LocalizationResource.CollectionIsFrozen);

// ReSharper disable RedundantAssignment
            int oldCount = this.Count;
// ReSharper restore RedundantAssignment

            this.WritePreamble();
            int index = this.IndexOf(item);
            if (index >= 0)
            {
                this.m_Collection.RemoveAt(index);
                this.m_Version++;
                this.WritePostscript();
                Debug.Assert(this.Count == oldCount - 1);
                return true;
            }
            Debug.Assert(this.Count <= oldCount);
            return false;
        }

        #endregion ICollection<DateTime> implementation

        #region IEnumerable<DateTime> implementation

        IEnumerator<DateTime> IEnumerable<DateTime>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable<DateTime> implementation

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion IEnumerable implementation

        private int AddHelper(DateTime value)
        {
            Debug.Assert(!this.IsFrozen);

            var oldCount = this.Count;

            int num = this.AddWithoutFiringPublicEvents(value);
            this.WritePostscript();

            Debug.Assert(num == this.Count - 1);
            Debug.Assert(this.Count == oldCount + 1);

            return num;
        }

        private int AddWithoutFiringPublicEvents(DateTime value)
        {
            Debug.Assert(!this.IsFrozen);

            var oldCount = this.Count;

            this.WritePreamble();
            this.m_Collection.Add(value);
            int num = this.m_Collection.Count - 1;
            this.m_Version++;

            Debug.Assert(num == this.Count - 1);
            Debug.Assert(this.Count == oldCount + 1);

            return num;
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "oldCollCount")]
        private void RemoveAtWithoutFiringPublicEvents(int index)
        {
            Debug.Assert(index >= 0);
            Debug.Assert(index < this.Count);
            Debug.Assert(!this.IsFrozen);

            var oldCollCount = this.m_Collection.Count;

            this.WritePreamble();
            Debug.Assert(index < this.m_Collection.Count);
            this.m_Collection.RemoveAt(index);
            this.m_Version++;

            Debug.Assert(this.m_Collection.Count == oldCollCount - 1);
        }

        private static DateTime Cast(object value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (!(value is DateTime))
                throw new ArgumentException(LocalizationResource.CollectionBadType, nameof(value));

            return (DateTime)value;
        }

        /// <summary>
        /// Calls base <see cref="Freezable.Clone"/> method.
        /// 
        /// <remarks>Created to prevent type casting.</remarks>
        /// </summary>
        /// <returns><see cref="DateTimeCollection"/></returns>
        public new DateTimeCollection Clone()
        {
            return (DateTimeCollection)base.Clone();
        }

        /// <summary>
        /// Calls base <see cref="Freezable.CloneCurrentValue"/> method.
        /// 
        /// <remarks>Created to prevent type casting.</remarks>
        /// </summary>
        /// <returns><see cref="DateTimeCollection"/></returns>
        public new DateTimeCollection CloneCurrentValue()
        {
            return (DateTimeCollection)base.CloneCurrentValue();
        }

        /// <summary>
        /// Converts a <c>string</c> representation of a collection of dates into an equivalent <see cref="DateTimeCollection"/>.
        /// </summary>
        /// <param name="source">The <c>string</c> representation of the collection of dates.</param>
        /// <param name="itemFormat">Format string for <see cref="DateTime"/> item in collection</param>
        /// <param name="separator">Separator (delimiter) for items in string</param>
        /// <returns>Returns the equivalent <see cref="DateTimeCollection"/>.</returns>
        public static DateTimeCollection Parse(
            string source,
            string itemFormat = DateTimeCollectionConverter.DefaultFormatString,
            char separator = DateTimeCollectionConverter.StringItemsSeparator)
        {
            if (itemFormat == null)
                throw new ArgumentNullException(nameof(itemFormat));

            return DateTimeCollectionConverter.ParseImpl(source, itemFormat, separator);
        }

        /// <summary>
        /// Creates a <c>string</c> representation of this collection.
        /// </summary>
        /// <param name="provider">Culture-specific formatting information.</param>
        /// <returns>Returns a <see cref="System.String"/> containing the values of this collection.</returns>
        public string ToString(IFormatProvider provider)
        {
            this.ReadPreamble();

            return DateTimeCollectionConverter.ConvertToStringImpl(
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

        /// <summary>
        /// Enumerates <see cref="DateTime"/> items in a <see cref="DateTimeCollection"/>.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<DateTime>
        {
            private DateTime m_Current;
            private readonly DateTimeCollection m_List;
            private readonly uint m_Version;
            private int m_Index;

            internal Enumerator(DateTimeCollection list)
            {
                Debug.Assert(list != null);

                this.m_List = list;
                this.m_Version = list.m_Version;
                this.m_Index = -1;
                this.m_Current = DateTime.MinValue;
            }

            /// <summary>
            /// Disposes resources
            /// </summary>
            public void Dispose()
            { }

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

                Debug.Assert(this.m_List.m_Collection != null);

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
        }
    }
}