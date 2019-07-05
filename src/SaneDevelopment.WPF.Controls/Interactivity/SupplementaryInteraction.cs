// -----------------------------------------------------------------------
// <copyright file="SupplementaryInteraction.cs" company="Sane Development">
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

namespace SaneDevelopment.WPF.Controls.Interactivity
{
    using System;
    using System.Windows;

    /// <summary>
    /// Provides attached dependency property <see cref="SupplementaryInteraction.TriggersProperty"/>
    /// for ability of setting it inside <c>Style.Setter</c> in XAML.
    ///
    /// See discussion in http://stackoverflow.com/questions/1647815/how-to-add-a-blend-behavior-in-a-style-setter.
    /// </summary>
    public static class SupplementaryInteraction
    {
#pragma warning disable SA1201 // Elements should appear in the correct order

        /// <summary>
        /// Get value of <see cref="SupplementaryInteraction.TriggersProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">Dependency object, to which <see cref="SupplementaryInteraction.TriggersProperty"/> attached.</param>
        /// <returns><see cref="TriggersCollection"/>.</returns>
        public static TriggersCollection GetTriggers(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException(nameof(dependencyObject));
            }

            return (TriggersCollection)dependencyObject.GetValue(TriggersProperty);
        }

        /// <summary>
        /// Set value to <see cref="SupplementaryInteraction.TriggersProperty"/>.
        /// </summary>
        /// <param name="dependencyObject">Dependency object, to which <see cref="SupplementaryInteraction.TriggersProperty"/> attached.</param>
        /// <param name="value">New value.</param>
        public static void SetTriggers(DependencyObject dependencyObject, TriggersCollection value)
        {
            if (dependencyObject == null)
            {
                throw new ArgumentNullException(nameof(dependencyObject));
            }

            dependencyObject.SetValue(TriggersProperty, value);
        }

        /// <summary>
        /// Attached dependency property as list of triggers <see cref="TriggersCollection"/>.
        /// </summary>
        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached(
                "Triggers",
                typeof(TriggersCollection),
                typeof(SupplementaryInteraction),
                new UIPropertyMetadata(null, OnPropertyTriggersChanged));

#pragma warning restore SA1201 // Elements should appear in the correct order

        private static void OnPropertyTriggersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var triggers = Microsoft.Xaml.Behaviors.Interaction.GetTriggers(d);
            if (triggers == null)
            {
                return;
            }

            if (!(e.NewValue is TriggersCollection newTriggers))
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