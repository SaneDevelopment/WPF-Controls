// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeCollection.cs" company="Sane Development">
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Класс реализует сериализацию коллекции дат в и из строки
    /// </summary>
    public class DateTimeCollectionValueSerializer : ValueSerializer
    {
        /// <summary>
        /// Функция проверяет возможность получения коллекции дат из строки
        /// </summary>
        /// <param name="value">Исходная строка</param>
        /// <param name="context">Контекст сериализации</param>
        /// <returns>Всегда <c>true</c></returns>
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        /// <summary>
        /// Функция проверяет возможность получения строкового представления коллекции дат из входного параметра
        /// </summary>
        /// <param name="value">Входной параметр</param>
        /// <param name="context">Контекст сериализации</param>
        /// <returns><c>true</c>, если параметр <paramref name="value"/> является коллекцией дат; иначе <c>false</c></returns>
        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return (value is DateTimeCollection);
        }

        /// <summary>
        /// Формирует коллекцию дат на основе заданной строки <paramref name="value"/>
        /// либо генерирует <see cref="NotSupportedException"/> или <see cref="FormatException"/>, если это невозможно
        /// </summary>
        /// <param name="value">Исходная строка</param>
        /// <param name="context">Контекст сериализации</param>
        /// <returns>Коллекция дат</returns>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            return DateTimeCollectionConverter.ParseImpl(
                value,
                DateTimeCollectionConverter.DefaultFormatString,
                DateTimeCollectionConverter.StringItemsSeparator);
        }

        /// <summary>
        /// Преобразует коллекцию дат в строку
        /// </summary>
        /// <param name="value">Колллекция дат</param>
        /// <param name="context">Контекст сериализации</param>
        /// <returns>Строковое представление коллекции</returns>
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
    /// Класс реализует конвертацию коллекции дат в и из строки
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
        /// Функция проверяет возможность получения коллекции дат из строки
        /// </summary>
        /// <param name="context">Контекст дескриптора</param>
        /// <param name="sourceType">Тип, из которого предполагается получение коллекции</param>
        /// <returns><c>true</c>, если <paramref name="sourceType"/> является строкой; иначе <c>false</c></returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
        }

        /// <summary>
        /// Функция проверяет возможность получения строкового представления коллекции дат из входного параметра
        /// </summary>
        /// <param name="context">Контекст дескриптора</param>
        /// <param name="destinationType">Тип, в который предполагается конвертация коллекции</param>
        /// <returns><c>true</c>, если <paramref name="destinationType"/> является строкой; иначе <c>false</c></returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(string)) || base.CanConvertTo(context, destinationType));
        }

        /// <summary>
        /// Формирует коллекцию дат на основе заданной строки <paramref name="value"/>
        /// </summary>
        /// <param name="context">Контекст дескриптора</param>
        /// <param name="culture">Используемая культура</param>
        /// <param name="value">Исходная строка</param>
        /// <returns>Коллекция дат</returns>
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
        /// Преобразует коллекцию дат в строку
        /// </summary>
        /// <param name="context">Контекст дескриптора</param>
        /// <param name="culture">Используемая культура</param>
        /// <param name="value">Колллекция дат</param>
        /// <param name="destinationType">Тип, в который предполагается конвертация коллекции</param>
        /// <returns>Строковое представление коллекции</returns>
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
            Contract.Requires<ArgumentNullException>(itemFormat != null);
            Contract.Assume(dates != null);

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
            Contract.Requires<ArgumentNullException>(itemFormat != null);

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
    /// Коллекция дат
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
        /// Инициализирует пустую коллекцию
        /// </summary>
        public DateTimeCollection()
        {
            this.m_Collection = new List<DateTime>();
        }

        /// <summary>
        /// Инициализирует коллекцию на основе заданного перечисления
        /// </summary>
        /// <param name="collection">Исходное перечисление дат</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public DateTimeCollection(IEnumerable<DateTime> collection)
        {
            Contract.Requires<ArgumentNullException>(collection != null);
            Contract.Assume(!this.IsFrozen);

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
        /// Инициализирует коллекцию заданной начальной емкости
        /// </summary>
        /// <param name="capacity">Начальная емкость коллекции</param>
        public DateTimeCollection(int capacity)
        {
            Contract.Requires<ArgumentOutOfRangeException>(capacity >= 0);

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
        /// Выполняет глубокое копирование коллекции
        /// </summary>
        /// <param name="source">Источник</param>
        protected override void CloneCore(Freezable source)
        {
            Contract.Assume(source != null);

            var doubles = (DateTimeCollection)source;
            base.CloneCore(source);
            Contract.Assume(doubles.m_Collection != null);
            int count = doubles.m_Collection.Count;
            this.m_Collection = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                this.m_Collection.Add(doubles.m_Collection[i]);
            }
        }

        /// <summary>
        /// Выполняет глубокое копирование коллекции, делая ее модифицируемой
        /// </summary>
        /// <param name="source">Источник</param>
        protected override void CloneCurrentValueCore(Freezable source)
        {
            Contract.Assume(source != null);

            var doubles = (DateTimeCollection)source;
            base.CloneCurrentValueCore(source);
            Contract.Assume(doubles.m_Collection != null);
            int count = doubles.m_Collection.Count;
            this.m_Collection = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                this.m_Collection.Add(doubles.m_Collection[i]);
            }
        }

        /// <summary>
        /// Новый объект коллекции
        /// </summary>
        /// <returns>Новый объект коллекции</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new DateTimeCollection();
        }

        /// <summary>
        /// Формирует "замороженный" клон коллекции
        /// </summary>
        /// <param name="source">Источник</param>
        protected override void GetAsFrozenCore(Freezable source)
        {
            Contract.Assume(source != null);

            var doubles = (DateTimeCollection)source;
            base.GetAsFrozenCore(source);
            Contract.Assume(doubles.m_Collection != null);
            int count = doubles.m_Collection.Count;
            this.m_Collection = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                this.m_Collection.Add(doubles.m_Collection[i]);
            }
        }

        /// <summary>
        /// Формирует "замороженный" клон коллекции
        /// </summary>
        /// <param name="source">Источник</param>
        protected override void GetCurrentValueAsFrozenCore(Freezable source)
        {
            var doubles = (DateTimeCollection)source;
            base.GetCurrentValueAsFrozenCore(source);
            Contract.Assume(doubles.m_Collection != null);
            int count = doubles.m_Collection.Count;
            this.m_Collection = new List<DateTime>(count);
            for (int i = 0; i < count; i++)
            {
                this.m_Collection.Add(doubles.m_Collection[i]);
            }
        }

        /// <summary>
        /// Преобразует коллекцию в строку
        /// </summary>
        /// <returns>Строковое представление коллекции</returns>
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
            Contract.Assume(format != null);

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
                Contract.Assert(index >= 0);
                Contract.Assert(index < this.Count);

                return this[index];
            }
            set
            {
                Contract.Assume(value != null);
                Contract.Assume(value is DateTime, LocalizedStrings.CollectionBadType);
                Contract.Assert(index >= 0);
                Contract.Assert(index < this.Count);

                this[index] = Cast(value);
            }
        }

        int IList.Add(object value)
        {
            Contract.Assume(!this.IsFrozen);
            Contract.Assume(value != null);
            Contract.Assume(value is DateTime, LocalizedStrings.CollectionBadType);

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
            Contract.Assume(index <= this.Count);
            Contract.Assume(value != null);
            Contract.Assume(value is DateTime, LocalizedStrings.CollectionBadType);

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
            Contract.Ensures(this.Count == Contract.OldValue(this.Count) - 1);
            Contract.Assume(!this.IsFrozen);
// ReSharper disable RedundantAssignment
            var oldCount = this.Count;
// ReSharper restore RedundantAssignment

            this.RemoveAtWithoutFiringPublicEvents(index);
            this.WritePostscript();

            Contract.Assume(this.Count == oldCount - 1);
        }

        /// <summary>
        /// Удаляет из коллекции элемент по заданному индексу
        /// </summary>
        /// <param name="index">Индекс удаляемого элемента</param>
        public void RemoveAt(int index)
        {
            ((IList)this).RemoveAt(index);
        }

        #endregion IList implementation

        #region ICollection implementation

        /// <summary>
        /// Размер коллекции
        /// </summary>
        public int Count
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() >= 0);
                Contract.Ensures(Contract.Result<int>() == this.m_Collection.Count);

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
                throw new ArgumentException(LocalizedStrings.CollectionBadRank);
            }
            int count = this.m_Collection.Count;
            try
            {
                for (int i = 0; i < count; i++)
                {
                    Contract.Assume(0 < array.Rank);
                    Contract.Assume(index + i <= array.GetUpperBound(0));
                    array.SetValue(this.m_Collection[i], index + i);
                }
            }
            catch (InvalidCastException exception)
            {
                throw new ArgumentException(LocalizedStrings.CollectionBadDestArray, exception);
            }
        }

        #endregion ICollection implementation

        #region IList<DateTime> implementation

        /// <summary>
        /// Элемент коллекции по заданному индексу
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <returns>Дата</returns>
        public DateTime this[int index]
        {
            get
            {
                this.ReadPreamble();
                Contract.Assert(index >= 0);
                Contract.Assume(index < this.Count);
                return this.m_Collection[index];
            }
            set
            {
                Contract.Assume(!this.IsFrozen);

                this.WritePreamble();

                Contract.Assert(index >= 0);
                Contract.Assume(index < this.Count);

                this.m_Collection[index] = value;
                this.m_Version++;
                this.WritePostscript();
            }
        }

        /// <summary>
        /// Индекс первого вхождения заданной даты в коллекции
        /// </summary>
        /// <param name="item">Искомая дата</param>
        /// <returns>Индекс найденной даты, начиная с нуля</returns>
        public int IndexOf(DateTime item)
        {
            this.ReadPreamble();
            return this.m_Collection.IndexOf(item);
        }

        /// <summary>
        /// Выставляет дату в заданную позицию
        /// </summary>
        /// <param name="index">Позиция</param>
        /// <param name="item">Дата</param>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "oldCount")]
        public void Insert(int index, DateTime item)
        {
            Contract.Ensures(this.Count == Contract.OldValue(this.Count) + 1);

            Contract.Assert(index >= 0);
            Contract.Assert(index <= this.Count);
            Contract.Assume(!this.IsFrozen);

// ReSharper disable RedundantAssignment
            var oldCount = this.Count;
// ReSharper restore RedundantAssignment

            this.WritePreamble();
            Contract.Assume(index <= this.m_Collection.Count);
            this.m_Collection.Insert(index, item);
            this.m_Version++;
            this.WritePostscript();
            Contract.Assume(this.Count == oldCount + 1);
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
        /// Добавляет дату в конец коллекции
        /// </summary>
        /// <param name="item">Дата</param>
        public void Add(DateTime item)
        {
            Contract.Assume(!this.IsFrozen);

            this.AddHelper(item);
        }

        /// <summary>
        /// Очищает коллекцию
        /// </summary>
        public void Clear()
        {
            Contract.Ensures(this.Count == 0);
            Contract.Ensures(((ICollection)this).IsSynchronized || this.Count == 0);

            Contract.Assume(!this.IsFrozen);

            this.WritePreamble();
            this.m_Collection.Clear();
            this.m_Version++;
            this.WritePostscript();

            Contract.Assume(((ICollection)this).IsSynchronized || this.Count == 0);
            Contract.Assume(this.Count == 0);
        }

        /// <summary>
        /// Ищет в коллекции дату
        /// </summary>
        /// <param name="item">Искомая дата</param>
        /// <returns>Найдена ли заданная дата в коллекции</returns>
        public bool Contains(DateTime item)
        {
            Contract.Ensures(!Contract.Result<bool>() || this.Count > 0);

            this.ReadPreamble();
            return this.m_Collection.Contains(item);
        }

        /// <summary>
        /// Копирует даты из коллекции в массив
        /// </summary>
        /// <param name="array">Массив-приемник</param>
        /// <param name="arrayIndex">Начальный индекс в массиве для копирования</param>
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
        /// Удаляет из коллекции первое вхождение заданной даты
        /// </summary>
        /// <param name="item">Дата</param>
        /// <returns>Был ли реально удален элемент</returns>
        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "oldCount")]
        public bool Remove(DateTime item)
        {
            Contract.Assume(!this.IsFrozen);
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
                Contract.Assume(this.Count == oldCount - 1);
                return true;
            }
            Contract.Assume(this.Count <= oldCount);
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
            Contract.Requires(!this.IsFrozen);
            Contract.Ensures(Contract.Result<int>() == this.Count - 1);
            Contract.Ensures(this.Count == Contract.OldValue(this.Count) + 1);
            Contract.Ensures(this.Count > Contract.OldValue(this.Count));

            int num = this.AddWithoutFiringPublicEvents(value);
            this.WritePostscript();
            Contract.Assume(num == this.Count - 1);
            return num;
        }

        private int AddWithoutFiringPublicEvents(DateTime value)
        {
            Contract.Requires(!this.IsFrozen);
            Contract.Ensures(Contract.Result<int>() == this.Count - 1);
            Contract.Ensures(this.Count == Contract.OldValue(this.Count) + 1);
            Contract.Ensures(this.Count > Contract.OldValue(this.Count));

            this.WritePreamble();
            this.m_Collection.Add(value);
            int num = this.m_Collection.Count - 1;
            this.m_Version++;
            return num;
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "oldCollCount")]
        private void RemoveAtWithoutFiringPublicEvents(int index)
        {
            Contract.Requires(index >= 0);
            Contract.Requires(index < this.Count);
            Contract.Requires(!this.IsFrozen);
            Contract.Ensures(this.m_Collection.Count == Contract.OldValue(this.m_Collection.Count) - 1);

// ReSharper disable RedundantAssignment
            var oldCollCount = this.m_Collection.Count;
// ReSharper restore RedundantAssignment
            this.WritePreamble();
            Contract.Assume(index < this.m_Collection.Count);
            this.m_Collection.RemoveAt(index);
            this.m_Version++;

            Contract.Assume(this.m_Collection.Count == oldCollCount - 1);
        }

        private static DateTime Cast(object value)
        {
            Contract.Requires<ArgumentNullException>(value != null);
            Contract.Requires<ArgumentException>(value is DateTime, LocalizedStrings.CollectionBadType);

            return (DateTime)value;
        }

        /// <summary>
        /// Вызывает одноименный базовый метод
        /// </summary>
        /// <returns>Коллекция дат</returns>
        public new DateTimeCollection Clone()
        {
            return (DateTimeCollection)base.Clone();
        }

        /// <summary>
        /// Вызывает одноименный базовый метод
        /// </summary>
        /// <returns>Коллекция дат</returns>
        public new DateTimeCollection CloneCurrentValue()
        {
            return (DateTimeCollection)base.CloneCurrentValue();
        }

        /// <summary>
        /// Выполняет синтаксический рабор строки <paramref name="source"/>, извлекая коллекцию дат
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <param name="itemFormat">Format string for <see cref="DateTime"/> item in collection</param>
        /// <param name="separator">Separator (delimiter) for items in string</param>
        /// <returns>Коллекция дат</returns>
        public static DateTimeCollection Parse(
            string source,
            string itemFormat = DateTimeCollectionConverter.DefaultFormatString,
            char separator = DateTimeCollectionConverter.StringItemsSeparator)
        {
            Contract.Requires<ArgumentNullException>(itemFormat != null);

            return DateTimeCollectionConverter.ParseImpl(source, itemFormat, separator);
        }

        /// <summary>
        /// Преобразует объект в строку
        /// </summary>
        /// <param name="provider">Формат-провайдер</param>
        /// <returns>Строковое представление коллекции дат</returns>
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
        /// Метод возвращает новый перечислитель коллекции
        /// </summary>
        /// <returns>Новый перечислитель коллекции</returns>
        public Enumerator GetEnumerator()
        {
            this.ReadPreamble();
            return new Enumerator(this);
        }

        /// <summary>
        /// Тип-перечислитель для колекции дат
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
                Contract.Requires(list != null);

                this.m_List = list;
                this.m_Version = list.m_Version;
                this.m_Index = -1;
                this.m_Current = DateTime.MinValue;
            }

            /// <summary>
            /// Выполняет освобождение ресурсов
            /// </summary>
            public void Dispose()
            { }

            /// <summary>
            /// Выполняет переход к следующему элементу последовательности
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                this.m_List.ReadPreamble();
                if (this.m_Version != this.m_List.m_Version)
                {
                    throw new InvalidOperationException(LocalizedStrings.EnumeratorCollectionChanged);
                }
                Contract.Assume(this.m_List.m_Collection != null);
                if ((this.m_Index > -2) && (this.m_Index < (this.m_List.m_Collection.Count - 1)))
                {
                    this.m_Current = this.m_List.m_Collection[++this.m_Index];
                    return true;
                }
                this.m_Index = -2;
                return false;
            }

            /// <summary>
            /// Сбрасывает перечислитель в начальное неинициализированное состояние
            /// </summary>
            public void Reset()
            {
                this.m_List.ReadPreamble();
                if (this.m_Version != this.m_List.m_Version)
                {
                    throw new InvalidOperationException(LocalizedStrings.EnumeratorCollectionChanged);
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
            /// Возвращает текущий элемент последовательности
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
                        throw new InvalidOperationException(LocalizedStrings.EnumeratorNotStarted);
                    }
                    throw new InvalidOperationException(LocalizedStrings.EnumeratorReachedEnd);
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"),
            SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.m_List != null);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(this.m_Collection != null);
            Contract.Invariant(this.m_Collection.Count >= 0);
            Contract.Invariant(this.Count >= 0);
            Contract.Invariant(this.Count == this.m_Collection.Count);
        }
    }
}