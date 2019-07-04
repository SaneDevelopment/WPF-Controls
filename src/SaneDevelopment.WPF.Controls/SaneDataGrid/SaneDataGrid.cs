// -----------------------------------------------------------------------
// <copyright file="SaneDataGrid.cs" company="Sane Development">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

#pragma warning disable CA1501 // Avoid excessive inheritance

    /// <summary>
    /// Control behaves absolutely like a <see cref="DataGrid"/>,
    /// but never "swallow" (handles) <see cref="UIElement.MouseWheelEvent"/> event (as <see cref="DataGrid"/> does).
    ///
    /// <remarks>See details in comment of Kevin Stumpf to http://blog.ramondeklein.nl/index.php/2009/07/24/scrollviewer-always-handles-the-mousewheel/.</remarks>
    /// </summary>
    public class SaneDataGrid : DataGrid
    {
        static SaneDataGrid()
        {
            EventManager.RegisterClassHandler(
                typeof(SaneDataGrid),
                Mouse.MouseWheelEvent,
                new MouseWheelEventHandler((sender, args) => { args.Handled = false; }),
                true);
        }
    }

#pragma warning restore CA1501 // Avoid excessive inheritance
}
