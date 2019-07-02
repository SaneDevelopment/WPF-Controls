// -----------------------------------------------------------------------
// <copyright file="UncheckableRadioButton.cs" company="Sane Development">
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
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Controls;

#pragma warning disable CA1501 // Avoid excessive inheritance

    /// <summary>
    /// Behaves similar to <see cref="RadioButton"/>,
    /// but every button in group can be "unchecked" so the whole group have no checked item.
    ///
    /// <remarks>See details in http://www.systenics.com/blog/uncheck-radio-button-in-wpf/.</remarks>
    /// </summary>
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1704:IdentifiersShouldBeSpelledCorrectly",
        MessageId = "Uncheckable",
        Justification = "Slang")]
    public class UncheckableRadioButton : RadioButton
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UncheckableRadioButton"/> class.
        /// </summary>
        public UncheckableRadioButton()
        {
            this.Checked += this.UncheckableRadioButton_Checked;
            this.Click += this.UncheckableRadioButton_Click;
        }

        /// <summary>
        /// Gets a value indicating whether radio button was "checked" after last click on it.
        /// </summary>
        /// <value>A value indicating whether radio button was "checked" after last click on it.</value>
        public bool WasChecked { get; private set; }

        #region IsCheckedChanged

#pragma warning disable SA1201 // Elements should appear in the correct order

        /// <summary>
        /// Gets or sets whether value of <see cref="System.Windows.Controls.Primitives.ToggleButton.IsChecked"/> changed.
        /// </summary>
        /// <value>Whether value of <see cref="System.Windows.Controls.Primitives.ToggleButton.IsChecked"/> changed.</value>
        public bool? IsCheckedChanged
        {
            get
            {
                var res = this.GetValue(IsCheckedChangedProperty);
                Debug.Assert(res != null, "res != null");
                return (bool?)res;
            }

            set
            {
                this.SetValue(IsCheckedChangedProperty, value);
            }
        }

                              /// <summary>
                              /// Dependency property for <see cref="UncheckableRadioButton.IsCheckedChanged"/>.
                              /// </summary>
        public static readonly DependencyProperty IsCheckedChangedProperty = DependencyProperty.Register(
            nameof(IsCheckedChanged),
            typeof(bool?),
            typeof(UncheckableRadioButton),
            new FrameworkPropertyMetadata(
                false,
                FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                OnIsCheckedChangedChanged));

        private static void OnIsCheckedChangedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Debug.Assert(d is UncheckableRadioButton, "d is UncheckableRadioButton");
            Debug.Assert(e.NewValue is bool, "e.NewValue is bool");

            ((UncheckableRadioButton)d).IsChecked = (bool)e.NewValue;
        }

#pragma warning restore SA1201 // Elements should appear in the correct order

        #endregion IsCheckedChanged

        private void UncheckableRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WasChecked)
            {
                this.IsCheckedChanged = !this.IsCheckedChanged;
            }

            this.WasChecked = true;
        }

        private void UncheckableRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.IsCheckedChanged = true;
            this.WasChecked = false;
        }
    }

#pragma warning restore CA1501 // Avoid excessive inheritance
}
