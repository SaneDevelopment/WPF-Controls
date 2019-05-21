// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleRangeSliders.cs" company="Sane Development">
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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace SaneDevelopment.WPF4.Controls
{
    /// <summary>
    /// Интерфейс для преобразования значения в строку.
    /// Объекты, реализующие этот интерфейс, используются для преобразования значений <see cref="SimpleRangeSlider{T, TInterval}"/>
    /// в их строковые представления для отображения пользователю (например, во всплывающий подсказках соответствующих ползунков).
    /// </summary>
    /// <typeparam name="T">Тип значений</typeparam>
    public interface IRangeValueToStringConverter<in T>
    {
        /// <summary>
        /// Функция преобразует значение <paramref name="value"/> в строку
        /// </summary>
        /// <param name="value">Значение для преобразования</param>
        /// <param name="thumbType">Тип ползунка, значение которого задано в <paramref name="value"/>.</param>
        /// <param name="parameter">Дополнительный произвольный параметер для преобразования</param>
        /// <returns>Строковое представление числа</returns>
        string Convert(T value, RangeThumbType thumbType, object parameter);
    }

    /// <summary>
    /// Базовый класс для интервальных слайдеров (ползунков)
    /// </summary>
    /// <typeparam name="T">Тип значений</typeparam>
    /// <typeparam name="TInterval">Тип интервала</typeparam>
    public abstract class SimpleRangeSlider<T, TInterval> : RangeBaseControl<T, TInterval>
    {
        #region Constructors

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static SimpleRangeSlider()
        {
            // Initialize CommandCollection & CommandLink(s)
            InitializeCommands();

            // Register Event Handler for the Thumb
            EventManager.RegisterClassHandler(
                typeof(SimpleRangeSlider<T, TInterval>),
                Thumb.DragStartedEvent,
                new DragStartedEventHandler(OnThumbDragStarted));
            EventManager.RegisterClassHandler(
                typeof(SimpleRangeSlider<T, TInterval>),
                Thumb.DragDeltaEvent,
                new DragDeltaEventHandler(OnThumbDragDelta));
            EventManager.RegisterClassHandler(
                typeof(SimpleRangeSlider<T, TInterval>),
                Thumb.DragCompletedEvent,
                new DragCompletedEventHandler(OnThumbDragCompleted));

            // Listen to MouseLeftButtonDown event to determine if slide should move focus to itself
            EventManager.RegisterClassHandler(
                typeof(SimpleRangeSlider<T, TInterval>),
                Mouse.MouseDownEvent,
                new MouseButtonEventHandler(OnMouseLeftButtonDown), true);
        }

        #endregion Constructors

        #region IRangeTrackTemplatedParent

        /// <summary>
        /// Метод выполняет обработку <see cref="FrameworkElement.OnApplyTemplate"/>, а именно,
        /// связывает некоторые свойства зависимостей с шаблонным родителем.
        /// </summary>
        /// <param name="templatedParent">Шаблонный родитель</param>
        /// <param name="track">Интервальный трэк</param>
        public override void OnApplyRangeTrackTemplate(DependencyObject templatedParent, RangeTrack<T, TInterval> track)
        {
            if (track == null)
                throw new ArgumentNullException("track");

            base.OnApplyRangeTrackTemplate(templatedParent, track);

            var templatedParentControl = templatedParent as SimpleRangeSlider<T, TInterval>;
            if (templatedParentControl != null)
            {
                track.BindToTemplatedParent(RangeTrack<T, TInterval>.OrientationProperty, OrientationProperty);
                track.BindChildToTemplatedParent(track.DecreaseRepeatButton, RepeatButton.DelayProperty, DelayProperty);
                track.BindChildToTemplatedParent(track.DecreaseRepeatButton, RepeatButton.IntervalProperty, IntervalProperty);
                track.BindChildToTemplatedParent(track.IncreaseRepeatButton, RepeatButton.DelayProperty, DelayProperty);
                track.BindChildToTemplatedParent(track.IncreaseRepeatButton, RepeatButton.IntervalProperty, IntervalProperty);
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Команда для сдвига интервала в большую сторону на большую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseRangeLarge { get; private set; }
        /// <summary>
        /// Команда для сдвига значения начала интервала в большую сторону на большую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseStartLarge { get; private set; }
        /// <summary>
        /// Команда для сдвига значения конца интервала в большую сторону на большую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseEndLarge { get; private set; }
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand IncreaseLargeByKey { get; set; }

        /// <summary>
        /// Команда для сдвига интервала в меньшую сторону на большую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseRangeLarge { get; private set; }
        /// <summary>
        /// Команда для сдвига значения начала интервала в меньшую сторону на большую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseStartLarge { get; private set; }
        /// <summary>
        /// Команда для сдвига значения конца интервала в меньшую сторону на большую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseEndLarge { get; private set; }
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand DecreaseLargeByKey { get; set; }

        /// <summary>
        /// Команда для сдвига интервала в большую сторону на маленькую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseRangeSmall { get; private set; }
        /// <summary>
        /// Команда для сдвига значения начала интервала в большую сторону на маленькую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseStartSmall { get; private set; }
        /// <summary>
        /// Команда для сдвига значения конца интервала в большую сторону на маленькую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseEndSmall { get; private set; }
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand IncreaseSmallByKey { get; set; }

        /// <summary>
        /// Команда для сдвига интервала в меньшую сторону на маленькую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseRangeSmall { get; private set; }
        /// <summary>
        /// Команда для сдвига значения начала интервала в меньшую сторону на маленькую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseStartSmall { get; private set; }
        /// <summary>
        /// Команда для сдвига значения конца интервала в меньшую сторону на маленькую величину
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseEndSmall { get; private set; }
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand DecreaseSmallByKey { get; set; }

        /// <summary>
        /// Команда для сдвига интервала в положение минимума
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MinimizeRangeValue { get; private set; }
        /// <summary>
        /// Команда для сдвига значения начала интервала в положение минимума
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MinimizeStartValue { get; private set; }
        /// <summary>
        /// Команда для сдвига значения конца интервала в положение минимума
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MinimizeEndValue { get; private set; }
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand MinimizeValueByKey { get; set; }

        /// <summary>
        /// Команда для сдвига интервала в положение максимума
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MaximizeRangeValue { get; private set; }
        /// <summary>
        /// Команда для сдвига значения начала интервала в положение максимума
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MaximizeStartValue { get; private set; }
        /// <summary>
        /// Команда для сдвига значения конца интервала в положение максимума
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MaximizeEndValue { get; private set; }
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand MaximizeValueByKey { get; set; }

        static void InitializeCommands()
        {
            Type thisType = typeof(SimpleRangeSlider<T, TInterval>);

            IncreaseRangeLarge = new RoutedCommand(c_IncreaseRangeLargeCommandName, thisType);
            IncreaseStartLarge = new RoutedCommand(c_IncreaseStartLargeCommandName, thisType);
            IncreaseEndLarge = new RoutedCommand(c_IncreaseEndLargeCommandName, thisType);
            IncreaseLargeByKey = new RoutedCommand(c_IncreaseLargeByKeyCommandName, thisType);

            DecreaseRangeLarge = new RoutedCommand(c_DecreaseRangeLargeCommandName, thisType);
            DecreaseStartLarge = new RoutedCommand(c_DecreaseStartLargeCommandName, thisType);
            DecreaseEndLarge = new RoutedCommand(c_DecreaseEndLargeCommandName, thisType);
            DecreaseLargeByKey = new RoutedCommand(c_DecreaseLargeByKeyCommandName, thisType);

            IncreaseRangeSmall = new RoutedCommand(c_IncreaseRangeSmallCommandName, thisType);
            IncreaseStartSmall = new RoutedCommand(c_IncreaseStartSmallCommandName, thisType);
            IncreaseEndSmall = new RoutedCommand(c_IncreaseEndSmallCommandName, thisType);
            IncreaseSmallByKey = new RoutedCommand(c_IncreaseSmallByKeyCommandName, thisType);

            DecreaseRangeSmall = new RoutedCommand(c_DecreaseRangeSmallCommandName, thisType);
            DecreaseStartSmall = new RoutedCommand(c_DecreaseStartSmallCommandName, thisType);
            DecreaseEndSmall = new RoutedCommand(c_DecreaseEndSmallCommandName, thisType);
            DecreaseSmallByKey = new RoutedCommand(c_DecreaseSmallByKeyCommandName, thisType);

            MinimizeRangeValue = new RoutedCommand(c_MinimizeRangeValueCommandName, thisType);
            MinimizeStartValue = new RoutedCommand(c_MinimizeStartValueCommandName, thisType);
            MinimizeEndValue = new RoutedCommand(c_MinimizeEndValueCommandName, thisType);
            MinimizeValueByKey = new RoutedCommand(c_MinimizeValueByKeyCommandName, thisType);

            MaximizeRangeValue = new RoutedCommand(c_MaximizeRangeValueCommandName, thisType);
            MaximizeStartValue = new RoutedCommand(c_MaximizeStartValueCommandName, thisType);
            MaximizeEndValue = new RoutedCommand(c_MaximizeEndValueCommandName, thisType);
            MaximizeValueByKey = new RoutedCommand(c_MaximizeValueByKeyCommandName, thisType);

            // Команды public регистрируются без горячих клавиш, т.к. для идентификации того или иного ползунка надо знать
            // какой именно Ctrl был нажат - правый или левый, что при задании KeyGesture невозможно (есть только ModifierKeys.Control).
            // Такие команды предназначены исключительно для вызова через Execute.
            // Команды, которые могут вызываться через сочетание клавиш объявлены как private,
            // чтобы их нельзя было вызвать из внешнего кода.
            // Эти команды обрабатываются внутри закрытого обработчика и анализируют, какой из Ctrl был нажат.
            //
            // Сделано это из-за того, что сочетание клавиш <Ctrl>+<кнопка> можно назначить только одной команде
            // и именно ее _CanExecute будет вызываться при нажатии на указанное сочетание.
            // Однако, условия возможности выполнения той или иной команды для левого и правого ползунка разные,
            // поэтому какое-то из сочетаний клавиш не будет работать из-за того, что запрещено выполнение команды,
            // привязанной к другому сочетанию.

            // Increase Large
            CommandHelpers.RegisterCommandHandler(
                thisType,
                IncreaseRangeLarge,
                OnIncreaseRangeLargeCommand,
                IncreaseEndValueCommand_CanExecute,
                new KeyGesture(Key.PageUp));
            CommandHelpers.RegisterCommandHandler(
                thisType,
                IncreaseStartLarge,
                OnIncreaseStartLargeCommand,
                IncreaseStartValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                IncreaseEndLarge,
                OnIncreaseEndLargeCommand,
                IncreaseEndValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                IncreaseLargeByKey,
                OnIncreaseLargeByKeyCommand,
                new KeyGesture(Key.PageUp, ModifierKeys.Control));

            // Decrease Large
            CommandHelpers.RegisterCommandHandler(
                thisType,
                DecreaseRangeLarge,
                OnDecreaseRangeLargeCommand,
                DecreaseStartValueCommand_CanExecute,
                new KeyGesture(Key.PageDown));
            CommandHelpers.RegisterCommandHandler(
                thisType,
                DecreaseStartLarge,
                OnDecreaseStartLargeCommand,
                DecreaseStartValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                DecreaseEndLarge,
                OnDecreaseEndLargeCommand,
                DecreaseEndValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                DecreaseLargeByKey,
                OnDecreaseLargeByKeyCommand,
                new KeyGesture(Key.PageDown, ModifierKeys.Control));

            // Increase Small
            CommandHelpers.RegisterCommandHandler(
                thisType,
                IncreaseRangeSmall,
                OnIncreaseRangeSmallCommand,
                IncreaseEndValueCommand_CanExecute,
                new KeyGesture(Key.Up),
                new KeyGesture(Key.Right));
            CommandHelpers.RegisterCommandHandler(
                thisType,
                IncreaseStartSmall,
                OnIncreaseStartSmallCommand,
                IncreaseStartValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                IncreaseEndSmall,
                OnIncreaseEndSmallCommand,
                IncreaseEndValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                IncreaseSmallByKey,
                OnIncreaseSmallByKeyCommand,
                new KeyGesture(Key.Up, ModifierKeys.Control),
                new KeyGesture(Key.Right, ModifierKeys.Control));

            // Decrease Small
            CommandHelpers.RegisterCommandHandler(
                thisType,
                DecreaseRangeSmall,
                OnDecreaseRangeSmallCommand,
                DecreaseStartValueCommand_CanExecute,
                new KeyGesture(Key.Down),
                new KeyGesture(Key.Left));
            CommandHelpers.RegisterCommandHandler(
                thisType,
                DecreaseStartSmall,
                OnDecreaseStartSmallCommand,
                DecreaseStartValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                DecreaseEndSmall,
                OnDecreaseEndSmallCommand,
                DecreaseEndValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                DecreaseSmallByKey,
                OnDecreaseSmallByKeyCommand,
                new KeyGesture(Key.Down, ModifierKeys.Control),
                new KeyGesture(Key.Left, ModifierKeys.Control));

            // Minimize
            CommandHelpers.RegisterCommandHandler(
                thisType,
                MinimizeRangeValue,
                OnMinimizeRangeValueCommand,
                DecreaseStartValueCommand_CanExecute,
                Key.Home);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                MinimizeStartValue,
                OnMinimizeStartValueCommand,
                DecreaseStartValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                MinimizeEndValue,
                OnMinimizeEndValueCommand,
                DecreaseEndValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                MinimizeValueByKey,
                OnMinimizeValueByKeyCommand,
                new KeyGesture(Key.Home, ModifierKeys.Control));

            // Maximize
            CommandHelpers.RegisterCommandHandler(
                thisType,
                MaximizeRangeValue,
                OnMaximizeRangeValueCommand,
                IncreaseEndValueCommand_CanExecute,
                Key.End);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                MaximizeStartValue,
                OnMaximizeStartValueCommand,
                IncreaseStartValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                MaximizeEndValue,
                OnMaximizeEndValueCommand,
                IncreaseEndValueCommand_CanExecute,
                null);
            CommandHelpers.RegisterCommandHandler(
                thisType,
                MaximizeValueByKey,
                OnMaximizeValueByKeyCommand,
                new KeyGesture(Key.End, ModifierKeys.Control));
        }

        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения начала интервала в большую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected abstract bool IncreaseStartValueCommandCanExecute();
        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения конца интервала в большую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected abstract bool IncreaseEndValueCommandCanExecute();
        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения начала интервала в меньшую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected abstract bool DecreaseStartValueCommandCanExecute();
        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения конца интервала в меньшую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected abstract bool DecreaseEndValueCommandCanExecute();

        private static void IncreaseStartValueCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = slider.IncreaseStartValueCommandCanExecute();
        }

        private static void IncreaseEndValueCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = slider.IncreaseEndValueCommandCanExecute();
        }

        private static void DecreaseStartValueCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = slider.DecreaseStartValueCommandCanExecute();
        }

        private static void DecreaseEndValueCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider == null)
            {
                e.CanExecute = false;
                return;
            }

            e.CanExecute = slider.DecreaseEndValueCommandCanExecute();
        }


        private static void OnIncreaseRangeSmallCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnIncreaseRangeSmall();
            }
        }

        private static void OnIncreaseStartSmallCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnIncreaseStartSmall();
            }
        }

        private static void OnIncreaseEndSmallCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnIncreaseEndSmall();
            }
        }

        private static void OnIncreaseSmallByKeyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                switch (DetectCommandThumbType(e))
                {
                    case RangeThumbType.StartThumb:
                        slider.OnIncreaseStartSmall();
                        break;
                    case RangeThumbType.EndThumb:
                        slider.OnIncreaseEndSmall();
                        break;
                }
            }
        }


        private static void OnDecreaseRangeSmallCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnDecreaseRangeSmall();
            }
        }

        private static void OnDecreaseStartSmallCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnDecreaseStartSmall();
            }
        }

        private static void OnDecreaseEndSmallCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnDecreaseEndSmall();
            }
        }

        private static void OnDecreaseSmallByKeyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                switch (DetectCommandThumbType(e))
                {
                    case RangeThumbType.StartThumb:
                        slider.OnDecreaseStartSmall();
                        break;
                    case RangeThumbType.EndThumb:
                        slider.OnDecreaseEndSmall();
                        break;
                }
            }
        }


        private static void OnMaximizeRangeValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnMaximizeRangeValue();
            }
        }

        private static void OnMaximizeStartValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                if (slider.IsSingleValue)
                {
                    slider.OnMaximizeRangeValue();
                }
                else
                {
                    slider.OnMaximizeStartValue();
                }
            }
        }

        private static void OnMaximizeEndValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                if (slider.IsSingleValue)
                {
                    slider.OnMaximizeRangeValue();
                }
                else
                {
                    slider.OnMaximizeEndValue();
                }
            }
        }

        private static void OnMaximizeValueByKeyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                if (slider.IsSingleValue)
                {
                    slider.OnMaximizeRangeValue();
                }
                else
                {
                    switch (DetectCommandThumbType(e))
                    {
                        case RangeThumbType.StartThumb:
                            slider.OnMaximizeStartValue();
                            break;
                        case RangeThumbType.EndThumb:
                            slider.OnMaximizeEndValue();
                            break;
                    }
                }
            }
        }


        private static void OnMinimizeRangeValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnMinimizeRangeValue();
            }
        }

        private static void OnMinimizeStartValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                if (slider.IsSingleValue)
                {
                    slider.OnMinimizeRangeValue();
                }
                else
                {
                    slider.OnMinimizeStartValue();
                }
            }
        }

        private static void OnMinimizeEndValueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                if (slider.IsSingleValue)
                {
                    slider.OnMinimizeRangeValue();
                }
                else
                {
                    slider.OnMinimizeEndValue();
                }
            }
        }

        private static void OnMinimizeValueByKeyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                if (slider.IsSingleValue)
                {
                    slider.OnMinimizeRangeValue();
                }
                else
                {
                    switch (DetectCommandThumbType(e))
                    {
                        case RangeThumbType.StartThumb:
                            slider.OnMinimizeStartValue();
                            break;
                        case RangeThumbType.EndThumb:
                            slider.OnMinimizeEndValue();
                            break;
                    }
                }
            }
        }


        private static void OnIncreaseRangeLargeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnIncreaseRangeLarge();
            }
        }

        private static void OnIncreaseStartLargeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnIncreaseStartLarge();
            }
        }

        private static void OnIncreaseEndLargeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnIncreaseEndLarge();
            }
        }

        private static void OnIncreaseLargeByKeyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                switch (DetectCommandThumbType(e))
                {
                    case RangeThumbType.StartThumb:
                        slider.OnIncreaseStartLarge();
                        break;
                    case RangeThumbType.EndThumb:
                        slider.OnIncreaseEndLarge();
                        break;
                }
            }
        }


        private static void OnDecreaseRangeLargeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnDecreaseRangeLarge();
            }
        }

        private static void OnDecreaseStartLargeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnDecreaseStartLarge();
            }
        }

        private static void OnDecreaseEndLargeCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                slider.OnDecreaseEndLarge();
            }
        }

        private static void OnDecreaseLargeByKeyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var slider = sender as SimpleRangeSlider<T, TInterval>;
            if (slider != null)
            {
                switch (DetectCommandThumbType(e))
                {
                    case RangeThumbType.StartThumb:
                        slider.OnDecreaseStartLarge();
                        break;
                    case RangeThumbType.EndThumb:
                        slider.OnDecreaseEndLarge();
                        break;
                }
            }
        }


        private static RangeThumbType DetectCommandThumbType(ExecutedRoutedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            var res = RangeThumbType.None;

            bool hasLeftCtrl = Keyboard.IsKeyDown(Key.LeftCtrl);
            bool hasRightCtrl = Keyboard.IsKeyDown(Key.RightCtrl);
            Contract.Assume(hasLeftCtrl ^ hasRightCtrl);

            if (hasLeftCtrl)
            {
                res = RangeThumbType.StartThumb;
            }
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            else if (hasRightCtrl)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            {
                res = RangeThumbType.EndThumb;
            }
            Contract.Assert(res != RangeThumbType.None, "res != RangeThumbType.None");

            return res;
        }

        #endregion Commands

        #region Dependency Properties

        #region Orientation Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.Orientation"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty OrientationProperty =
// ReSharper restore StaticFieldInGenericType
                DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SimpleRangeSlider<T, TInterval>),
                                          new FrameworkPropertyMetadata(Orientation.Horizontal),
                                          DependencyPropertyUtil.IsValidOrientation);

        /// <summary>
        /// Ориентация
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public Orientation Orientation
        {
            get
            {
                var res = GetValue(OrientationProperty);
                Contract.Assume(res != null);
                return (Orientation) res;
            }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region IsDragRangeEnabled Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.IsDragRangeEnabled"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty IsDragRangeEnabledProperty =
// ReSharper restore StaticFieldInGenericType
                DependencyProperty.Register("IsDragRangeEnabled", typeof(bool), typeof(SimpleRangeSlider<T, TInterval>),
                                          new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Разрешено ли перетаскивание интервала соответствующим ползунком
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsDragRangeEnabled
        {
            get
            {
                var res = GetValue(IsDragRangeEnabledProperty);
                Contract.Assume(res != null);
                return (bool) res;
            }
            set { SetValue(IsDragRangeEnabledProperty, value); }
        }

        #endregion

        #region IsRangeDragging

        /// <summary>
        /// Находится ли контрол в состоянии изменения значения области выделения путем перетаскивания любого из ползунков.
        /// Позволяет узнать о том, что пользователь находится в процессе выбора величины области выделения,
        /// захватив и перемещая в данный момент один из ползунков.
        /// </summary>
        [Category("Common")]
        public bool IsRangeDragging
        {
            get
            {
                var res = GetValue(IsRangeDraggingProperty);
                Contract.Assume(res != null);
                return (bool) res;
            }
            private set { SetValue(IsRangeDraggingPropertyKey, value); }
        }

// ReSharper disable StaticFieldInGenericType
// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey IsRangeDraggingPropertyKey =
// ReSharper restore InconsistentNaming
// ReSharper restore StaticFieldInGenericType
            DependencyProperty.RegisterReadOnly(
                "IsRangeDragging",
                typeof(bool),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(false,
                    OnIsRangeDraggingChanged));

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty IsRangeDraggingProperty = IsRangeDraggingPropertyKey.DependencyProperty;
// ReSharper restore StaticFieldInGenericType

        private static void OnIsRangeDraggingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Contract.Requires(obj is SimpleRangeSlider<T, TInterval>);
            Contract.Requires(args.OldValue is bool);
            Contract.Requires(args.NewValue is bool);

            var element = obj as SimpleRangeSlider<T, TInterval>;
            Contract.Assert(element != null, "element != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (element == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

            element.OnIsRangeDraggingChanged((bool)args.OldValue, (bool)args.NewValue);
        }

        /// <summary>
        /// Обработчик изменения значения свойства <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/>
        /// </summary>
        /// <param name="oldValue">Предыдущее значение</param>
        /// <param name="newValue">Новое значение</param>
        protected virtual void OnIsRangeDraggingChanged(bool oldValue, bool newValue)
        {
            var newEventArgs = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue)
            {
                RoutedEvent = IsRangeDraggingChangedEvent
            };
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region IsRaiseValueChangedWhileDragging Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.IsRaiseValueChangedWhileDragging"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty IsRaiseValueChangedWhileDraggingProperty =
// ReSharper restore StaticFieldInGenericType
            DependencyProperty.Register(
                "IsRaiseValueChangedWhileDragging",
                typeof(bool),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender),
                DependencyPropertyUtil.IsValidBoolValue);

        /// <summary>
        /// Генерировать событие <see cref="RangeBaseControl{T, TInterval}.ValueChanged"/>,
        /// когда элемент находится в состоянии изменения значения области выделения путем перетаскивания любого из ползунков,
        /// иными словами, когда <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/> <c>== true</c>.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsRaiseValueChangedWhileDragging
        {
            get
            {
                var res = GetValue(IsRaiseValueChangedWhileDraggingProperty);
                Contract.Assume(res != null);
                return (bool) res;
            }
            set { SetValue(IsRaiseValueChangedWhileDraggingProperty, value); }
        }

        #endregion

        #region Delay Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.Delay"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty DelayProperty = RepeatButton.DelayProperty.AddOwner(
// ReSharper restore StaticFieldInGenericType
            typeof(SimpleRangeSlider<T, TInterval>),
            new FrameworkPropertyMetadata(GetKeyboardDelay()));

        internal static int GetKeyboardDelay()
        {
            int keyboardDelay = SystemParameters.KeyboardDelay;
            if ((keyboardDelay < 0) || (keyboardDelay > 3))
            {
                keyboardDelay = 0;
            }
            return ((keyboardDelay + 1) * 250);
        }

        /// <summary>
        /// Определяет величину ожидания в милисекундах до того, как начинается повторение.
        /// Должно быть неотрицательной.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public int Delay
        {
            get
            {
                var res = GetValue(DelayProperty);
                Contract.Assume(res != null);
                return (int)res;
            }
            set
            {
                SetValue(DelayProperty, value);
            }
        }

        #endregion Delay Property

        #region Interval Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.Interval"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty IntervalProperty = RepeatButton.IntervalProperty.AddOwner(
// ReSharper restore StaticFieldInGenericType
            typeof(SimpleRangeSlider<T, TInterval>),
            new FrameworkPropertyMetadata(GetKeyboardSpeed()));

        internal static int GetKeyboardSpeed()
        {
            int keyboardSpeed = SystemParameters.KeyboardSpeed;
            if ((keyboardSpeed < 0) || (keyboardSpeed > 0x1f))
            {
                keyboardSpeed = 0x1f;
            }
            return ((((0x1f - keyboardSpeed) * 0x16f) / 0x1f) + 0x21);
        }

        /// <summary>
        /// Определяет величину времени в милисекундах между повторениями, когда повторения начато.
        /// Должна быть неотрицательной.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public int Interval
        {
            get
            {
                var res = GetValue(IntervalProperty);
                Contract.Assume(res != null);
                return (int)res;
            }
            set
            {
                SetValue(IntervalProperty, value);
            }
        }

        #endregion Interval Property


        #region AutoToolTipValueConverter Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty AutoToolTipValueConverterProperty =
// ReSharper restore StaticFieldInGenericType
            DependencyProperty.Register(
                "AutoToolTipValueConverter",
                typeof(IRangeValueToStringConverter<T>),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Конвертер значений в их строковые представления для отображения во всплывающих подсказках.
        /// Если задано это значение, то <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> игнорируется.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public IRangeValueToStringConverter<T> AutoToolTipValueConverter
        {
            get { return (IRangeValueToStringConverter<T>)GetValue(AutoToolTipValueConverterProperty); }
            set { SetValue(AutoToolTipValueConverterProperty, value); }
        }

        #endregion

        #region AutoToolTipValueConverterParameter Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverterParameter"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty AutoToolTipValueConverterParameterProperty =
// ReSharper restore StaticFieldInGenericType
            DependencyProperty.Register(
                "AutoToolTipValueConverterParameter",
                typeof(object),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Параметр конвертера числовых значений в их строковые представления
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public object AutoToolTipValueConverterParameter
        {
            get { return GetValue(AutoToolTipValueConverterParameterProperty); }
            set { SetValue(AutoToolTipValueConverterParameterProperty, value); }
        }

        #endregion

        #region AutoToolTipFormat Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty AutoToolTipFormatProperty
// ReSharper restore StaticFieldInGenericType
            = DependencyProperty.Register("AutoToolTipFormat", typeof(string), typeof(SimpleRangeSlider<T, TInterval>),
                                          new FrameworkPropertyMetadata());

        /// <summary>
        /// Формат отображения числа во всплывающей подсказке.
        /// Используется только, если не задан <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public string AutoToolTipFormat
        {
            get
            {
                return (string)GetValue(AutoToolTipFormatProperty);
            }
            set
            {
                SetValue(AutoToolTipFormatProperty, value);
            }
        }

        #endregion

        #region AutoToolTipPlacement Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipPlacement"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty AutoToolTipPlacementProperty
// ReSharper restore StaticFieldInGenericType
            = DependencyProperty.Register("AutoToolTipPlacement", typeof(AutoToolTipPlacement), typeof(SimpleRangeSlider<T, TInterval>),
                                          new FrameworkPropertyMetadata(AutoToolTipPlacement.None),
                                          DependencyPropertyUtil.IsValidAutoToolTipPlacement);

        /// <summary>
        /// Относительное метоположение отображения всплывающей подсказки
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get
            {
                var res = GetValue(AutoToolTipPlacementProperty);
                Contract.Assume(res != null);
                return (AutoToolTipPlacement)res;
            }
            set
            {
                SetValue(AutoToolTipPlacementProperty, value);
            }
        }

        #endregion


        #region StartReservedSpace Property

// ReSharper disable StaticFieldInGenericType
// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey StartReservedSpacePropertyKey
// ReSharper restore InconsistentNaming
// ReSharper restore StaticFieldInGenericType
            = DependencyProperty.RegisterReadOnly("StartReservedSpace", typeof(double), typeof(SimpleRangeSlider<T, TInterval>),
            new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.StartReservedSpace"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty StartReservedSpaceProperty = StartReservedSpacePropertyKey.DependencyProperty;
// ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Слайдер использует <see cref="SimpleRangeSlider{T, TInterval}.StartReservedSpaceProperty"/>
        /// для вычисления отступа шкалы делений (<see cref="TickBar"/>) слева/снизу
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public double StartReservedSpace
        {
            get
            {
                var res = GetValue(StartReservedSpaceProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            private set { SetValue(StartReservedSpacePropertyKey, value); }
        }

        #endregion

        #region EndReservedSpace Property

// ReSharper disable StaticFieldInGenericType
// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey EndReservedSpacePropertyKey
// ReSharper restore InconsistentNaming
// ReSharper restore StaticFieldInGenericType
            = DependencyProperty.RegisterReadOnly("EndReservedSpace", typeof(double), typeof(SimpleRangeSlider<T, TInterval>),
            new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.EndReservedSpace"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty EndReservedSpaceProperty = EndReservedSpacePropertyKey.DependencyProperty;
// ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Слайдер использует <see cref="SimpleRangeSlider{T, TInterval}.EndReservedSpaceProperty"/>
        /// для вычисления отступа шкалы делений (<see cref="TickBar"/>) справа/сверху
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public double EndReservedSpace
        {
            get
            {
                var res = GetValue(EndReservedSpaceProperty);
                Contract.Assume(res != null);
                return (double)res;
            }
            private set { SetValue(EndReservedSpacePropertyKey, value); }
        }

        #endregion


        #region TickMark support

        #region IsSnapToTickEnabled property

        /// <summary>
        /// Если <c>true</c>, то слайдер будет автоматически смещать ползунок к ближайшей метке на шкале
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsSnapToTickEnabled
        {
            get
            {
                var res = GetValue(IsSnapToTickEnabledProperty);
                Contract.Assume(res != null);
                return (bool)res;
            }
            set
            {
                SetValue(IsSnapToTickEnabledProperty, value);
            }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty IsSnapToTickEnabledProperty
// ReSharper restore StaticFieldInGenericType
            = DependencyProperty.Register("IsSnapToTickEnabled",
            typeof(bool),
            typeof(SimpleRangeSlider<T, TInterval>),
            new FrameworkPropertyMetadata(false, OnIsSnapToTickEnabledChanged));

        private static void OnIsSnapToTickEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Contract.Requires(obj is SimpleRangeSlider<T, TInterval>);
            Contract.Requires(args.OldValue is bool);
            Contract.Requires(args.NewValue is bool);

            var element = obj as SimpleRangeSlider<T, TInterval>;
            Contract.Assert(element != null, "element != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (element == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

            var newValue = (bool)args.NewValue;
            if (newValue)
            {
                // если перемещение ползунков возможно только по меткам на шкале,
                // то проверка на минимальный размер интервала отключается из-за сложности вычисления этого размера
                // для неравномерных меток.
                // Теоретически эту проверку можно не отключать,
                // но для этого нужно принудительно сделать минимальный размер кратным шагу меток (для равномерных),
                // либо равным минимальному из расстояний между метками на шкале (для неравномерных). Т.е. так или иначе
                // отслеживание свойства MinRangeValue становится трудоемким и неочевидным.
                element.MinRangeValueEnabled = false;

                // если флажок взводится (теперь ползунки могут быть только на метках шкалы),
                // то надо принудительно выравнять текущее положение ползунков до ближайших меток.
                // Минус этого шага: изменяются положения ползунков и, как следствие, размер интервала
                // (но иногда этого может и не происходить).
                element.AlignValuesToTicks();
            }
            else
            {
                // если для слайдера отключается режим единого (одного) значения,
                // то проверка на минимальный размер интервала можно включить, а можно и не включать -
                // все зависит от значения этого признака по умолчанию в элементе управления.
                element.MinRangeValueEnabled = element.DefaultMinRangeValueEnabled;
            }
        }

        #endregion

        #region TickPlacement property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.TickPlacement"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty TickPlacementProperty
// ReSharper restore StaticFieldInGenericType
            = DependencyProperty.Register("TickPlacement", typeof(TickPlacement), typeof(SimpleRangeSlider<T, TInterval>),
                                          new FrameworkPropertyMetadata(TickPlacement.None),
                                          IsValidTickPlacement);

        /// <summary>
        /// Местоположение шкалы делений относительно ползунков
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public TickPlacement TickPlacement
        {
            get
            {
                var res = GetValue(TickPlacementProperty);
                Contract.Assume(res != null);
                return (TickPlacement)res;
            }
            set
            {
                SetValue(TickPlacementProperty, value);
            }
        }

        private static bool IsValidTickPlacement(object o)
        {
            if (!(o is TickPlacement))
                return false;

            var value = (TickPlacement)o;
            return value == TickPlacement.None ||
                   value == TickPlacement.TopLeft ||
                   value == TickPlacement.BottomRight ||
                   value == TickPlacement.Both;
        }

        #endregion

        #region TickFrequency property

        /// <summary>
        /// Частота (или расстояние между ближайшими метками) на шкале делений.
        /// Когда <see cref="SimpleRangeSlider{T, TInterval}.TypedTicksCollection"/> не равен <c>null</c>
        /// слайдер игнорирует <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>
        /// и рисует только те метки, которые заданы в коллекции <see cref="SimpleRangeSlider{T, TInterval}.TypedTicksCollection"/>.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public TInterval TickFrequency
        {
            get
            {
                var res = GetValue(TickFrequencyProperty);
                Contract.Assume(res != null);
                return (TInterval)res;
            }
            set
            {
                SetValue(TickFrequencyProperty, value);
            }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty TickFrequencyProperty
// ReSharper restore StaticFieldInGenericType
            = DependencyProperty.Register("TickFrequency", typeof(TInterval), typeof(SimpleRangeSlider<T, TInterval>),
            new FrameworkPropertyMetadata(default(TInterval), OnTickFrequencyChanged),
            IsValidTickFrequency);

        private static void OnTickFrequencyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Contract.Requires(obj is SimpleRangeSlider<T, TInterval>);
            Contract.Requires(args.OldValue is TInterval);
            Contract.Requires(args.NewValue is TInterval);

            var element = obj as SimpleRangeSlider<T, TInterval>;
            Contract.Assert(element != null, "element != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (element == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

            if (element.IsSnapToTickEnabled)
            {
                // если местоположение меток меняется и при этом включена привязка к ближайшей метке,
                // то надо принудительно выравнять текущее положение ползунков до ближайших меток.
                // Минус этого шага: изменяются положения ползунков и, как следствие, размер интервала
                // (но иногда этого может и не происходить).
                element.AlignValuesToTicks();
            }

            element.OnTickFrequencyChanged((TInterval)args.OldValue, (TInterval)args.NewValue);
        }

        /// <summary>
        /// Вызывается, когда изменяется значение <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>
        /// </summary>
        /// <param name="oldTickFrequency">Предыдущее значение</param>
        /// <param name="newTickFrequency">Новое значение</param>
        protected virtual void OnTickFrequencyChanged(TInterval oldTickFrequency, TInterval newTickFrequency)
        { }

        internal static bool IsValidTickFrequency(object value)
        {
            return DependencyPropertyUtil.IsValidChange(typeof(TInterval), value);
        }

        #endregion

        #region TickLabelConverter Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.TickLabelConverter"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty TickLabelConverterProperty =
// ReSharper restore StaticFieldInGenericType
            DependencyProperty.Register(
                "TickLabelConverter",
                typeof(IDoubleToStringConverter),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Конвертер числовых значений меток шкалы делений в их строковые представления для отображения пользователю.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public IDoubleToStringConverter TickLabelConverter
        {
            get { return (IDoubleToStringConverter)GetValue(TickLabelConverterProperty); }
            set { SetValue(TickLabelConverterProperty, value); }
        }

        #endregion

        #region TickLabelConverterParameter Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.TickLabelConverterParameter"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty TickLabelConverterParameterProperty =
// ReSharper restore StaticFieldInGenericType
            DependencyProperty.Register(
                "TickLabelConverterParameter",
                typeof(object),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Параметр конвертера числовых значений меток шкалы делений в их строковые представления для отображения пользователю.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public object TickLabelConverterParameter
        {
            get { return GetValue(TickLabelConverterParameterProperty); }
            set { SetValue(TickLabelConverterParameterProperty, value); }
        }

        #endregion

        #endregion TickMark support

        #endregion // Dependency Properties

        #region Public Events

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDraggingChanged"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly RoutedEvent IsRangeDraggingChangedEvent =
// ReSharper restore StaticFieldInGenericType
            EventManager.RegisterRoutedEvent(
                "IsRangeDraggingChanged",
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(SimpleRangeSlider<T, TInterval>));

        /// <summary>
        /// Событие изменения значения свойства <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/>
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsRangeDraggingChanged
        {
            add { AddHandler(IsRangeDraggingChangedEvent, value); }
            remove { RemoveHandler(IsRangeDraggingChangedEvent, value); }
        }


        /// <summary>
        /// Свойство зависимости для <see cref="SimpleRangeSlider{T, TInterval}.RangeDragCompleted"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly RoutedEvent RangeDragCompletedEvent =
// ReSharper restore StaticFieldInGenericType
            EventManager.RegisterRoutedEvent(
                "RangeDragCompleted",
                RoutingStrategy.Bubble,
                typeof(EventHandler<RangeDragCompletedEventArgs<T>>),
                typeof(SimpleRangeSlider<T, TInterval>));

        /// <summary>
        /// Событие завершения перетаскивания одного из ползунков
        /// </summary>
        public event EventHandler<RangeDragCompletedEventArgs<T>> RangeDragCompleted
        {
            add { AddHandler(RangeDragCompletedEvent, value); }
            remove { RemoveHandler(RangeDragCompletedEvent, value); }
        }

        /// <summary>
        /// Вызывается, когда пользователь прекращает перетаскивать ползунок.
        /// Здесь просто инициируется событие <see cref="SimpleRangeSlider{T, TInterval}.RangeDragCompleted"/>.
        /// </summary>
        /// <param name="oldStartValue">Предыдущее значение начала интервала</param>
        /// <param name="oldEndValue">Предыдущее значение конца интервала</param>
        /// <param name="newStartValue">Новое значение начала интервала</param>
        /// <param name="newEndValue">Новое значение конца интервала</param>
        protected virtual void OnRangeDragCompleted(T oldStartValue, T oldEndValue,
            T newStartValue, T newEndValue)
        {
            var newEventArgs = new RangeDragCompletedEventArgs<T>(oldStartValue, oldEndValue, newStartValue, newEndValue)
            {
                RoutedEvent = RangeDragCompletedEvent
            };
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region Event Handlers

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            Contract.Requires<ArgumentException>(sender is SimpleRangeSlider<T, TInterval>);

            var slider = sender as SimpleRangeSlider<T, TInterval>;
            slider.OnThumbDragStarted(e);
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            Contract.Requires<ArgumentException>(e.OriginalSource is Thumb);
            Contract.Requires<ArgumentException>(sender is SimpleRangeSlider<T, TInterval>);

            var slider = sender as SimpleRangeSlider<T, TInterval>;
            slider.OnThumbDragDelta(e);
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Contract.Requires<ArgumentException>(sender is SimpleRangeSlider<T, TInterval>);

            var slider = sender as SimpleRangeSlider<T, TInterval>;
            slider.OnThumbDragCompleted(e);
        }

        /// <summary>
        /// Вызывается, когда пользователь начинает перетаскивать ползунок
        /// </summary>
        /// <param name="e">Параметры события</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected virtual void OnThumbDragStarted(DragStartedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(e != null);

            // Show AutoToolTip if needed.
            var thumb = e.OriginalSource as Thumb;

            if ((thumb == null) || (this.AutoToolTipPlacement == AutoToolTipPlacement.None))
            {
                return;
            }
            RangeThumbType thumbType = GetThumbType(thumb);

            Contract.Assume(!m_RangeValueData.IsRangeDragging);
            m_RangeValueData.IsRangeDragging = true;
            m_RangeValueData.RangeStart = StartValue;
            m_RangeValueData.RangeEnd = EndValue;

            // Save original tooltip
            m_ThumbOriginalToolTip = thumb.ToolTip;

            if (m_AutoToolTip == null)
            {
                m_AutoToolTip = new ToolTip
                    {
                        Placement = PlacementMode.Custom,
                        CustomPopupPlacementCallback =
                            this.AutoToolTipCustomPlacementCallback
                    };
            }
            m_AutoToolTip.PlacementTarget = thumb;
            m_AutoToolTip.Tag = thumbType;

            thumb.ToolTip = m_AutoToolTip;
            m_AutoToolTip.Content = GetAutoToolTipContent(IsSingleValue ? RangeThumbType.StartThumb : thumbType);
            m_AutoToolTip.IsOpen = true;

            var offset = m_AutoToolTip.HorizontalOffset;
            m_AutoToolTip.HorizontalOffset = offset + 1;
            m_AutoToolTip.HorizontalOffset = offset;
            //((Popup)m_AutoToolTip.Parent).Reposition();
        }

        /// <summary>
        /// Вызывается, когда пользователь смещает захваченный мышкой ползунок
        /// </summary>
        /// <param name="e">Параметры события</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected virtual void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(e != null);
            Contract.Requires<ArgumentException>(e.OriginalSource is Thumb);

            IsRangeDragging = true; // сделано здесь для того, чтобы не выставлять зря этот признак в обработчике DragStarted,
            // т.к. в DragStarted он будет выставлен даже если реального смещения не произошло, а был только лишь щелчок мышкой на элементе (захват)

            var thumb = e.OriginalSource as Thumb;
            RangeThumbType thumbType = GetThumbType(thumb);
            // Convert to Track's co-ordinate
            if (Track != null && thumbType != RangeThumbType.None)
            {
                double valueFromDistance = Track.ValueFromDistance(e.HorizontalChange, e.VerticalChange);

                UpdateValueByThumbTypeAndDelta(IsSingleValue ? RangeThumbType.RangeThumb : thumbType, valueFromDistance);

                // Show AutoToolTip if needed
                if (this.AutoToolTipPlacement != AutoToolTipPlacement.None)
                {
                    if (m_AutoToolTip == null)
                    {
                        m_AutoToolTip = new ToolTip {Tag = thumbType};
                    }

                    m_AutoToolTip.Content = GetAutoToolTipContent(IsSingleValue ? RangeThumbType.StartThumb : thumbType);

                    if (!Equals(thumb.ToolTip, this.m_AutoToolTip))
                    {
                        thumb.ToolTip = m_AutoToolTip;
                    }

                    //Contract.Assume(m_AutoToolTip != null);
                    if (!m_AutoToolTip.IsOpen)
                    {
                        m_AutoToolTip.IsOpen = true;
                    }
                    var offset = m_AutoToolTip.HorizontalOffset;
                    m_AutoToolTip.HorizontalOffset = offset + 1;
                    m_AutoToolTip.HorizontalOffset = offset;
                    //((Popup)m_AutoToolTip.Parent).Reposition();
                }
            }
        }

        /// <summary>
        /// Вызывается, когда пользователь прекращает перетаскивать ползунок
        /// </summary>
        /// <param name="e">Параметры события</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected virtual void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            Contract.Requires<ArgumentNullException>(e != null);

            var thumb = e.OriginalSource as Thumb;

            if ((thumb == null) || (this.AutoToolTipPlacement == AutoToolTipPlacement.None))
            {
                return;
            }
            //RangeThumbType thumbType = GetThumbType(thumb);

            Contract.Assume(m_RangeValueData.IsRangeDragging);
            var oldRangeValueData = m_RangeValueData;
            m_RangeValueData.IsRangeDragging = false;

            if (IsRangeDragging)
            {
                IsRangeDragging = false;
                OnRangeDragCompleted(oldRangeValueData.RangeStart, oldRangeValueData.RangeEnd,
                    StartValue, EndValue);
                // если генерация события ValueChanged была отключена,
                // то это событие надо сгенерить вручную по окончании перетаскивания ползунков, иначе оно будет "проглочено".
                if (!IsRaiseValueChangedWhileDragging &&
                    (!DoubleUtil.AreClose(ValueToDouble(oldRangeValueData.RangeStart), ValueToDouble(StartValue)) ||
                     !DoubleUtil.AreClose(ValueToDouble(oldRangeValueData.RangeEnd), ValueToDouble(EndValue))))
                {
                    base.OnValueChanged(oldRangeValueData.RangeStart, oldRangeValueData.RangeEnd, StartValue, EndValue);
                }
            }

            // Show AutoToolTip if needed.
            if (m_AutoToolTip != null)
            {
                m_AutoToolTip.IsOpen = false;
            }

            thumb.ToolTip = m_ThumbOriginalToolTip;
        }

        #endregion Event Handlers

        #region Override Functions

        /// <summary>
        /// Обработчик события <see cref="Mouse.MouseDownEvent"/>.
        /// Его назначение перенести фокус ввода на <see cref="SimpleRangeSlider{T, TInterval}"/>,
        /// когда пользователь нажимает левую кнопку мыши на любой части данного слайдера, которая не может иметь фокус ввода
        /// (is not focusable).
        /// </summary>
        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            var slider = (SimpleRangeSlider<T, TInterval>)sender;

            // When someone click on the SimpleRangeSlider's part, and it's not focusable
            // SimpleRangeSlider need to take the focus in order to process keyboard correctly
            if (!slider.IsKeyboardFocusWithin)
            {
                e.Handled = slider.Focus() || e.Handled;
            }
        }

        /// <summary>
        /// Выполняет расположение контрола <see cref="SimpleRangeSlider{T, TInterval}" />.
        /// </summary>
        /// <param name="finalSize">Область, доступная для <see cref="SimpleRangeSlider{T, TInterval}" />.</param>
        /// <returns>Размер <see cref="Size"/>, который будет использоваться для содержимого <see cref="SimpleRangeSlider{T, TInterval}" />.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);

            UpdateTrackBackgroundPositionAndSize();

            return size;
        }

        /// <summary>
        /// Вызывается, когда изменяется значение свойства <see cref="RangeBaseControl{T, TInterval}.StartValue"/>.
        /// Перегружено здесь для того, чтобы обновить расположение и размер элемента <see cref="SimpleRangeSlider{T, TInterval}.TrackBackground"/>.
        /// </summary>
        protected override void OnStartValueChanged(T oldValue, T newValue)
        {
            base.OnStartValueChanged(oldValue, newValue);
            UpdateTrackBackgroundPositionAndSize();
        }

        /// <summary>
        /// Обработчик <see cref="FrameworkElement.OnApplyTemplate"/>.
        /// Вызывается, когда создано визуальное дерева элемента.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TrackBackground = GetTemplateChild(c_TrackBackgroundName) as FrameworkElement;

            Track = GetTemplateChild(c_TrackName) as RangeTrack<T, TInterval>;
            if (Track != null)
            {
                Track.DoApplyTemplate();

                if (Track.StartThumb != null)
                {
                    Track.StartThumb.SizeChanged += (s, e) =>
                        {
                            var thumb = s as Thumb;
                            if (thumb != null)
                            {
                                this.StartReservedSpace = (this.Orientation == Orientation.Horizontal) ?
                                                         thumb.ActualWidth :
                                                         thumb.ActualHeight;
                            }
                        };
                }

                if (Track.EndThumb != null)
                {
                    Track.EndThumb.SizeChanged += (s, e) =>
                        {
                            var thumb = s as Thumb;
                            if (thumb != null)
                            {
                                this.EndReservedSpace = (this.Orientation == Orientation.Horizontal) ?
                                                       thumb.ActualWidth :
                                                       thumb.ActualHeight;
                            }
                        };
                }
            }

            if (m_AutoToolTip != null)
            {
                Contract.Assume(m_AutoToolTip.Tag is RangeThumbType);
// ReSharper disable ConditionIsAlwaysTrueOrFalse
                RangeThumbType thumbType = (m_AutoToolTip.Tag is RangeThumbType) ?
// ReSharper restore ConditionIsAlwaysTrueOrFalse
                    (RangeThumbType)m_AutoToolTip.Tag :
                    RangeThumbType.None;
                Thumb targetThumb = null;
                if (Track != null)
                {
                    switch (thumbType)
                    {
                        case RangeThumbType.StartThumb:
                            targetThumb = Track.StartThumb;
                            break;
                        case RangeThumbType.RangeThumb:
                            targetThumb = Track.RangeThumb;
                            break;
                        case RangeThumbType.EndThumb:
                            targetThumb = Track.EndThumb;
                            break;
                    }
                }
                m_AutoToolTip.PlacementTarget = targetThumb;
            }
        }

        /// <summary>
        /// Вызывается, когда меняется хотя бы одно из значений <see cref="RangeBaseControl{T, TInterval}.StartValue"/>,
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/> или <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>.
        /// Перегружено здесь для того, чтобы учесть <see cref="SimpleRangeSlider{T, TInterval}.IsRaiseValueChangedWhileDragging"/>.
        /// </summary>
        /// <param name="oldStartValue">Старое значение начала интервала</param>
        /// <param name="oldEndValue">Старое значение конца интервала</param>
        /// <param name="newStartValue">Новое значение начала интервала</param>
        /// <param name="newEndValue">Новое значение конца интервала</param>
        protected override void OnValueChanged(T oldStartValue, T oldEndValue, T newStartValue, T newEndValue)
        {
            if (!IsRangeDragging || IsRaiseValueChangedWhileDragging)
            {
                base.OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
            }
        }

        #endregion Override Functions

        #region Abstract Functions

        /// <summary>
        /// Свойство возвращает коллекцию значений меток соответствующего типа для шкалы делений.
        /// Должно быть реализовано в наследниках.
        /// </summary>
        protected abstract ITicksCollection<T> TypedTicksCollection { get; }

        /// <summary>
        /// Метод возвращает строку для отображения во всплывающей подсказке.
        /// Должен быть реализован в наследниках.
        /// </summary>
        /// <param name="value">Текущее значение</param>
        /// <param name="thumbType">Тип ползунка, значение которого <paramref name="value"/>.</param>
        /// <returns>Строковое представление значения <paramref name="value"/> для ползунка, заданного <paramref name="thumbType"/>.</returns>
        [Pure]
        protected abstract string GetAutoToolTipString(T value, RangeThumbType thumbType);

        #endregion

        #region Virtual Functions

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.IncreaseRangeLarge"/>
        /// </summary>
        protected virtual void OnIncreaseRangeLarge()
        {
            MoveRangeToNextTick(this.LargeChange, false);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.IncreaseStartLarge"/>
        /// </summary>
        protected virtual void OnIncreaseStartLarge()
        {
            MoveToNextTick(this.LargeChange, false, true);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.IncreaseEndLarge"/>
        /// </summary>
        protected virtual void OnIncreaseEndLarge()
        {
            MoveToNextTick(this.LargeChange, false, false);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.DecreaseRangeLarge"/>
        /// </summary>
        protected virtual void OnDecreaseRangeLarge()
        {
            MoveRangeToNextTick(this.LargeChange, true);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.DecreaseStartLarge"/>
        /// </summary>
        protected virtual void OnDecreaseStartLarge()
        {
            MoveToNextTick(this.LargeChange, true, true);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.DecreaseEndLarge"/>
        /// </summary>
        protected virtual void OnDecreaseEndLarge()
        {
            MoveToNextTick(this.LargeChange, true, false);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.IncreaseRangeSmall"/>
        /// </summary>
        protected virtual void OnIncreaseRangeSmall()
        {
            MoveRangeToNextTick(this.SmallChange, false);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.IncreaseStartSmall"/>
        /// </summary>
        protected virtual void OnIncreaseStartSmall()
        {
            MoveToNextTick(this.SmallChange, false, true);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.IncreaseEndSmall"/>
        /// </summary>
        protected virtual void OnIncreaseEndSmall()
        {
            MoveToNextTick(this.SmallChange, false, false);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.DecreaseRangeSmall"/>
        /// </summary>
        protected virtual void OnDecreaseRangeSmall()
        {
            MoveRangeToNextTick(this.SmallChange, true);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.DecreaseStartSmall"/>
        /// </summary>
        protected virtual void OnDecreaseStartSmall()
        {
            MoveToNextTick(this.SmallChange, true, true);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.DecreaseEndSmall"/>
        /// </summary>
        protected virtual void OnDecreaseEndSmall()
        {
            MoveToNextTick(this.SmallChange, true, false);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.MaximizeRangeValue"/>
        /// </summary>
        protected virtual void OnMaximizeRangeValue()
        {
            double delta = ValueToDouble(Maximum) - ValueToDouble(EndValue);
            TInterval oldRangeValue = RangeValue;
            T oldStartValue = StartValue, oldEndValue = EndValue;
            IsRangeValueChanging = true;
            try
            {
                this.SetCurrentValue(EndValueProperty, Maximum);
                this.SetCurrentValue(StartValueProperty, DoubleToValue(ValueToDouble(StartValue) + delta));
            }
            finally
            {
                IsRangeValueChanging = false;
            }
            TInterval newRangeValue = RangeValue;
            T newStartValue = StartValue, newEndValue = EndValue;
            if (!DoubleUtil.AreClose(ValueToDouble(oldStartValue), ValueToDouble(newStartValue)) ||
                !DoubleUtil.AreClose(ValueToDouble(oldEndValue), ValueToDouble(newEndValue)))
            {
                OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
            }
            if (!DoubleUtil.AreClose(IntervalToDouble(oldRangeValue), IntervalToDouble(newRangeValue)))
            {
                OnRangeValueChanged(oldRangeValue, newRangeValue);
            }
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.MaximizeStartValue"/>
        /// </summary>
        protected virtual void OnMaximizeStartValue()
        {
            this.SetCurrentValue(StartValueProperty, Maximum);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.MaximizeEndValue"/>
        /// </summary>
        protected virtual void OnMaximizeEndValue()
        {
            this.SetCurrentValue(EndValueProperty, Maximum);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.MinimizeRangeValue"/>
        /// </summary>
        protected virtual void OnMinimizeRangeValue()
        {
            double delta = ValueToDouble(StartValue) - ValueToDouble(Minimum);
            TInterval oldRangeValue = RangeValue;
            T oldStartValue = StartValue, oldEndValue = EndValue;
            IsRangeValueChanging = true;
            try
            {
                this.SetCurrentValue(StartValueProperty, Minimum);
                this.SetCurrentValue(EndValueProperty, DoubleToValue(ValueToDouble(EndValue) - delta));
            }
            finally
            {
                IsRangeValueChanging = false;
            }
            TInterval newRangeValue = RangeValue;
            T newStartValue = StartValue, newEndValue = EndValue;
            if (!DoubleUtil.AreClose(ValueToDouble(oldStartValue), ValueToDouble(newStartValue)) ||
                !DoubleUtil.AreClose(ValueToDouble(oldEndValue), ValueToDouble(newEndValue)))
            {
                OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
            }
            if (!DoubleUtil.AreClose(IntervalToDouble(oldRangeValue), IntervalToDouble(newRangeValue)))
            {
                OnRangeValueChanged(oldRangeValue, newRangeValue);
            }
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.MinimizeStartValue"/>
        /// </summary>
        protected virtual void OnMinimizeStartValue()
        {
            this.SetCurrentValue(StartValueProperty, Minimum);
        }

        /// <summary>
        /// Обработчик вызова команды <see cref="SimpleRangeSlider{T, TInterval}.MinimizeEndValue"/>
        /// </summary>
        protected virtual void OnMinimizeEndValue()
        {
            this.SetCurrentValue(EndValueProperty, Minimum);
        }

        #endregion Virtual Functions

        #region Helper Functions

        [Pure]
        private string GetAutoToolTipContent(RangeThumbType thumbType)
        {
            var res = new StringBuilder();

            if (thumbType == RangeThumbType.StartThumb || thumbType == RangeThumbType.RangeThumb)
            {
                res.Append(GetAutoToolTipString(StartValue, thumbType) ?? string.Empty);
            }
            if (thumbType == RangeThumbType.RangeThumb)
            {
                res.Append(c_RangeDelimiter);
            }
            if (thumbType == RangeThumbType.EndThumb || thumbType == RangeThumbType.RangeThumb)
            {
                res.Append(GetAutoToolTipString(EndValue, thumbType) ?? string.Empty);
            }
            return res.ToString();
        }

        private CustomPopupPlacement[] AutoToolTipCustomPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            switch (this.AutoToolTipPlacement)
            {
                case AutoToolTipPlacement.TopLeft:
                    if (Orientation == Orientation.Horizontal)
                    {
                        // Place popup at top of thumb
                        return new[]
                            {
                                new CustomPopupPlacement(
                                    new Point((targetSize.Width - popupSize.Width)*0.5, -popupSize.Height),
                                    PopupPrimaryAxis.Horizontal)
                            };
                    }
                    // Place popup at left of thumb
                    return new[]
                        {
                            new CustomPopupPlacement(
                                new Point(-popupSize.Width, (targetSize.Height - popupSize.Height)*0.5),
                                PopupPrimaryAxis.Vertical)
                        };

                case AutoToolTipPlacement.BottomRight:
                    if (Orientation == Orientation.Horizontal)
                    {
                        // Place popup at bottom of thumb
                        return new[]
                            {
                                new CustomPopupPlacement(
                                    new Point((targetSize.Width - popupSize.Width)*0.5, targetSize.Height),
                                    PopupPrimaryAxis.Horizontal)
                            };
                    }
                    // Place popup at right of thumb
                    return new[]
                        {
                            new CustomPopupPlacement(
                                new Point(targetSize.Width, (targetSize.Height - popupSize.Height)*0.5),
                                PopupPrimaryAxis.Vertical)
                        };

                default:
                    return new CustomPopupPlacement[] { };
            }
        }

        /// <summary>
        /// Метод изменяет размер и местоположение элемента <see cref="SimpleRangeSlider{T, TInterval}.TrackBackground"/>
        /// </summary>
        private void UpdateTrackBackgroundPositionAndSize()
        {
            if (Track == null || TrackBackground == null)
            {
                return;
            }

            Size startThumbSize = (Track.StartThumb != null) ? Track.StartThumb.RenderSize : new Size(0d, 0d);
            Size endThumbSize = (Track.EndThumb != null) ? Track.EndThumb.RenderSize : new Size(0d, 0d);
            var margin = TrackBackground.Margin;
            if (Orientation == Orientation.Horizontal)
            {
                margin.Left = startThumbSize.Width;
                margin.Right = endThumbSize.Width;
            }
            else
            {
                margin.Top = startThumbSize.Height;
                margin.Bottom = endThumbSize.Height;
            }
            TrackBackground.Margin = margin;
        }

        /// <summary>
        /// Вычисляет ближайшее к переданному значению <paramref name="value"/> значение метки на шкале делений.
        /// Если входное значение находится точно посередине между двумя соседними метками, то ближайшей принимается с большим значением.
        /// </summary>
        /// <param name="value">Входное значение, для которого необходимо вычислить ближайшую метку.</param>
        /// <param name="isStartThumb">Входное значение представляет собой позицию начала интервала</param>
        /// <param name="useFullScale">Если <c>true</c>, то ближайшая метка ищется на всей шкале,
        /// независимо от значения соседнего ползунка -
        /// это означает, что найденная метка может лежать за пределами допустимого для заданного значения интервала.
        /// Если <c>false</c>, то ближайшая метка ищется в допустимом интервале - в этом случае может возникнуть ситуация,
        /// когда необходимая (ближайшая) метка находится за пределами допустимого интервала, тогда возвращается одна из границ интервала
        /// и если эта граница допустимого интервала сама не выровнена по шкале, то и возвращенное значение будет не лежать на метке.</param>
        /// <returns>Если <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/> равно <c>true</c>,
        /// то рассчитанное ближайшее значение метки на шкале делений.
        /// Иначе просто возвращается переданное <paramref name="value"/>.</returns>
        private double SnapToTick(double value, bool isStartThumb, bool useFullScale)
        {
            if (IsSnapToTickEnabled)
            {
                T min = Minimum, max = Maximum;
                double previous = ValueToDouble((useFullScale || isStartThumb) ? min : StartValue);
                double next = ValueToDouble((!useFullScale && isStartThumb) ? EndValue : max);

                ITicksCollection<T> ticks = TypedTicksCollection;

                // If ticks collection is available, use it.
                // Note that ticks may be unsorted.
                if ((ticks != null) && (ticks.Count > 0))
                {
                    for (int i = 0; i < ticks.Count; i++)
                    {
                        double tick = ValueToDouble(ticks[i]);
                        if (DoubleUtil.AreClose(tick, value))
                        {
                            return value;
                        }

                        if (DoubleUtil.LessThan(tick, value) && DoubleUtil.GreaterThan(tick, previous))
                        {
                            previous = tick;
                        }
                        else if (DoubleUtil.GreaterThan(tick, value) && DoubleUtil.LessThan(tick, next))
                        {
                            next = tick;
                        }
                    }
                }
                else if (DoubleUtil.GreaterThan(IntervalToDouble(TickFrequency), 0.0))
                {
                    double tickFrequency = IntervalToDouble(TickFrequency);
                    previous = ValueToDouble(min) + (Math.Round(((value - ValueToDouble(min)) / tickFrequency)) * tickFrequency);
                    next = Math.Min(ValueToDouble(max), previous + tickFrequency);
                }

                // Choose the closest value between previous and next. If tie, snap to 'next'.
                value = DoubleUtil.GreaterThanOrClose(value, (previous + next) * 0.5) ? next : previous;
            }

            return value;
        }

        /// <summary>
        /// Если <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/> равно <c>true</c>, то
        /// функция выравнивает значения <see cref="RangeBaseControl{T, TInterval}.StartValue"/> и
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/> смещая их к ближайшей метке на шкале.
        /// Есть вероятность того, что будет изменено значение интервала <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>.
        /// Если <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/> равно <c>false</c>, то функция ничего не делает.
        /// </summary>
        protected void AlignValuesToTicks()
        {
            if (!IsSnapToTickEnabled)
                return;

            T oldStartValue = StartValue, oldEndValue = EndValue;
            double startValue = ValueToDouble(oldStartValue), endValue = ValueToDouble(oldEndValue);
            double snappedStartValue = SnapToTick(startValue, true, true);
            double snappedEndValue = SnapToTick(endValue, false, true);

            Contract.Assume(DoubleUtil.LessThanOrClose(snappedStartValue, snappedEndValue));
            if (DoubleUtil.GreaterThan(snappedStartValue, snappedEndValue))
                return;

            TInterval oldRangeValue = RangeValue;
            IsRangeValueChanging = true;
            try
            {

                if (snappedStartValue < endValue)
                {
                    // сначала смело смещаем левый ползунок, т.к. при смещении он не "упрется" в правый
                    if (!DoubleUtil.AreClose(snappedStartValue, startValue))
                    {
                        this.SetCurrentValue(StartValueProperty, DoubleToValue(snappedStartValue));
                    }
                    if (!DoubleUtil.AreClose(snappedEndValue, endValue))
                    {
                        this.SetCurrentValue(EndValueProperty, DoubleToValue(snappedEndValue));
                    }
                }
                else
                {
                    // сначала смещаем правый ползунок, т.к. он "мешает" сдвинуться левому
                    if (!DoubleUtil.AreClose(snappedEndValue, endValue))
                    {
                        this.SetCurrentValue(EndValueProperty, DoubleToValue(snappedEndValue));
                    }
                    if (!DoubleUtil.AreClose(snappedStartValue, startValue))
                    {
                        this.SetCurrentValue(StartValueProperty, DoubleToValue(snappedStartValue));
                    }
                }
            }
            finally
            {
                IsRangeValueChanging = false;
            }
            TInterval newRangeValue = RangeValue;
            T newStartValue = StartValue, newEndValue = EndValue;
            if (!DoubleUtil.AreClose(ValueToDouble(oldStartValue), ValueToDouble(newStartValue)) ||
                !DoubleUtil.AreClose(ValueToDouble(oldEndValue), ValueToDouble(newEndValue)))
            {
                OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
            }
            if (!DoubleUtil.AreClose(IntervalToDouble(oldRangeValue), IntervalToDouble(newRangeValue)))
            {
                OnRangeValueChanged(oldRangeValue, newRangeValue);
            }
        }

        /// <summary>
        /// Метод выполняет реальный сдвиг одного из ползунков (и выставляет значение соответствующего свойства <see cref="RangeBaseControl{T, TInterval}.StartValue"/> 
        /// или <see cref="RangeBaseControl{T, TInterval}.EndValue"/>) на величину <paramref name="direction"/> относительно текущего положения,
        /// если измененное положение отличается от текущего.
        /// При этом сдвиг происходит к соответствующей метке на шкале делений, если в этом есть необходимость.
        /// В результате вызова этой функции параметр <paramref name="direction"/> получает значение величины реального сдвига ползунка,
        /// т.к. он может отличаться от переданного в функцию.
        /// </summary>
        /// <param name="direction">Величина сдвига. Может быть меньше нуля.</param>
        /// <param name="isStartThumb"><c>true</c>, если выполняется сдвиг ползунка начала интервала, и <c>false</c> - если конца интервала.</param>
        /// <returns><c>true</c>, если в результате выполнения функции произошел рельный сдвиг ползунка
        /// и изменение значения соответствующего свойства.</returns>
        private bool InternalMoveToNextTick(ref double direction, bool isStartThumb)
        {
            Contract.Requires(!double.IsNaN(direction));
            Contract.Requires(!DoubleUtil.AreClose(direction, 0.0));
            Contract.Ensures(!double.IsNaN(direction));

            double value = ValueToDouble(isStartThumb ? this.StartValue : this.EndValue);
            double min = ValueToDouble(isStartThumb ? this.Minimum : this.StartValue);
            double max = ValueToDouble(isStartThumb ? this.EndValue : this.Maximum);

            // Find the next value by snapping
            double next = SnapToTick(Math.Max(min, Math.Min(max, value + direction)), isStartThumb, false);

            bool greaterThan = direction > 0; //search for the next tick greater than value?

            // If the snapping brought us back to value, find the next tick point
            if (DoubleUtil.AreClose(next, value)
                && !(greaterThan && DoubleUtil.AreClose(value, max))  // Stop if searching up if already at Max
                && !(!greaterThan && DoubleUtil.AreClose(value, min))) // Stop if searching down if already at Min
            {
                ITicksCollection<T> ticks = TypedTicksCollection;

                // If ticks collection is available, use it.
                // Note that ticks may be unsorted.
                if ((ticks != null) && (ticks.Count > 0))
                {
                    for (int i = 0; i < ticks.Count; i++)
                    {
                        double tick = ValueToDouble(ticks[i]);

                        // Find the smallest tick greater than value or the largest tick less than value
                        if ((greaterThan && DoubleUtil.GreaterThan(tick, value) && (DoubleUtil.LessThan(tick, next) || DoubleUtil.AreClose(next, value)))
                         || (!greaterThan && DoubleUtil.LessThan(tick, value) && (DoubleUtil.GreaterThan(tick, next) || DoubleUtil.AreClose(next, value))))
                        {
                            next = tick;
                        }
                    }
                }
                else if (DoubleUtil.GreaterThan(IntervalToDouble(TickFrequency), 0.0))
                {
                    // Find the current tick we are at
                    double tickFrequency = IntervalToDouble(TickFrequency);
                    double tickNumber = Math.Round((value - ValueToDouble(Minimum)) / tickFrequency) + Math.Sign(direction);
                    next = ValueToDouble(Minimum) + tickNumber * tickFrequency;
                }
            }

            // Update if we've found a better value
            bool changed = !DoubleUtil.AreClose(next, value);
            if (changed)
            {
                direction = next - value;
                this.SetCurrentValue(isStartThumb ? StartValueProperty : EndValueProperty, DoubleToValue(next));
            }
            Contract.Assume(!double.IsNaN(direction));
            return changed;
        }

        /// <summary>
        /// Сдвигает один из ползунков (и выставляет значение соответствующего свойства <see cref="RangeBaseControl{T, TInterval}.StartValue"/> 
        /// или <see cref="RangeBaseControl{T, TInterval}.EndValue"/>) на величину <paramref name="direction"/> относительно текущего положения,
        /// если измененное положение отличается от текущего.
        /// При этом сдвиг происходит к соответствующей метке на шкале делений, если в этом есть необходимость.
        /// </summary>
        /// <param name="direction">Величина сдвига. Должна быть положительной, т.к. "отрицательность" задается параметром <paramref name="isNegativeDirection"/></param>
        /// <param name="isNegativeDirection">Сдвиг должен происходить в отрицательную сторону (сторону уменьшения)</param>
        /// <param name="isStartThumb"><c>true</c>, если выполняется сдвиг ползунка начала интервала, и <c>false</c> - если конца интервала.</param>
        /// <returns><c>true</c>, если в результате выполнения функции произошел рельный сдвиг ползунка
        /// и изменение значения соответствующего свойства.</returns>
        public bool MoveToNextTick(TInterval direction, bool isNegativeDirection, bool isStartThumb)
        {
            if (IsSingleValue)
            {
                return MoveRangeToNextTick(direction, isNegativeDirection);
            }
            double doubleDirection = (isNegativeDirection ? -1.0 : 1.0) * IntervalToDouble(direction);
            Contract.Assume(!DoubleUtil.AreClose(doubleDirection, 0.0));
            Contract.Assume(!double.IsNaN(doubleDirection));
            return InternalMoveToNextTick(ref doubleDirection, isStartThumb);
        }

        /// <summary>
        /// Сдвигает весь интервал на величину <paramref name="direction"/> относительно текущего положения
        /// (т.е. одновременно и ползунок начала и ползунок конца интервала)
        /// если измененное положение отличается от текущего.
        /// При этом сдвиг происходит к соответствующей метке на шкале делений, если в этом есть необходимость.
        /// В отличие от последовательного сдвига ползунков на одинаковую величину,
        /// вызов этой функции не приводит к генерации события <see cref="RangeBaseControl{T, TInterval}.RangeValueChangedEvent"/>,
        /// т.к. на самом деле величина интервала в результате остается без изменений.
        /// </summary>
        /// <param name="direction">Величина сдвига. Должна быть положительной, т.к. "отрицательность" задается параметром <paramref name="isNegativeDirection"/></param>
        /// <param name="isNegativeDirection">Сдвиг должен происходить в отрицательную сторону (сторону уменьшения)</param>
        /// <returns><c>true</c>, если в результате выполнения функции произошел рельный сдвиг ползунков
        /// и изменение значений соответствующих свойств.</returns>
        public bool MoveRangeToNextTick(TInterval direction, bool isNegativeDirection)
        {
            bool changed = false;
            double doubleDirection = (isNegativeDirection ? -1.0 : 1.0) * IntervalToDouble(direction);
            TInterval oldRangeValue = RangeValue;
            T oldStartValue = StartValue, oldEndValue = EndValue;
            IsRangeValueChanging = true;
            try
            {
                if (doubleDirection > 0)
                {
                    Contract.Assume(!DoubleUtil.AreClose(doubleDirection, 0.0));
                    changed = InternalMoveToNextTick(ref doubleDirection, false);
                    if (changed)
                    {
                        Contract.Assume(!DoubleUtil.AreClose(doubleDirection, 0.0));
                        InternalMoveToNextTick(ref doubleDirection, true);
                    }
                }
                else if (doubleDirection < 0)
                {
                    Contract.Assume(!DoubleUtil.AreClose(doubleDirection, 0.0));
                    changed = InternalMoveToNextTick(ref doubleDirection, true);
                    if (changed)
                    {
                        Contract.Assume(!DoubleUtil.AreClose(doubleDirection, 0.0));
                        InternalMoveToNextTick(ref doubleDirection, false);
                    }
                }
            }
            finally
            {
                IsRangeValueChanging = false;
            }
            TInterval newRangeValue = RangeValue;
            T newStartValue = StartValue, newEndValue = EndValue;
            if (!DoubleUtil.AreClose(ValueToDouble(oldStartValue), ValueToDouble(newStartValue)) ||
                !DoubleUtil.AreClose(ValueToDouble(oldEndValue), ValueToDouble(newEndValue)))
            {
                OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
            }
            if (!DoubleUtil.AreClose(IntervalToDouble(oldRangeValue), IntervalToDouble(newRangeValue)))
            {
                OnRangeValueChanged(oldRangeValue, newRangeValue);
            }
            return changed;
        }

        /// <summary>
        /// Обновляет значение свойства <see cref="RangeBaseControl{T, TInterval}.StartValue"/> или
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/>, смещая его на величину <paramref name="delta"/>.
        /// При необходимости выполняется смещение к ближайшей метке на шкале.
        /// </summary>
        /// <param name="delta">Величина смещения.
        /// Если 0, то либо ничего не делается, либо выполняется выравнивание заданного ползунка к ближайшей метке на шкале делений.</param>
        /// <param name="isStartThumb"><c>true</c>, если смещается начало интервала, <c>false</c> - если конца.</param>
        /// <returns>Возвращается величина реально выполненного смещения,
        /// т.к. она может отличаться от переданного в параметре <paramref name="delta"/> из-за наличия ограничений.</returns>
        protected double UpdateValueByDelta(double delta, bool isStartThumb)
        {
            double realDelta = 0d; // actually worked delta
            double startValue = ValueToDouble(StartValue), endValue = ValueToDouble(EndValue);
            double newValue = (isStartThumb ? startValue : endValue) + delta;
            if (DoubleUtil.IsDoubleFinite(newValue))
            {
                Double snappedValue = SnapToTick(newValue, isStartThumb, false);

                if (!DoubleUtil.AreClose(snappedValue, (isStartThumb ? startValue : endValue)))
                {
                    if (isStartThumb)
                    {
                        newValue = Math.Max(ValueToDouble(this.Minimum), Math.Min(endValue, snappedValue));
                        realDelta = newValue - startValue;
                        this.SetCurrentValue(StartValueProperty, DoubleToValue(newValue));
                    }
                    else
                    {
                        newValue = Math.Max(startValue, Math.Min(ValueToDouble(this.Maximum), snappedValue));
                        realDelta = newValue - endValue;
                        this.SetCurrentValue(EndValueProperty, DoubleToValue(newValue));
                    }
                }
            }
            return realDelta;
        }

        /// <summary>
        /// Обновляет значение свойства <see cref="RangeBaseControl{T, TInterval}.StartValue"/> или
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/>, смещая его на величину <paramref name="delta"/>.
        /// При необходимости выполняется смещение к ближайшей метке на шкале.
        /// </summary>
        /// <param name="thumbType">Тип ползунка, в зависимости от которого изменяются значения</param>
        /// <param name="delta">Величина смещения.
        /// Если 0, то либо ничего не делается, либо выполняется выравнивание заданного ползунка к ближайшей метке на шкале делений.</param>
        protected void UpdateValueByThumbTypeAndDelta(RangeThumbType thumbType, double delta)
        {
            switch (thumbType)
            {
                case RangeThumbType.StartThumb:
                    UpdateValueByDelta(delta, true);
                    break;

                case RangeThumbType.RangeThumb:
                    TInterval oldRangeValue = RangeValue;
                    T oldStartValue = StartValue, oldEndValue = EndValue;
                    IsRangeValueChanging = true;
                    try
                    {
                        if (DoubleUtil.GreaterThanOrClose(delta, 0.0))
                        {
                            double endValue = ValueToDouble(EndValue), max = ValueToDouble(Maximum);
                            if (DoubleUtil.LessThan(endValue, max))
                            {
                                delta = UpdateValueByDelta(delta, false);
                                UpdateValueByDelta(delta, true);
                            }
                        }
                        else
                        {
                            double startValue = ValueToDouble(StartValue), min = ValueToDouble(Minimum);
                            if (DoubleUtil.GreaterThan(startValue, min))
                            {
                                delta = UpdateValueByDelta(delta, true);
                                UpdateValueByDelta(delta, false);
                            }
                        }
                    }
                    finally
                    {
                        IsRangeValueChanging = false;
                    }
                    TInterval newRangeValue = RangeValue;
                    T newStartValue = StartValue, newEndValue = EndValue;
                    if (!DoubleUtil.AreClose(ValueToDouble(oldStartValue), ValueToDouble(newStartValue)) ||
                        !DoubleUtil.AreClose(ValueToDouble(oldEndValue), ValueToDouble(newEndValue)))
                    {
                        OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
                    }
                    if (!DoubleUtil.AreClose(IntervalToDouble(oldRangeValue), IntervalToDouble(newRangeValue)))
                    {
                        OnRangeValueChanged(oldRangeValue, newRangeValue);
                    }
                    break;

                case RangeThumbType.EndThumb:
                    UpdateValueByDelta(delta, false);
                    break;
            }
        }

        private RangeThumbType GetThumbType(Thumb thumb)
        {
            if (thumb == null || Track == null)
                return RangeThumbType.None;

            if (Equals(thumb, this.Track.StartThumb))
            {
                return RangeThumbType.StartThumb;
            }
            if (Equals(thumb, this.Track.RangeThumb))
            {
                return RangeThumbType.RangeThumb;
            }
            if (Equals(thumb, this.Track.EndThumb))
            {
                return RangeThumbType.EndThumb;
            }
            return RangeThumbType.None;
        }

        #endregion Helper Functions

        /// <summary>
        /// Ссылка на интервальный трэк <see cref="RangeTrack{T, TInterval}"/> ползунка
        /// </summary>
        public RangeTrack<T, TInterval> Track { get; private set; }

        internal FrameworkElement TrackBackground { get; set; }

        #region Private Fields

        private const string c_TrackName = "PART_Track";
        private const string c_TrackBackgroundName = "PART_TrackBackground";
        private const string c_RangeDelimiter = "..";

        private const string c_IncreaseRangeLargeCommandName = "IncreaseRangeLarge";
        private const string c_IncreaseStartLargeCommandName = "IncreaseStartLarge";
        private const string c_IncreaseEndLargeCommandName = "IncreaseEndLarge";
        private const string c_IncreaseLargeByKeyCommandName = "IncreaseLargeByKey";

        private const string c_DecreaseRangeLargeCommandName = "DecreaseRangeLarge";
        private const string c_DecreaseStartLargeCommandName = "DecreaseStartLarge";
        private const string c_DecreaseEndLargeCommandName = "DecreaseEndLarge";
        private const string c_DecreaseLargeByKeyCommandName = "DecreaseLargeByKey";

        private const string c_IncreaseRangeSmallCommandName = "IncreaseRangeSmall";
        private const string c_IncreaseStartSmallCommandName = "IncreaseStartSmall";
        private const string c_IncreaseEndSmallCommandName = "IncreaseEndSmall";
        private const string c_IncreaseSmallByKeyCommandName = "IncreaseSmallByKey";

        private const string c_DecreaseRangeSmallCommandName = "DecreaseRangeSmall";
        private const string c_DecreaseStartSmallCommandName = "DecreaseStartSmall";
        private const string c_DecreaseEndSmallCommandName = "DecreaseEndSmall";
        private const string c_DecreaseSmallByKeyCommandName = "DecreaseSmallByKey";

        private const string c_MinimizeRangeValueCommandName = "MinimizeRangeValue";
        private const string c_MinimizeStartValueCommandName = "MinimizeStartValue";
        private const string c_MinimizeEndValueCommandName = "MinimizeEndValue";
        private const string c_MinimizeValueByKeyCommandName = "MinimizeValueByKey";

        private const string c_MaximizeRangeValueCommandName = "MaximizeRangeValue";
        private const string c_MaximizeStartValueCommandName = "MaximizeStartValue";
        private const string c_MaximizeEndValueCommandName = "MaximizeEndValue";
        private const string c_MaximizeValueByKeyCommandName = "MaximizeValueByKey";

        // SimpleRangeSlider required parts
        private ToolTip m_AutoToolTip;
        private object m_ThumbOriginalToolTip;

        private RangeValueData<T> m_RangeValueData;

        #endregion Private Fields
    }

    /// <summary>
    /// Интервальный ползунок, использующий в качестве значений и интервалов числа типа <see cref="double"/>.
    /// </summary>
    [TemplatePart(Name = "PART_Track", Type = typeof(NumericRangeTrack))]
    [TemplatePart(Name = "PART_TrackBackground", Type = typeof(FrameworkElement))]
    [Description("Simple Numeric Range Slider")]
    public class SimpleNumericRangeSlider : SimpleRangeSlider<double, double>
    {
        private const double c_DefaultTickFrequency = 1.0,
                             c_DefaultSmallChange = 0.1,
                             c_DefaultLargeChange = 1.0;

        private const double c_DefaultMinimum = 0.0,
                             c_DefaultMaximum = 10.0;

        private const string c_DefaultAutoToolTipFormat = "N";

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static SimpleNumericRangeSlider()
        {
            Type thisType = typeof(SimpleNumericRangeSlider);

            // Register all PropertyTypeMetadata
            MinimumProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(c_DefaultMinimum, FrameworkPropertyMetadataOptions.AffectsMeasure));
            MaximumProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(c_DefaultMaximum, FrameworkPropertyMetadataOptions.AffectsMeasure));
            MinRangeValueProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
            StartValueProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(c_DefaultMinimum, FrameworkPropertyMetadataOptions.AffectsMeasure));
            EndValueProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(c_DefaultMaximum, FrameworkPropertyMetadataOptions.AffectsMeasure));

            TickFrequencyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(c_DefaultTickFrequency));
            TickLabelConverterProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(null));
            TickLabelConverterParameterProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(null));

            SmallChangeProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(c_DefaultSmallChange));
            LargeChangeProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(c_DefaultLargeChange));

            DefaultStyleKeyProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(thisType));
        }

        #region override functions

        /// <summary>
        /// Метод преобразования числа в значение
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Всегда <paramref name="value"/></returns>
        protected override double DoubleToValue(double value)
        {
            return value;
        }
        /// <summary>
        /// Метод преобразования значения в число
        /// </summary>
        /// <param name="value">Значение</param>
        /// <returns>Всегда <paramref name="value"/></returns>
        protected override double ValueToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Метод преобразования числа в интервал
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Всегда <paramref name="value"/></returns>
        protected override double DoubleToInterval(double value)
        {
            return value;
        }
        /// <summary>
        /// Метод преобразования интервала в число
        /// </summary>
        /// <param name="value">Интервал</param>
        /// <returns>Всегда <paramref name="value"/></returns>
        protected override double IntervalToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Метод выполняет корректировку устанавливаемого значения свойства <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>
        /// </summary>
        /// <param name="value">Устанавливаемое значение</param>
        /// <returns>Скорректированное (если необходимо) значение</returns>
        protected override object CoerceMinRangeValue(object value)
        {
            double newValue =
                DependencyPropertyUtil.ExtractDouble(
                    value,
                    MinRangeValueProperty.DefaultMetadata == null
                        ? 0.0
                        : (double) (MinRangeValueProperty.DefaultMetadata.DefaultValue ?? 0.0));

            if (!MinRangeValueEnabled || DoubleUtil.LessThan(newValue, 0.0))
            {
                newValue = 0.0;
            }
            else
            {
                double range = Maximum - Minimum;
                if (DoubleUtil.GreaterThan(newValue, range))
                {
                    newValue = range;
                }
            }

            return newValue;
        }
        /// <summary>
        /// Метод выполняет корректировку устанавливаемого значения свойства <see cref="RangeBaseControl{T, TInterval}.Maximum"/>
        /// </summary>
        /// <param name="value">Устанавливаемое значение</param>
        /// <returns>Скорректированное (если необходимо) значение</returns>
        protected override object CoerceMaximum(object value)
        {
            double newValue =
                DependencyPropertyUtil.ExtractDouble(
                    value,
                    MaximumProperty.DefaultMetadata == null
                        ? 0.0
                        : (double)
                          (MaximumProperty.DefaultMetadata.DefaultValue ??
                           0.0));

            double minimum = Minimum;
            if (newValue < minimum)
            {
                newValue = minimum;
            }
            return newValue;
        }

        /// <summary>
        /// Текущая величина интервала
        /// </summary>
        protected override double CurrentRangeValue
        {
            get
            {
                return EndValue - StartValue;
            }
        }

        /// <summary>
        /// Свойство возвращает коллекцию значений числовых меток типа <see cref="double"/> для шкалы делений.
        /// </summary>
        protected override ITicksCollection<double> TypedTicksCollection
        {
            get
            {
                var ticks = Ticks;
                return ticks == null ? null : new DoubleTicksCollection(Ticks);
            }
        }

        /// <summary>
        /// Метод возвращает строку для отображения во всплывающей подсказке.
        /// Для преобразования значения в строку метод использует конвертер <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>
        /// с параметром <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverterParameter"/>,
        /// либо формат, заданный в <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/>
        /// и точность отображения чисел, заданную в <see cref="SimpleNumericRangeSlider.AutoToolTipPrecision"/>,
        /// если конвертер <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/> не задан (либо равен <c>null</c>).
        /// Если ни <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>,
        /// ни <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> не заданы, то используется общий числовой формат.
        /// </summary>
        /// <param name="value">Текущее значение</param>
        /// <param name="thumbType">Тип ползунка, значение которого <paramref name="value"/>.</param>
        /// <returns>Строковое представление значения <paramref name="value"/> для ползунка, заданного <paramref name="thumbType"/>.</returns>
        protected override string GetAutoToolTipString(double value, RangeThumbType thumbType)
        {
            string res;
            if (this.AutoToolTipValueConverter == null)
            {
                string frmt = this.AutoToolTipFormat;
                if (string.IsNullOrEmpty(frmt))
                {
                    frmt = c_DefaultAutoToolTipFormat;
                }
                var format = (NumberFormatInfo)(NumberFormatInfo.CurrentInfo.Clone());
                Contract.Assume(this.AutoToolTipPrecision >= 0 && this.AutoToolTipPrecision <= 99);
                format.NumberDecimalDigits = this.AutoToolTipPrecision;
                try
                {
                    res = value.ToString(frmt, format);
                }
                catch (FormatException)
                {
                    res = LocalizedStrings.BadAutoToolTipFormat;
                }
            }
            else
            {
                res = this.AutoToolTipValueConverter.Convert(value, thumbType, this.AutoToolTipValueConverterParameter);
            }

            return res;
        }

        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения начала интервала в большую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected override bool IncreaseStartValueCommandCanExecute()
        {
            bool res = IsSingleValue ?
                DoubleUtil.LessThan(StartValue, Maximum) :
                DoubleUtil.LessThan(StartValue, EndValue);
            return res;
        }
        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения конца интервала в большую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected override bool IncreaseEndValueCommandCanExecute()
        {
            return DoubleUtil.LessThan(EndValue, Maximum);
        }
        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения начала интервала в меньшую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected override bool DecreaseStartValueCommandCanExecute()
        {
            return DoubleUtil.LessThan(Minimum, StartValue);
        }
        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения конца интервала в меньшую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected override bool DecreaseEndValueCommandCanExecute()
        {
            bool res = IsSingleValue ?
                DoubleUtil.LessThan(Minimum, EndValue) :
                DoubleUtil.LessThan(StartValue, EndValue);
            return res;
        }

        #endregion

        #region Dependency Properties

        #region AutoToolTipPrecision Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleNumericRangeSlider.AutoToolTipPrecision"/>
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register(
            "AutoToolTipPrecision",
            typeof(int),
            typeof(SimpleNumericRangeSlider),
            new FrameworkPropertyMetadata(0),
            DependencyPropertyUtil.IsValidAutoToolTipPrecision);

        /// <summary>
        /// Точность отображения чисел с плавающей точкой во всплывающей подсказке, используемой по умолчанию.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public int AutoToolTipPrecision
        {
            get
            {
                var res = GetValue(AutoToolTipPrecisionProperty);
                Contract.Assume(res != null);
                return (int)res;
            }
            set
            {
                SetValue(AutoToolTipPrecisionProperty, value);
            }
        }

        #endregion

        #region Ticks Property

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleNumericRangeSlider.Ticks"/>
        /// </summary>
        public static readonly DependencyProperty TicksProperty
            = DependencyProperty.Register("Ticks", typeof(DoubleCollection), typeof(SimpleNumericRangeSlider),
            new FrameworkPropertyMetadata((new DoubleCollection()).GetAsFrozen(),
                OnTicksChanged));

        /// <summary>
        /// Коллекция значений меток на шкале делений.
        /// Когда <see cref="SimpleNumericRangeSlider.Ticks"/> не равен <c>null</c>
        /// слайдер игнорирует <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>
        /// и рисует только те метки, которые заданы в коллекции <see cref="SimpleNumericRangeSlider.Ticks"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Bindable(true), Category("Appearance")]
        public DoubleCollection Ticks
        {
            get
            {
                return (DoubleCollection)GetValue(TicksProperty);
            }
            set
            {
                SetValue(TicksProperty, value);
            }
        }

        private static void OnTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Contract.Requires(d is SimpleNumericRangeSlider);

            var element = (SimpleNumericRangeSlider)d;
            if (element.IsSnapToTickEnabled)
            {
                // если местоположение меток меняется и при этом включена привязка к ближайшей метке,
                // то надо принудительно выравнять текущее положение ползунков до ближайших меток.
                // Минус этого шага: изменяются положения ползунков и, как следствие, размер интервала
                // (но иногда этого может и не происходить).
                element.AlignValuesToTicks();
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Интервальный ползунок, использующий в качестве значений <see cref="DateTime"/>, а интервалов <see cref="TimeSpan"/>.
    /// </summary>
    [TemplatePart(Name = "PART_Track", Type = typeof(DateTimeRangeTrack))]
    [TemplatePart(Name = "PART_TrackBackground", Type = typeof(FrameworkElement))]
    [Description("Simple Date&Time Range Slider")]
    public class SimpleDateTimeRangeSlider : SimpleRangeSlider<DateTime, TimeSpan>
    {
        private static readonly TimeSpan s_DefaultTickFrequency = TimeSpan.FromDays(365),
                                         s_DefaultLargeChange = TimeSpan.FromDays(365);

        private static readonly DateTime s_DefaultMinimum = new DateTime(1900, 1, 1),
                                         s_DefaultMaximum = new DateTime(9999, 12, 31);

        private const string c_DefaultAutoToolTipFormat = "dd-MM-yyyy HH:mm:ss";

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static SimpleDateTimeRangeSlider()
        {
            Type thisType = typeof(SimpleDateTimeRangeSlider);

            DateTime from = s_DefaultMinimum, to = s_DefaultMaximum;
            TimeSpan minRange = TimeSpan.Zero, tickFrequency = s_DefaultTickFrequency;
            TimeSpan smallChange = TimeSpan.FromDays(1.0), largeChange = s_DefaultLargeChange;

            // Register all PropertyTypeMetadata
            MinimumProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(from, FrameworkPropertyMetadataOptions.AffectsMeasure));
            MaximumProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(to, FrameworkPropertyMetadataOptions.AffectsMeasure));
            MinRangeValueProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(minRange, FrameworkPropertyMetadataOptions.AffectsMeasure));
            StartValueProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(from, FrameworkPropertyMetadataOptions.AffectsMeasure));
            EndValueProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(to, FrameworkPropertyMetadataOptions.AffectsMeasure));

            TickFrequencyProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(tickFrequency));
            TickLabelConverterProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(new DefaultDateTimeTickLabelToStringConverter()));
            TickLabelConverterParameterProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(null));

            SmallChangeProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(smallChange));
            LargeChangeProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(largeChange));

            DefaultStyleKeyProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(thisType));
        }

        #region override functions

        /// <summary>
        /// Метод преобразования числа в дату
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Дата, сформированная на основе <paramref name="value"/> тактов.</returns>
        protected override DateTime DoubleToValue(double value)
        {
            return (value > 10.0) ? new DateTime((long)value) : DateTime.MinValue;
        }
        /// <summary>
        /// Метод преобразования даты в число
        /// </summary>
        /// <param name="value">Дата</param>
        /// <returns>Число тактов даты <paramref name="value"/></returns>
        protected override double ValueToDouble(DateTime value)
        {
            return value.Ticks;
        }

        /// <summary>
        /// Метод преобразования числа в интервал
        /// </summary>
        /// <param name="value">Число</param>
        /// <returns>Объкт <see cref="TimeSpan"/>, сформированный на основе <paramref name="value"/> тактов.</returns>
        protected override TimeSpan DoubleToInterval(double value)
        {
            return (value > 10.0) ? TimeSpan.FromTicks((long)value) : TimeSpan.Zero;
        }
        /// <summary>
        /// Метод преобразования интервала в число
        /// </summary>
        /// <param name="value">Объект типа <see cref="TimeSpan"/></param>
        /// <returns>Число тактов в <paramref name="value"/></returns>
        protected override double IntervalToDouble(TimeSpan value)
        {
            return value.Ticks;
        }

        /// <summary>
        /// Метод выполняет корректировку устанавливаемого значения свойства <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>
        /// </summary>
        /// <param name="value">Устанавливаемое значение</param>
        /// <returns>Скорректированное (если необходимо) значение</returns>
        protected override object CoerceMinRangeValue(object value)
        {
            TimeSpan newValue = TimeSpan.Zero;
            if (value is TimeSpan)
            {
                newValue = (TimeSpan)value;
            }

            if (!MinRangeValueEnabled || newValue < TimeSpan.Zero)
            {
                newValue = TimeSpan.Zero;
            }
            else
            {
                TimeSpan range = Maximum - Minimum;
                if (newValue > range)
                {
                    newValue = range;
                }
            }

            return newValue;
        }
        /// <summary>
        /// Метод выполняет корректировку устанавливаемого значения свойства <see cref="RangeBaseControl{T, TInterval}.Maximum"/>
        /// </summary>
        /// <param name="value">Устанавливаемое значение</param>
        /// <returns>Скорректированное (если необходимо) значение</returns>
        protected override object CoerceMaximum(object value)
        {
            DateTime newValue = DateTime.MinValue;
            if (value is DateTime)
            {
                newValue = (DateTime)value;
            }

            DateTime minimum = Minimum;
            if (newValue < minimum)
            {
                newValue = minimum;
            }
            return newValue;
        }

        /// <summary>
        /// Текущая величина интервала
        /// </summary>
        protected override TimeSpan CurrentRangeValue
        {
            get
            {
                return EndValue - StartValue;
            }
        }

        /// <summary>
        /// Свойство возвращает коллекцию значений меток типа <see cref="DateTime"/> для шкалы делений.
        /// </summary>
        protected override ITicksCollection<DateTime> TypedTicksCollection
        {
            get
            {
                var ticks = Ticks;
                return ticks == null ? null : new DateTimeTicksCollection(Ticks);
            }
        }

        /// <summary>
        /// Метод возвращает строку для отображения во всплывающей подсказке.
        /// Для преобразования значения в строку метод использует конвертер <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>
        /// с параметром <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverterParameter"/>,
        /// либо формат, заданный в <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/>,
        /// если конвертер <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/> не задан (либо равен <c>null</c>).
        /// Если ни <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>,
        /// ни <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> не заданы, то используется формат "dd-MM-yyyy HH:mm:ss".
        /// </summary>
        /// <param name="value">Текущее значение</param>
        /// <param name="thumbType">Тип ползунка, значение которого <paramref name="value"/>.</param>
        /// <returns>Строковое представление значения <paramref name="value"/> для ползунка, заданного <paramref name="thumbType"/>.</returns>
        protected override string GetAutoToolTipString(DateTime value, RangeThumbType thumbType)
        {
            string res;
            if (this.AutoToolTipValueConverter == null)
            {
                string frmt = this.AutoToolTipFormat;
                if (string.IsNullOrEmpty(frmt))
                {
                    frmt = c_DefaultAutoToolTipFormat;
                }
                try
                {
                    res = value.ToString(frmt, CultureInfo.CurrentCulture);
                }
                catch (FormatException)
                {
                    res = LocalizedStrings.BadAutoToolTipFormat;
                }
            }
            else
            {
                res = this.AutoToolTipValueConverter.Convert(value, thumbType, this.AutoToolTipValueConverterParameter);
            }

            return res;
        }

        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения начала интервала в большую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected override bool IncreaseStartValueCommandCanExecute()
        {
            bool res = IsSingleValue ?
                StartValue < Maximum :
                StartValue < EndValue;
            return res;
        }
        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения конца интервала в большую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected override bool IncreaseEndValueCommandCanExecute()
        {
            return EndValue < Maximum;
        }
        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения начала интервала в меньшую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected override bool DecreaseStartValueCommandCanExecute()
        {
            return Minimum < StartValue;
        }
        /// <summary>
        /// Метод проверяет возможность выполнения команды сдвига значения конца интервала в меньшую сторону
        /// </summary>
        /// <returns><c>true</c>, если выполнение команды возможно, иначе <c>false</c></returns>
        protected override bool DecreaseEndValueCommandCanExecute()
        {
            bool res = IsSingleValue ?
                Minimum < EndValue :
                StartValue < EndValue;
            return res;
        }

        /// <summary>
        /// Вызывается, когда меняется значение <see cref="RangeBaseControl{T, TInterval}.Minimum"/>.
        /// Перегружено здесь для изменения значения свойства <see cref="SimpleDateTimeRangeSlider.MinimumAsDouble"/>.
        /// </summary>
        /// <param name="oldMinimum">Старое значение</param>
        /// <param name="newMinimum">Новое значение</param>
        protected override void OnMinimumChanged(DateTime oldMinimum, DateTime newMinimum)
        {
            this.MinimumAsDouble = ValueToDouble(this.Minimum);

            base.OnMinimumChanged(oldMinimum, newMinimum);
        }

        /// <summary>
        /// Вызывается, когда меняется значение <see cref="RangeBaseControl{T, TInterval}.Maximum"/>
        /// Перегружено здесь для изменения значения свойства <see cref="SimpleDateTimeRangeSlider.MaximumAsDouble"/>.
        /// </summary>
        /// <param name="oldMaximum">Старое значение</param>
        /// <param name="newMaximum">Новое значение</param>
        protected override void OnMaximumChanged(DateTime oldMaximum, DateTime newMaximum)
        {
            this.MaximumAsDouble = ValueToDouble(this.Maximum);

            base.OnMaximumChanged(oldMaximum, newMaximum);
        }

        /// <summary>
        /// Вызывается, когда изменяется значение <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>.
        /// Перегружено здесь для того, чтобы обновить значение свойства <see cref="SimpleDateTimeRangeSlider.TickFrequencyAsDouble"/>.
        /// </summary>
        /// <param name="oldTickFrequency">Предыдущее значение</param>
        /// <param name="newTickFrequency">Новое значение</param>
        protected override void OnTickFrequencyChanged(TimeSpan oldTickFrequency, TimeSpan newTickFrequency)
        {
            this.TickFrequencyAsDouble = IntervalToDouble(this.TickFrequency);

            base.OnTickFrequencyChanged(oldTickFrequency, newTickFrequency);
        }

        #endregion

        #region Dependency Properties

        #region MinimumAsDouble

        /// <summary>
        /// Значение свойства <see cref="RangeBaseControl{T, TInterval}.Minimum"/>, представленное в виде числа с плавающей точкой.
        /// Используется для связи с другими элементами управления, использующими значения типа <see cref="double"/>,
        /// например, с <see cref="Slider"/>.
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public double MinimumAsDouble
        {
            get
            {
                var res = GetValue(MinimumAsDoubleProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            private set { SetValue(MinimumAsDoublePropertyKey, value); }
        }

// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey MinimumAsDoublePropertyKey =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "MinimumAsDouble",
                typeof(double),
                typeof(SimpleDateTimeRangeSlider),
                // the default value is necessary for readonly dependency property evaluated by changes from another property
                // without that the initial value of readonly property wont be initialized correctly
                new FrameworkPropertyMetadata((double)s_DefaultMinimum.Ticks));

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleDateTimeRangeSlider.MinimumAsDouble"/>
        /// </summary>
        public static readonly DependencyProperty MinimumAsDoubleProperty = MinimumAsDoublePropertyKey.DependencyProperty;

        #endregion

        #region MaximumAsDouble

        /// <summary>
        /// Значение свойства <see cref="RangeBaseControl{T, TInterval}.Maximum"/>, представленное в виде числа с плавающей точкой.
        /// Используется для связи с другими элементами управления, использующими значения типа <see cref="double"/>,
        /// например, с <see cref="Slider"/>.
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public double MaximumAsDouble
        {
            get
            {
                var res = GetValue(MaximumAsDoubleProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            private set { SetValue(MaximumAsDoublePropertyKey, value); }
        }

// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey MaximumAsDoublePropertyKey =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "MaximumAsDouble",
                typeof(double),
                typeof(SimpleDateTimeRangeSlider),
                // the default value is necessary for readonly dependency property evaluated by changes from another property
                // without that the initial value of readonly property wont be initialized correctly
                new FrameworkPropertyMetadata((double)s_DefaultMaximum.Ticks));

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleDateTimeRangeSlider.MaximumAsDouble"/>
        /// </summary>
        public static readonly DependencyProperty MaximumAsDoubleProperty = MaximumAsDoublePropertyKey.DependencyProperty;

        #endregion

        #region Ticks

        /// <summary>
        /// Коллекция значений меток на шкале делений.
        /// Когда <see cref="SimpleDateTimeRangeSlider.Ticks"/> не равен <c>null</c>
        /// слайдер игнорирует <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>
        /// и рисует только те метки, которые заданы в коллекции <see cref="SimpleDateTimeRangeSlider.Ticks"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [Bindable(true), Category("Appearance")]
        public DateTimeCollection Ticks
        {
            get
            {
                return (DateTimeCollection)GetValue(TicksProperty);
            }
            set
            {
                SetValue(TicksProperty, value);
            }
        }

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleDateTimeRangeSlider.Ticks"/>
        /// </summary>
        public static readonly DependencyProperty TicksProperty
            = DependencyProperty.Register(
                "Ticks",
                typeof (DateTimeCollection),
                typeof (SimpleDateTimeRangeSlider),
                // the default value is necessary for readonly dependency property evaluated by changes from another property
                // without that the initial value of readonly property wont be initialized correctly
                new FrameworkPropertyMetadata((new DateTimeCollection()).GetAsFrozen(), OnTicksChanged));

        private static void OnTicksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Contract.Requires(d is SimpleDateTimeRangeSlider);

            var element = (SimpleDateTimeRangeSlider)d;
            if (element.IsSnapToTickEnabled)
            {
                // если местоположение меток меняется и при этом включена привязка к ближайшей метке,
                // то надо принудительно выравнять текущее положение ползунков до ближайших меток.
                // Минус этого шага: изменяются положения ползунков и, как следствие, размер интервала
                // (но иногда этого может и не происходить).
                element.AlignValuesToTicks();
            }
            element.TicksAsDouble = element.Ticks == null ? null : new DoubleCollection(from tick in element.Ticks select (double)tick.Ticks);
        }

        #endregion

        #region TicksAsDouble

        /// <summary>
        /// Значение свойства <see cref="SimpleDateTimeRangeSlider.Ticks"/>, представленное в виде коллекции чисел с плавающей точкой.
        /// Используется для связи с другими элементами управления, использующими значения типа <see cref="double"/>,
        /// например, с <see cref="TickBar"/>.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public DoubleCollection TicksAsDouble
        {
            get { return (DoubleCollection)GetValue(TicksAsDoubleProperty); }
            private set { SetValue(TicksAsDoublePropertyKey, value); }
        }

// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey TicksAsDoublePropertyKey =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "TicksAsDouble",
                typeof(DoubleCollection),
                typeof(SimpleDateTimeRangeSlider),
                // the default value is necessary for readonly dependency property evaluated by changes from another property
                // without that the initial value of readonly property wont be initialized correctly
                new FrameworkPropertyMetadata((new DoubleCollection()).GetAsFrozen()));

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleDateTimeRangeSlider.TicksAsDouble"/>
        /// </summary>
        public static readonly DependencyProperty TicksAsDoubleProperty = TicksAsDoublePropertyKey.DependencyProperty;

        #endregion

        #region TickFrequencyAsDouble

        /// <summary>
        /// Значение свойства <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>, представленное в виде коллекции чисел с плавающей точкой.
        /// Используется для связи с другими элементами управления, использующими значения типа <see cref="double"/>,
        /// например, с <see cref="TickBar"/>.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public double TickFrequencyAsDouble
        {
            get
            {
                var res = GetValue(TickFrequencyAsDoubleProperty);
                Contract.Assume(res != null);
                return (double) res;
            }
            private set { SetValue(TickFrequencyAsDoublePropertyKey, value); }
        }

// ReSharper disable InconsistentNaming
        private static readonly DependencyPropertyKey TickFrequencyAsDoublePropertyKey =
// ReSharper restore InconsistentNaming
            DependencyProperty.RegisterReadOnly(
                "TickFrequencyAsDouble",
                typeof(double),
                typeof(SimpleDateTimeRangeSlider),
                // the default value is necessary for readonly dependency property evaluated by changes from another property
                // without that the initial value of readonly property wont be initialized correctly
                new FrameworkPropertyMetadata((double)s_DefaultTickFrequency.Ticks));

        /// <summary>
        /// Свойство зависимости для <see cref="SimpleDateTimeRangeSlider.TickFrequencyAsDouble"/>
        /// </summary>
        public static readonly DependencyProperty TickFrequencyAsDoubleProperty = TickFrequencyAsDoublePropertyKey.DependencyProperty;

        #endregion

        #endregion
    }

    /// <summary>
    /// Класс для преобразования дат в строку, используемый по умолчанию в <see cref="SimpleDateTimeRangeSlider"/>
    /// </summary>
    public sealed class DefaultDateTimeTickLabelToStringConverter : IDoubleToStringConverter
    {
        /// <summary>
        /// Выполняет преобразование даты, представленной числом тактов, в строку.
        /// </summary>
        /// <param name="value">Число тактов</param>
        /// <param name="parameter">Дополнительный произвольный параметер для преобразования</param>
        /// <returns>Строковое представление полученной даты,
        /// либо строка, сигнализирующая о некорректном значении <paramref name="value"/> (зависит от версии и используемого языка).</returns>
        public string Convert(double value, object parameter)
        {
            var longTicks = (long)value;
            if (longTicks < DateTime.MinValue.Ticks || longTicks > DateTime.MaxValue.Ticks)
            {
                return LocalizedStrings.BadDateTimeTicksValue;
            }
            Contract.Assume(longTicks <= 0x2bca2875f4373fffL);
            var dt = new DateTime(longTicks);
            return dt.ToString(CultureInfo.CurrentCulture);
        }
    }

    internal struct RangeValueData<T>
    {
        internal bool IsRangeDragging { get; set; }
        internal T RangeStart { get; set; }
        internal T RangeEnd { get; set; }
    }
}
