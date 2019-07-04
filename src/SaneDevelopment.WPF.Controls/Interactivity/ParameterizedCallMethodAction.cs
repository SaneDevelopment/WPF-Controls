// -----------------------------------------------------------------------
// <copyright file="ParameterizedCallMethodAction.cs" company="Sane Development">
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
    using Microsoft.Xaml.Behaviors.Core;

    /// <summary>
    /// Class behaves like <see cref="CallMethodAction"/> (derives from it).
    /// This class appends to base class several dependency properties (additional parameters),
    /// which will pass to method <see cref="CallMethodAction.MethodName"/> while <see cref="CallMethodAction.Invoke"/>.
    /// </summary>
#pragma warning disable CA1501 // Avoid excessive inheritance
    public class ParameterizedCallMethodAction : CallMethodAction
#pragma warning restore CA1501 // Avoid excessive inheritance
    {
#pragma warning disable SA1201 // Elements should appear in the correct order

        /// <summary>
        /// Gets or sets parameter #1.
        /// </summary>
        /// <value>Parameter #1 value.</value>
        public object Parameter
        {
            get { return this.GetValue(ParameterProperty); }
            set { this.SetValue(ParameterProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ParameterizedCallMethodAction.Parameter"/>.
        /// </summary>
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register(
                "Parameter",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Gets or sets parameter #2.
        /// </summary>
        /// <value>Parameter #2 value.</value>
        public object Parameter2
        {
            get { return this.GetValue(Parameter2Property); }
            set { this.SetValue(Parameter2Property, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ParameterizedCallMethodAction.Parameter2"/>.
        /// </summary>
        public static readonly DependencyProperty Parameter2Property =
            DependencyProperty.Register(
                "Parameter2",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Gets or sets parameter #3.
        /// </summary>
        /// <value>Parameter #3 value.</value>
        public object Parameter3
        {
            get { return this.GetValue(Parameter3Property); }
            set { this.SetValue(Parameter3Property, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ParameterizedCallMethodAction.Parameter3"/>.
        /// </summary>
        public static readonly DependencyProperty Parameter3Property =
            DependencyProperty.Register(
                "Parameter3",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Gets or sets parameter #4.
        /// </summary>
        /// <value>Parameter #4 value.</value>
        public object Parameter4
        {
            get { return this.GetValue(Parameter4Property); }
            set { this.SetValue(Parameter4Property, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ParameterizedCallMethodAction.Parameter4"/>.
        /// </summary>
        public static readonly DependencyProperty Parameter4Property =
            DependencyProperty.Register(
                "Parameter4",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Gets or sets parameter #5.
        /// </summary>
        /// <value>Parameter #5 value.</value>
        public object Parameter5
        {
            get { return this.GetValue(Parameter5Property); }
            set { this.SetValue(Parameter5Property, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ParameterizedCallMethodAction.Parameter5"/>.
        /// </summary>
        public static readonly DependencyProperty Parameter5Property =
            DependencyProperty.Register(
                "Parameter5",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Gets or sets parameter #6.
        /// </summary>
        /// <value>Parameter #6 value.</value>
        public object Parameter6
        {
            get { return this.GetValue(Parameter6Property); }
            set { this.SetValue(Parameter6Property, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ParameterizedCallMethodAction.Parameter6"/>.
        /// </summary>
        public static readonly DependencyProperty Parameter6Property =
            DependencyProperty.Register(
                "Parameter6",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Gets or sets parameter #7.
        /// </summary>
        /// <value>Parameter #7 value.</value>
        public object Parameter7
        {
            get { return this.GetValue(Parameter7Property); }
            set { this.SetValue(Parameter7Property, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ParameterizedCallMethodAction.Parameter7"/>.
        /// </summary>
        public static readonly DependencyProperty Parameter7Property =
            DependencyProperty.Register(
                "Parameter7",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

#pragma warning restore SA1201 // Elements should appear in the correct order

        /// <summary>
        /// Invokes method <see cref="CallMethodAction.MethodName"/>.
        /// Calls base method <see cref="CallMethodAction.Invoke"/>,
        /// but before wraps initial event's arguments and parameters into <see cref="ParameterizedEventArgs"/>.
        /// </summary>
        /// <param name="parameter">Initial parameter value.</param>
        protected override void Invoke(object parameter)
        {
            if (parameter is EventArgs)
            {
                var typedArgs = (EventArgs)parameter;
                parameter = new ParameterizedEventArgs(
                    typedArgs,
                    this.Parameter,
                    this.Parameter2,
                    this.Parameter3,
                    this.Parameter4,
                    this.Parameter5,
                    this.Parameter6,
                    this.Parameter7);
            }

            base.Invoke(parameter);
        }
    }
}