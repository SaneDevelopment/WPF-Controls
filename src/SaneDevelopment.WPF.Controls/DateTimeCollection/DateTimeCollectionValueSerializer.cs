// -----------------------------------------------------------------------
// <copyright file="DateTimeCollectionValueSerializer.cs" company="Sane Development">
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
    using System.Globalization;
    using System.Windows.Markup;

    /// <summary>
    /// Provides serialization of <see cref="DateTimeCollection"/> from and to string.
    /// </summary>
    public class DateTimeCollectionValueSerializer : ValueSerializer
    {
        /// <summary>
        /// Determines whether the specified <c>string</c> can be converted to an instance of the <see cref="DateTimeCollection"/>.
        /// </summary>
        /// <param name="value">The string to evaluate for conversion.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns>Always <c>true</c>.</returns>
        public override bool CanConvertFromString(string value, IValueSerializerContext context)
        {
            return true;
        }

        /// <summary>
        /// Determines whether the specified object can be converted into a <c>string</c>.
        /// </summary>
        /// <param name="value">The object to evaluate for conversion.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns><c>true</c>, if <paramref name="value"/> is instance of <see cref="DateTimeCollection"/>; otherwise <c>false</c>.</returns>
        public override bool CanConvertToString(object value, IValueSerializerContext context)
        {
            return value is DateTimeCollection;
        }

        /// <summary>
        /// Converts a <c>string</c> to an instance of the <see cref="DateTimeCollection"/>
        /// or raises <see cref="NotSupportedException"/> or <see cref="FormatException"/>, if conversion failed.
        /// </summary>
        /// <param name="value">The string to convert.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns><see cref="DateTimeCollection"/>.</returns>
        public override object ConvertFromString(string value, IValueSerializerContext context)
        {
            return DateTimeCollectionConverter.ParseCore(
                value,
                DateTimeCollectionConverter.DefaultFormatString,
                DateTimeCollectionConverter.StringItemsSeparator);
        }

        /// <summary>
        /// Converts the specified object to a <c>string</c>.
        /// </summary>
        /// <param name="value"><see cref="DateTimeCollection"/>.</param>
        /// <param name="context">Context information that is used for conversion.</param>
        /// <returns>A string representation of the specified object.</returns>
        public override string ConvertToString(object value, IValueSerializerContext context)
        {
            var dates = value as DateTimeCollection;
            if (dates == null)
            {
                return base.ConvertToString(value, context);
            }

            return DateTimeCollectionConverter.ConvertToStringCore(
                dates,
                DateTimeCollectionConverter.DefaultFormatString,
                CultureInfo.InvariantCulture,
                DateTimeCollectionConverter.StringItemsSeparator);
        }
    }
}