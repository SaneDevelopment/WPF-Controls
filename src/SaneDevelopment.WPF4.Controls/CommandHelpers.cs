﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandHelpers.cs" company="Sane Development">
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
using System.Windows.Input;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Класс содержит набор функций для компактной регистрации соответствующих обработчиков команд
    /// </summary>
    public static class CommandHelpers
    {
        private static void PrivateRegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            params InputGesture[] inputGestures)
        {
            CommandManager.RegisterClassCommandBinding(controlType,
                new CommandBinding(command, executedRoutedEventHandler, canExecuteRoutedEventHandler));
            if (inputGestures != null)
            {
                foreach (var gesture in inputGestures)
                {
                    if (gesture != null)
                    {
                        CommandManager.RegisterClassInputBinding(controlType, new InputBinding(command, gesture));
                    }
                }
            }
        }

        /// <summary>
        /// Регистрирует обработчик команды
        /// </summary>
        /// <param name="controlType">Тип контрола</param>
        /// <param name="command">Команда</param>
        /// <param name="executedRoutedEventHandler">Обработчик выполнения команды</param>
        /// <param name="inputGesture">Горячая клавиша</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            InputGesture inputGesture)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, new[] { inputGesture });
        }

        /// <summary>
        /// Регистрирует обработчик команды
        /// </summary>
        /// <param name="controlType">Тип контрола</param>
        /// <param name="command">Команда</param>
        /// <param name="executedRoutedEventHandler">Обработчик выполнения команды</param>
        /// <param name="canExecuteRoutedEventHandler">Обработчик проверки возможности выполнения команды</param>
        /// <param name="inputGesture">Горячая клавиша</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            InputGesture inputGesture)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, new[] { inputGesture });
        }

        /// <summary>
        /// Регистрирует обработчик команды
        /// </summary>
        /// <param name="controlType">Тип контрола</param>
        /// <param name="command">Команда</param>
        /// <param name="executedRoutedEventHandler">Обработчик выполнения команды</param>
        /// <param name="key">Горячая клавиша</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            Key key)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, new InputGesture[] { new KeyGesture(key) });
        }

        /// <summary>
        /// Регистрирует обработчик команды
        /// </summary>
        /// <param name="controlType">Тип контрола</param>
        /// <param name="command">Команда</param>
        /// <param name="executedRoutedEventHandler">Обработчик выполнения команды</param>
        /// <param name="canExecuteRoutedEventHandler">Обработчик проверки возможности выполнения команды</param>
        /// <param name="key">Горячая клавиша</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            Key key)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, new InputGesture[] { new KeyGesture(key) });
        }

        /// <summary>
        /// Регистрирует обработчик команды
        /// </summary>
        /// <param name="controlType">Тип контрола</param>
        /// <param name="command">Команда</param>
        /// <param name="executedRoutedEventHandler">Обработчик выполнения команды</param>
        /// <param name="inputGesture">Горячая клавиша</param>
        /// <param name="inputGesture2">Горячая клавиша</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            InputGesture inputGesture,
            InputGesture inputGesture2)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, null, new[] { inputGesture, inputGesture2 });
        }

        /// <summary>
        /// Регистрирует обработчик команды
        /// </summary>
        /// <param name="controlType">Тип контрола</param>
        /// <param name="command">Команда</param>
        /// <param name="executedRoutedEventHandler">Обработчик выполнения команды</param>
        /// <param name="canExecuteRoutedEventHandler">Обработчик проверки возможности выполнения команды</param>
        /// <param name="inputGesture">Горячая клавиша</param>
        /// <param name="inputGesture2">Горячая клавиша</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            InputGesture inputGesture,
            InputGesture inputGesture2)
        {
            PrivateRegisterCommandHandler(controlType, command, executedRoutedEventHandler, canExecuteRoutedEventHandler, new[] { inputGesture, inputGesture2 });
        }
    }
}
