// -----------------------------------------------------------------------
// <copyright file="ParameterizedEventArgs.cs" company="Sane Development">
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

    /// <summary>
    /// Class provides container for parameter set when invoking <see cref="ParameterizedCallMethodAction"/>.
    /// </summary>
    public class ParameterizedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterizedEventArgs"/> class.
        /// </summary>
        /// <param name="initialEventArgs">Initial event's arguments.</param>
        /// <param name="parameter">Parameter #1.</param>
        /// <param name="parameter2">Parameter #2.</param>
        /// <param name="parameter3">Parameter #3.</param>
        /// <param name="parameter4">Parameter #4.</param>
        /// <param name="parameter5">Parameter #5.</param>
        /// <param name="parameter6">Parameter #6.</param>
        /// <param name="parameter7">Parameter #7.</param>
        public ParameterizedEventArgs(
            EventArgs initialEventArgs,
            object parameter,
            object parameter2,
            object parameter3,
            object parameter4,
            object parameter5,
            object parameter6,
            object parameter7)
        {
            this.InitialEventArgs = initialEventArgs;
            this.Parameter = parameter;
            this.Parameter2 = parameter2;
            this.Parameter3 = parameter3;
            this.Parameter4 = parameter4;
            this.Parameter5 = parameter5;
            this.Parameter6 = parameter6;
            this.Parameter7 = parameter7;
        }

        /// <summary>
        /// Gets initial event's arguments.
        /// </summary>
        /// <value>Initial event's arguments.</value>
        public EventArgs InitialEventArgs { get; private set; }

        /// <summary>
        /// Gets parameter value from <see cref="ParameterizedCallMethodAction.Parameter"/>.
        /// </summary>
        /// <value>Parameter value from <see cref="ParameterizedCallMethodAction.Parameter"/>.</value>
        public object Parameter { get; private set; }

        /// <summary>
        /// Gets parameter value from <see cref="ParameterizedCallMethodAction.Parameter2"/>.
        /// </summary>
        /// <value>Parameter value from <see cref="ParameterizedCallMethodAction.Parameter2"/>.</value>
        public object Parameter2 { get; private set; }

        /// <summary>
        /// Gets parameter value from <see cref="ParameterizedCallMethodAction.Parameter3"/>.
        /// </summary>
        /// <value>Parameter value from <see cref="ParameterizedCallMethodAction.Parameter3"/>.</value>
        public object Parameter3 { get; private set; }

        /// <summary>
        /// Gets parameter value from <see cref="ParameterizedCallMethodAction.Parameter4"/>.
        /// </summary>
        /// <value>Parameter value from <see cref="ParameterizedCallMethodAction.Parameter4"/>.</value>
        public object Parameter4 { get; private set; }

        /// <summary>
        /// Gets parameter value from <see cref="ParameterizedCallMethodAction.Parameter5"/>.
        /// </summary>
        /// <value>Parameter value from <see cref="ParameterizedCallMethodAction.Parameter5"/>.</value>
        public object Parameter5 { get; private set; }

        /// <summary>
        /// Gets parameter value from <see cref="ParameterizedCallMethodAction.Parameter6"/>.
        /// </summary>
        /// <value>Parameter value from <see cref="ParameterizedCallMethodAction.Parameter6"/>.</value>
        public object Parameter6 { get; private set; }

        /// <summary>
        /// Gets parameter value from <see cref="ParameterizedCallMethodAction.Parameter7"/>.
        /// </summary>
        /// <value>Parameter value from <see cref="ParameterizedCallMethodAction.Parameter7"/>.</value>
        public object Parameter7 { get; private set; }
    }
}