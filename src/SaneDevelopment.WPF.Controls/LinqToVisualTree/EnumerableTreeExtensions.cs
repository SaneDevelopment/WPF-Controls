// -----------------------------------------------------------------------
// <copyright file="EnumerableTreeExtensions.cs" company="Sane Development">
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
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;

    /// <summary>
    /// See http://www.scottlogic.co.uk/blog/colin/2010/03/linq-to-visual-tree/.
    /// </summary>
    public static class EnumerableTreeExtensions
    {
        /// <summary>
        /// Applies the given function to each of the items in the supplied
        /// IEnumerable, which match the given type.
        /// </summary>
        /// <param name="items">Items to drill down.</param>
        /// <param name="function">Function to apply.</param>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <returns>Items of given type.</returns>
        // [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter"),
        // SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures"),
        // SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DrillDown")]
        public static IEnumerable<DependencyObject> DrillDown<T>(
            this IEnumerable<DependencyObject> items,
            Func<DependencyObject, IEnumerable<DependencyObject>> function)
            where T : DependencyObject
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (function == null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            return items.SelectMany(function).OfType<T>();
        }


        /// <summary>
        /// Returns a collection of descendant elements.
        /// </summary>
        /// <param name="items">Items to work.</param>
        /// <returns>Descendant elements.</returns>
        public static IEnumerable<DependencyObject> Descendants(this IEnumerable<DependencyObject> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown(i => i.Descendants());
        }

        /// <summary>
        /// Returns a collection containing this element and all descendant elements.
        /// </summary>
        /// <param name="items">Items to work.</param>
        /// <returns>This element and all descendant elements.</returns>
        public static IEnumerable<DependencyObject> DescendantsAndSelf(this IEnumerable<DependencyObject> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown(i => i.DescendantsAndSelf());
        }

        /// <summary>
        /// Returns a collection of ancestor elements.
        /// </summary>
        /// <param name="items">Items to work.</param>
        /// <returns>Collection of ancestor elements.</returns>
        public static IEnumerable<DependencyObject> Ancestors(this IEnumerable<DependencyObject> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown(i => i.Ancestors());
        }

        /// <summary>
        /// Returns a collection containing this element and all ancestor elements.
        /// </summary>
        /// <param name="items">Items to work.</param>
        /// <returns>Collection containing this element and all ancestor elements.</returns>
        public static IEnumerable<DependencyObject> AncestorsAndSelf(this IEnumerable<DependencyObject> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown(i => i.AncestorsAndSelf());
        }

        /// <summary>
        /// Returns a collection of child elements.
        /// </summary>
        /// <param name="items">Items to work.</param>
        /// <returns>Collection of child elements.</returns>
        public static IEnumerable<DependencyObject> Elements(this IEnumerable<DependencyObject> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown(i => i.Elements());
        }

        /// <summary>
        /// Returns a collection containing this element and all child elements.
        /// </summary>
        /// <param name="items">Items to work.</param>
        /// <returns>Collection containing this element and all child elements.</returns>
        public static IEnumerable<DependencyObject> ElementsAndSelf(this IEnumerable<DependencyObject> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown(i => i.ElementsAndSelf());
        }


        /// <summary>
        /// Returns a collection of descendant elements which match the given type.
        /// </summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <param name="items">Items to work.</param>
        /// <returns>Collection of descendant elements which match the given type.</returns>
        // [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<DependencyObject> Descendants<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown<T>(i => i.Descendants());
        }

        /// <summary>
        /// Returns a collection containing this element and all descendant elements.
        /// which match the given type.
        /// </summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <param name="items">Items to work.</param>
        /// <returns>Collection containing this element and all descendant elements.
        /// which match the given type.</returns>
        // [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<DependencyObject> DescendantsAndSelf<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown<T>(i => i.DescendantsAndSelf());
        }

        /// <summary>
        /// Returns a collection of ancestor elements which match the given type.
        /// </summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <param name="items">Items to work.</param>
        /// <returns>Collection of ancestor elements which match the given type.</returns>
        // [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<DependencyObject> Ancestors<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown<T>(i => i.Ancestors());
        }

        /// <summary>
        /// Returns a collection containing this element and all ancestor elements.
        /// which match the given type.
        /// </summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <param name="items">Items to work.</param>
        /// <returns>Collection containing this element and all ancestor elements.
        /// which match the given type.</returns>
        // [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<DependencyObject> AncestorsAndSelf<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown<T>(i => i.AncestorsAndSelf());
        }

        /// <summary>
        /// Returns a collection of child elements which match the given type.
        /// </summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <param name="items">Items to work.</param>
        /// <returns>Collection of child elements which match the given type.</returns>
        // [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<DependencyObject> Elements<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown<T>(i => i.Elements());
        }

        /// <summary>
        /// Returns a collection containing this element and all child elements.
        /// which match the given type.
        /// </summary>
        /// <typeparam name="T">Type to match.</typeparam>
        /// <param name="items">Items to work.</param>
        /// <returns>Collection containing this element and all child elements.
        /// which match the given type.</returns>
        // [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public static IEnumerable<DependencyObject> ElementsAndSelf<T>(this IEnumerable<DependencyObject> items)
            where T : DependencyObject
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            return items.DrillDown<T>(i => i.ElementsAndSelf());
        }

        /// <summary>
        /// Applies the given function to each of the items in the supplied
        /// IEnumerable.
        /// </summary>
        private static IEnumerable<DependencyObject> DrillDown(
            this IEnumerable<DependencyObject> items,
            Func<DependencyObject, IEnumerable<DependencyObject>> function)
        {
            Debug.Assert(items != null, "items != null");
            Debug.Assert(function != null, "function != null");

            return items.SelectMany(function);
        }
    }
}
