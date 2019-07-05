// -----------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="Sane Development">
//
// Sane Development WPF Controls Library Unit Tests.
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
    Justification = "We use regions to group several tests united by similar testing subject.")]

[assembly: SuppressMessage(
    "StyleCop.CSharp.DocumentationRules",
    "SA1600:Elements should be documented",
    Justification = "Not need in unit tests project")]
[assembly: SuppressMessage(
    "StyleCop.CSharp.SpecialRules",
    "SA0001:XML comment analysis is disabled due to project configuration",
    Justification = "Not need in unit tests project")]

[assembly: SuppressMessage(
    "Naming",
    "CA1707:Identifiers should not contain underscores",
    Justification = "Not need in unit tests project")]
[assembly: SuppressMessage(
    "Design",
    "CA1014:Mark assemblies with CLSCompliant",
    Justification = "Not need in unit tests project")]
