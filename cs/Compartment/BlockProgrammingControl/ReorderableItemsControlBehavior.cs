using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Diagnostics;
using Microsoft.Xaml.Behaviors;
using System.Windows.Media;

namespace BlockProgramming
{
    /// <summary>
    /// ItemControl Classにドラッグ・ドロップによる並べ替えを実装する添付ビヘイビア
    /// </summary>
    public class ReorderableItemsControlBehavior
    {
        public static readonly DependencyProperty CallbackProperty
            = DependencyProperty.RegisterAttached(
                "Callback",
                typeof(Action<int>),
                typeof(ReorderableItemsControlBehavior),
                new PropertyMetadata(null, OnCallbackpropertyChanged));

        private static DragDropObject temporaryData;

        private static ItemsControl itemsControl;

        private class DragDropObject
        {
            public Point Start { get; set; }
            public FrameworkElement DraggedItem { get; set; }
            public bool IsDroppable { get; set; }
            public bool CanStartDragging(Point current)
            {
                return (current - this.Start).Length - MinimumDragPoint.Length > 0;
            }

            private static readonly Vector MinimumDragPoint = new Vector(SystemParameters.MinimumHorizontalDragDistance, SystemParameters.MinimumVerticalDragDistance);
        }

        public static Action<int> GetCallback(DependencyObject target)
        {
            return (Action<int>)target.GetValue(CallbackProperty);
        }

        public static void SetCallback(DependencyObject target, Action<int> callback)
        {
            target.SetValue(CallbackProperty, callback);
        }

        private static void OnCallbackpropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            itemsControl = (ItemsControl)d;

            Debug.WriteLine($"type: {d.GetType()}");
            if (itemsControl is null) return;

            if (GetCallback(itemsControl) != null)
            {
                itemsControl.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                itemsControl.PreviewMouseMove += OnPreviewMouseMove;
                itemsControl.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
                itemsControl.PreviewDragEnter += OnPreviewDragEnter;
                itemsControl.PreviewDragLeave += OnPreviewDragLeave;
                itemsControl.PreviewDrop += OnPreviewDrop;
            }
            else
            {
                itemsControl.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
                itemsControl.PreviewMouseMove -= OnPreviewMouseMove;
                itemsControl.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
                itemsControl.PreviewDragEnter -= OnPreviewDragEnter;
                itemsControl.PreviewDragLeave -= OnPreviewDragLeave;
                itemsControl.PreviewDrop -= OnPreviewDrop;
            }

        }

        private static void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as FrameworkElement;
            var item = itemsControl.ContainerFromElement((DependencyObject)e.OriginalSource);
            if (control is null || item is null) return;

            Debug.WriteLine($"Dragged Item: {item.GetType()} index of {itemsControl.ItemContainerGenerator.IndexFromContainer(item)}");

            temporaryData = new DragDropObject()
            {
                Start = e.GetPosition(Window.GetWindow(control)),
                DraggedItem = item as FrameworkElement
            };
        }

        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (temporaryData != null)
            {
                var control = sender as FrameworkElement;
                var current = e.GetPosition(Window.GetWindow(control));
                if (temporaryData.CanStartDragging(current))
                {
                    DragDrop.DoDragDrop(control, temporaryData.DraggedItem, DragDropEffects.Move);
                    temporaryData = null;
                }
            }
        }

        private static void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            temporaryData = null;
        }

        private static void OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            if (temporaryData != null) temporaryData.IsDroppable = true;
        }

        private static void OnPreviewDragLeave(object sender, DragEventArgs e)
        {
            if (temporaryData != null) temporaryData.IsDroppable = false;
        }

        private static void OnPreviewDrop(object sender, DragEventArgs e)
        {
            if (temporaryData?.IsDroppable ?? false)
            {
                var itemsControl = sender as ItemsControl;

                if (itemsControl?.ItemContainerGenerator.IndexFromContainer(temporaryData.DraggedItem) >= 0)
                {
                    var targetContainer = itemsControl.ContainerFromElement((DependencyObject)e.OriginalSource);
                    var index = targetContainer != null ? itemsControl.ItemContainerGenerator.IndexFromContainer(targetContainer) : -1;
                    if (index >= 0)
                    {
                        var callback = GetCallback(itemsControl);
                        callback(index);
                    }
                }
            }
        }
    }

}
