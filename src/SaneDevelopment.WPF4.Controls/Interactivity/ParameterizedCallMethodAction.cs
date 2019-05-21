// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterizedCallMethodAction.cs" company="Sane Development">
//
//   Sane Development WPF Controls Library
//
//   The BSD 3-Clause License
//
//   Copyright (c) 2011-2019, Sane Development
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
using System.Windows;

namespace SaneDevelopment.WPF4.Controls.Interactivity
{
    /// <summary>
    /// Класс реализует оболочку вокруг набора параметров при обработке вызова <see cref="ParameterizedCallMethodAction"/>
    /// </summary>
    public class ParameterizedEventArgs : EventArgs
    {
        /// <summary>
        /// Конструирует объект класса
        /// </summary>
        /// <param name="initialEventArgs">Аргументы исходного события</param>
        /// <param name="parameter">Первый параметр</param>
        /// <param name="parameter2">Второй параметр</param>
        /// <param name="parameter3">Третий параметр</param>
        /// <param name="parameter4">Четвертый параметр</param>
        /// <param name="parameter5">Пятый параметр</param>
        /// <param name="parameter6">Шестой параметр</param>
        /// <param name="parameter7">Седьмой параметр</param>
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
            InitialEventArgs = initialEventArgs;
            Parameter = parameter;
            Parameter2 = parameter2;
            Parameter3 = parameter3;
            Parameter4 = parameter4;
            Parameter5 = parameter5; 
            Parameter6 = parameter6;
            Parameter7 = parameter7;
        }

        /// <summary>
        /// Аргуметы исходного события
        /// </summary>
        public EventArgs InitialEventArgs { get; private set; }

        /// <summary>
        /// Значение первого параметра, заданного в <see cref="ParameterizedCallMethodAction.Parameter"/>
        /// </summary>
        public object Parameter { get; private set; }

        /// <summary>
        /// Значение второго параметра, заданного в <see cref="ParameterizedCallMethodAction.Parameter2"/>
        /// </summary>
        public object Parameter2 { get; private set; }

        /// <summary>
        /// Значение третьего параметра, заданного в <see cref="ParameterizedCallMethodAction.Parameter3"/>
        /// </summary>
        public object Parameter3 { get; private set; }

        /// <summary>
        /// Значение четвертого параметра, заданного в <see cref="ParameterizedCallMethodAction.Parameter4"/>
        /// </summary>
        public object Parameter4 { get; private set; }

        /// <summary>
        /// Значение пятого параметра, заданного в <see cref="ParameterizedCallMethodAction.Parameter4"/>
        /// </summary>
        public object Parameter5 { get; private set; }

        /// <summary>
        /// Значение шестого параметра, заданного в <see cref="ParameterizedCallMethodAction.Parameter5"/>
        /// </summary>
        public object Parameter6 { get; private set; }

        /// <summary>
        /// Значение седьмого параметра, заданного в <see cref="ParameterizedCallMethodAction.Parameter7"/>
        /// </summary>
        public object Parameter7 { get; private set; }
    }

    /// <summary>
    /// Класс представляет собой <see cref="Microsoft.Expression.Interactivity.Core.CallMethodAction"/> (наследуется от него).
    /// Данный класс добавляет к своему родительскому классу два свойства зависимости (параметры),
    /// которые будут переданы вызываемому методу в момент выполнения <see cref="Microsoft.Expression.Interactivity.Core.CallMethodAction.Invoke"/>.
    /// </summary>
    public class ParameterizedCallMethodAction : Microsoft.Expression.Interactivity.Core.CallMethodAction
    {
        /// <summary>
        /// Первый параметр
        /// </summary>
        public object Parameter
        {
            get { return GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ParameterizedCallMethodAction.Parameter"/>
        /// </summary>
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register(
                "Parameter",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Второй параметр
        /// </summary>
        public object Parameter2
        {
            get { return GetValue(Parameter2Property); }
            set { SetValue(Parameter2Property, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ParameterizedCallMethodAction.Parameter2"/>
        /// </summary>
        public static readonly DependencyProperty Parameter2Property =
            DependencyProperty.Register(
                "Parameter2",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Третий параметр
        /// </summary>
        public object Parameter3
        {
            get { return GetValue(Parameter3Property); }
            set { SetValue(Parameter3Property, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ParameterizedCallMethodAction.Parameter3"/>
        /// </summary>
        public static readonly DependencyProperty Parameter3Property =
            DependencyProperty.Register(
                "Parameter3",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Четвертый параметр
        /// </summary>
        public object Parameter4
        {
            get { return GetValue(Parameter4Property); }
            set { SetValue(Parameter4Property, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ParameterizedCallMethodAction.Parameter4"/>
        /// </summary>
        public static readonly DependencyProperty Parameter4Property =
            DependencyProperty.Register(
                "Parameter4",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Пятый параметр
        /// </summary>
        public object Parameter5
        {
            get { return GetValue(Parameter5Property); }
            set { SetValue(Parameter5Property, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ParameterizedCallMethodAction.Parameter5"/>
        /// </summary>
        public static readonly DependencyProperty Parameter5Property =
            DependencyProperty.Register(
                "Parameter5",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Шестой параметр
        /// </summary>
        public object Parameter6
        {
            get { return GetValue(Parameter6Property); }
            set { SetValue(Parameter6Property, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ParameterizedCallMethodAction.Parameter6"/>
        /// </summary>
        public static readonly DependencyProperty Parameter6Property =
            DependencyProperty.Register(
                "Parameter6",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Седьмой параметр
        /// </summary>
        public object Parameter7
        {
            get { return GetValue(Parameter7Property); }
            set { SetValue(Parameter7Property, value); }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="ParameterizedCallMethodAction.Parameter7"/>
        /// </summary>
        public static readonly DependencyProperty Parameter7Property =
            DependencyProperty.Register(
                "Parameter7",
                typeof(object),
                typeof(ParameterizedCallMethodAction));

        /// <summary>
        /// Выполняет соответствующее действие.
        /// Вызывает метод базового класса <see cref="Microsoft.Expression.Interactivity.Core.CallMethodAction.Invoke"/>,
        /// предварительно "завернув" заданные параметры и исходный параметр события в <see cref="ParameterizedEventArgs"/>
        /// </summary>
        /// <param name="parameter">Параметр вызова исходного метода</param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        protected override void Invoke(object parameter)
        {
            if (parameter is EventArgs)
            {
                var typedArgs = (EventArgs)parameter;
                parameter = new ParameterizedEventArgs(
                    typedArgs,
                    Parameter, 
                    Parameter2, 
                    Parameter3,
                    Parameter4,
                    Parameter5,
                    Parameter6,
                    Parameter7);
            }

            base.Invoke(parameter);
        }
    }
}