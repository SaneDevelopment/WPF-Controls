// -----------------------------------------------------------------------
// <copyright file="SimpleRangeSlider.cs" company="Sane Development">
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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// Common class for range sliders.
    ///
    /// Separated to decrease code duplication.
    /// </summary>
    /// <typeparam name="T">Values type.</typeparam>
    /// <typeparam name="TInterval">Interval type.</typeparam>
    public abstract class SimpleRangeSlider<T, TInterval> : RangeBaseControl<T, TInterval>
    {
        /// <summary>
        /// Pre-defined name of the part track inside slider XAML.
        /// </summary>
        public const string TrackPartName = "PART_Track";

        /// <summary>
        /// Pre-defined name of the part track background inside slider XAML.
        /// </summary>
        public const string TrackBackgroundPartName = "PART_TrackBackground";

        /// <summary>
        /// Delimiter for range text in auto tooltip.
        /// </summary>
        public const string AutoToolTipTextRangeDelimiter = "..";

#pragma warning disable CA1000 // Do not declare static members on generic types
#pragma warning disable SA1202 // Elements should be ordered by access
#pragma warning disable SA1201 // Elements should appear in the correct order

        #region Private Fields

#pragma warning disable SA1303 // Const field names should begin with upper-case letter
#pragma warning disable SA1310 // Field names should not contain underscore
#pragma warning disable SA1308 // Variable names should not be prefixed

        // SimpleRangeSlider required parts
        private ToolTip m_AutoToolTip;
        private object m_ThumbOriginalToolTip;

        /// <summary>
        /// Store here the range value data when dragging started.
        /// </summary>
        private RangeValueData<T> m_RangeValueData;

#pragma warning restore SA1308 // Variable names should not be prefixed
#pragma warning restore SA1310 // Field names should not contain underscore
#pragma warning restore SA1303 // Const field names should begin with upper-case letter

        #endregion Private Fields

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
                new MouseButtonEventHandler(OnMouseLeftButtonDown),
                true);
        }

        /// <summary>
        /// Gets reference to the range track <see cref="RangeTrack{T, TInterval}"/> of this slider.
        /// </summary>
        /// <value>Reference to the range track <see cref="RangeTrack{T, TInterval}"/> of this slider.</value>
        public RangeTrack<T, TInterval> Track { get; private set; }

        /// <summary>
        /// Gets or sets track background element.
        /// </summary>
        /// <value>Track background element.</value>
        internal FrameworkElement TrackBackground { get; set; }


        #region Commands

        #region Large increasing

        /// <summary>
        /// Gets routed command for large increasing of interval (range) value.
        /// </summary>
        /// <value>Routed command for large increasing of interval (range) value.</value>
        public static RoutedCommand IncreaseRangeLarge { get; private set; }

        /// <summary>
        /// Gets routed command for large increasing of start value.
        /// </summary>
        /// <value>Routed command for large increasing of start value.</value>
        public static RoutedCommand IncreaseStartLarge { get; private set; }

        /// <summary>
        /// Gets routed command for large increasing of end value.
        /// </summary>
        /// <value>Routed command for large increasing of end value.</value>
        public static RoutedCommand IncreaseEndLarge { get; private set; }

        private static RoutedCommand IncreaseLargeByKey { get; set; }

        #endregion Large increasing

        #region Large decreasing

        /// <summary>
        /// Gets routed command for large decreasing of interval (range) value.
        /// </summary>
        /// <value>Routed command for large decreasing of interval (range) value.</value>
        public static RoutedCommand DecreaseRangeLarge { get; private set; }

        /// <summary>
        /// Gets routed command for large decreasing of start value.
        /// </summary>
        /// <value>Routed command for large decreasing of start value.</value>
        public static RoutedCommand DecreaseStartLarge { get; private set; }

        /// <summary>
        /// Gets routed command for large decreasing of end value.
        /// </summary>
        /// <value>Routed command for large decreasing of end value.</value>
        public static RoutedCommand DecreaseEndLarge { get; private set; }

        private static RoutedCommand DecreaseLargeByKey { get; set; }

        #endregion Large decreasing

        #region Small increasing

        /// <summary>
        /// Gets routed command for small increasing of interval (range) value.
        /// </summary>
        /// <value>Routed command for small increasing of interval (range) value.</value>
        public static RoutedCommand IncreaseRangeSmall { get; private set; }

        /// <summary>
        /// Gets routed command for small increasing of start value.
        /// </summary>
        /// <value>Routed command for small increasing of start value.</value>
        public static RoutedCommand IncreaseStartSmall { get; private set; }

        /// <summary>
        /// Gets routed command for small increasing of end value.
        /// </summary>
        /// <value>Routed command for small increasing of end value.</value>
        public static RoutedCommand IncreaseEndSmall { get; private set; }

        private static RoutedCommand IncreaseSmallByKey { get; set; }

        #endregion Small increasing

        #region Small decreasing

        /// <summary>
        /// Gets routed command for small decreasing of interval (range) value.
        /// </summary>
        /// <value>Routed command for small decreasing of interval (range) value.</value>
        public static RoutedCommand DecreaseRangeSmall { get; private set; }

        /// <summary>
        /// Gets routed command for small decreasing of start value.
        /// </summary>
        /// <value>Routed command for small decreasing of start value.</value>
        public static RoutedCommand DecreaseStartSmall { get; private set; }

        /// <summary>
        /// Gets routed command for small decreasing of end value.
        /// </summary>
        /// <value>Routed command for small decreasing of end value.</value>
        public static RoutedCommand DecreaseEndSmall { get; private set; }

        private static RoutedCommand DecreaseSmallByKey { get; set; }

        #endregion Small decreasing

        #region Minimize

        /// <summary>
        /// Gets routed command for minimizing of interval (range) value.
        /// </summary>
        /// <value>Routed command for minimizing of interval (range) value.</value>
        public static RoutedCommand MinimizeRangeValue { get; private set; }

        /// <summary>
        /// Gets routed command for minimizing of start value.
        /// </summary>
        /// <value>Routed command for minimizing of start value.</value>
        public static RoutedCommand MinimizeStartValue { get; private set; }

        /// <summary>
        /// Gets routed command for minimizing of end value.
        /// </summary>
        /// <value>Routed command for minimizing of end value.</value>
        public static RoutedCommand MinimizeEndValue { get; private set; }

        private static RoutedCommand MinimizeValueByKey { get; set; }

        #endregion Minimize

        #region Maximize

        /// <summary>
        /// Gets routed command for maximizing of interval (range) value.
        /// </summary>
        /// <value>Routed command for maximizing of interval (range) value.</value>
        public static RoutedCommand MaximizeRangeValue { get; private set; }

        /// <summary>
        /// Gets routed command for maximizing of start value.
        /// </summary>
        /// <value>Routed command for maximizing of start value.</value>
        public static RoutedCommand MaximizeStartValue { get; private set; }

        /// <summary>
        /// Gets routed command for maximizing of end value.
        /// </summary>
        /// <value>Routed command for maximizing of end value.</value>
        public static RoutedCommand MaximizeEndValue { get; private set; }

        private static RoutedCommand MaximizeValueByKey { get; set; }

        #endregion Maximize

        private static void InitializeCommands()
        {
            Type thisType = typeof(SimpleRangeSlider<T, TInterval>);

            IncreaseRangeLarge = new RoutedCommand(nameof(IncreaseRangeLarge), thisType);
            IncreaseStartLarge = new RoutedCommand(nameof(IncreaseStartLarge), thisType);
            IncreaseEndLarge = new RoutedCommand(nameof(IncreaseEndLarge), thisType);
            IncreaseLargeByKey = new RoutedCommand(nameof(IncreaseLargeByKey), thisType);

            DecreaseRangeLarge = new RoutedCommand(nameof(DecreaseRangeLarge), thisType);
            DecreaseStartLarge = new RoutedCommand(nameof(DecreaseStartLarge), thisType);
            DecreaseEndLarge = new RoutedCommand(nameof(DecreaseEndLarge), thisType);
            DecreaseLargeByKey = new RoutedCommand(nameof(DecreaseLargeByKey), thisType);

            IncreaseRangeSmall = new RoutedCommand(nameof(IncreaseRangeSmall), thisType);
            IncreaseStartSmall = new RoutedCommand(nameof(IncreaseStartSmall), thisType);
            IncreaseEndSmall = new RoutedCommand(nameof(IncreaseEndSmall), thisType);
            IncreaseSmallByKey = new RoutedCommand(nameof(IncreaseSmallByKey), thisType);

            DecreaseRangeSmall = new RoutedCommand(nameof(DecreaseRangeSmall), thisType);
            DecreaseStartSmall = new RoutedCommand(nameof(DecreaseStartSmall), thisType);
            DecreaseEndSmall = new RoutedCommand(nameof(DecreaseEndSmall), thisType);
            DecreaseSmallByKey = new RoutedCommand(nameof(DecreaseSmallByKey), thisType);

            MinimizeRangeValue = new RoutedCommand(nameof(MinimizeRangeValue), thisType);
            MinimizeStartValue = new RoutedCommand(nameof(MinimizeStartValue), thisType);
            MinimizeEndValue = new RoutedCommand(nameof(MinimizeEndValue), thisType);
            MinimizeValueByKey = new RoutedCommand(nameof(MinimizeValueByKey), thisType);

            MaximizeRangeValue = new RoutedCommand(nameof(MaximizeRangeValue), thisType);
            MaximizeStartValue = new RoutedCommand(nameof(MaximizeStartValue), thisType);
            MaximizeEndValue = new RoutedCommand(nameof(MaximizeEndValue), thisType);
            MaximizeValueByKey = new RoutedCommand(nameof(MaximizeValueByKey), thisType);

            // Commands with "public" specification registers without hot keys,
            // because for identifying the thumb we need to know what <Ctrl> was pressed - left or right.
            // But we can't know it, because KeyGesture has only ModifierKeys.Control modifier (i.e. there's no LeftControl or RightControl).
            // Such commands can be invoked only via Execute method.
            // Commands, that can be invoked via key gestures, declared as "private", to prevent invoking them from external code.
            // Such commands handles in some private method, analizes what <Ctrl> was pressed and invokes appropriate "public" command.
            //
            // We did that way because we can associate every <Ctrl>+<key> to only one command,
            // and only for that command will be called ..._CanExecute method (while pressing mentioned keys).
            // Though, the conditions of "execution-ability" are different for left and right thumbs,
            // therefore some combination won't be involved because of "disabling" of another combination - this is wrong.

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
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the increasing of start value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
        protected abstract bool IncreaseStartValueCommandCanExecute();

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the increasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
        protected abstract bool IncreaseEndValueCommandCanExecute();

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of start value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
        protected abstract bool DecreaseStartValueCommandCanExecute();

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c>.</returns>
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
            {
                throw new ArgumentNullException(nameof(e));
            }

            var res = RangeThumbType.None;

            bool hasLeftCtrl = Keyboard.IsKeyDown(Key.LeftCtrl);
            bool hasRightCtrl = Keyboard.IsKeyDown(Key.RightCtrl);
            Debug.Assert(hasLeftCtrl ^ hasRightCtrl, "hasLeftCtrl ^ hasRightCtrl");

            if (hasLeftCtrl)
            {
                res = RangeThumbType.StartThumb;
            }
            else if (hasRightCtrl)
            {
                res = RangeThumbType.EndThumb;
            }

            Debug.Assert(res != RangeThumbType.None, "res != RangeThumbType.None");

            return res;
        }

        #endregion Commands

        #region Dependency Properties

        #region Orientation Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.Orientation"/>.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
                DependencyProperty.Register(
                    nameof(Orientation),
                    typeof(Orientation),
                    typeof(SimpleRangeSlider<T, TInterval>),
                    new FrameworkPropertyMetadata(Orientation.Horizontal),
                    DependencyPropertyUtil.IsValidOrientation);

        /// <summary>
        /// Gets or sets control orientation.
        /// </summary>
        /// <value>Control orientation.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public Orientation Orientation
        {
            get
            {
                var res = this.GetValue(OrientationProperty);
                Debug.Assert(res != null, "res != null");
                return (Orientation)res;
            }

            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        #endregion

        #region IsDragRangeEnabled Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.IsDragRangeEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty IsDragRangeEnabledProperty =
                DependencyProperty.Register(
                    nameof(IsDragRangeEnabled),
                    typeof(bool),
                    typeof(SimpleRangeSlider<T, TInterval>),
                    new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether enabled the whole range dragging by appropriate thumb.
        /// </summary>
        /// <value>A value indicating whether enabled the whole range dragging by appropriate thumb.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public bool IsDragRangeEnabled
        {
            get
            {
                var res = this.GetValue(IsDragRangeEnabledProperty);
                Debug.Assert(res != null, "res != null");
                return (bool)res;
            }

            set
            {
                this.SetValue(IsDragRangeEnabledProperty, value);
            }
        }

        #endregion

        #region IsRangeDragging

        /// <summary>
        /// Gets a value indicating whether the control is in process of changing some value via dragging a thumb.
        /// Lets to know that user is moving the one of the thumbs right now.
        /// </summary>
        /// <value>A value indicating whether the control is in process of changing some value via dragging a thumb.</value>
        [Category("Common")]
        public bool IsRangeDragging
        {
            get
            {
                var res = this.GetValue(IsRangeDraggingProperty);
                Debug.Assert(res != null, "res != null");
                return (bool)res;
            }

            private set
            {
                this.SetValue(IsRangeDraggingPropertyKey, value);
            }
        }

        private static readonly DependencyPropertyKey IsRangeDraggingPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(IsRangeDragging),
                typeof(bool),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(false, OnIsRangeDraggingChanged));

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/>.
        /// </summary>
        public static readonly DependencyProperty IsRangeDraggingProperty = IsRangeDraggingPropertyKey.DependencyProperty;

        private static void OnIsRangeDraggingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is SimpleRangeSlider<T, TInterval>, "obj is SimpleRangeSlider<T, TInterval>");
            Debug.Assert(args.OldValue is bool, "args.OldValue is bool");
            Debug.Assert(args.NewValue is bool, "args.NewValue is bool");

            var element = obj as SimpleRangeSlider<T, TInterval>;
            Debug.Assert(element != null, "element != null");
            if (element == null)
            {
                return;
            }

            element.OnIsRangeDraggingChanged((bool)args.OldValue, (bool)args.NewValue);
        }

        /// <summary>
        /// Handler of changing the <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/> property.
        /// </summary>
        /// <param name="oldValue">The previous value of the property as reported by a property changed event.</param>
        /// <param name="newValue">The new value of a property as reported by a property changed event.</param>
        protected virtual void OnIsRangeDraggingChanged(bool oldValue, bool newValue)
        {
            var newEventArgs = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue)
            {
                RoutedEvent = IsRangeDraggingChangedEvent,
            };
            this.RaiseEvent(newEventArgs);
        }

        #endregion

        #region IsRaiseValueChangedWhileDragging Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.IsRaiseValueChangedWhileDragging"/>.
        /// </summary>
        public static readonly DependencyProperty IsRaiseValueChangedWhileDraggingProperty =
            DependencyProperty.Register(
                nameof(IsRaiseValueChangedWhileDragging),
                typeof(bool),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.AffectsRender),
                DependencyPropertyUtil.IsValidBoolValue);

        /// <summary>
        /// Gets or sets a value indicating whether to raise <see cref="RangeBaseControl{T, TInterval}.ValueChanged"/> event,
        /// when control is in process of dragging some thumb,
        /// in other words, when <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/> is <c>true</c>.
        /// </summary>
        /// <value>A value indicating whether to raise <see cref="RangeBaseControl{T, TInterval}.ValueChanged"/> event,
        /// when control is in process of dragging some thumb.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public bool IsRaiseValueChangedWhileDragging
        {
            get
            {
                var res = this.GetValue(IsRaiseValueChangedWhileDraggingProperty);
                Debug.Assert(res != null, "res != null");
                return (bool)res;
            }

            set
            {
                this.SetValue(IsRaiseValueChangedWhileDraggingProperty, value);
            }
        }

        #endregion

        #region Delay Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.Delay"/>.
        /// </summary>
        public static readonly DependencyProperty DelayProperty = RepeatButton.DelayProperty.AddOwner(
            typeof(SimpleRangeSlider<T, TInterval>),
            new FrameworkPropertyMetadata(GetKeyboardDelay()));

        private static int GetKeyboardDelay()
        {
            int keyboardDelay = SystemParameters.KeyboardDelay;
            if ((keyboardDelay < 0) || (keyboardDelay > 3))
            {
                keyboardDelay = 0;
            }

            return (keyboardDelay + 1) * 250;
        }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, the <see cref="System.Windows.Controls.Primitives.RepeatButton"/>
        /// waits while it is pressed before it starts repeating.
        /// The value must be non-negative.
        /// </summary>
        /// <value>The amount of time, in milliseconds, the <see cref="System.Windows.Controls.Primitives.RepeatButton"/>
        /// waits while it is pressed before it starts repeating.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public int Delay
        {
            get
            {
                var res = this.GetValue(DelayProperty);
                Debug.Assert(res != null, "res != null");
                return (int)res;
            }

            set
            {
                this.SetValue(DelayProperty, value);
            }
        }

        #endregion Delay Property

        #region Interval Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.Interval"/>.
        /// </summary>
        public static readonly DependencyProperty IntervalProperty = RepeatButton.IntervalProperty.AddOwner(
            typeof(SimpleRangeSlider<T, TInterval>),
            new FrameworkPropertyMetadata(GetKeyboardSpeed()));

        private static int GetKeyboardSpeed()
        {
            int keyboardSpeed = SystemParameters.KeyboardSpeed;
            if ((keyboardSpeed < 0) || (keyboardSpeed > 0x1f))
            {
                keyboardSpeed = 0x1f;
            }

            return (((0x1f - keyboardSpeed) * 0x16f) / 0x1f) + 0x21;
        }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, between repeats once repeating starts.
        /// The value must be non-negative.
        /// </summary>
        /// <value>The amount of time, in milliseconds, between repeats once repeating starts.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public int Interval
        {
            get
            {
                var res = this.GetValue(IntervalProperty);
                Debug.Assert(res != null, "res != null");
                return (int)res;
            }

            set
            {
                this.SetValue(IntervalProperty, value);
            }
        }

        #endregion Interval Property


        #region AutoToolTipValueConverter Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>.
        /// </summary>
        public static readonly DependencyProperty AutoToolTipValueConverterProperty =
            DependencyProperty.Register(
                nameof(AutoToolTipValueConverter),
                typeof(IRangeValueToStringConverter<T>),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets converter of values into their string representations for showing in the tooltips in UI.
        /// If not <c>null</c>, then <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> ignores.
        /// </summary>
        /// <value>Converter of values into their string representations for showing in the tooltips in UI.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public IRangeValueToStringConverter<T> AutoToolTipValueConverter
        {
            get { return (IRangeValueToStringConverter<T>)this.GetValue(AutoToolTipValueConverterProperty); }
            set { this.SetValue(AutoToolTipValueConverterProperty, value); }
        }

        #endregion

        #region AutoToolTipValueConverterParameter Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverterParameter"/>.
        /// </summary>
        public static readonly DependencyProperty AutoToolTipValueConverterParameterProperty =
            DependencyProperty.Register(
                nameof(AutoToolTipValueConverterParameter),
                typeof(object),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets additional parameter for <see cref="AutoToolTipValueConverter"/>.
        /// </summary>
        /// <value>Additional parameter for <see cref="AutoToolTipValueConverter"/>.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public object AutoToolTipValueConverterParameter
        {
            get { return this.GetValue(AutoToolTipValueConverterParameterProperty); }
            set { this.SetValue(AutoToolTipValueConverterParameterProperty, value); }
        }

        #endregion

        #region AutoToolTipFormat Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/>.
        /// </summary>
        public static readonly DependencyProperty AutoToolTipFormatProperty
            = DependencyProperty.Register(
                nameof(AutoToolTipFormat),
                typeof(string),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata());

        /// <summary>
        /// Gets or sets format string for showing values in the tooltips in UI.
        /// Ignores if <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/> is not <c>null</c>.
        /// </summary>
        /// <value>Format string for showing values in the tooltips in UI.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public string AutoToolTipFormat
        {
            get
            {
                return (string)this.GetValue(AutoToolTipFormatProperty);
            }

            set
            {
                this.SetValue(AutoToolTipFormatProperty, value);
            }
        }

        #endregion

        #region AutoToolTipPlacement Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipPlacement"/>.
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPlacementProperty
            = DependencyProperty.Register(
                nameof(AutoToolTipPlacement),
                typeof(AutoToolTipPlacement),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(AutoToolTipPlacement.None),
                DependencyPropertyUtil.IsValidAutoToolTipPlacement);

        /// <summary>
        /// Gets or sets the placement where automatic <see cref="System.Windows.Controls.ToolTip"/> is positioned on the control.
        /// </summary>
        /// <value>The placement where automatic <see cref="System.Windows.Controls.ToolTip"/> is positioned on the control.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get
            {
                var res = this.GetValue(AutoToolTipPlacementProperty);
                Debug.Assert(res != null, "res != null");
                return (AutoToolTipPlacement)res;
            }

            set
            {
                this.SetValue(AutoToolTipPlacementProperty, value);
            }
        }

        #endregion


        #region StartReservedSpace Property

        private static readonly DependencyPropertyKey StartReservedSpacePropertyKey
            = DependencyProperty.RegisterReadOnly(
                nameof(StartReservedSpace),
                typeof(double),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.StartReservedSpace"/>.
        /// </summary>
        public static readonly DependencyProperty StartReservedSpaceProperty = StartReservedSpacePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets slider uses <see cref="SimpleRangeSlider{T, TInterval}.StartReservedSpaceProperty"/>
        /// for evaluation of indent of <see cref="TickBar"/> to the left/bottom.
        /// </summary>
        /// <value>Slider uses <see cref="SimpleRangeSlider{T, TInterval}.StartReservedSpaceProperty"/>
        /// for evaluation of indent of <see cref="TickBar"/> to the left/bottom.</value>
        [Bindable(true)]
        [Category("Appearance")]
        public double StartReservedSpace
        {
            get
            {
                var res = this.GetValue(StartReservedSpaceProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            private set
            {
                this.SetValue(StartReservedSpacePropertyKey, value);
            }
        }

        #endregion

        #region EndReservedSpace Property

        private static readonly DependencyPropertyKey EndReservedSpacePropertyKey
            = DependencyProperty.RegisterReadOnly(
                nameof(EndReservedSpace),
                typeof(double),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.EndReservedSpace"/>.
        /// </summary>
        public static readonly DependencyProperty EndReservedSpaceProperty = EndReservedSpacePropertyKey.DependencyProperty;

        /// <summary>
        /// Gets slider uses <see cref="SimpleRangeSlider{T, TInterval}.EndReservedSpaceProperty"/>
        /// for evaluation of indent of <see cref="TickBar"/> to the right/top.
        /// </summary>
        /// <value>Slider uses <see cref="SimpleRangeSlider{T, TInterval}.EndReservedSpaceProperty"/>
        /// for evaluation of indent of <see cref="TickBar"/> to the right/top.</value>
        [Bindable(true)]
        [Category("Appearance")]
        public double EndReservedSpace
        {
            get
            {
                var res = this.GetValue(EndReservedSpaceProperty);
                Debug.Assert(res != null, "res != null");
                return (double)res;
            }

            private set
            {
                this.SetValue(EndReservedSpacePropertyKey, value);
            }
        }

        #endregion


        #region TickMark support

        #region IsSnapToTickEnabled property

        /// <summary>
        /// Gets or sets a value indicating whether the slider automatically moves
        /// the <see cref="System.Windows.Controls.Primitives.Track.Thumb"/> to the closest tick mark.
        /// </summary>
        /// <value>A value indicating whether the slider automatically moves
        /// the <see cref="System.Windows.Controls.Primitives.Track.Thumb"/> to the closest tick mark.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public bool IsSnapToTickEnabled
        {
            get
            {
                var res = this.GetValue(IsSnapToTickEnabledProperty);
                Debug.Assert(res != null, "res != null");
                return (bool)res;
            }

            set
            {
                this.SetValue(IsSnapToTickEnabledProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/>.
        /// </summary>
        public static readonly DependencyProperty IsSnapToTickEnabledProperty
            = DependencyProperty.Register(
                nameof(IsSnapToTickEnabled),
                typeof(bool),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(false, OnIsSnapToTickEnabledChanged));

        private static void OnIsSnapToTickEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is SimpleRangeSlider<T, TInterval>, "obj is SimpleRangeSlider<T, TInterval>");
            Debug.Assert(args.OldValue is bool, "args.OldValue is bool");
            Debug.Assert(args.NewValue is bool, "args.NewValue is bool");

            var element = obj as SimpleRangeSlider<T, TInterval>;
            Debug.Assert(element != null, "element != null");
            if (element == null)
            {
                return;
            }

            var newValue = (bool)args.NewValue;
            if (newValue)
            {
                // if movements of thumbs available only over tick marks,
                // then validation of min range value is OFF, because of difficalty of such validations for irregular ticks.
                // in theory, we may not switch OFF that validation,
                // but for that we have to compulsory set the minimum range value to multiple to ticks step (for regular),
                // or to equal to minimum interval (distance) value between tick marks (for irregular).
                // Anyway, validation of MinRangeValue property become difficult and laborious.
                element.MinRangeValueEnabled = false;

                // if IsSnapToTickEnabled goes to ON (from now thumbs can be only over tick marks),
                // then we have to align current values (thumbs) to nearest ticks.
                // shortcoming of that way: thumbs positions can be changed, therefore interval (range) value can be changed too
                // (but not necessarily).
                element.AlignValuesToTicks();
            }
            else
            {
                // if IsSnapToTickEnabled mode is OFF,
                // then we turns ON the validation over min range value.
                // or may be not - it depends of default value of appropriate dependency property.
                element.MinRangeValueEnabled = element.DefaultMinRangeValueEnabled;
            }
        }

        #endregion

        #region TickPlacement property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.TickPlacement"/>.
        /// </summary>
        public static readonly DependencyProperty TickPlacementProperty
            = DependencyProperty.Register(
                nameof(TickPlacement),
                typeof(TickPlacement),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(TickPlacement.None),
                IsValidTickPlacement);

        /// <summary>
        /// Gets or sets position of tick marks in a slider control with respect to the track that the control implements.
        /// </summary>
        /// <value>Position of tick marks in a slider control with respect to the track that the control implements.</value>
        [Bindable(true)]
        [Category("Appearance")]
        public TickPlacement TickPlacement
        {
            get
            {
                var res = this.GetValue(TickPlacementProperty);
                Debug.Assert(res != null, "res != null");
                return (TickPlacement)res;
            }

            set
            {
                this.SetValue(TickPlacementProperty, value);
            }
        }

        private static bool IsValidTickPlacement(object o)
        {
            if (!(o is TickPlacement))
            {
                return false;
            }

            var value = (TickPlacement)o;
            return value == TickPlacement.None ||
                   value == TickPlacement.TopLeft ||
                   value == TickPlacement.BottomRight ||
                   value == TickPlacement.Both;
        }

        #endregion

        #region TickFrequency property

        /// <summary>
        /// Gets or sets the interval between tick marks.
        /// Ignores if <see cref="SimpleRangeSlider{T, TInterval}.TypedTicksCollection"/> is not <c>null</c>.
        /// </summary>
        /// <value>The interval between tick marks.</value>
        [Bindable(true)]
        [Category("Appearance")]
        public TInterval TickFrequency
        {
            get
            {
                var res = this.GetValue(TickFrequencyProperty);
                Debug.Assert(res != null, "res != null");
                return (TInterval)res;
            }

            set
            {
                this.SetValue(TickFrequencyProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>.
        /// </summary>
        public static readonly DependencyProperty TickFrequencyProperty
            = DependencyProperty.Register(
                nameof(TickFrequency),
                typeof(TInterval),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(default(TInterval), OnTickFrequencyChanged),
                IsValidTickFrequency);

        private static void OnTickFrequencyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is SimpleRangeSlider<T, TInterval>, "obj is SimpleRangeSlider<T, TInterval>");
            Debug.Assert(args.OldValue is TInterval, "args.OldValue is TInterval");
            Debug.Assert(args.NewValue is TInterval, "args.NewValue is TInterval");

            var element = obj as SimpleRangeSlider<T, TInterval>;
            Debug.Assert(element != null, "element != null");
            if (element == null)
            {
                return;
            }

            if (element.IsSnapToTickEnabled)
            {
                // if tick's positions change and IsSnapToTickEnabled is ON,
                // then we have to align current values (thumbs) to nearest ticks.
                // shortcoming of that way: thumbs positions can be changed, therefore interval (range) value can be changed too
                // (but not necessarily).
                element.AlignValuesToTicks();
            }

            element.OnTickFrequencyChanged((TInterval)args.OldValue, (TInterval)args.NewValue);
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/> changes.
        /// </summary>
        /// <param name="oldTickFrequency">Gets the value of the <see cref="TickFrequency"/> before the change.</param>
        /// <param name="newTickFrequency">Gets the value of the <see cref="TickFrequency"/> after the change.</param>
        protected virtual void OnTickFrequencyChanged(TInterval oldTickFrequency, TInterval newTickFrequency)
        {
        }

        private static bool IsValidTickFrequency(object value)
        {
            return DependencyPropertyUtil.IsValidChange(typeof(TInterval), value);
        }

        #endregion

        #region TickLabelNumericFormat Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.TickLabelNumericFormat"/>.
        /// </summary>
        public static readonly DependencyProperty TickLabelNumericFormatProperty =
            DependencyProperty.Register(
                nameof(TickLabelNumericFormat),
                typeof(string),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets format string for conversion numeric labels to text.
        /// Empty string interprets as <c>null</c>.
        /// Uses only when <see cref="TickLabelConverter"/> is <c>null</c>.
        /// </summary>
        /// <value>Format string for conversion numeric labels to text.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public string TickLabelNumericFormat
        {
            get { return (string)this.GetValue(TickLabelNumericFormatProperty); }
            set { this.SetValue(TickLabelNumericFormatProperty, value); }
        }

        #endregion

        #region TickLabelConverter Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.TickLabelConverter"/>.
        /// </summary>
        public static readonly DependencyProperty TickLabelConverterProperty =
            DependencyProperty.Register(
                nameof(TickLabelConverter),
                typeof(IDoubleToStringConverter),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the converter of tick values to their string representations for showing in UI.
        /// </summary>
        /// <value>The converter of tick values to their string representations for showing in UI.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public IDoubleToStringConverter TickLabelConverter
        {
            get { return (IDoubleToStringConverter)this.GetValue(TickLabelConverterProperty); }
            set { this.SetValue(TickLabelConverterProperty, value); }
        }

        #endregion

        #region TickLabelConverterParameter Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.TickLabelConverterParameter"/>.
        /// </summary>
        public static readonly DependencyProperty TickLabelConverterParameterProperty =
            DependencyProperty.Register(
                nameof(TickLabelConverterParameter),
                typeof(object),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets additional parameter for <see cref="TickLabelConverter"/>.
        /// </summary>
        /// <value>Additional parameter for <see cref="TickLabelConverter"/>.</value>
        [Bindable(true)]
        [Category("Behavior")]
        public object TickLabelConverterParameter
        {
            get { return this.GetValue(TickLabelConverterParameterProperty); }
            set { this.SetValue(TickLabelConverterParameterProperty, value); }
        }

        #endregion

        #endregion TickMark support

        #endregion Dependency Properties

        #region Public Events

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDraggingChanged"/>.
        /// </summary>
        public static readonly RoutedEvent IsRangeDraggingChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(IsRangeDraggingChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<bool>),
                typeof(SimpleRangeSlider<T, TInterval>));

        /// <summary>
        /// Occurs when <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/> changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsRangeDraggingChanged
        {
            add { this.AddHandler(IsRangeDraggingChangedEvent, value); }
            remove { this.RemoveHandler(IsRangeDraggingChangedEvent, value); }
        }


        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.RangeDragCompleted"/>.
        /// </summary>
        public static readonly RoutedEvent RangeDragCompletedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(RangeDragCompleted),
                RoutingStrategy.Bubble,
                typeof(EventHandler<RangeDragCompletedEventArgs<T>>),
                typeof(SimpleRangeSlider<T, TInterval>));

        /// <summary>
        /// Occurs when the process of dragging of any of thumbs completed.
        /// </summary>
        public event EventHandler<RangeDragCompletedEventArgs<T>> RangeDragCompleted
        {
            add { this.AddHandler(RangeDragCompletedEvent, value); }
            remove { this.RemoveHandler(RangeDragCompletedEvent, value); }
        }

        /// <summary>
        /// Calls when user stops dragging of thumb.
        /// Here we simply raise <see cref="SimpleRangeSlider{T, TInterval}.RangeDragCompleted"/>.
        /// </summary>
        /// <param name="oldStartValue">The value of the <see cref="RangeBaseControl{T, TInterval}.StartValue"/> property before the drag started.</param>
        /// <param name="oldEndValue">The value of the <see cref="RangeBaseControl{T, TInterval}.EndValue"/> property before the drag started.</param>
        /// <param name="newStartValue">The value of the <see cref="RangeBaseControl{T, TInterval}.StartValue"/> property after the drag completed.</param>
        /// <param name="newEndValue">The value of the <see cref="RangeBaseControl{T, TInterval}.EndValue"/> property after the drag completed.</param>
        protected virtual void OnRangeDragCompleted(
            T oldStartValue,
            T oldEndValue,
            T newStartValue,
            T newEndValue)
        {
            var newEventArgs = new RangeDragCompletedEventArgs<T>(oldStartValue, oldEndValue, newStartValue, newEndValue)
            {
                RoutedEvent = RangeDragCompletedEvent,
            };
            this.RaiseEvent(newEventArgs);
        }

        #endregion

        #region Event Handlers

        private static void OnThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            if (!(sender is SimpleRangeSlider<T, TInterval>))
            {
                throw new ArgumentOutOfRangeException(nameof(sender));
            }

            var slider = sender as SimpleRangeSlider<T, TInterval>;
            slider.OnThumbDragStarted(e);
        }

        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!(sender is SimpleRangeSlider<T, TInterval>))
            {
                throw new ArgumentOutOfRangeException(nameof(sender));
            }

            if (!(e.OriginalSource is Thumb))
            {
                throw new ArgumentOutOfRangeException(nameof(e));
            }

            var slider = sender as SimpleRangeSlider<T, TInterval>;
            slider.OnThumbDragDelta(e);
        }

        private static void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!(sender is SimpleRangeSlider<T, TInterval>))
            {
                throw new ArgumentOutOfRangeException(nameof(sender));
            }

            var slider = sender as SimpleRangeSlider<T, TInterval>;
            slider.OnThumbDragCompleted(e);
        }

        /// <summary>
        /// Calls when user starts dragging the thumb.
        /// </summary>
        /// <param name="e">Information about event.</param>
        protected virtual void OnThumbDragStarted(DragStartedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Show AutoToolTip if needed.
            var thumb = e.OriginalSource as Thumb;
            if (thumb == null)
            {
                return;
            }

            // remember range values on the moment when drag started
            Debug.Assert(!this.m_RangeValueData.IsRangeDragging, "!this.m_RangeValueData.IsRangeDragging");
            this.m_RangeValueData.IsRangeDragging = true;
            this.m_RangeValueData.RangeStart = this.StartValue;
            this.m_RangeValueData.RangeEnd = this.EndValue;


            if (this.AutoToolTipPlacement == AutoToolTipPlacement.None)
            {
                return;
            }

            RangeThumbType thumbType = this.GetThumbType(thumb);

            // Save original tooltip
            this.m_ThumbOriginalToolTip = thumb.ToolTip;

            if (this.m_AutoToolTip == null)
            {
                this.m_AutoToolTip = new ToolTip
                {
                    Placement = PlacementMode.Custom,
                    CustomPopupPlacementCallback = this.AutoToolTipCustomPlacementCallback,
                };
            }

            this.m_AutoToolTip.PlacementTarget = thumb;
            this.m_AutoToolTip.Tag = thumbType;

            thumb.ToolTip = this.m_AutoToolTip;
            this.m_AutoToolTip.Content = this.GetAutoToolTipContent(this.IsSingleValue ? RangeThumbType.StartThumb : thumbType);
            this.m_AutoToolTip.IsOpen = true;

            var offset = this.m_AutoToolTip.HorizontalOffset;
            this.m_AutoToolTip.HorizontalOffset = offset + 1;
            this.m_AutoToolTip.HorizontalOffset = offset;
            // ((Popup)m_AutoToolTip.Parent).Reposition();
        }

        /// <summary>
        /// Calls when user moves the captured thumb.
        /// </summary>
        /// <param name="e">Information about event.</param>
        protected virtual void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (!(e.OriginalSource is Thumb))
            {
                throw new ArgumentOutOfRangeException(nameof(e));
            }

            this.IsRangeDragging = true; // do it here in order to not useless set this indicator in handler of DragStarted,
            // because in DragStarted this indicator will be set up even there was no real movement,
            // but was only mouse click over element (capture).

            var thumb = e.OriginalSource as Thumb;
            RangeThumbType thumbType = this.GetThumbType(thumb);
            // Convert to Track's co-ordinate
            if (this.Track != null && thumbType != RangeThumbType.None)
            {
                double valueFromDistance = this.Track.ValueFromDistance(e.HorizontalChange, e.VerticalChange);

                this.UpdateValueByThumbTypeAndDelta(this.IsSingleValue ? RangeThumbType.RangeThumb : thumbType, valueFromDistance);

                // Show AutoToolTip if needed
                if (this.AutoToolTipPlacement != AutoToolTipPlacement.None)
                {
                    if (this.m_AutoToolTip == null)
                    {
                        this.m_AutoToolTip = new ToolTip { Tag = thumbType };
                    }

                    this.m_AutoToolTip.Content = this.GetAutoToolTipContent(this.IsSingleValue ? RangeThumbType.StartThumb : thumbType);

                    if (!Equals(thumb.ToolTip, this.m_AutoToolTip))
                    {
                        thumb.ToolTip = this.m_AutoToolTip;
                    }

                    // Debug.Assert(this.m_AutoToolTip != null);
                    if (!this.m_AutoToolTip.IsOpen)
                    {
                        this.m_AutoToolTip.IsOpen = true;
                    }

                    var offset = this.m_AutoToolTip.HorizontalOffset;
                    this.m_AutoToolTip.HorizontalOffset = offset + 1;
                    this.m_AutoToolTip.HorizontalOffset = offset;
                    // ((Popup)m_AutoToolTip.Parent).Reposition();
                }
            }
        }

        /// <summary>
        /// Calls when user completed dragging the thumb.
        /// </summary>
        /// <param name="e">Information about event.</param>
        protected virtual void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var thumb = e.OriginalSource as Thumb;

            if (thumb == null)
            {
                return;
            }

            Debug.Assert(this.m_RangeValueData.IsRangeDragging, "this.m_RangeValueData.IsRangeDragging");
            var oldRangeValueData = this.m_RangeValueData;
            this.m_RangeValueData.IsRangeDragging = false;

            if (this.IsRangeDragging)
            {
                this.IsRangeDragging = false;
                this.OnRangeDragCompleted(
                    oldRangeValueData.RangeStart,
                    oldRangeValueData.RangeEnd,
                    this.StartValue,
                    this.EndValue);

                // if raising of ValueChanged event was OFF,
                // then we need to manually generate that event when drag completed,
                // otherwise it will be lost ("swallowed")
                if (!this.IsRaiseValueChangedWhileDragging &&
                    (!DoubleUtil.AreClose(this.ValueToDouble(oldRangeValueData.RangeStart), this.ValueToDouble(this.StartValue)) ||
                     !DoubleUtil.AreClose(this.ValueToDouble(oldRangeValueData.RangeEnd), this.ValueToDouble(this.EndValue))))
                {
                    base.OnValueChanged(oldRangeValueData.RangeStart, oldRangeValueData.RangeEnd, this.StartValue, this.EndValue);
                }
            }

            if (this.AutoToolTipPlacement == AutoToolTipPlacement.None)
            {
                return;
            }

            // Show AutoToolTip if needed.
            if (this.m_AutoToolTip != null)
            {
                this.m_AutoToolTip.IsOpen = false;
            }

            thumb.ToolTip = this.m_ThumbOriginalToolTip;
        }

        #endregion Event Handlers

        #region IRangeTrackTemplatedParent

        /// <summary>
        /// Method handles <see cref="FrameworkElement.OnApplyTemplate"/>,
        /// notably bind some dependency properties with templated parent.
        /// </summary>
        /// <param name="templatedParent">Templated parent.</param>
        /// <param name="track">Any range track.</param>
        public override void OnApplyRangeTrackTemplate(DependencyObject templatedParent, RangeTrack<T, TInterval> track)
        {
            if (track == null)
            {
                throw new ArgumentNullException(nameof(track));
            }

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

        #region Override Functions

        /// <summary>
        /// The handler for <see cref="Mouse.MouseDownEvent"/> event.
        /// Its purpose is to move input focus to <see cref="SimpleRangeSlider{T, TInterval}"/>,
        /// when user presses left mouse button over any part (element) of this slider,
        /// which is not focusable.
        /// </summary>
        private static void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            var slider = (SimpleRangeSlider<T, TInterval>)sender;

            // When someone click on the SimpleRangeSlider's part, and it's not focusable
            // SimpleRangeSlider need to take the focus in order to process keyboard correctly
            if (!slider.IsKeyboardFocusWithin)
            {
                e.Handled = slider.Focus() || e.Handled;
            }
        }

        /// <summary>
        /// Called to arrange and size the content of a <see cref="SimpleRangeSlider{T, TInterval}" />.
        /// </summary>
        /// <param name="arrangeBounds">The computed size that is used to arrange the content.</param>
        /// <returns>The size, which will be used for the content of a <see cref="SimpleRangeSlider{T, TInterval}" />.</returns>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            Size size = base.ArrangeOverride(arrangeBounds);

            this.UpdateTrackBackgroundPositionAndSize();

            return size;
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="RangeBaseControl{T, TInterval}.StartValue"/> changes.
        ///
        /// Overridden here for purpose of updating the position and size of <see cref="SimpleRangeSlider{T, TInterval}.TrackBackground"/>.
        /// </summary>
        /// <param name="oldValue">The value of the property before the change reported by the relevant event or state change.</param>
        /// <param name="newValue">The value of the property after the change reported by the relevant event or state change.</param>
        protected override void OnStartValueChanged(T oldValue, T newValue)
        {
            base.OnStartValueChanged(oldValue, newValue);
            this.UpdateTrackBackgroundPositionAndSize();
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="System.Windows.FrameworkElement.ApplyTemplate"/>().
        ///
        /// Invoked when visual tree of an element is created.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.TrackBackground = this.GetTemplateChild(TrackBackgroundPartName) as FrameworkElement;

            this.Track = this.GetTemplateChild(TrackPartName) as RangeTrack<T, TInterval>;
            if (this.Track != null)
            {
                this.Track.DoApplyTemplate();

                if (this.Track.StartThumb != null)
                {
                    this.Track.StartThumb.SizeChanged += (s, e) =>
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

                if (this.Track.EndThumb != null)
                {
                    this.Track.EndThumb.SizeChanged += (s, e) =>
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

            if (this.m_AutoToolTip != null)
            {
                Debug.Assert(this.m_AutoToolTip.Tag is RangeThumbType, "this.m_AutoToolTip.Tag is RangeThumbType");
                RangeThumbType thumbType = (this.m_AutoToolTip.Tag is RangeThumbType) ?
                    (RangeThumbType)this.m_AutoToolTip.Tag :
                    RangeThumbType.None;

                Thumb targetThumb = null;
                if (this.Track != null)
                {
                    switch (thumbType)
                    {
                        case RangeThumbType.StartThumb:
                            targetThumb = this.Track.StartThumb;
                            break;
                        case RangeThumbType.RangeThumb:
                            targetThumb = this.Track.RangeThumb;
                            break;
                        case RangeThumbType.EndThumb:
                            targetThumb = this.Track.EndThumb;
                            break;
                    }
                }

                this.m_AutoToolTip.PlacementTarget = targetThumb;
            }
        }

        /// <summary>
        /// Calls whenever the effective value of any of following properties changes: <see cref="RangeBaseControl{T, TInterval}.StartValue"/>,
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/> or <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>.
        ///
        /// Overridden here to treat with <see cref="SimpleRangeSlider{T, TInterval}.IsRaiseValueChangedWhileDragging"/>.
        /// </summary>
        /// <param name="oldStartValue">The value of the <see cref="RangeBaseControl{T, TInterval}.StartValue"/> property before the change reported by the relevant event or state change.</param>
        /// <param name="oldEndValue">The value of the <see cref="RangeBaseControl{T, TInterval}.EndValue"/> property before the change reported by the relevant event or state change.</param>
        /// <param name="newStartValue">The value of the <see cref="RangeBaseControl{T, TInterval}.StartValue"/> property after the change reported by the relevant event or state change.</param>
        /// <param name="newEndValue">The value of the <see cref="RangeBaseControl{T, TInterval}.EndValue"/> property after the change reported by the relevant event or state change.</param>
        protected override void OnValueChanged(T oldStartValue, T oldEndValue, T newStartValue, T newEndValue)
        {
            if (!this.IsRangeDragging || this.IsRaiseValueChangedWhileDragging)
            {
                base.OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
            }
        }

        #endregion Override Functions

        #region Abstract Functions

        /// <summary>
        /// Gets the collection of tick marks of appropriate type.
        ///
        /// Must be implemented in derived classes.
        /// </summary>
        /// <value>The collection of tick marks of appropriate type.</value>
        protected abstract ITicksCollection<T> TypedTicksCollection { get; }

        /// <summary>
        /// Gets the string representation of <paramref name="value"/> for showing in tooltips.
        ///
        /// Must be implemented in derived classes.
        /// </summary>
        /// <param name="value">Current value.</param>
        /// <param name="thumbType">The type of thumb which value is <paramref name="value"/>.</param>
        /// <returns>String representation of <paramref name="value"/> for thumb, which type is <paramref name="thumbType"/>.</returns>
        protected abstract string GetAutoToolTipString(T value, RangeThumbType thumbType);

        #endregion

        #region Virtual Functions

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseRangeLarge"/>.
        /// </summary>
        protected virtual void OnIncreaseRangeLarge()
        {
            this.MoveRangeToNextTick(this.LargeChange, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseStartLarge"/>.
        /// </summary>
        protected virtual void OnIncreaseStartLarge()
        {
            this.MoveToNextTick(this.LargeChange, false, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseEndLarge"/>.
        /// </summary>
        protected virtual void OnIncreaseEndLarge()
        {
            this.MoveToNextTick(this.LargeChange, false, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseRangeLarge"/>.
        /// </summary>
        protected virtual void OnDecreaseRangeLarge()
        {
            this.MoveRangeToNextTick(this.LargeChange, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseStartLarge"/>.
        /// </summary>
        protected virtual void OnDecreaseStartLarge()
        {
            this.MoveToNextTick(this.LargeChange, true, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseEndLarge"/>.
        /// </summary>
        protected virtual void OnDecreaseEndLarge()
        {
            this.MoveToNextTick(this.LargeChange, true, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseRangeSmall"/>.
        /// </summary>
        protected virtual void OnIncreaseRangeSmall()
        {
            this.MoveRangeToNextTick(this.SmallChange, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseStartSmall"/>.
        /// </summary>
        protected virtual void OnIncreaseStartSmall()
        {
            this.MoveToNextTick(this.SmallChange, false, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseEndSmall"/>.
        /// </summary>
        protected virtual void OnIncreaseEndSmall()
        {
            this.MoveToNextTick(this.SmallChange, false, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseRangeSmall"/>.
        /// </summary>
        protected virtual void OnDecreaseRangeSmall()
        {
            this.MoveRangeToNextTick(this.SmallChange, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseStartSmall"/>.
        /// </summary>
        protected virtual void OnDecreaseStartSmall()
        {
            this.MoveToNextTick(this.SmallChange, true, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseEndSmall"/>.
        /// </summary>
        protected virtual void OnDecreaseEndSmall()
        {
            this.MoveToNextTick(this.SmallChange, true, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MaximizeRangeValue"/>.
        /// </summary>
        protected virtual void OnMaximizeRangeValue()
        {
            double delta = this.ValueToDouble(this.Maximum) - this.ValueToDouble(this.EndValue);
            TInterval oldRangeValue = this.RangeValue;
            T oldStartValue = this.StartValue, oldEndValue = this.EndValue;
            this.IsRangeValueChanging = true;
            try
            {
                this.SetCurrentValue(EndValueProperty, this.Maximum);
                this.SetCurrentValue(StartValueProperty, this.DoubleToValue(this.ValueToDouble(this.StartValue) + delta));
            }
            finally
            {
                this.IsRangeValueChanging = false;
            }

            TInterval newRangeValue = this.RangeValue;
            T newStartValue = this.StartValue, newEndValue = this.EndValue;
            if (!DoubleUtil.AreClose(this.ValueToDouble(oldStartValue), this.ValueToDouble(newStartValue)) ||
                !DoubleUtil.AreClose(this.ValueToDouble(oldEndValue), this.ValueToDouble(newEndValue)))
            {
                this.OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
            }

            if (!DoubleUtil.AreClose(this.IntervalToDouble(oldRangeValue), this.IntervalToDouble(newRangeValue)))
            {
                this.OnRangeValueChanged(oldRangeValue, newRangeValue);
            }
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MaximizeStartValue"/>.
        /// </summary>
        protected virtual void OnMaximizeStartValue()
        {
            this.SetCurrentValue(StartValueProperty, this.Maximum);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MaximizeEndValue"/>.
        /// </summary>
        protected virtual void OnMaximizeEndValue()
        {
            this.SetCurrentValue(EndValueProperty, this.Maximum);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MinimizeRangeValue"/>.
        /// </summary>
        protected virtual void OnMinimizeRangeValue()
        {
            double delta = this.ValueToDouble(this.StartValue) - this.ValueToDouble(this.Minimum);
            TInterval oldRangeValue = this.RangeValue;
            T oldStartValue = this.StartValue, oldEndValue = this.EndValue;
            this.IsRangeValueChanging = true;
            try
            {
                this.SetCurrentValue(StartValueProperty, this.Minimum);
                this.SetCurrentValue(EndValueProperty, this.DoubleToValue(this.ValueToDouble(this.EndValue) - delta));
            }
            finally
            {
                this.IsRangeValueChanging = false;
            }

            TInterval newRangeValue = this.RangeValue;
            T newStartValue = this.StartValue, newEndValue = this.EndValue;
            if (!DoubleUtil.AreClose(this.ValueToDouble(oldStartValue), this.ValueToDouble(newStartValue)) ||
                !DoubleUtil.AreClose(this.ValueToDouble(oldEndValue), this.ValueToDouble(newEndValue)))
            {
                this.OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
            }

            if (!DoubleUtil.AreClose(this.IntervalToDouble(oldRangeValue), this.IntervalToDouble(newRangeValue)))
            {
                this.OnRangeValueChanged(oldRangeValue, newRangeValue);
            }
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MinimizeStartValue"/>.
        /// </summary>
        protected virtual void OnMinimizeStartValue()
        {
            this.SetCurrentValue(StartValueProperty, this.Minimum);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MinimizeEndValue"/>.
        /// </summary>
        protected virtual void OnMinimizeEndValue()
        {
            this.SetCurrentValue(EndValueProperty, this.Minimum);
        }

        #endregion Virtual Functions

        #region Helper Functions

        private string GetAutoToolTipContent(RangeThumbType thumbType)
        {
            var res = new StringBuilder();

            if (thumbType == RangeThumbType.StartThumb || thumbType == RangeThumbType.RangeThumb)
            {
                res.Append(this.GetAutoToolTipString(this.StartValue, thumbType) ?? string.Empty);
            }

            if (thumbType == RangeThumbType.RangeThumb)
            {
                res.Append(AutoToolTipTextRangeDelimiter);
            }

            if (thumbType == RangeThumbType.EndThumb || thumbType == RangeThumbType.RangeThumb)
            {
                res.Append(this.GetAutoToolTipString(this.EndValue, thumbType) ?? string.Empty);
            }

            return res.ToString();
        }

        private CustomPopupPlacement[] AutoToolTipCustomPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            switch (this.AutoToolTipPlacement)
            {
                case AutoToolTipPlacement.TopLeft:
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        // Place popup at top of thumb
                        return new[]
                            {
                                new CustomPopupPlacement(
                                    new Point((targetSize.Width - popupSize.Width) * 0.5, -popupSize.Height),
                                    PopupPrimaryAxis.Horizontal),
                            };
                    }

                    // Place popup at left of thumb
                    return new[]
                        {
                            new CustomPopupPlacement(
                                new Point(-popupSize.Width, (targetSize.Height - popupSize.Height) * 0.5),
                                PopupPrimaryAxis.Vertical),
                        };

                case AutoToolTipPlacement.BottomRight:
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        // Place popup at bottom of thumb
                        return new[]
                            {
                                new CustomPopupPlacement(
                                    new Point((targetSize.Width - popupSize.Width) * 0.5, targetSize.Height),
                                    PopupPrimaryAxis.Horizontal),
                            };
                    }

                    // Place popup at right of thumb
                    return new[]
                        {
                            new CustomPopupPlacement(
                                new Point(targetSize.Width, (targetSize.Height - popupSize.Height) * 0.5),
                                PopupPrimaryAxis.Vertical),
                        };

                default:
                    return Array.Empty<CustomPopupPlacement>();
            }
        }

        /// <summary>
        /// Method changes the size and position of element <see cref="SimpleRangeSlider{T, TInterval}.TrackBackground"/>.
        /// </summary>
        private void UpdateTrackBackgroundPositionAndSize()
        {
            if (this.Track == null || this.TrackBackground == null)
            {
                return;
            }

            Size startThumbSize = (this.Track.StartThumb != null) ? this.Track.StartThumb.RenderSize : new Size(0d, 0d);
            Size endThumbSize = (this.Track.EndThumb != null) ? this.Track.EndThumb.RenderSize : new Size(0d, 0d);
            var margin = this.TrackBackground.Margin;
            if (this.Orientation == Orientation.Horizontal)
            {
                margin.Left = startThumbSize.Width;
                margin.Right = endThumbSize.Width;
            }
            else
            {
                margin.Top = startThumbSize.Height;
                margin.Bottom = endThumbSize.Height;
            }

            this.TrackBackground.Margin = margin;
        }

        /// <summary>
        /// Evaluates nearest to the <paramref name="value"/> tick value on the tick bar.
        /// If <paramref name="value"/> located exactly in the middle of the neighboring ticks,
        /// then the nearest assume equal to the greatest value.
        /// </summary>
        /// <param name="value">Income value which have to be snaped to the nearest tick mark.</param>
        /// <param name="isStartThumb">Whether income value is value of start thumb.</param>
        /// <param name="useFullScale">If <c>true</c>, then the nearest value searches over the whole tick bar's scale,
        /// independently of value of the other thumb -
        /// that means, that evaluated value can be placed outside of available for that value interval.
        /// If <c>false</c>, then the nearest value searches inside of available interval -
        /// in that case needed nearest tick mark can be outside of available interval,
        /// but this function returns one of the bounds and if that bound not snaped to tick mark,
        /// then returned value wont be snaped too.</param>
        /// <returns>If <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/> is <c>true</c>,
        /// then evaluated nearest tick mark value on the tick bar.
        /// Otherwise, <paramref name="value"/> itself.</returns>
        private double SnapToTick(double value, bool isStartThumb, bool useFullScale)
        {
            if (this.IsSnapToTickEnabled)
            {
                T min = this.Minimum, max = this.Maximum;
                double previous = this.ValueToDouble((useFullScale || isStartThumb) ? min : this.StartValue);
                double next = this.ValueToDouble((!useFullScale && isStartThumb) ? this.EndValue : max);

                ITicksCollection<T> ticks = this.TypedTicksCollection;

                // If ticks collection is available, use it.
                // Note that ticks may be unsorted.
                if ((ticks != null) && (ticks.Count > 0))
                {
                    for (int i = 0; i < ticks.Count; i++)
                    {
                        double tick = this.ValueToDouble(ticks[i]);
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
                else if (DoubleUtil.GreaterThan(this.IntervalToDouble(this.TickFrequency), 0.0))
                {
                    double tickFrequency = this.IntervalToDouble(this.TickFrequency);
                    previous = this.ValueToDouble(min) + (Math.Round((value - this.ValueToDouble(min)) / tickFrequency) * tickFrequency);
                    next = Math.Min(this.ValueToDouble(max), previous + tickFrequency);
                }

                // Choose the closest value between previous and next. If tie, snap to 'next'.
                value = DoubleUtil.GreaterThanOrClose(value, (previous + next) * 0.5) ? next : previous;
            }

            return value;
        }

        /// <summary>
        /// If <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/> is <c>true</c>,
        /// then method aligns values <see cref="RangeBaseControl{T, TInterval}.StartValue"/> and
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/>, moving them to the nearest tick mark.
        /// There is probability that will be changed value of interval <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>.
        /// If <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/> is <c>false</c>, then method do nothing.
        /// </summary>
        protected void AlignValuesToTicks()
        {
            if (!this.IsSnapToTickEnabled)
            {
                return;
            }

            T oldStartValue = this.StartValue, oldEndValue = this.EndValue;
            double startValue = this.ValueToDouble(oldStartValue), endValue = this.ValueToDouble(oldEndValue);
            double snappedStartValue = this.SnapToTick(startValue, true, true);
            double snappedEndValue = this.SnapToTick(endValue, false, true);

            Debug.Assert(DoubleUtil.LessThanOrClose(snappedStartValue, snappedEndValue), "DoubleUtil.LessThanOrClose(snappedStartValue, snappedEndValue)");
            if (DoubleUtil.GreaterThan(snappedStartValue, snappedEndValue))
            {
                return;
            }

            TInterval oldRangeValue = this.RangeValue;
            this.IsRangeValueChanging = true;
            try
            {
                if (snappedStartValue < endValue)
                {
                    // at first safely move the left thumb, because it wont thrust to the right thumb.
                    if (!DoubleUtil.AreClose(snappedStartValue, startValue))
                    {
                        this.SetCurrentValue(StartValueProperty, this.DoubleToValue(snappedStartValue));
                    }

                    if (!DoubleUtil.AreClose(snappedEndValue, endValue))
                    {
                        this.SetCurrentValue(EndValueProperty, this.DoubleToValue(snappedEndValue));
                    }
                }
                else
                {
                    // at first safely move the right thumb, because it is in the way of the left thumb ("hampers" it).
                    if (!DoubleUtil.AreClose(snappedEndValue, endValue))
                    {
                        this.SetCurrentValue(EndValueProperty, this.DoubleToValue(snappedEndValue));
                    }

                    if (!DoubleUtil.AreClose(snappedStartValue, startValue))
                    {
                        this.SetCurrentValue(StartValueProperty, this.DoubleToValue(snappedStartValue));
                    }
                }
            }
            finally
            {
                this.IsRangeValueChanging = false;
            }

            TInterval newRangeValue = this.RangeValue;
            T newStartValue = this.StartValue, newEndValue = this.EndValue;
            if (!DoubleUtil.AreClose(this.ValueToDouble(oldStartValue), this.ValueToDouble(newStartValue)) ||
                !DoubleUtil.AreClose(this.ValueToDouble(oldEndValue), this.ValueToDouble(newEndValue)))
            {
                this.OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
            }

            if (!DoubleUtil.AreClose(this.IntervalToDouble(oldRangeValue), this.IntervalToDouble(newRangeValue)))
            {
                this.OnRangeValueChanged(oldRangeValue, newRangeValue);
            }
        }

        /// <summary>
        /// Method performs real move of one of the thumbs (and changes value of appropriate property: <see cref="RangeBaseControl{T, TInterval}.StartValue"/>
        /// or <see cref="RangeBaseControl{T, TInterval}.EndValue"/>) on the value of <paramref name="direction"/> relatively to the current position,
        /// if new position differ to the current.
        /// Herewith, move performs to appropriate tick mark if needed.
        /// When method returns, the value of parameter <paramref name="direction"/> goes to the real value of movement,
        /// because in fact it can be different from initial parameter value.
        /// </summary>
        /// <param name="direction">Move value. Can be negative.</param>
        /// <param name="isStartThumb"><c>true</c>, if require move the start thumb, and <c>false</c> - if the end thumb.</param>
        /// <returns><c>true</c> if real movement happened and appropriate property value changed; otherwise, <c>false</c>.</returns>
        private bool InternalMoveToNextTick(ref double direction, bool isStartThumb)
        {
            Debug.Assert(!double.IsNaN(direction), "!double.IsNaN(direction)");
            Debug.Assert(!DoubleUtil.AreClose(direction, 0.0), "!DoubleUtil.AreClose(direction, 0.0)");
            Debug.Assert(!double.IsNaN(direction), "!double.IsNaN(direction)");

            double value = this.ValueToDouble(isStartThumb ? this.StartValue : this.EndValue);
            double min = this.ValueToDouble(isStartThumb ? this.Minimum : this.StartValue);
            double max = this.ValueToDouble(isStartThumb ? this.EndValue : this.Maximum);

            // Find the next value by snapping
            double next = this.SnapToTick(Math.Max(min, Math.Min(max, value + direction)), isStartThumb, false);

            bool greaterThan = direction > 0; // search for the next tick greater than value?

            // If the snapping brought us back to value, find the next tick point
            if (DoubleUtil.AreClose(next, value)
                && !(greaterThan && DoubleUtil.AreClose(value, max)) // Stop if searching up if already at Max
                && !(!greaterThan && DoubleUtil.AreClose(value, min)) // Stop if searching down if already at Min
                )
            {
                ITicksCollection<T> ticks = this.TypedTicksCollection;

                // If ticks collection is available, use it.
                // Note that ticks may be unsorted.
                if ((ticks != null) && (ticks.Count > 0))
                {
                    for (int i = 0; i < ticks.Count; i++)
                    {
                        double tick = this.ValueToDouble(ticks[i]);

                        // Find the smallest tick greater than value or the largest tick less than value
                        if ((greaterThan && DoubleUtil.GreaterThan(tick, value) && (DoubleUtil.LessThan(tick, next) || DoubleUtil.AreClose(next, value)))
                         || (!greaterThan && DoubleUtil.LessThan(tick, value) && (DoubleUtil.GreaterThan(tick, next) || DoubleUtil.AreClose(next, value))))
                        {
                            next = tick;
                        }
                    }
                }
                else if (DoubleUtil.GreaterThan(this.IntervalToDouble(this.TickFrequency), 0.0))
                {
                    // Find the current tick we are at
                    double tickFrequency = this.IntervalToDouble(this.TickFrequency);
                    double tickNumber = Math.Round((value - this.ValueToDouble(this.Minimum)) / tickFrequency) + Math.Sign(direction);
                    next = this.ValueToDouble(this.Minimum) + (tickNumber * tickFrequency);
                }
            }

            // Update if we've found a better value
            bool changed = !DoubleUtil.AreClose(next, value);
            if (changed)
            {
                direction = next - value;
                this.SetCurrentValue(isStartThumb ? StartValueProperty : EndValueProperty, this.DoubleToValue(next));
            }

            Debug.Assert(!double.IsNaN(direction), "!double.IsNaN(direction)");
            return changed;
        }

        /// <summary>
        /// Method moves one of the thumbs (and changes value of appropriate property: <see cref="RangeBaseControl{T, TInterval}.StartValue"/>
        /// or <see cref="RangeBaseControl{T, TInterval}.EndValue"/>) on the value of <paramref name="direction"/> relatively to the current position,
        /// if new position differ to the current.
        /// Herewith, move performs to appropriate tick mark if needed.
        /// </summary>
        /// <param name="direction">Move value. <c>double</c> representation of this value must be not negative,
        /// because "negativeness" controls by <paramref name="isNegativeDirection"/> value.
        /// Moreover, some interval types <typeparamref name="TInterval"/> may not support negative values at all.</param>
        /// <param name="isNegativeDirection">Indicates whether move should be performed to "negative" (increasing) direction.</param>
        /// <param name="isStartThumb"><c>true</c>, if require move the start thumb, and <c>false</c> - if the end thumb.</param>
        /// <returns><c>true</c> if real movement happened and appropriate property value changed; otherwise, <c>false</c>.</returns>
        public bool MoveToNextTick(TInterval direction, bool isNegativeDirection, bool isStartThumb)
        {
            if (this.IsSingleValue)
            {
                return this.MoveRangeToNextTick(direction, isNegativeDirection);
            }

            double doubleDirection = (isNegativeDirection ? -1.0 : 1.0) * this.IntervalToDouble(direction);

            if (DoubleUtil.AreClose(doubleDirection, 0.0))
            {
                return false;
            }

            Debug.Assert(!double.IsNaN(doubleDirection), "!double.IsNaN(doubleDirection)");
            return this.InternalMoveToNextTick(ref doubleDirection, isStartThumb);
        }

        /// <summary>
        /// Method moves the whole interval on the value of <paramref name="direction"/> relatively to the current position
        /// (i.e. start thumb and end thumb simultaneously),
        /// if new position differ to the current.
        /// Herewith, move performs to appropriate tick mark if needed.
        /// In contrast to sequential move of the thumbs on the same value
        /// this method doesn't raise <see cref="RangeBaseControl{T, TInterval}.RangeValueChangedEvent"/> event,
        /// because interval value (distance) remains the same.
        /// </summary>
        /// <param name="direction">Move value. <c>double</c> representation of this value must be not negative,
        /// because "negativeness" controls by <paramref name="isNegativeDirection"/> value.
        /// Moreover, some interval types <typeparamref name="TInterval"/> may not support negative values at all.</param>
        /// <param name="isNegativeDirection">Indicates whether move should be performed to "negative" (increasing) direction.</param>
        /// <returns><c>true</c> if real movement happened and appropriate properties' values changed; otherwise, <c>false</c>.</returns>
        public bool MoveRangeToNextTick(TInterval direction, bool isNegativeDirection)
        {
            bool changed = false;
            double doubleDirection = (isNegativeDirection ? -1.0 : 1.0) * this.IntervalToDouble(direction);
            TInterval oldRangeValue = this.RangeValue;
            T oldStartValue = this.StartValue, oldEndValue = this.EndValue;
            this.IsRangeValueChanging = true;
            try
            {
                if (doubleDirection > 0)
                {
                    Debug.Assert(!DoubleUtil.AreClose(doubleDirection, 0.0), "!DoubleUtil.AreClose(doubleDirection, 0.0)");
                    changed = this.InternalMoveToNextTick(ref doubleDirection, false);
                    if (changed)
                    {
                        Debug.Assert(!DoubleUtil.AreClose(doubleDirection, 0.0), "!DoubleUtil.AreClose(doubleDirection, 0.0)");
                        this.InternalMoveToNextTick(ref doubleDirection, true);
                    }
                }
                else if (doubleDirection < 0)
                {
                    Debug.Assert(!DoubleUtil.AreClose(doubleDirection, 0.0), "!DoubleUtil.AreClose(doubleDirection, 0.0)");
                    changed = this.InternalMoveToNextTick(ref doubleDirection, true);
                    if (changed)
                    {
                        Debug.Assert(!DoubleUtil.AreClose(doubleDirection, 0.0), "!DoubleUtil.AreClose(doubleDirection, 0.0)");
                        this.InternalMoveToNextTick(ref doubleDirection, false);
                    }
                }
            }
            finally
            {
                this.IsRangeValueChanging = false;
            }

            TInterval newRangeValue = this.RangeValue;
            T newStartValue = this.StartValue, newEndValue = this.EndValue;
            if (!DoubleUtil.AreClose(this.ValueToDouble(oldStartValue), this.ValueToDouble(newStartValue)) ||
                !DoubleUtil.AreClose(this.ValueToDouble(oldEndValue), this.ValueToDouble(newEndValue)))
            {
                this.OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
            }

            if (!DoubleUtil.AreClose(this.IntervalToDouble(oldRangeValue), this.IntervalToDouble(newRangeValue)))
            {
                this.OnRangeValueChanged(oldRangeValue, newRangeValue);
            }

            return changed;
        }

        /// <summary>
        /// Updates the value of property <see cref="RangeBaseControl{T, TInterval}.StartValue"/> or
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/>, shifting it on the value of <paramref name="delta"/>.
        /// Herewith, shift performs to appropriate tick mark if needed.
        /// </summary>
        /// <param name="delta">Shift value.
        /// If 0, either do nothing or snap appropriate thumb to the nearest tick mark.</param>
        /// <param name="isStartThumb"><c>true</c>, if require shift the start thumb, and <c>false</c> - if the end thumb.</param>
        /// <returns>Real value of shift, because it can be different from parameter <paramref name="delta"/> value
        /// due to range limitations.</returns>
        protected double UpdateValueByDelta(double delta, bool isStartThumb)
        {
            double realDelta = 0d; // actually worked delta
            double startValue = this.ValueToDouble(this.StartValue), endValue = this.ValueToDouble(this.EndValue);
            double newValue = (isStartThumb ? startValue : endValue) + delta;
            if (DoubleUtil.IsDoubleFinite(newValue))
            {
                var snappedValue = this.SnapToTick(newValue, isStartThumb, false);

                if (!DoubleUtil.AreClose(snappedValue, isStartThumb ? startValue : endValue))
                {
                    if (isStartThumb)
                    {
                        newValue = Math.Max(this.ValueToDouble(this.Minimum), Math.Min(endValue, snappedValue));
                        realDelta = newValue - startValue;
                        this.SetCurrentValue(StartValueProperty, this.DoubleToValue(newValue));
                    }
                    else
                    {
                        newValue = Math.Max(startValue, Math.Min(this.ValueToDouble(this.Maximum), snappedValue));
                        realDelta = newValue - endValue;
                        this.SetCurrentValue(EndValueProperty, this.DoubleToValue(newValue));
                    }
                }
            }

            return realDelta;
        }

        /// <summary>
        /// Updates the value of property <see cref="RangeBaseControl{T, TInterval}.StartValue"/> or
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/>, shifting it on the value of <paramref name="delta"/>.
        /// Herewith, shift performs to appropriate tick mark if needed.
        /// </summary>
        /// <param name="thumbType">Thumb type, which value updates.</param>
        /// <param name="delta">Shift value.
        /// If 0, either do nothing or snap appropriate thumb to the nearest tick mark.</param>
        protected void UpdateValueByThumbTypeAndDelta(RangeThumbType thumbType, double delta)
        {
            switch (thumbType)
            {
                case RangeThumbType.StartThumb:
                    this.UpdateValueByDelta(delta, true);
                    break;

                case RangeThumbType.RangeThumb:
                    TInterval oldRangeValue = this.RangeValue;
                    T oldStartValue = this.StartValue, oldEndValue = this.EndValue;
                    this.IsRangeValueChanging = true;
                    try
                    {
                        if (DoubleUtil.GreaterThanOrClose(delta, 0.0))
                        {
                            double endValue = this.ValueToDouble(this.EndValue), max = this.ValueToDouble(this.Maximum);
                            if (DoubleUtil.LessThan(endValue, max))
                            {
                                delta = this.UpdateValueByDelta(delta, false);
                                this.UpdateValueByDelta(delta, true);
                            }
                        }
                        else
                        {
                            double startValue = this.ValueToDouble(this.StartValue), min = this.ValueToDouble(this.Minimum);
                            if (DoubleUtil.GreaterThan(startValue, min))
                            {
                                delta = this.UpdateValueByDelta(delta, true);
                                this.UpdateValueByDelta(delta, false);
                            }
                        }
                    }
                    finally
                    {
                        this.IsRangeValueChanging = false;
                    }

                    TInterval newRangeValue = this.RangeValue;
                    T newStartValue = this.StartValue, newEndValue = this.EndValue;
                    if (!DoubleUtil.AreClose(this.ValueToDouble(oldStartValue), this.ValueToDouble(newStartValue)) ||
                        !DoubleUtil.AreClose(this.ValueToDouble(oldEndValue), this.ValueToDouble(newEndValue)))
                    {
                        this.OnValueChanged(oldStartValue, oldEndValue, newStartValue, newEndValue);
                    }

                    if (!DoubleUtil.AreClose(this.IntervalToDouble(oldRangeValue), this.IntervalToDouble(newRangeValue)))
                    {
                        this.OnRangeValueChanged(oldRangeValue, newRangeValue);
                    }

                    break;

                case RangeThumbType.EndThumb:
                    this.UpdateValueByDelta(delta, false);
                    break;
            }
        }

        private RangeThumbType GetThumbType(Thumb thumb)
        {
            if (thumb == null || this.Track == null)
            {
                return RangeThumbType.None;
            }

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

#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1202 // Elements should be ordered by access
#pragma warning restore CA1000 // Do not declare static members on generic types
    }
}
