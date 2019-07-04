// -----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Sane Development">
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

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "StyleCop.CSharp.ReadabilityRules",
    "SA1124:Do not use regions",
    Justification = "We use regions to collapse private fields, groups of dependency properties and other WPF-specific code.")]

[assembly: SuppressMessage(
    "Microsoft.Naming",
    "CA1709:IdentifiersShouldBeCasedCorrectly",
    MessageId = "WPF",
    Scope = "namespace",
    Target = "SaneDevelopment.WPF.Controls",
    Justification = "WPF is commonly used abbreviation.")]
[assembly: SuppressMessage(
    "Microsoft.Naming",
    "CA1709:IdentifiersShouldBeCasedCorrectly",
    MessageId = "WPF",
    Justification = "WPF is commonly used abbreviation.")]
[assembly: SuppressMessage(
    "Microsoft.Naming",
    "CA1709:IdentifiersShouldBeCasedCorrectly",
    MessageId = "WPF",
    Scope = "namespace",
    Target = "SaneDevelopment.WPF.Controls.Interactivity",
    Justification = "WPF is commonly used abbreviation.")]
[assembly: SuppressMessage(
    "Microsoft.Naming",
    "CA1709:IdentifiersShouldBeCasedCorrectly",
    MessageId = "WPF",
    Scope = "namespace",
    Target = "SaneDevelopment.WPF.Controls.LinqToVisualTree",
    Justification = "WPF is commonly used abbreviation.")]
