// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UncheckableRadioButton.cs" company="Sane Development">
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

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Windows;
using System.Windows.Controls;

namespace SaneDevelopment.WPF.Controls
{
    /// <summary>
    /// Behaves similar to <see cref="RadioButton"/>,
    /// but every button in group can be "unchecked" so the whole group have no checked item.
    /// 
    /// <remarks>See details in http://www.systenics.com/blog/uncheck-radio-button-in-wpf/</remarks>
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uncheckable")]
    public class UncheckableRadioButton : RadioButton
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="UncheckableRadioButton"/>
        /// </summary>
        public UncheckableRadioButton()
        {
            Checked += UncheckableRadioButton_Checked;
            Click += UncheckableRadioButton_Click;
        }

        private void UncheckableRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (WasChecked)
            {
                IsCheckedChanged = !IsCheckedChanged;
            }
            WasChecked = true;
        }

        private void UncheckableRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            IsCheckedChanged = true;
            WasChecked = false;
        }

        /// <summary>
        /// Whether radio button was "checked" after last click on it.
        /// </summary>
        public bool WasChecked { get; private set; }

        /// <summary>
        /// Whether value of <see cref="System.Windows.Controls.Primitives.ToggleButton.IsChecked"/> changed.
        /// </summary>
        public bool? IsCheckedChanged
        {
            get
            {
                var res = GetValue(IsCheckedChangedProperty);
                Contract.Assume(res != null);
                return (bool?) res;
            }
            set { SetValue(IsCheckedChangedProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="UncheckableRadioButton.IsCheckedChanged"/>
        /// </summary>
        public static readonly DependencyProperty IsCheckedChangedProperty = DependencyProperty.Register(
            "IsCheckedChanged",
            typeof(bool?),
            typeof(UncheckableRadioButton),
            new FrameworkPropertyMetadata(false,
                FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnIsCheckedChangedChanged));

        private static void OnIsCheckedChangedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Contract.Requires(d is UncheckableRadioButton);
            Contract.Requires(e.NewValue is bool);

            ((UncheckableRadioButton)d).IsChecked = (bool)e.NewValue;
        }
    }
}
