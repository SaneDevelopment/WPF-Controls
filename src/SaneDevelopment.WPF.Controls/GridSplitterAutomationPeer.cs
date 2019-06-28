// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GridSplitterAutomationPeer.cs" company="Sane Development">
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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;

[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member",
    Target = "SaneDevelopment.WPF.Controls.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.CanMove")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member",
    Target = "SaneDevelopment.WPF.Controls.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.CanResize")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member",
    Target = "SaneDevelopment.WPF.Controls.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.CanRotate")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member",
    Target = "SaneDevelopment.WPF.Controls.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Move(System.Double,System.Double)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member",
    Target = "SaneDevelopment.WPF.Controls.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Resize(System.Double,System.Double)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UIAOperationCannotBePerformed", Scope = "member",
    Target = "SaneDevelopment.WPF.Controls.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Resize(System.Double,System.Double)")]
[assembly: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member",
    Target = "SaneDevelopment.WPF.Controls.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Rotate(System.Double)")]
[assembly: SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "UIAOperationCannotBePerformed", Scope = "member",
    Target = "SaneDevelopment.WPF.Controls.GridSplitterAutomationPeer.#System.Windows.Automation.Provider.ITransformProvider.Rotate(System.Double)")]

namespace SaneDevelopment.WPF.Controls
{
// ReSharper disable RedundantBaseQualifier

    /// <summary>
    /// Exposes <see cref="T:System.Windows.Controls.GridSplitter" /> types to UI Automation.
    /// </summary>
    [System.Diagnostics.Contracts.ContractVerification(false)]
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
                throw new ArgumentOutOfRangeException("x");
            }
            if (double.IsInfinity(y) || double.IsNaN(y))
            {
                throw new ArgumentOutOfRangeException("y");
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
// ReSharper restore RedundantBaseQualifier
}