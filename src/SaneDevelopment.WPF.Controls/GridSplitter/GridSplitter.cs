// -----------------------------------------------------------------------
// <copyright file="GridSplitter.cs" company="Sane Development">
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
    using System.Collections;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    // NOTE: This is a fixed version GridSplitter to demonstrate where
    // the bug is in the original System.WindowsControls.GridSplitter code.
    // https://connect.microsoft.com/VisualStudio/feedback/details/483010/wpf-gridsplitter-randomly-jumps-when-resizing

    // This code was lifted from Reflectors Disassembler view

    // See comments for 'MoveSplitter' method below

#pragma warning disable CA1501 // Avoid excessive inheritance
#pragma warning disable CA1200 // Avoid using cref tags with a prefix
#pragma warning disable SA1202 // Elements should be ordered by access
#pragma warning disable SA1100 // Do not prefix calls with base unless local implementation exists
#pragma warning disable SA1204 // Static elements should appear before instance elements
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1005 // Single line comments should begin with single space
#pragma warning disable SA1609 // Property documentation should have value
#pragma warning disable SA1201 // Elements should appear in the correct order
#pragma warning disable SA1623 // Property summary documentation should match accessors
#pragma warning disable SA1306 // Field names should begin with lower-case letter
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable CA1801 // Review unused parameters

    /// <summary>
    /// Represents the control that redistributes space between columns or rows of a <see cref="Grid"/> control.
    /// </summary>
    [StyleTypedProperty(Property = "PreviewStyle", StyleTargetType = typeof(Control))]
    [Obsolete("Use System.Windows.Controls.GridSplitter instead")]
    public class GridSplitter : Thumb
    {
        private ResizeData _resizeData;

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.GridSplitter.DragIncrement" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.GridSplitter.DragIncrement" /> dependency property.</returns>
        public static readonly DependencyProperty DragIncrementProperty = DependencyProperty.Register(
            "DragIncrement",
            typeof(double),
            typeof(GridSplitter),
            new FrameworkPropertyMetadata(1.0),
            new ValidateValueCallback(GridSplitter.IsValidDelta));

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.GridSplitter.KeyboardIncrement" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.GridSplitter.KeyboardIncrement" /> dependency property.</returns>
        public static readonly DependencyProperty KeyboardIncrementProperty = DependencyProperty.Register("KeyboardIncrement", typeof(double), typeof(GridSplitter), new FrameworkPropertyMetadata(10.0), new ValidateValueCallback(GridSplitter.IsValidDelta));

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.GridSplitter.PreviewStyle" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewStyleProperty = DependencyProperty.Register("PreviewStyle", typeof(Style), typeof(GridSplitter), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.GridSplitter.ResizeBehavior" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResizeBehaviorProperty = DependencyProperty.Register("ResizeBehavior", typeof(GridResizeBehavior), typeof(GridSplitter), new FrameworkPropertyMetadata(GridResizeBehavior.BasedOnAlignment), new ValidateValueCallback(GridSplitter.IsValidResizeBehavior));

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.GridSplitter.ResizeDirection" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.GridSplitter.ResizeDirection" /> dependency property.</returns>
        public static readonly DependencyProperty ResizeDirectionProperty = DependencyProperty.Register("ResizeDirection", typeof(GridResizeDirection), typeof(GridSplitter), new FrameworkPropertyMetadata(GridResizeDirection.Auto, new PropertyChangedCallback(GridSplitter.UpdateCursor)), new ValidateValueCallback(GridSplitter.IsValidResizeDirection));

        /// <summary>
        /// Identifies the <see cref="P:System.Windows.Controls.GridSplitter.ShowsPreview" /> dependency property.
        /// </summary>
        /// <returns>The identifier for the <see cref="P:System.Windows.Controls.GridSplitter.ShowsPreview" /> dependency property.</returns>
        public static readonly DependencyProperty ShowsPreviewProperty = DependencyProperty.Register("ShowsPreview", typeof(bool), typeof(GridSplitter), new FrameworkPropertyMetadata(false));

        static GridSplitter()
        {
            EventManager.RegisterClassHandler(typeof(GridSplitter), Thumb.DragStartedEvent, new DragStartedEventHandler(GridSplitter.OnDragStarted));
            EventManager.RegisterClassHandler(typeof(GridSplitter), Thumb.DragDeltaEvent, new DragDeltaEventHandler(GridSplitter.OnDragDelta));
            EventManager.RegisterClassHandler(typeof(GridSplitter), Thumb.DragCompletedEvent, new DragCompletedEventHandler(GridSplitter.OnDragCompleted));
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(GridSplitter), new FrameworkPropertyMetadata(typeof(GridSplitter)));
            UIElement.FocusableProperty.OverrideMetadata(typeof(GridSplitter), new FrameworkPropertyMetadata(true));
            FrameworkElement.HorizontalAlignmentProperty.OverrideMetadata(typeof(GridSplitter), new FrameworkPropertyMetadata(HorizontalAlignment.Right));
            FrameworkElement.CursorProperty.OverrideMetadata(typeof(GridSplitter), new FrameworkPropertyMetadata(null, new CoerceValueCallback(GridSplitter.CoerceCursor)));
        }

        private void CancelResize()
        {
            DependencyObject parent = base.Parent;
            if (this._resizeData.ShowsPreview)
            {
                this.RemovePreviewAdorner();
            }
            else
            {
                SetDefinitionLength(this._resizeData.Definition1, this._resizeData.OriginalDefinition1Length);
                SetDefinitionLength(this._resizeData.Definition2, this._resizeData.OriginalDefinition2Length);
            }

            this._resizeData = null;
        }

        private static object CoerceCursor(DependencyObject o, object value)
        {
            GridSplitter splitter = (GridSplitter)o;
            BaseValueSource internal2 = DependencyPropertyHelper.GetValueSource(splitter, FrameworkElement.CursorProperty).BaseValueSource;
            if ((value == null) && (internal2 == BaseValueSource.Default))
            {
                switch (splitter.GetEffectiveResizeDirection())
                {
                    case GridResizeDirection.Columns:
                        return Cursors.SizeWE;

                    case GridResizeDirection.Rows:
                        return Cursors.SizeNS;
                }
            }

            return value;
        }

        private double GetActualLength(DefinitionBase definition)
        {
            ColumnDefinition definition2 = definition as ColumnDefinition;
            if (definition2 != null)
            {
                return definition2.ActualWidth;
            }

            return ((RowDefinition)definition).ActualHeight;
        }

        private void GetDeltaConstraints(out double minDelta, out double maxDelta)
        {
            double actualLength1 = this.GetActualLength(this._resizeData.Definition1);
            double minSizeValue1 = ResizeData.UserMinSizeValueCache(this._resizeData.Definition1);
            double maxSizeValue1 = ResizeData.UserMaxSizeValueCache(this._resizeData.Definition1);
            double actualLength2 = this.GetActualLength(this._resizeData.Definition2);
            double minSizeValue2 = ResizeData.UserMinSizeValueCache(this._resizeData.Definition2);
            double maxSizeValue2 = ResizeData.UserMaxSizeValueCache(this._resizeData.Definition2);
            if (this._resizeData.SplitterIndex == this._resizeData.Definition1Index)
            {
                minSizeValue1 = Math.Max(minSizeValue1, this._resizeData.SplitterLength);
            }
            else if (this._resizeData.SplitterIndex == this._resizeData.Definition2Index)
            {
                minSizeValue2 = Math.Max(minSizeValue2, this._resizeData.SplitterLength);
            }

            if (this._resizeData.SplitBehavior == SplitBehavior.Split)
            {
                minDelta = -Math.Min((double)(actualLength1 - minSizeValue1), (double)(maxSizeValue2 - actualLength2));
                maxDelta = Math.Min((double)(maxSizeValue1 - actualLength1), (double)(actualLength2 - minSizeValue2));
            }
            else if (this._resizeData.SplitBehavior == SplitBehavior.Resize1)
            {
                minDelta = minSizeValue1 - actualLength1;
                maxDelta = maxSizeValue1 - actualLength1;
            }
            else
            {
                minDelta = actualLength2 - maxSizeValue2;
                maxDelta = actualLength2 - minSizeValue2;
            }
        }

        private GridResizeBehavior GetEffectiveResizeBehavior(GridResizeDirection direction)
        {
            GridResizeBehavior resizeBehavior = this.ResizeBehavior;
            if (resizeBehavior != GridResizeBehavior.BasedOnAlignment)
            {
                return resizeBehavior;
            }

            if (direction != GridResizeDirection.Columns)
            {
                switch (base.VerticalAlignment)
                {
                    case VerticalAlignment.Top:
                        return GridResizeBehavior.PreviousAndCurrent;

                    case VerticalAlignment.Center:
                        goto Label_0058;

                    case VerticalAlignment.Bottom:
                        return GridResizeBehavior.CurrentAndNext;
                }
            }
            else
            {
                switch (base.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        return GridResizeBehavior.PreviousAndCurrent;

                    case HorizontalAlignment.Right:
                        return GridResizeBehavior.CurrentAndNext;
                }

                return GridResizeBehavior.PreviousAndNext;
            }

        Label_0058:
            return GridResizeBehavior.PreviousAndNext;
        }

        private GridResizeDirection GetEffectiveResizeDirection()
        {
            GridResizeDirection resizeDirection = this.ResizeDirection;
            if (resizeDirection != GridResizeDirection.Auto)
            {
                return resizeDirection;
            }

            if (base.HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                return GridResizeDirection.Columns;
            }

            if ((base.VerticalAlignment == VerticalAlignment.Stretch) && (base.ActualWidth <= base.ActualHeight))
            {
                return GridResizeDirection.Columns;
            }

            return GridResizeDirection.Rows;
        }

        private static DefinitionBase GetGridDefinition(Grid grid, int index, GridResizeDirection direction)
        {
            if (direction != GridResizeDirection.Columns)
            {
                return grid.RowDefinitions[index];
            }

            return grid.ColumnDefinitions[index];
        }

        private void InitializeData(bool ShowsPreview)
        {
            Grid parent = base.Parent as Grid;
            if (parent != null)
            {
                this._resizeData = new ResizeData();
                this._resizeData.Grid = parent;
                this._resizeData.ShowsPreview = ShowsPreview;
                this._resizeData.ResizeDirection = this.GetEffectiveResizeDirection();
                this._resizeData.ResizeBehavior = this.GetEffectiveResizeBehavior(this._resizeData.ResizeDirection);
                this._resizeData.SplitterLength = Math.Min(base.ActualWidth, base.ActualHeight);
                // CTW
                this._resizeData.ParentUsesLayoutRounding = parent.UseLayoutRounding;
                if (!this.SetupDefinitionsToResize())
                {
                    this._resizeData = null;
                }
                else
                {
                    this.SetupPreview();
                }
            }
        }

        private static bool IsStar(DefinitionBase definition)
        {
            return ResizeData.UserSizeValueCache(definition).IsStar;
        }

        private static bool IsValidDelta(object o)
        {
            double d = (double)o;
            return (d > 0.0) && !double.IsPositiveInfinity(d);
        }

        private static bool IsValidResizeBehavior(object o)
        {
            GridResizeBehavior behavior = (GridResizeBehavior)o;
            if (((behavior != GridResizeBehavior.BasedOnAlignment) && (behavior != GridResizeBehavior.CurrentAndNext)) && (behavior != GridResizeBehavior.PreviousAndCurrent))
            {
                return behavior == GridResizeBehavior.PreviousAndNext;
            }

            return true;
        }

        private static bool IsValidResizeDirection(object o)
        {
            GridResizeDirection direction = (GridResizeDirection)o;
            if ((direction != GridResizeDirection.Auto) && (direction != GridResizeDirection.Columns))
            {
                return direction == GridResizeDirection.Rows;
            }

            return true;
        }

        internal bool KeyboardMoveSplitter(double horizontalChange, double verticalChange)
        {
            if (this._resizeData != null)
            {
                return false;
            }

            this.InitializeData(false);
            if (this._resizeData == null)
            {
                return false;
            }

            if (base.FlowDirection == FlowDirection.RightToLeft)
            {
                horizontalChange = -horizontalChange;
            }

            this.MoveSplitter(horizontalChange, verticalChange);
            this._resizeData = null;
            return true;
        }



        // This is what the original MS.Internal.DoubleUtil looks like
        // in assembly WindowsBase.dll
        //internal static class DoubleUtil
        //{
        //    ...
        //    private static bool AreClose(double value1, double value2)
        //    {
        //        if(value1 == value2)
        //        {
        //            return true;
        //        }
        //        double num1 = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 2.2204460492503131E-16;
        //        double num2 = value1 - value2;
        //        return ((-num1 < num2) && (num1 > num2));
        //    }
        //}

        // This is what System.Windows.LayoutDoubleUtil version looks like
        // in assembly PresentationFramework.dll
        // To me a difference of less than 1.53E-06 (0.00000153) sounds
        // very adequate precission for UI related mesurements (maybe not?)
        //internal static class LayoutDoubleUtil
        //{
        //    ...

        internal static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }

            double num = value1 - value2;
            return (num < 1.53E-06) && (num > -1.53E-06);
        }

        //}

        // Additional fixup for use with Grid when UseLayoutRounding="true"
        internal static bool AreCloseWithLayoutRounding(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }

            double num = value1 - value2;
            return (num <= 1) && (num >= -1);
        }

        // See above comments
        private void MoveSplitter(double horizontalChange, double verticalChange)
        {
            double changeSize = (this._resizeData.ResizeDirection == GridResizeDirection.Columns) ? horizontalChange : verticalChange;
            DefinitionBase definition1 = this._resizeData.Definition1;
            DefinitionBase definition2 = this._resizeData.Definition2;
            if ((definition1 != null) && (definition2 != null))
            {
                double actualLength1 = this.GetActualLength(definition1);
                double actualLength2 = this.GetActualLength(definition2);

                // Offending code
                // NOTE: Original code uses DoubleUtil.AreClose(...) as commented above
                // if((this._resizeData.SplitBehavior == SplitBehavior.Split) && !DoubleUtil.AreClose((double)(actualLength + num3), (double)(this._resizeData.OriginalDefinition1ActualLength + this._resizeData.OriginalDefinition2ActualLength)))

                // Updated code uses LayoutDoubleUtil.AreClose(...) as commented above
                bool areClose;
                if (this._resizeData.ParentUsesLayoutRounding)
                {
                    areClose = AreCloseWithLayoutRounding((double)(actualLength1 + actualLength2), (double)(this._resizeData.OriginalDefinition1ActualLength + this._resizeData.OriginalDefinition2ActualLength));
                }
                else
                {
                    areClose = AreClose((double)(actualLength1 + actualLength2), (double)(this._resizeData.OriginalDefinition1ActualLength + this._resizeData.OriginalDefinition2ActualLength));
                }

                if ((this._resizeData.SplitBehavior == SplitBehavior.Split) && !areClose)
                {
                    this.CancelResize();
                }
                else
                {
                    double deltaMin;
                    double deltaMax;
                    this.GetDeltaConstraints(out deltaMin, out deltaMax);
                    if (base.FlowDirection != this._resizeData.Grid.FlowDirection)
                    {
                        changeSize = -changeSize;
                    }

                    changeSize = Math.Min(Math.Max(changeSize, deltaMin), deltaMax);
                    double newLength1 = actualLength1 + changeSize;
                    double newLength2 = (actualLength1 + actualLength2) - newLength1;
                    this.SetLengths(newLength1, newLength2);
                }
            }
        }

        /// <summary>
        /// Creates the implementation of <see cref="T:System.Windows.Automation.Peers.AutomationPeer" /> for the <see cref="T:System.Windows.Controls.GridSplitter" /> control.
        /// </summary>
        /// <returns>A new <see cref="T:System.Windows.Automation.Peers.GridSplitterAutomationPeer" /> for this <see cref="T:System.Windows.Controls.ToolTip" /> control.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new GridSplitterAutomationPeer(this);
        }

        private void OnDragCompleted(DragCompletedEventArgs e)
        {
            if (this._resizeData != null)
            {
                if (this._resizeData.ShowsPreview)
                {
                    this.MoveSplitter(this._resizeData.Adorner.OffsetX, this._resizeData.Adorner.OffsetY);
                    this.RemovePreviewAdorner();
                }

                this._resizeData = null;
            }
        }

        private static void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            (sender as GridSplitter).OnDragCompleted(e);
        }

        private void OnDragDelta(DragDeltaEventArgs e)
        {
            if (this._resizeData != null)
            {
                double horizontalChange = e.HorizontalChange;
                double verticalChange = e.VerticalChange;
                double dragIncrement = this.DragIncrement;
                horizontalChange = Math.Round((double)(horizontalChange / dragIncrement)) * dragIncrement;
                verticalChange = Math.Round((double)(verticalChange / dragIncrement)) * dragIncrement;
                if (this._resizeData.ShowsPreview)
                {
                    if (this._resizeData.ResizeDirection == GridResizeDirection.Columns)
                    {
                        this._resizeData.Adorner.OffsetX = Math.Min(Math.Max(horizontalChange, this._resizeData.MinChange), this._resizeData.MaxChange);
                    }
                    else
                    {
                        this._resizeData.Adorner.OffsetY = Math.Min(Math.Max(verticalChange, this._resizeData.MinChange), this._resizeData.MaxChange);
                    }
                }
                else
                {
                    this.MoveSplitter(horizontalChange, verticalChange);
                }
            }
        }

        private static void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            (sender as GridSplitter).OnDragDelta(e);
        }

        private void OnDragStarted(DragStartedEventArgs e)
        {
            this.InitializeData(this.ShowsPreview);
        }

        private static void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            (sender as GridSplitter).OnDragStarted(e);
        }

        /// <summary>
        /// Called when a key is pressed.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.KeyEventArgs" /> that contains the event data.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            switch (e.Key)
            {
                case Key.Left:
                    e.Handled = this.KeyboardMoveSplitter(-this.KeyboardIncrement, 0.0);
                    return;

                case Key.Up:
                    e.Handled = this.KeyboardMoveSplitter(0.0, -this.KeyboardIncrement);
                    return;

                case Key.Right:
                    e.Handled = this.KeyboardMoveSplitter(this.KeyboardIncrement, 0.0);
                    return;

                case Key.Down:
                    e.Handled = this.KeyboardMoveSplitter(0.0, this.KeyboardIncrement);
                    break;

                case Key.Escape:
                    if (this._resizeData == null)
                    {
                        break;
                    }

                    this.CancelResize();
                    e.Handled = true;
                    return;

                default:
                    return;
            }
        }

        /// <summary>
        /// Called when the <see cref="T:System.Windows.Controls.GridSplitter" /> control loses keyboard focus.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs" /> that contains the event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            if (this._resizeData != null)
            {
                this.CancelResize();
            }
        }

        /// <summary>
        /// Responds to a change in the dimensions of the <see cref="T:System.Windows.Controls.GridSplitter" /> control.
        /// </summary>
        /// <param name="sizeInfo">Information about the change in size of the <see cref="T:System.Windows.Controls.GridSplitter" />.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            base.CoerceValue(FrameworkElement.CursorProperty);
        }

        private void RemovePreviewAdorner()
        {
            if (this._resizeData.Adorner != null)
            {
                (VisualTreeHelper.GetParent(this._resizeData.Adorner) as AdornerLayer).Remove(this._resizeData.Adorner);
            }
        }

        private static void SetDefinitionLength(DefinitionBase definition, GridLength length)
        {
            definition.SetValue((definition is ColumnDefinition) ? ColumnDefinition.WidthProperty : RowDefinition.HeightProperty, length);
        }

        private void SetLengths(double definition1Pixels, double definition2Pixels)
        {
            if (this._resizeData.SplitBehavior == SplitBehavior.Split)
            {
                IEnumerable enumerable = (this._resizeData.ResizeDirection == GridResizeDirection.Columns) ? ((IEnumerable)this._resizeData.Grid.ColumnDefinitions) : ((IEnumerable)this._resizeData.Grid.RowDefinitions);
                int num = 0;
                foreach (DefinitionBase base2 in enumerable)
                {
                    if (num == this._resizeData.Definition1Index)
                    {
                        SetDefinitionLength(base2, new GridLength(definition1Pixels, GridUnitType.Star));
                    }
                    else if (num == this._resizeData.Definition2Index)
                    {
                        SetDefinitionLength(base2, new GridLength(definition2Pixels, GridUnitType.Star));
                    }
                    else if (IsStar(base2))
                    {
                        SetDefinitionLength(base2, new GridLength(this.GetActualLength(base2), GridUnitType.Star));
                    }

                    num++;
                }
            }
            else if (this._resizeData.SplitBehavior == SplitBehavior.Resize1)
            {
                SetDefinitionLength(this._resizeData.Definition1, new GridLength(definition1Pixels));
            }
            else
            {
                SetDefinitionLength(this._resizeData.Definition2, new GridLength(definition2Pixels));
            }
        }

        private bool SetupDefinitionsToResize()
        {
            int num4 = (int)base.GetValue((this._resizeData.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnSpanProperty : Grid.RowSpanProperty);
            if (num4 == 1)
            {
                int num2;
                int num3;
                int num = (int)base.GetValue((this._resizeData.ResizeDirection == GridResizeDirection.Columns) ? Grid.ColumnProperty : Grid.RowProperty);
                switch (this._resizeData.ResizeBehavior)
                {
                    case GridResizeBehavior.CurrentAndNext:
                        num2 = num;
                        num3 = num + 1;
                        break;

                    case GridResizeBehavior.PreviousAndCurrent:
                        num2 = num - 1;
                        num3 = num;
                        break;

                    default:
                        num2 = num - 1;
                        num3 = num + 1;
                        break;
                }

                int num5 = (this._resizeData.ResizeDirection == GridResizeDirection.Columns) ? this._resizeData.Grid.ColumnDefinitions.Count : this._resizeData.Grid.RowDefinitions.Count;
                if ((num2 >= 0) && (num3 < num5))
                {
                    this._resizeData.SplitterIndex = num;
                    this._resizeData.Definition1Index = num2;
                    this._resizeData.Definition1 = GetGridDefinition(this._resizeData.Grid, num2, this._resizeData.ResizeDirection);
                    this._resizeData.OriginalDefinition1Length = ResizeData.UserSizeValueCache(this._resizeData.Definition1);
                    this._resizeData.OriginalDefinition1ActualLength = this.GetActualLength(this._resizeData.Definition1);
                    this._resizeData.Definition2Index = num3;
                    this._resizeData.Definition2 = GetGridDefinition(this._resizeData.Grid, num3, this._resizeData.ResizeDirection);
                    this._resizeData.OriginalDefinition2Length = ResizeData.UserSizeValueCache(this._resizeData.Definition2);
                    this._resizeData.OriginalDefinition2ActualLength = this.GetActualLength(this._resizeData.Definition2);
                    bool flag = IsStar(this._resizeData.Definition1);
                    bool flag2 = IsStar(this._resizeData.Definition2);
                    if (flag && flag2)
                    {
                        this._resizeData.SplitBehavior = SplitBehavior.Split;
                    }
                    else
                    {
                        this._resizeData.SplitBehavior = !flag ? SplitBehavior.Resize1 : SplitBehavior.Resize2;
                    }

                    return true;
                }
            }

            return false;
        }

        private void SetupPreview()
        {
            if (this._resizeData.ShowsPreview)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this._resizeData.Grid);
                if (adornerLayer != null)
                {
                    this._resizeData.Adorner = new PreviewAdorner(this, this.PreviewStyle);
                    adornerLayer.Add(this._resizeData.Adorner);
                    this.GetDeltaConstraints(out this._resizeData.MinChange, out this._resizeData.MaxChange);
                }
            }
        }

        private static void UpdateCursor(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            o.CoerceValue(FrameworkElement.CursorProperty);
        }

        /// <summary>
        /// Gets or sets the minimum distance that a user must drag a mouse to resize rows or columns with a <see cref="T:System.Windows.Controls.GridSplitter" /> control.
        /// </summary>
        /// <returns>A value that represents the minimum distance that a user must use the mouse to drag a <see cref="T:System.Windows.Controls.GridSplitter" /> to resize rows or columns. The default is 1.</returns>
        public double DragIncrement
        {
            get
            {
                return (double)base.GetValue(DragIncrementProperty);
            }

            set
            {
                base.SetValue(DragIncrementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the distance that each press of an arrow key moves a <see cref="T:System.Windows.Controls.GridSplitter" /> control.
        /// </summary>
        /// <returns>The distance that the <see cref="T:System.Windows.Controls.GridSplitter" /> moves for each press of an arrow key. The default is 10. </returns>
        public double KeyboardIncrement
        {
            get
            {
                return (double)base.GetValue(KeyboardIncrementProperty);
            }

            set
            {
                base.SetValue(KeyboardIncrementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style that customizes the appearance, effects, or other style characteristics for the <see cref="T:System.Windows.Controls.GridSplitter" /> control preview indicator that is displayed when the <see cref="P:System.Windows.Controls.GridSplitter.ShowsPreview" /> property is set to true.
        /// </summary>
        /// <returns>Returns the <see cref="T:System.Windows.Style" /> for the preview indicator that shows the potential change in <see cref="T:System.Windows.Controls.Grid" /> dimensions as you move the <see cref="T:System.Windows.Controls.GridSplitter" /> control. The default is the style that the current theme supplies.</returns>
        public Style PreviewStyle
        {
            get
            {
                return (Style)base.GetValue(PreviewStyleProperty);
            }

            set
            {
                base.SetValue(PreviewStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets which columns or rows are resized relative to the column or row for which the <see cref="T:System.Windows.Controls.GridSplitter" /> control is defined.
        /// </summary>
        /// <returns>One of the enumeration values that indicates which columns or rows are resized by this <see cref="T:System.Windows.Controls.GridSplitter" /> control. The default is <see cref="F:System.Windows.Controls.GridResizeBehavior.BasedOnAlignment" />.</returns>
        public GridResizeBehavior ResizeBehavior
        {
            get
            {
                return (GridResizeBehavior)base.GetValue(ResizeBehaviorProperty);
            }

            set
            {
                base.SetValue(ResizeBehaviorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="T:System.Windows.Controls.GridSplitter" /> control resizes rows or columns.
        /// </summary>
        /// <returns>One of the enumeration values that specifies whether to resize rows or columns. The default is <see cref="F:System.Windows.Controls.GridResizeDirection.Auto" />.</returns>
        public GridResizeDirection ResizeDirection
        {
            get
            {
                return (GridResizeDirection)base.GetValue(ResizeDirectionProperty);
            }

            set
            {
                base.SetValue(ResizeDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="T:System.Windows.Controls.GridSplitter" /> control updates the column or row size as the user drags the control.
        /// </summary>
        /// <returns>true if a <see cref="T:System.Windows.Controls.GridSplitter" /> preview is displayed; otherwise, false. The default is false.</returns>
        public bool ShowsPreview
        {
            get
            {
                return (bool)base.GetValue(ShowsPreviewProperty);
            }

            set
            {
                base.SetValue(ShowsPreviewProperty, value);
            }
        }


        private sealed class PreviewAdorner : Adorner
        {
            // Fields
            private Decorator _decorator;
            private TranslateTransform Translation;

            // Methods
            public PreviewAdorner(GridSplitter GridSplitter, Style previewStyle)
                : base(GridSplitter)
            {
                Control control = new Control();
                control.Style = previewStyle;
                control.IsEnabled = false;
                this.Translation = new TranslateTransform();
                this._decorator = new Decorator();
                this._decorator.Child = control;
                this._decorator.RenderTransform = this.Translation;
                base.AddVisualChild(this._decorator);
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                this._decorator.Arrange(new Rect(new Point(), finalSize));
                return finalSize;
            }

            protected override Visual GetVisualChild(int index)
            {
                if (index != 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return this._decorator;
            }

            // Properties
            public double OffsetX
            {
                get
                {
                    return this.Translation.X;
                }

                set
                {
                    this.Translation.X = value;
                }
            }

            public double OffsetY
            {
                get
                {
                    return this.Translation.Y;
                }

                set
                {
                    this.Translation.Y = value;
                }
            }

            protected override int VisualChildrenCount
            {
                get
                {
                    return 1;
                }
            }
        }

        private class ResizeData
        {
            // Fields
            public GridSplitter.PreviewAdorner Adorner;
            public DefinitionBase Definition1;
            public int Definition1Index;
            public DefinitionBase Definition2;
            public int Definition2Index;
            public Grid Grid;
            public double MaxChange;
            public double MinChange;
            public double OriginalDefinition1ActualLength;
            public GridLength OriginalDefinition1Length;
            public double OriginalDefinition2ActualLength;
            public GridLength OriginalDefinition2Length;
            public GridResizeBehavior ResizeBehavior;
            public GridResizeDirection ResizeDirection;
            public bool ShowsPreview;
            public GridSplitter.SplitBehavior SplitBehavior;
            public int SplitterIndex;
            public double SplitterLength;
            // CTW
            public bool ParentUsesLayoutRounding;

            public static GridLength UserSizeValueCache(DefinitionBase def)
            {
                if (def is RowDefinition)
                {
                    return (GridLength)def.GetValue(RowDefinition.HeightProperty);
                }
                else
                {
                    return (GridLength)def.GetValue(ColumnDefinition.WidthProperty);
                }
            }

            public static double UserMinSizeValueCache(DefinitionBase def)
            {
                if (def is RowDefinition)
                {
                    return (double)def.GetValue(RowDefinition.MinHeightProperty);
                }
                else
                {
                    return (double)def.GetValue(ColumnDefinition.MinWidthProperty);
                }
            }

            public static double UserMaxSizeValueCache(DefinitionBase def)
            {
                if (def is RowDefinition)
                {
                    return (double)def.GetValue(RowDefinition.MaxHeightProperty);
                }
                else
                {
                    return (double)def.GetValue(ColumnDefinition.MaxWidthProperty);
                }
            }
        }

        private enum SplitBehavior
        {
            Split,
            Resize1,
            Resize2,
        }
    }

#pragma warning disable CA1801 // Review unused parameters
#pragma warning disable CA1822 // Mark members as static
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore SA1401 // Fields should be private
#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore SA1306 // Field names should begin with lower-case letter
#pragma warning restore SA1623 // Property summary documentation should match accessors
#pragma warning restore SA1201 // Elements should appear in the correct order
#pragma warning restore SA1609 // Property documentation should have value
#pragma warning restore SA1005 // Single line comments should begin with single space
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
#pragma warning restore SA1204 // Static elements should appear before instance elements
#pragma warning restore SA1100 // Do not prefix calls with base unless local implementation exists
#pragma warning restore SA1202 // Elements should be ordered by access
#pragma warning restore CA1200 // Avoid using cref tags with a prefix
#pragma warning restore CA1501 // Avoid excessive inheritance
}