// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationHelper.cs" company="Sane Development">
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

using System.Linq;
using System.Windows;
using System.Windows.Controls;

using SaneDevelopment.WPF.Controls.LinqToVisualTree;

namespace SaneDevelopment.WPF.Controls
{
    /// <summary>
    /// Provides handy helper methods for WPF data validation
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
        /// <param name="obj">Object to validate</param>
        /// <returns><c>true</c> if object <paramref name="obj"/> is valid; otherwise, <c>false</c>.</returns>
        public static bool IsValid(DependencyObject obj)
        {
            // http://stackoverflow.com/questions/127477/detecting-wpf-validation-errors
            // The dependency object is valid if it has no errors, 
            // and all of its children (that are dependency objects) are error-free.
            var hasError = Validation.GetHasError(obj);
            if (hasError)
                return false;

            var children = obj.Descendants();
            var allChildrenAreValid = children
                .All(o => !Validation.GetHasError(o));

            return allChildrenAreValid;
        }
    }
}