// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimpleRangeSliders.cs" company="Sane Development">
//
//   Sane Development WPF Controls Library
//
//   The BSD 3-Clause License
//
//   Copyright (c) Sane Development
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using SaneDevelopment.WPF.Controls.Properties;

namespace SaneDevelopment.WPF.Controls
{
    /// <summary>
    /// Describes operation of conversion some range slider value to string.
    /// 
    /// Objects that implement this interface supposed to use for conversion values of <see cref="SimpleRangeSlider{T, TInterval}"/>
    /// into their string representation for showing in UI to user
    /// (e.g. in tooltips etc.)
    /// </summary>
    /// <typeparam name="T">Range slider value type</typeparam>
    public interface IRangeValueToStringConverter<in T>
    {
        /// <summary>
        /// Converts <paramref name="value"/> to <c>string</c>.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="thumbType">Type of the thumb, which value is <paramref name="value"/>.</param>
        /// <param name="parameter">Additional parameter for convertion.</param>
        /// <returns>String representation of <paramref name="value"/></returns>
        string Convert(T value, RangeThumbType thumbType, object parameter);
    }

    /// <summary>
    /// Common class for range sliders.
    /// 
    /// Separated to decrease code duplication.
    /// </summary>
    /// <typeparam name="T">Values type</typeparam>
    /// <typeparam name="TInterval">Interval type</typeparam>
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
        /// Method handles <see cref="FrameworkElement.OnApplyTemplate"/>,
        /// notably bind some dependency properties with templated parent.
        /// </summary>
        /// <param name="templatedParent">Templated parent</param>
        /// <param name="track">Any range track</param>
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

        #region Large increasing

        /// <summary>
        /// Routed command for large increasing of interval (range) value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseRangeLarge { get; private set; }
        
        /// <summary>
        /// Routed command for large increasing of start value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseStartLarge { get; private set; }
        
        /// <summary>
        /// Routed command for large increasing of end value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseEndLarge { get; private set; }
        
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand IncreaseLargeByKey { get; set; }

        #endregion Large increasing

        #region Large decreasing

        /// <summary>
        /// Routed command for large decreasing of interval (range) value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseRangeLarge { get; private set; }
        
        /// <summary>
        /// Routed command for large decreasing of start value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseStartLarge { get; private set; }
        
        /// <summary>
        /// Routed command for large decreasing of end value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseEndLarge { get; private set; }
        
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand DecreaseLargeByKey { get; set; }

        #endregion Large decreasing

        #region Small increasing

        /// <summary>
        /// Routed command for small increasing of interval (range) value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseRangeSmall { get; private set; }
        
        /// <summary>
        /// Routed command for small increasing of start value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseStartSmall { get; private set; }
        
        /// <summary>
        /// Routed command for small increasing of end value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand IncreaseEndSmall { get; private set; }
        
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand IncreaseSmallByKey { get; set; }

        #endregion Small increasing

        #region Small decreasing

        /// <summary>
        /// Routed command for small decreasing of interval (range) value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseRangeSmall { get; private set; }
        
        /// <summary>
        /// Routed command for small decreasing of start value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseStartSmall { get; private set; }
        
        /// <summary>
        /// Routed command for small decreasing of end value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand DecreaseEndSmall { get; private set; }
        
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand DecreaseSmallByKey { get; set; }

        #endregion Small decreasing

        #region Minimize

        /// <summary>
        /// Routed command for minimizing of interval (range) value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MinimizeRangeValue { get; private set; }
        
        /// <summary>
        /// Routed command for minimizing of start value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MinimizeStartValue { get; private set; }
        
        /// <summary>
        /// Routed command for minimizing of end value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MinimizeEndValue { get; private set; }
        
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand MinimizeValueByKey { get; set; }

        #endregion Minimize

        #region Maximize

        /// <summary>
        /// Routed command for maximizing of interval (range) value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MaximizeRangeValue { get; private set; }
        
        /// <summary>
        /// Routed command for maximizing of start value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MaximizeStartValue { get; private set; }
        
        /// <summary>
        /// Routed command for maximizing of end value
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static RoutedCommand MaximizeEndValue { get; private set; }
        
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        private static RoutedCommand MaximizeValueByKey { get; set; }

        #endregion Maximize

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
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
        protected abstract bool IncreaseStartValueCommandCanExecute();
        
        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the increasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
        protected abstract bool IncreaseEndValueCommandCanExecute();
        
        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of start value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
        protected abstract bool DecreaseStartValueCommandCanExecute();
        
        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
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
            Debug.Assert(hasLeftCtrl ^ hasRightCtrl);

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
            Debug.Assert(res != RangeThumbType.None, "res != RangeThumbType.None");

            return res;
        }

        #endregion Commands

        #region Dependency Properties

        #region Orientation Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.Orientation"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty OrientationProperty =
// ReSharper restore StaticFieldInGenericType
                DependencyProperty.Register("Orientation", typeof(Orientation), typeof(SimpleRangeSlider<T, TInterval>),
                                          new FrameworkPropertyMetadata(Orientation.Horizontal),
                                          DependencyPropertyUtil.IsValidOrientation);

        /// <summary>
        /// Control orientation
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public Orientation Orientation
        {
            get
            {
                var res = GetValue(OrientationProperty);
                Debug.Assert(res != null);
                return (Orientation) res;
            }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region IsDragRangeEnabled Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.IsDragRangeEnabled"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty IsDragRangeEnabledProperty =
// ReSharper restore StaticFieldInGenericType
                DependencyProperty.Register("IsDragRangeEnabled", typeof(bool), typeof(SimpleRangeSlider<T, TInterval>),
                                          new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Whether enabled the whole range dragging by appropriate thumb.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsDragRangeEnabled
        {
            get
            {
                var res = GetValue(IsDragRangeEnabledProperty);
                Debug.Assert(res != null);
                return (bool) res;
            }
            set { SetValue(IsDragRangeEnabledProperty, value); }
        }

        #endregion

        #region IsRangeDragging

        /// <summary>
        /// Gets the value that indicates whether the control is in process of changing some value via dragging a thumb.
        /// Lets to know that user is moving the one of the thumbs right now.
        /// </summary>
        [Category("Common")]
        public bool IsRangeDragging
        {
            get
            {
                var res = GetValue(IsRangeDraggingProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty IsRangeDraggingProperty = IsRangeDraggingPropertyKey.DependencyProperty;
// ReSharper restore StaticFieldInGenericType

        private static void OnIsRangeDraggingChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Debug.Assert(obj is SimpleRangeSlider<T, TInterval>);
            Debug.Assert(args.OldValue is bool);
            Debug.Assert(args.NewValue is bool);

            var element = obj as SimpleRangeSlider<T, TInterval>;
            Debug.Assert(element != null, "element != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (element == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

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
                RoutedEvent = IsRangeDraggingChangedEvent
            };
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region IsRaiseValueChangedWhileDragging Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.IsRaiseValueChangedWhileDragging"/>
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
        /// Whether to raise <see cref="RangeBaseControl{T, TInterval}.ValueChanged"/> event,
        /// when control is in process of dragging some thumb,
        /// in other words, when <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/> is <c>true</c>.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsRaiseValueChangedWhileDragging
        {
            get
            {
                var res = GetValue(IsRaiseValueChangedWhileDraggingProperty);
                Debug.Assert(res != null);
                return (bool) res;
            }
            set { SetValue(IsRaiseValueChangedWhileDraggingProperty, value); }
        }

        #endregion

        #region Delay Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.Delay"/>
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
        /// Gets or sets the amount of time, in milliseconds, the <see cref="System.Windows.Controls.Primitives.RepeatButton"/>
        /// waits while it is pressed before it starts repeating.
        /// The value must be non-negative.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public int Delay
        {
            get
            {
                var res = GetValue(DelayProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.Interval"/>
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
        /// Gets or sets the amount of time, in milliseconds, between repeats once repeating starts.
        /// The value must be non-negative.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public int Interval
        {
            get
            {
                var res = GetValue(IntervalProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>
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
        /// Converter of values into their string representations for showing in the tooltips in UI.
        /// If not <c>null</c>, then <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> ignores.
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
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverterParameter"/>
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
        /// Additional parameter for <see cref="AutoToolTipValueConverter"/>.
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
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty AutoToolTipFormatProperty
// ReSharper restore StaticFieldInGenericType
            = DependencyProperty.Register("AutoToolTipFormat", typeof(string), typeof(SimpleRangeSlider<T, TInterval>),
                                          new FrameworkPropertyMetadata());

        /// <summary>
        /// Format string for showing values in the tooltips in UI.
        /// Ignores if <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/> is not <c>null</c>.
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
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipPlacement"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty AutoToolTipPlacementProperty
// ReSharper restore StaticFieldInGenericType
            = DependencyProperty.Register("AutoToolTipPlacement", typeof(AutoToolTipPlacement), typeof(SimpleRangeSlider<T, TInterval>),
                                          new FrameworkPropertyMetadata(AutoToolTipPlacement.None),
                                          DependencyPropertyUtil.IsValidAutoToolTipPlacement);

        /// <summary>
        /// The placement where automatic <see cref="System.Windows.Controls.ToolTip"/> is positioned on the control.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public AutoToolTipPlacement AutoToolTipPlacement
        {
            get
            {
                var res = GetValue(AutoToolTipPlacementProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.StartReservedSpace"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty StartReservedSpaceProperty = StartReservedSpacePropertyKey.DependencyProperty;
// ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Slider uses <see cref="SimpleRangeSlider{T, TInterval}.StartReservedSpaceProperty"/>
        /// for evaluation of indent of <see cref="TickBar"/> to the left/bottom
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public double StartReservedSpace
        {
            get
            {
                var res = GetValue(StartReservedSpaceProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.EndReservedSpace"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty EndReservedSpaceProperty = EndReservedSpacePropertyKey.DependencyProperty;
// ReSharper restore StaticFieldInGenericType

        /// <summary>
        /// Slider uses <see cref="SimpleRangeSlider{T, TInterval}.EndReservedSpaceProperty"/>
        /// for evaluation of indent of <see cref="TickBar"/> to the right/top.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public double EndReservedSpace
        {
            get
            {
                var res = GetValue(EndReservedSpaceProperty);
                Debug.Assert(res != null);
                return (double)res;
            }
            private set { SetValue(EndReservedSpacePropertyKey, value); }
        }

        #endregion


        #region TickMark support

        #region IsSnapToTickEnabled property

        /// <summary>
        /// Gets or sets a value that indicates whether the slider automatically moves
        /// the <see cref="System.Windows.Controls.Primitives.Track.Thumb"/> to the closest tick mark.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public bool IsSnapToTickEnabled
        {
            get
            {
                var res = GetValue(IsSnapToTickEnabledProperty);
                Debug.Assert(res != null);
                return (bool)res;
            }
            set
            {
                SetValue(IsSnapToTickEnabledProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/>
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
            Debug.Assert(obj is SimpleRangeSlider<T, TInterval>);
            Debug.Assert(args.OldValue is bool);
            Debug.Assert(args.NewValue is bool);

            var element = obj as SimpleRangeSlider<T, TInterval>;
            Debug.Assert(element != null, "element != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (element == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

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
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.TickPlacement"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
// ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty TickPlacementProperty
// ReSharper restore StaticFieldInGenericType
            = DependencyProperty.Register("TickPlacement", typeof(TickPlacement), typeof(SimpleRangeSlider<T, TInterval>),
                                          new FrameworkPropertyMetadata(TickPlacement.None),
                                          IsValidTickPlacement);

        /// <summary>
        /// Position of tick marks in a slider control with respect to the track that the control implements.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public TickPlacement TickPlacement
        {
            get
            {
                var res = GetValue(TickPlacementProperty);
                Debug.Assert(res != null);
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
        /// Gets or sets the interval between tick marks.
        /// Ignores if <see cref="SimpleRangeSlider{T, TInterval}.TypedTicksCollection"/> is not <c>null</c>.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public TInterval TickFrequency
        {
            get
            {
                var res = GetValue(TickFrequencyProperty);
                Debug.Assert(res != null);
                return (TInterval)res;
            }
            set
            {
                SetValue(TickFrequencyProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>
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
            Debug.Assert(obj is SimpleRangeSlider<T, TInterval>);
            Debug.Assert(args.OldValue is TInterval);
            Debug.Assert(args.NewValue is TInterval);

            var element = obj as SimpleRangeSlider<T, TInterval>;
            Debug.Assert(element != null, "element != null");
// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable HeuristicUnreachableCode
            if (element == null) return;
// ReSharper restore HeuristicUnreachableCode
// ReSharper restore ConditionIsAlwaysTrueOrFalse

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
        { }

        internal static bool IsValidTickFrequency(object value)
        {
            return DependencyPropertyUtil.IsValidChange(typeof(TInterval), value);
        }

        #endregion

        #region TickLabelNumericFormat Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.TickLabelNumericFormat"/>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        // ReSharper disable StaticFieldInGenericType
        public static readonly DependencyProperty TickLabelNumericFormatProperty =
            // ReSharper restore StaticFieldInGenericType
            DependencyProperty.Register(
                "TickLabelNumericFormat",
                typeof(string),
                typeof(SimpleRangeSlider<T, TInterval>),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Format string for conversion numeric labels to text.
        /// Empty string interprets as <c>null</c>.
        /// Uses only when <see cref="TickLabelConverter"/> is <c>null</c>.
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public string TickLabelNumericFormat
        {
            get { return (string)GetValue(TickLabelNumericFormatProperty); }
            set { SetValue(TickLabelNumericFormatProperty, value); }
        }

        #endregion

        #region TickLabelConverter Property

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.TickLabelConverter"/>
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
        /// Gets or sets the converter of tick values to their string representations for showing in UI.
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
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.TickLabelConverterParameter"/>
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
        /// Additional parameter for <see cref="TickLabelConverter"/>
        /// </summary>
        [Bindable(true), Category("Behavior")]
        public object TickLabelConverterParameter
        {
            get { return GetValue(TickLabelConverterParameterProperty); }
            set { SetValue(TickLabelConverterParameterProperty, value); }
        }

        #endregion

        #endregion TickMark support

        #endregion Dependency Properties

        #region Public Events

        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDraggingChanged"/>
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
        /// Occurs when <see cref="SimpleRangeSlider{T, TInterval}.IsRangeDragging"/> changed.
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> IsRangeDraggingChanged
        {
            add { AddHandler(IsRangeDraggingChangedEvent, value); }
            remove { RemoveHandler(IsRangeDraggingChangedEvent, value); }
        }


        /// <summary>
        /// Dependency property for <see cref="SimpleRangeSlider{T, TInterval}.RangeDragCompleted"/>
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
        /// Occurs when the process of dragging of any of thumbs completed.
        /// </summary>
        public event EventHandler<RangeDragCompletedEventArgs<T>> RangeDragCompleted
        {
            add { AddHandler(RangeDragCompletedEvent, value); }
            remove { RemoveHandler(RangeDragCompletedEvent, value); }
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
            T oldStartValue, T oldEndValue,
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
            if (!(sender is SimpleRangeSlider<T, TInterval>))
                throw new ArgumentOutOfRangeException(nameof(sender));

            var slider = sender as SimpleRangeSlider<T, TInterval>;
            slider.OnThumbDragStarted(e);
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            if (!(sender is SimpleRangeSlider<T, TInterval>))
                throw new ArgumentOutOfRangeException(nameof(sender));
            if (!(e.OriginalSource is Thumb))
                throw new ArgumentOutOfRangeException(nameof(e));

            var slider = sender as SimpleRangeSlider<T, TInterval>;
            slider.OnThumbDragDelta(e);
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        private static void OnThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (!(sender is SimpleRangeSlider<T, TInterval>))
                throw new ArgumentOutOfRangeException(nameof(sender));

            var slider = sender as SimpleRangeSlider<T, TInterval>;
            slider.OnThumbDragCompleted(e);
        }

        /// <summary>
        /// Calls when user starts dragging the thumb.
        /// </summary>
        /// <param name="e">Information about event.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected virtual void OnThumbDragStarted(DragStartedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            // Show AutoToolTip if needed.
            var thumb = e.OriginalSource as Thumb;

            if (thumb == null)
            {
                return;
            }

            // remember range values on the moment when drag started
            Debug.Assert(!m_RangeValueData.IsRangeDragging);
            m_RangeValueData.IsRangeDragging = true;
            m_RangeValueData.RangeStart = StartValue;
            m_RangeValueData.RangeEnd = EndValue;


            if (this.AutoToolTipPlacement == AutoToolTipPlacement.None)
            {
                return;
            }
            RangeThumbType thumbType = GetThumbType(thumb);

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
        /// Calls when user moves the captured thumb.
        /// </summary>
        /// <param name="e">Information about event.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected virtual void OnThumbDragDelta(DragDeltaEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));
            if (!(e.OriginalSource is Thumb))
                throw new ArgumentOutOfRangeException(nameof(e));

            IsRangeDragging = true; // do it here in order to not useless set this indicator in handler of DragStarted,
            // because in DragStarted this indicator will be set up even there was no real movement,
            // but was only mouse click over element (capture).

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

                    //Debug.Assert(m_AutoToolTip != null);
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
        /// Calls when user completed dragging the thumb.
        /// </summary>
        /// <param name="e">Information about event.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected virtual void OnThumbDragCompleted(DragCompletedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException(nameof(e));

            var thumb = e.OriginalSource as Thumb;

            if (thumb == null)
            {
                return;
            }

            Debug.Assert(m_RangeValueData.IsRangeDragging);
            var oldRangeValueData = m_RangeValueData;
            m_RangeValueData.IsRangeDragging = false;

            if (IsRangeDragging)
            {
                IsRangeDragging = false;
                OnRangeDragCompleted(oldRangeValueData.RangeStart, oldRangeValueData.RangeEnd,
                                     StartValue, EndValue);

                // if raising of ValueChanged event was OFF,
                // then we need to manually generate that event when drag completed,
                // otherwise it will be lost ("swallowed")
                if (!IsRaiseValueChangedWhileDragging &&
                    (!DoubleUtil.AreClose(ValueToDouble(oldRangeValueData.RangeStart), ValueToDouble(StartValue)) ||
                     !DoubleUtil.AreClose(ValueToDouble(oldRangeValueData.RangeEnd), ValueToDouble(EndValue))))
                {
                    base.OnValueChanged(oldRangeValueData.RangeStart, oldRangeValueData.RangeEnd, StartValue, EndValue);
                }
            }

            if (this.AutoToolTipPlacement == AutoToolTipPlacement.None)
            {
                return;
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
        /// The handler for <see cref="Mouse.MouseDownEvent"/> event.
        /// Its purpose is to move input focus to <see cref="SimpleRangeSlider{T, TInterval}"/>,
        /// when user presses left mouse button over any part (element) of this slider,
        /// which is not focusable.
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
        /// Called to arrange and size the content of a <see cref="SimpleRangeSlider{T, TInterval}" />.
        /// </summary>
        /// <param name="finalSize">The computed size that is used to arrange the content.</param>
        /// <returns>The size, which will be used for the content of a <see cref="SimpleRangeSlider{T, TInterval}" />.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size size = base.ArrangeOverride(finalSize);

            UpdateTrackBackgroundPositionAndSize();

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
            UpdateTrackBackgroundPositionAndSize();
        }

        /// <summary>
        /// Is invoked whenever application code or internal processes call <see cref="System.Windows.FrameworkElement.ApplyTemplate"/>().
        /// 
        /// Invoked when visual tree of an element is created.
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
                Debug.Assert(m_AutoToolTip.Tag is RangeThumbType);
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
            if (!IsRangeDragging || IsRaiseValueChangedWhileDragging)
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
        protected abstract ITicksCollection<T> TypedTicksCollection { get; }

        /// <summary>
        /// Gets the string representation of <paramref name="value"/> for showing in tooltips.
        /// 
        /// Must be implemented in derived classes.
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="thumbType">The type of thumb which value is <paramref name="value"/>.</param>
        /// <returns>String representation of <paramref name="value"/> for thumb, which type is <paramref name="thumbType"/>.</returns>
        protected abstract string GetAutoToolTipString(T value, RangeThumbType thumbType);

        #endregion

        #region Virtual Functions

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseRangeLarge"/>
        /// </summary>
        protected virtual void OnIncreaseRangeLarge()
        {
            MoveRangeToNextTick(this.LargeChange, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseStartLarge"/>
        /// </summary>
        protected virtual void OnIncreaseStartLarge()
        {
            MoveToNextTick(this.LargeChange, false, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseEndLarge"/>
        /// </summary>
        protected virtual void OnIncreaseEndLarge()
        {
            MoveToNextTick(this.LargeChange, false, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseRangeLarge"/>
        /// </summary>
        protected virtual void OnDecreaseRangeLarge()
        {
            MoveRangeToNextTick(this.LargeChange, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseStartLarge"/>
        /// </summary>
        protected virtual void OnDecreaseStartLarge()
        {
            MoveToNextTick(this.LargeChange, true, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseEndLarge"/>
        /// </summary>
        protected virtual void OnDecreaseEndLarge()
        {
            MoveToNextTick(this.LargeChange, true, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseRangeSmall"/>
        /// </summary>
        protected virtual void OnIncreaseRangeSmall()
        {
            MoveRangeToNextTick(this.SmallChange, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseStartSmall"/>
        /// </summary>
        protected virtual void OnIncreaseStartSmall()
        {
            MoveToNextTick(this.SmallChange, false, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.IncreaseEndSmall"/>
        /// </summary>
        protected virtual void OnIncreaseEndSmall()
        {
            MoveToNextTick(this.SmallChange, false, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseRangeSmall"/>
        /// </summary>
        protected virtual void OnDecreaseRangeSmall()
        {
            MoveRangeToNextTick(this.SmallChange, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseStartSmall"/>
        /// </summary>
        protected virtual void OnDecreaseStartSmall()
        {
            MoveToNextTick(this.SmallChange, true, true);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.DecreaseEndSmall"/>
        /// </summary>
        protected virtual void OnDecreaseEndSmall()
        {
            MoveToNextTick(this.SmallChange, true, false);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MaximizeRangeValue"/>
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
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MaximizeStartValue"/>
        /// </summary>
        protected virtual void OnMaximizeStartValue()
        {
            this.SetCurrentValue(StartValueProperty, Maximum);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MaximizeEndValue"/>
        /// </summary>
        protected virtual void OnMaximizeEndValue()
        {
            this.SetCurrentValue(EndValueProperty, Maximum);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MinimizeRangeValue"/>
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
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MinimizeStartValue"/>
        /// </summary>
        protected virtual void OnMinimizeStartValue()
        {
            this.SetCurrentValue(StartValueProperty, Minimum);
        }

        /// <summary>
        /// Handler for <see cref="SimpleRangeSlider{T, TInterval}.MinimizeEndValue"/>
        /// </summary>
        protected virtual void OnMinimizeEndValue()
        {
            this.SetCurrentValue(EndValueProperty, Minimum);
        }

        #endregion Virtual Functions

        #region Helper Functions

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
        /// Method changes the size and position of element <see cref="SimpleRangeSlider{T, TInterval}.TrackBackground"/>.
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
        /// If <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/> is <c>true</c>,
        /// then method aligns values <see cref="RangeBaseControl{T, TInterval}.StartValue"/> and
        /// <see cref="RangeBaseControl{T, TInterval}.EndValue"/>, moving them to the nearest tick mark.
        /// There is probability that will be changed value of interval <see cref="RangeBaseControl{T, TInterval}.RangeValue"/>.
        /// If <see cref="SimpleRangeSlider{T, TInterval}.IsSnapToTickEnabled"/> is <c>false</c>, then method do nothing.
        /// </summary>
        protected void AlignValuesToTicks()
        {
            if (!IsSnapToTickEnabled)
                return;

            T oldStartValue = StartValue, oldEndValue = EndValue;
            double startValue = ValueToDouble(oldStartValue), endValue = ValueToDouble(oldEndValue);
            double snappedStartValue = SnapToTick(startValue, true, true);
            double snappedEndValue = SnapToTick(endValue, false, true);

            Debug.Assert(DoubleUtil.LessThanOrClose(snappedStartValue, snappedEndValue));
            if (DoubleUtil.GreaterThan(snappedStartValue, snappedEndValue))
                return;

            TInterval oldRangeValue = RangeValue;
            IsRangeValueChanging = true;
            try
            {

                if (snappedStartValue < endValue)
                {
                    // at first safely move the left thumb, because it wont thrust to the right thumb.
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
                    // at first safely move the right thumb, because it is in the way of the left thumb ("hampers" it).
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
            Debug.Assert(!double.IsNaN(direction));
            Debug.Assert(!DoubleUtil.AreClose(direction, 0.0));
            Debug.Assert(!double.IsNaN(direction));

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
            Debug.Assert(!double.IsNaN(direction));
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
            if (IsSingleValue)
            {
                return MoveRangeToNextTick(direction, isNegativeDirection);
            }
            double doubleDirection = (isNegativeDirection ? -1.0 : 1.0) * IntervalToDouble(direction);

            if (DoubleUtil.AreClose(doubleDirection, 0.0))
            {
                return false;
            }
            Debug.Assert(!double.IsNaN(doubleDirection));
            return InternalMoveToNextTick(ref doubleDirection, isStartThumb);
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
            double doubleDirection = (isNegativeDirection ? -1.0 : 1.0) * IntervalToDouble(direction);
            TInterval oldRangeValue = RangeValue;
            T oldStartValue = StartValue, oldEndValue = EndValue;
            IsRangeValueChanging = true;
            try
            {
                if (doubleDirection > 0)
                {
                    Debug.Assert(!DoubleUtil.AreClose(doubleDirection, 0.0));
                    changed = InternalMoveToNextTick(ref doubleDirection, false);
                    if (changed)
                    {
                        Debug.Assert(!DoubleUtil.AreClose(doubleDirection, 0.0));
                        InternalMoveToNextTick(ref doubleDirection, true);
                    }
                }
                else if (doubleDirection < 0)
                {
                    Debug.Assert(!DoubleUtil.AreClose(doubleDirection, 0.0));
                    changed = InternalMoveToNextTick(ref doubleDirection, true);
                    if (changed)
                    {
                        Debug.Assert(!DoubleUtil.AreClose(doubleDirection, 0.0));
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
        /// Reference to the range track <see cref="RangeTrack{T, TInterval}"/> of this slider.
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

        /// <summary>
        /// Store here the range value data when dragging started
        /// </summary>
        private RangeValueData<T> m_RangeValueData;

        #endregion Private Fields
    }

    /// <summary>
    /// Range slider, that uses <see cref="double"/> as type for values and interval.
    /// </summary>
    [TemplatePart(Name = "PART_Track", Type = typeof(NumericRangeTrack))]
    [TemplatePart(Name = "PART_TrackBackground", Type = typeof(FrameworkElement))]
    [Description("Simple Numeric Range Slider")]
    public class SimpleNumericRangeSlider : SimpleRangeSlider<double, double>
    {
        #region Private fields

        private const double c_DefaultTickFrequency = 1.0,
                             c_DefaultSmallChange = 0.1,
                             c_DefaultLargeChange = 1.0;

        private const double c_DefaultMinimum = 0.0,
                             c_DefaultMaximum = 10.0;

        #endregion Private fields

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
            TickLabelNumericFormatProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(null));
            TickLabelConverterProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(null));
            TickLabelConverterParameterProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(null));

            SmallChangeProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(c_DefaultSmallChange));
            LargeChangeProperty.OverrideMetadata(thisType, new FrameworkPropertyMetadata(c_DefaultLargeChange));

            DefaultStyleKeyProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(thisType));
        }

        /// <summary>
        /// Format string used when <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> is <c>null</c> or empty
        /// </summary>
        public const string DefaultAutoToolTipFormat = "N";

        #region override functions

        /// <summary>
        /// Converts number to value type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Always <paramref name="value"/></returns>
        protected override double DoubleToValue(double value)
        {
            return value;
        }
        
        /// <summary>
        /// Converts value to number
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Always <paramref name="value"/></returns>
        protected override double ValueToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Converts number to interval type
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Always <paramref name="value"/></returns>
        protected override double DoubleToInterval(double value)
        {
            return value;
        }
        
        /// <summary>
        /// Converts interval value to number
        /// </summary>
        /// <param name="value">Interval value</param>
        /// <returns>Always <paramref name="value"/></returns>
        protected override double IntervalToDouble(double value)
        {
            return value;
        }

        /// <summary>
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>
        /// </summary>
        /// <param name="value">Value to coerce</param>
        /// <returns>Coerced (if needed) value</returns>
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
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.Maximum"/>
        /// </summary>
        /// <param name="value">Value to coerce</param>
        /// <returns>Coerced (if needed) value</returns>
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
        /// Current interval (range) value
        /// </summary>
        protected override double CurrentRangeValue
        {
            get
            {
                return EndValue - StartValue;
            }
        }

        /// <summary>
        /// Gets the collection of tick marks of <see cref="double"/> type.
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
        /// Gets the string representation of <paramref name="value"/> for showing in tooltips.
        /// 
        /// For conversion uses either <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>
        /// with parameter <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverterParameter"/>,
        /// or a format string from <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/>
        /// and numeric precision from <see cref="SimpleNumericRangeSlider.AutoToolTipPrecision"/>
        /// (if converter <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/> is <c>null</c>).
        /// If neither <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>
        /// nor <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> is set (not <c>null</c>),
        /// then uses gerenal nimeric format (<see cref="DefaultAutoToolTipFormat"/>).
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="thumbType">The type of thumb which value is <paramref name="value"/>.</param>
        /// <returns>String representation of <paramref name="value"/> for thumb, which type is <paramref name="thumbType"/>.</returns>
        protected override string GetAutoToolTipString(double value, RangeThumbType thumbType)
        {
            string res;
            if (this.AutoToolTipValueConverter == null)
            {
                string frmt = this.AutoToolTipFormat;
                if (string.IsNullOrEmpty(frmt))
                {
                    frmt = DefaultAutoToolTipFormat;
                }
                var format = (NumberFormatInfo)(NumberFormatInfo.CurrentInfo.Clone());
                Debug.Assert(this.AutoToolTipPrecision >= DependencyPropertyUtil.MinimumAutoToolTipPrecision &&
                                this.AutoToolTipPrecision <= DependencyPropertyUtil.MaximumAutoToolTipPrecision);
                format.NumberDecimalDigits = this.AutoToolTipPrecision;
                try
                {
                    res = value.ToString(frmt, format);
                }
                catch (FormatException)
                {
                    res = LocalizationResource.BadAutoToolTipFormat;
                }
            }
            else
            {
                res = this.AutoToolTipValueConverter.Convert(value, thumbType, this.AutoToolTipValueConverterParameter);
            }

            return res;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the increasing of start value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
        protected override bool IncreaseStartValueCommandCanExecute()
        {
            bool res = IsSingleValue ?
                DoubleUtil.LessThan(StartValue, Maximum) :
                DoubleUtil.LessThan(StartValue, EndValue);
            return res;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the increasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
        protected override bool IncreaseEndValueCommandCanExecute()
        {
            return DoubleUtil.LessThan(EndValue, Maximum);
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of start value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
        protected override bool DecreaseStartValueCommandCanExecute()
        {
            return DoubleUtil.LessThan(Minimum, StartValue);
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
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
        /// Dependency property for <see cref="SimpleNumericRangeSlider.AutoToolTipPrecision"/>
        /// </summary>
        public static readonly DependencyProperty AutoToolTipPrecisionProperty = DependencyProperty.Register(
            "AutoToolTipPrecision",
            typeof(int),
            typeof(SimpleNumericRangeSlider),
            new FrameworkPropertyMetadata(0),
            DependencyPropertyUtil.IsValidAutoToolTipPrecision);

        /// <summary>
        /// Floating-point precision of numbers for showing in the auto tooltips in UI.
        /// </summary>
        [Bindable(true), Category("Appearance")]
        public int AutoToolTipPrecision
        {
            get
            {
                var res = GetValue(AutoToolTipPrecisionProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="SimpleNumericRangeSlider.Ticks"/>
        /// </summary>
        public static readonly DependencyProperty TicksProperty
            = DependencyProperty.Register(
                "Ticks",
                typeof(DoubleCollection),
                typeof(SimpleNumericRangeSlider),
                new FrameworkPropertyMetadata((new DoubleCollection()).GetAsFrozen(), OnTicksChanged));

        /// <summary>
        /// Gets or sets collection of numeric tick marks.
        /// If <see cref="SimpleNumericRangeSlider.Ticks"/> is not <c>null</c>
        /// slider ignores <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>
        /// and draws only tick marks from this collection.
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
            Debug.Assert(d is SimpleNumericRangeSlider);

            var element = (SimpleNumericRangeSlider)d;
            if (element.IsSnapToTickEnabled)
            {
                // if tick's positions change and IsSnapToTickEnabled is ON,
                // then we have to align current values (thumbs) to nearest ticks.
                // shortcoming of that way: thumbs positions can be changed, therefore interval (range) value can be changed too
                // (but not necessarily).
                element.AlignValuesToTicks();
            }
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// Range slider, that uses <see cref="DateTime"/> as type for values and <see cref="TimeSpan"/> for interval.
    /// </summary>
    [TemplatePart(Name = "PART_Track", Type = typeof(DateTimeRangeTrack))]
    [TemplatePart(Name = "PART_TrackBackground", Type = typeof(FrameworkElement))]
    [Description("Simple Date&Time Range Slider")]
    public class SimpleDateTimeRangeSlider : SimpleRangeSlider<DateTime, TimeSpan>
    {
        #region Private fields

        private static readonly TimeSpan s_DefaultTickFrequency = TimeSpan.FromDays(365),
                                         s_DefaultLargeChange = TimeSpan.FromDays(365);

        private static readonly DateTime s_DefaultMinimum = new DateTime(1900, 1, 1),
                                         s_DefaultMaximum = new DateTime(9999, 12, 31);

        #endregion Private fields

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
            TickLabelNumericFormatProperty.OverrideMetadata(thisType,
                new FrameworkPropertyMetadata(null));
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

        /// <summary>
        /// Format string used when <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> is <c>null</c> or empty
        /// </summary>
        public const string DefaultAutoToolTipFormat = "dd-MM-yyyy HH:mm:ss";

        #region override functions

        /// <summary>
        /// Converts number to date
        /// </summary>
        /// <param name="value">NUmber to convert</param>
        /// <returns>Date initialized to a <paramref name="value"/> number of ticks.</returns>
        protected override DateTime DoubleToValue(double value)
        {
            return (value > 10.0) ? new DateTime((long)value) : DateTime.MinValue;
        }
        
        /// <summary>
        /// Converts date to number.
        /// </summary>
        /// <param name="value">Date to convert</param>
        /// <returns>The number of ticks that represent the date and time of <paramref name="value"/>.</returns>
        protected override double ValueToDouble(DateTime value)
        {
            return value.Ticks;
        }

        /// <summary>
        /// Converts number to interval
        /// </summary>
        /// <param name="value">Number to convert</param>
        /// <returns>Returns a <see cref="System.TimeSpan"/> that represents <paramref name="value"/> time,
        /// where the <paramref name="value"/> is in units of ticks.
        /// For very small <paramref name="value"/> returns <see cref="TimeSpan.Zero"/>.</returns>
        protected override TimeSpan DoubleToInterval(double value)
        {
            return (value > 10.0) ? TimeSpan.FromTicks((long)value) : TimeSpan.Zero;
        }
        
        /// <summary>
        /// Converts interval to number
        /// </summary>
        /// <param name="value">Interval value as a <see cref="TimeSpan"/></param>
        /// <returns>The number of ticks that represent <paramref name="value"/>.</returns>
        protected override double IntervalToDouble(TimeSpan value)
        {
            return value.Ticks;
        }

        /// <summary>
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.MinRangeValue"/>
        /// </summary>
        /// <param name="value">Value to coerce</param>
        /// <returns>Coerced (if needed) value</returns>
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
        /// Method for coerce the value of <see cref="RangeBaseControl{T, TInterval}.Maximum"/>
        /// </summary>
        /// <param name="value">Value to coerce</param>
        /// <returns>Coerced (if needed) value</returns>
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
        /// Current interval (range) value
        /// </summary>
        protected override TimeSpan CurrentRangeValue
        {
            get
            {
                return EndValue - StartValue;
            }
        }

        /// <summary>
        /// Gets the collection of tick marks of <see cref="DateTime"/> type.
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
        /// Gets the string representation of <paramref name="value"/> for showing in tooltips.
        /// 
        /// For conversion uses either <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>
        /// with parameter <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverterParameter"/>,
        /// or a format string from <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/>
        /// and numeric precision from <see cref="SimpleNumericRangeSlider.AutoToolTipPrecision"/>
        /// (if converter <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/> is <c>null</c>).
        /// If neither <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipValueConverter"/>
        /// nor <see cref="SimpleRangeSlider{T, TInterval}.AutoToolTipFormat"/> is set (not <c>null</c>),
        /// then uses "dd-MM-yyyy HH:mm:ss" (<see cref="DefaultAutoToolTipFormat"/>).
        /// </summary>
        /// <param name="value">Current value</param>
        /// <param name="thumbType">The type of thumb which value is <paramref name="value"/>.</param>
        /// <returns>String representation of <paramref name="value"/> for thumb, which type is <paramref name="thumbType"/>.</returns>
        protected override string GetAutoToolTipString(DateTime value, RangeThumbType thumbType)
        {
            string res;
            if (this.AutoToolTipValueConverter == null)
            {
                string frmt = this.AutoToolTipFormat;
                if (string.IsNullOrEmpty(frmt))
                {
                    frmt = DefaultAutoToolTipFormat;
                }
                try
                {
                    res = value.ToString(frmt, CultureInfo.CurrentCulture);
                }
                catch (FormatException)
                {
                    res = LocalizationResource.BadAutoToolTipFormat;
                }
            }
            else
            {
                res = this.AutoToolTipValueConverter.Convert(value, thumbType, this.AutoToolTipValueConverterParameter);
            }

            return res;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the increasing of start value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
        protected override bool IncreaseStartValueCommandCanExecute()
        {
            bool res = IsSingleValue
                           ? StartValue < Maximum
                           : StartValue < EndValue;
            return res;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the increasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
        protected override bool IncreaseEndValueCommandCanExecute()
        {
            return EndValue < Maximum;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of start value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
        protected override bool DecreaseStartValueCommandCanExecute()
        {
            return Minimum < StartValue;
        }

        /// <summary>
        /// The handler for the <see cref="System.Windows.Input.CommandBinding.CanExecute"/> event
        /// on the decreasing of end value routed command.
        /// </summary>
        /// <returns><c>true</c> if command associated with this event can be executed on the command target; otherwise, <c>false</c></returns>
        protected override bool DecreaseEndValueCommandCanExecute()
        {
            bool res = IsSingleValue
                           ? Minimum < EndValue
                           : StartValue < EndValue;
            return res;
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="RangeBaseControl{T, TInterval}.Minimum"/> changes.
        /// 
        /// Overridden here to update the value in <see cref="SimpleDateTimeRangeSlider.MinimumAsDouble"/>.
        /// </summary>
        /// <param name="oldMinimum">The value of the property before the change reported by the relevant event or state change.</param>
        /// <param name="newMinimum">The value of the property after the change reported by the relevant event or state change.</param>
        protected override void OnMinimumChanged(DateTime oldMinimum, DateTime newMinimum)
        {
            this.MinimumAsDouble = ValueToDouble(this.Minimum);

            base.OnMinimumChanged(oldMinimum, newMinimum);
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="RangeBaseControl{T, TInterval}.Maximum"/> changes.
        /// 
        /// Overridden here to update the value in <see cref="SimpleDateTimeRangeSlider.MaximumAsDouble"/>.
        /// </summary>
        /// <param name="oldMaximum">The value of the property before the change reported by the relevant event or state change.</param>
        /// <param name="newMaximum">The value of the property after the change reported by the relevant event or state change.</param>
        protected override void OnMaximumChanged(DateTime oldMaximum, DateTime newMaximum)
        {
            this.MaximumAsDouble = ValueToDouble(this.Maximum);

            base.OnMaximumChanged(oldMaximum, newMaximum);
        }

        /// <summary>
        /// Calls whenever the effective value of the <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/> changes.
        /// 
        /// Overridden here to update the value in <see cref="SimpleDateTimeRangeSlider.TickFrequencyAsDouble"/>.
        /// </summary>
        /// <param name="oldTickFrequency">Gets the value of the <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/> before the change.</param>
        /// <param name="newTickFrequency">Gets the value of the <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/> after the change.</param>
        protected override void OnTickFrequencyChanged(TimeSpan oldTickFrequency, TimeSpan newTickFrequency)
        {
            this.TickFrequencyAsDouble = IntervalToDouble(this.TickFrequency);

            base.OnTickFrequencyChanged(oldTickFrequency, newTickFrequency);
        }

        #endregion

        #region Dependency Properties

        #region MinimumAsDouble

        /// <summary>
        /// Gets the value of <see cref="RangeBaseControl{T, TInterval}.Minimum"/> interpreted as <c>double</c>.
        /// 
        /// Can be used for binding with framework elements that works with <see cref="double"/> values,
        /// e.g. <see cref="Slider"/>.
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public double MinimumAsDouble
        {
            get
            {
                var res = GetValue(MinimumAsDoubleProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="SimpleDateTimeRangeSlider.MinimumAsDouble"/>
        /// </summary>
        public static readonly DependencyProperty MinimumAsDoubleProperty = MinimumAsDoublePropertyKey.DependencyProperty;

        #endregion

        #region MaximumAsDouble

        /// <summary>
        /// Gets the value of <see cref="RangeBaseControl{T, TInterval}.Maximum"/> interpreted as <c>double</c>.
        /// 
        /// Can be used for binding with framework elements that works with <see cref="double"/> values,
        /// e.g. <see cref="Slider"/>.
        /// </summary>
        [Category("Behavior"), Bindable(true)]
        public double MaximumAsDouble
        {
            get
            {
                var res = GetValue(MaximumAsDoubleProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="SimpleDateTimeRangeSlider.MaximumAsDouble"/>
        /// </summary>
        public static readonly DependencyProperty MaximumAsDoubleProperty = MaximumAsDoublePropertyKey.DependencyProperty;

        #endregion

        #region Ticks

        /// <summary>
        /// Gets or sets collection of numeric tick marks.
        /// If <see cref="SimpleDateTimeRangeSlider.Ticks"/> is not <c>null</c>
        /// slider ignores <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/>
        /// and draws only tick marks from this collection.
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
        /// Dependency property for <see cref="SimpleDateTimeRangeSlider.Ticks"/>
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
            Debug.Assert(d is SimpleDateTimeRangeSlider);

            var element = (SimpleDateTimeRangeSlider)d;
            if (element.IsSnapToTickEnabled)
            {
                // if tick's positions change and IsSnapToTickEnabled is ON,
                // then we have to align current values (thumbs) to nearest ticks.
                // shortcoming of that way: thumbs positions can be changed, therefore interval (range) value can be changed too
                // (but not necessarily).
                element.AlignValuesToTicks();
            }
            element.TicksAsDouble = element.Ticks == null
                                        ? null
                                        : new DoubleCollection(element.Ticks.Select(tick => (double) tick.Ticks));
        }

        #endregion

        #region TicksAsDouble

        /// <summary>
        /// Gets the value of <see cref="SimpleDateTimeRangeSlider.Ticks"/> interpreted as <c>double</c>.
        /// 
        /// Can be used for binding with framework elements that works with <see cref="double"/> values,
        /// e.g. <see cref="TickBar"/>.
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
        /// Dependency property for <see cref="SimpleDateTimeRangeSlider.TicksAsDouble"/>
        /// </summary>
        public static readonly DependencyProperty TicksAsDoubleProperty = TicksAsDoublePropertyKey.DependencyProperty;

        #endregion

        #region TickFrequencyAsDouble

        /// <summary>
        /// Gets the value of <see cref="SimpleRangeSlider{T, TInterval}.TickFrequency"/> interpreted as collection of <c>double</c> numbers.
        /// 
        /// Can be used for binding with framework elements that works with <see cref="double"/> values,
        /// e.g. <see cref="TickBar"/>.
        /// </summary>
        [Category("Appearance"), Bindable(true)]
        public double TickFrequencyAsDouble
        {
            get
            {
                var res = GetValue(TickFrequencyAsDoubleProperty);
                Debug.Assert(res != null);
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
        /// Dependency property for <see cref="SimpleDateTimeRangeSlider.TickFrequencyAsDouble"/>
        /// </summary>
        public static readonly DependencyProperty TickFrequencyAsDoubleProperty = TickFrequencyAsDoublePropertyKey.DependencyProperty;

        #endregion

        #endregion
    }

    /// <summary>
    /// Class for converting <see cref="DateTime"/> value to <c>string</c> using its ticks value.
    /// 
    /// Uses as default converter for <see cref="SimpleDateTimeRangeSlider"/>.
    /// </summary>
    public sealed class DefaultDateTimeTickLabelToStringConverter : IDoubleToStringConverter
    {
        /// <summary>
        /// Converts date's ticks value to the string representation of that date.
        /// </summary>
        /// <param name="value">Number of ticks</param>
        /// <param name="parameter">If set, used as a format string in <see cref="DateTime.ToString(string,IFormatProvider)"/> method.</param>
        /// <returns>String representation of date
        /// or string indicates wrong <paramref name="value"/> or <paramref name="parameter"/> value
        /// (depends on version and culture settings).</returns>
        public string Convert(double value, object parameter)
        {
            var longTicks = (long)value;
            if (longTicks < DateTime.MinValue.Ticks || longTicks > (DateTime.MaxValue.Ticks + 1))
            {
                // allow minimum excess over DateTime.MaxValue.Ticks because of loss of accuracy while casting from double
                return LocalizationResource.BadDateTimeTicksValue;
            }
            if (longTicks == DateTime.MaxValue.Ticks + 1)
                longTicks = DateTime.MaxValue.Ticks;

            if (parameter != null && !(parameter is string))
            {
                return LocalizationResource.BadDateTimeTicksValue;
            }
            var frmt = parameter as string;

            Debug.Assert(longTicks <= 0x2bca2875f4373fffL); // DateTime.MaxValue.Ticks
            var dt = new DateTime(longTicks);

            if (frmt == null)
            {
                return dt.ToString(CultureInfo.CurrentCulture);
            }

            try
            {
                return dt.ToString(frmt, CultureInfo.CurrentCulture);
            }
            catch (FormatException)
            {
                return LocalizationResource.BadDateTimeTicksFormat;
            }
        }
    }

    internal struct RangeValueData<T>
    {
        internal bool IsRangeDragging { get; set; }
        internal T RangeStart { get; set; }
        internal T RangeEnd { get; set; }
    }
}
