// -----------------------------------------------------------------------
// <copyright file="ValidationHelper.cs" company="Sane Development">
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
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using SaneDevelopment.WPF.Controls.LinqToVisualTree;

    /// <summary>
    /// Provides handy helper methods for WPF data validation.
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Check the validity of dependency object.
        /// Object is valid if it has no errors
        /// and all its children (descendants), which are dependency objects, has no errors too
        /// (i.e. is valid recursively).
        ///
        /// <remarks>See details in http://stackoverflow.com/questions/127477/detecting-wpf-validation-errors
        /// and in http://www.scottlogic.co.uk/blog/colin/2010/03/linq-to-visual-tree/</remarks>
        /// </summary>
        /// <param name="dependencyObject">Object to validate.</param>
        /// <returns><c>true</c> if object <paramref name="dependencyObject"/> is valid; otherwise, <c>false</c>.</returns>
        public static bool IsValid(DependencyObject dependencyObject)
        {
            // http://stackoverflow.com/questions/127477/detecting-wpf-validation-errors
            // The dependency object is valid if it has no errors,
            // and all of its children (that are dependency objects) are error-free.
            var hasError = Validation.GetHasError(dependencyObject);
            if (hasError)
            {
                return false;
            }

            var children = dependencyObject.Descendants();
            var allChildrenAreValid = children
                .All(o => !Validation.GetHasError(o));

            return allChildrenAreValid;
        }
    }
}