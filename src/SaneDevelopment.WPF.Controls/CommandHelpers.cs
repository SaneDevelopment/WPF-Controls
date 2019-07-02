// -----------------------------------------------------------------------
// <copyright file="CommandHelpers.cs" company="Sane Development">
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
    using System.Windows.Input;

    /// <summary>
    /// Class provides methods for handy shortened command binding registration.
    /// </summary>
    public static class CommandHelpers
    {
        /// <summary>
        /// Registers command handler.
        /// </summary>
        /// <param name="controlType">Control type.</param>
        /// <param name="command">Command.</param>
        /// <param name="executedRoutedEventHandler">Command handler.</param>
        /// <param name="inputGesture">Input gesture.</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            InputGesture inputGesture)
        {
            PrivateRegisterCommandHandler(
                controlType,
                command,
                executedRoutedEventHandler,
                null,
                new[] { inputGesture });
        }

        /// <summary>
        /// Registers command handler.
        /// </summary>
        /// <param name="controlType">Control type.</param>
        /// <param name="command">Command.</param>
        /// <param name="executedRoutedEventHandler">Command handler.</param>
        /// <param name="canExecuteRoutedEventHandler">Handler for <see cref="CommandBinding.CanExecute"/> event.</param>
        /// <param name="inputGesture">Input gesture.</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            InputGesture inputGesture)
        {
            PrivateRegisterCommandHandler(
                controlType,
                command,
                executedRoutedEventHandler,
                canExecuteRoutedEventHandler,
                new[] { inputGesture });
        }

        /// <summary>
        /// Registers command handler.
        /// </summary>
        /// <param name="controlType">Control type.</param>
        /// <param name="command">Command.</param>
        /// <param name="executedRoutedEventHandler">Command handler.</param>
        /// <param name="key">Input gesture.</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            Key key)
        {
            PrivateRegisterCommandHandler(
                controlType,
                command,
                executedRoutedEventHandler,
                null,
                new InputGesture[] { new KeyGesture(key) });
        }

        /// <summary>
        /// Registers command handler.
        /// </summary>
        /// <param name="controlType">Control type.</param>
        /// <param name="command">Command.</param>
        /// <param name="executedRoutedEventHandler">Command handler.</param>
        /// <param name="canExecuteRoutedEventHandler">Handler for <see cref="CommandBinding.CanExecute"/> event.</param>
        /// <param name="key">Input gesture.</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            Key key)
        {
            PrivateRegisterCommandHandler(
                controlType,
                command,
                executedRoutedEventHandler,
                canExecuteRoutedEventHandler,
                new InputGesture[] { new KeyGesture(key) });
        }

        /// <summary>
        /// Registers command handler.
        /// </summary>
        /// <param name="controlType">Control type.</param>
        /// <param name="command">Command.</param>
        /// <param name="executedRoutedEventHandler">Command handler.</param>
        /// <param name="inputGesture">Input gesture.</param>
        /// <param name="inputGesture2">Input gesture 2.</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            InputGesture inputGesture,
            InputGesture inputGesture2)
        {
            PrivateRegisterCommandHandler(
                controlType,
                command,
                executedRoutedEventHandler,
                null,
                new[] { inputGesture, inputGesture2 });
        }

        /// <summary>
        /// Registers command handler.
        /// </summary>
        /// <param name="controlType">Control type.</param>
        /// <param name="command">Command.</param>
        /// <param name="executedRoutedEventHandler">Command handler.</param>
        /// <param name="canExecuteRoutedEventHandler">Handler for <see cref="CommandBinding.CanExecute"/> event.</param>
        /// <param name="inputGesture">Input gesture.</param>
        /// <param name="inputGesture2">Input gesture 2.</param>
        public static void RegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            InputGesture inputGesture,
            InputGesture inputGesture2)
        {
            PrivateRegisterCommandHandler(
                controlType,
                command,
                executedRoutedEventHandler,
                canExecuteRoutedEventHandler,
                new[] { inputGesture, inputGesture2 });
        }

        private static void PrivateRegisterCommandHandler(
            Type controlType,
            RoutedCommand command,
            ExecutedRoutedEventHandler executedRoutedEventHandler,
            CanExecuteRoutedEventHandler canExecuteRoutedEventHandler,
            params InputGesture[] inputGestures)
        {
            CommandManager.RegisterClassCommandBinding(
                controlType,
                new CommandBinding(command, executedRoutedEventHandler, canExecuteRoutedEventHandler));

            if (inputGestures == null)
            {
                return;
            }

            foreach (var gesture in inputGestures)
            {
                if (gesture != null)
                {
                    CommandManager.RegisterClassInputBinding(controlType, new InputBinding(command, gesture));
                }
            }
        }
    }
}
