﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SupplementaryInteraction.cs" company="Sane Development">
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace SaneDevelopment.WPF4.Controls.Interactivity
{
    /// <summary>
    /// List of triggers for utilize inside <see cref="SupplementaryInteraction.TriggersProperty"/>
    /// </summary>
    public class TriggersCollection : List<Microsoft.Xaml.Behaviors.TriggerBase>
    { }

    /// <summary>
    /// Provides attached dependency property <see cref="SupplementaryInteraction.TriggersProperty"/>
    /// for ability of setting it inside <c>Style.Setter</c> in XAML.
    /// 
    /// See discussion in http://stackoverflow.com/questions/1647815/how-to-add-a-blend-behavior-in-a-style-setter
    /// </summary>
    public static class SupplementaryInteraction
    {
        /// <summary>
        /// Get value of <see cref="SupplementaryInteraction.TriggersProperty"/>
        /// </summary>
        /// <param name="obj">Dependency object, to which <see cref="SupplementaryInteraction.TriggersProperty"/> attached</param>
        /// <returns><see cref="SaneDevelopment.WPF4.Controls.Interactivity.TriggersCollection"/></returns>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static TriggersCollection GetTriggers(DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            return (TriggersCollection)obj.GetValue(TriggersProperty);
        }

        /// <summary>
        /// Set value to <see cref="SupplementaryInteraction.TriggersProperty"/>
        /// </summary>
        /// <param name="obj">Dependency object, to which <see cref="SupplementaryInteraction.TriggersProperty"/> attached</param>
        /// <param name="value">New value</param>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public static void SetTriggers(DependencyObject obj, TriggersCollection value)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            obj.SetValue(TriggersProperty, value);
        }

        /// <summary>
        /// Attached dependency property as list of triggers <see cref="SaneDevelopment.WPF4.Controls.Interactivity.TriggersCollection"/>
        /// </summary>
        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached(
                "Triggers",
                typeof (TriggersCollection),
                typeof (SupplementaryInteraction),
                new UIPropertyMetadata(null, OnPropertyTriggersChanged));

        private static void OnPropertyTriggersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var triggers = Microsoft.Xaml.Behaviors.Interaction.GetTriggers(d);
            if (triggers == null)
            {
                return;
            }

            var newTriggers = e.NewValue as TriggersCollection;
            if (newTriggers == null)
            {
                return;
            }

            foreach (var newTrigger in newTriggers)
            {
                triggers.Add(newTrigger);
            }
        }
    }
}