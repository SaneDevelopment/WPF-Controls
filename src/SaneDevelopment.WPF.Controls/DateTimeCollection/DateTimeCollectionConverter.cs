// -----------------------------------------------------------------------
// <copyright file="DateTimeCollectionConverter.cs" company="Sane Development">
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Provides a unified way of converting <see cref="DateTimeCollection"/> to other types
    /// (only <c>string</c> for now).
    /// </summary>
    public sealed class DateTimeCollectionConverter : TypeConverter
    {
        /// <summary>
        /// Default format string while convert <see cref="DateTimeCollection"/> to <c>string</c>.
        /// </summary>
        public const string DefaultFormatString = "d-M-yyyy H:m:s";

        /// <summary>
        /// Default items separator in string representation of <see cref="DateTimeCollection"/>.
        /// </summary>
        public const char StringItemsSeparator = ',';


        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of <see cref="DateTimeCollection"/>,
        /// using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="System.Type"/> that represents the type you want to convert from.</param>
        /// <returns><c>true</c>, if <paramref name="sourceType"/> is <c>string</c>; otherwise, <c>false</c>.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="System.Type"/> that represents the type you want to convert to.</param>
        /// <returns><c>true</c>, if <paramref name="destinationType"/> is <c>string</c>; otherwise, <c>false</c>.</returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string)) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the <see cref="DateTimeCollection"/>, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <c>string</c> to convert.</param>
        /// <returns><see cref="DateTimeCollection"/>.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                throw this.GetConvertFromException(null);
            }

            if (value is string source)
            {
                return ParseCore(source, DefaultFormatString, StringItemsSeparator);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="System.Globalization.CultureInfo"/>. If <c>null</c> is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="System.Type"/> to convert the value parameter to.</param>
        /// <returns>An <see cref="object"/> that represents the converted value.</returns>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if ((value is DateTimeCollection dates) && destinationType == typeof(string))
            {
                return ConvertToStringCore(dates, DefaultFormatString, culture, StringItemsSeparator);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <summary>
        /// Convert dates to string.
        /// </summary>
        /// <param name="dates">Dates to convert.</param>
        /// <param name="itemFormat">Item format string.</param>
        /// <param name="provider">Item format provider.</param>
        /// <param name="delimiter">Items delimiter.</param>
        /// <returns>String representation of <paramref name="dates"/>.</returns>
        internal static string ConvertToStringCore(
            IEnumerable<DateTime> dates,
            string itemFormat,
            IFormatProvider provider,
            char delimiter)
        {
            if (itemFormat == null)
            {
                throw new ArgumentNullException(nameof(itemFormat));
            }

            Debug.Assert(dates != null, "dates != null");

            var datesCopy = dates.ToList();

            if (datesCopy.Count == 0)
            {
                return string.Empty;
            }

            var format = "{0:" + itemFormat + "}";

            var builder = new StringBuilder();
            for (int i = 0; i < datesCopy.Count; i++)
            {
                _ = builder.AppendFormat(provider, format, new object[] { datesCopy[i] });
                if (i != (datesCopy.Count - 1))
                {
                    _ = builder.Append(delimiter);
                }
            }

            datesCopy.Clear();

            return builder.ToString();
        }

        /// <summary>
        /// Parse recieved string and return <see cref="DateTimeCollection"/>.
        /// </summary>
        /// <param name="source">Source string.</param>
        /// <param name="itemFormat">Item format string.</param>
        /// <param name="separator">Items delimiter.</param>
        /// <returns><see cref="DateTimeCollection"/>.</returns>
        internal static DateTimeCollection ParseCore(string source, string itemFormat, char separator)
        {
            if (itemFormat == null)
            {
                throw new ArgumentNullException(nameof(itemFormat));
            }

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
}