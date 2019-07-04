// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Sane Development">
//
// Sane Development WPF Controls Library Samples.
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

namespace SaneDevelopment.WPF.Controls.Samples
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using SaneDevelopment.WPF.Controls.Interactivity;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
#pragma warning disable CA1501 // Avoid excessive inheritance
    public partial class MainWindow : Window
#pragma warning restore CA1501 // Avoid excessive inheritance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
        }
    }
}
