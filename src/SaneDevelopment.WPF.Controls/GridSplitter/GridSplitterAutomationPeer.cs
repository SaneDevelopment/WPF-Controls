// -----------------------------------------------------------------------
// <copyright file="GridSplitterAutomationPeer.cs" company="Sane Development">
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
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;

#pragma warning disable CA1501 // Avoid excessive inheritance
#pragma warning disable CA1200 // Avoid using cref tags with a prefix
#pragma warning disable SA1202 // Elements should be ordered by access
#pragma warning disable SA1100 // Do not prefix calls with base unless local implementation exists
#pragma warning disable SA1204 // Static elements should appear before instance elements
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1005 // Single line comments should begin with single space
#pragma warning disable SA1609 // Property documentation should have value
#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1623 // Property summary documentation should match accessors
#pragma warning disable SA1306 // Field names should begin with lower-case letter
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable CA1303 // Do not pass literals as localized parameters
#pragma warning disable SA1642 // Constructor summary documentation should begin with standard text
#pragma warning disable CA1033 // Interface methods should be callable by child types

    /// <summary>
    /// Exposes <see cref="T:System.Windows.Controls.GridSplitter" /> types to UI Automation.
    /// </summary>
    [Obsolete("Use System.Windows.Automation.Peers.GridSplitterAutomationPeer instead")]
    public class GridSplitterAutomationPeer : ThumbAutomationPeer, ITransformProvider
    {
                              /// <summary>
                              /// Initializes a new instance of the <see cref="T:System.Windows.Automation.Peers.GridSplitterAutomationPeer" /> class.
                              /// </summary>
                              /// <param name="owner">The <see cref="T:System.Windows.Controls.GridSplitter" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.GridSplitterAutomationPeer" />.</param>
        public GridSplitterAutomationPeer(GridSplitter owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets the name of the <see cref="T:System.Windows.Controls.GridSplitter" /> that is associated with this <see cref="T:System.Windows.Automation.Peers.GridSplitterAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.
        /// </summary>
        /// <returns>A string that contains "GridSplitter".</returns>
        protected override string GetClassNameCore()
        {
            return "GridSplitter";
        }

        /// <summary>
        /// Gets the control pattern for the <see cref="T:System.Windows.Automation.Peers.GridSplitterAutomationPeer" />. This method is called by <see cref="M:System.Windows.Automation.Peers.AutomationPeer.GetClassName" />.
        /// </summary>
        /// <param name="patternInterface">A value in the enumeration.</param>
        /// <returns>If <paramref name="patternInterface" /> is <see cref="F:System.Windows.Automation.Peers.PatternInterface.Transform" />, this method returns a this pointer; otherwise this method returns null.</returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Transform)
            {
                return this;
            }

            return null;
        }

        /// <summary>
        /// This type or member supports the Windows Presentation Foundation (WPF) infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <param name="x">Absolute screen coordinates of the left side of the control.</param>
        /// <param name="y">Absolute screen coordinates of the top of the control.</param>
        void ITransformProvider.Move(double x, double y)
        {
            if (!base.IsEnabled())
            {
                throw new InvalidOperationException("Element not enabled.");
            }

            if (double.IsInfinity(x) || double.IsNaN(x))
            {
                throw new ArgumentOutOfRangeException(nameof(x));
            }

            if (double.IsInfinity(y) || double.IsNaN(y))
            {
                throw new ArgumentOutOfRangeException(nameof(y));
            }

            ((GridSplitter)base.Owner).KeyboardMoveSplitter(x, y);
        }

        /// <summary>
        /// This type or member supports the Windows Presentation Foundation (WPF) infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <param name="width">The new width of the window, in pixels.</param>
        /// <param name="height">The new height of the window, in pixels.</param>
        void ITransformProvider.Resize(double width, double height)
        {
            throw new InvalidOperationException("UIA_OperationCannotBePerformed");
        }

        /// <summary>
        /// This type or member supports the Windows Presentation Foundation (WPF) infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <param name="degrees">The number of degrees to rotate the control.</param>
        void ITransformProvider.Rotate(double degrees)
        {
            throw new InvalidOperationException("UIA_OperationCannotBePerformed");
        }

        /// <summary>
        /// This type or member supports the Windows Presentation Foundation (WPF) infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <returns>true if the element can be moved; otherwise false.</returns>
        bool ITransformProvider.CanMove
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// This type or member supports the Windows Presentation Foundation (WPF) infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <returns>true if the element can be resized; otherwise false.</returns>
        bool ITransformProvider.CanResize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// This type or member supports the Windows Presentation Foundation (WPF) infrastructure and is not intended to be used directly from your code.
        /// </summary>
        /// <returns>true if the element can rotate; otherwise false.</returns>
        bool ITransformProvider.CanRotate
        {
            get
            {
                return false;
            }
        }
    }

#pragma warning restore CA1033 // Interface methods should be callable by child types
#pragma warning restore SA1642 // Constructor summary documentation should begin with standard text
#pragma warning restore CA1303 // Do not pass literals as localized parameters
#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable CA1822 // Mark members as static
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore SA1306 // Field names should begin with lower-case letter
#pragma warning restore SA1623 // Property summary documentation should match accessors
#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1609 // Property documentation should have value
#pragma warning restore SA1005 // Single line comments should begin with single space
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
#pragma warning restore SA1204 // Static elements should appear before instance elements
#pragma warning restore SA1100 // Do not prefix calls with base unless local implementation exists
#pragma warning restore SA1202 // Elements should be ordered by access
#pragma warning restore CA1200 // Avoid using cref tags with a prefix
#pragma warning restore CA1501 // Avoid excessive inheritance
}