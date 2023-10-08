using System.Windows;

namespace LogToGrafity
{
    public interface IFileDragEnterHandler
    {
        /// <summary>
        /// Return false to cancel the drag and drop operation.
        /// </summary>
        bool OnDragEnter(string[] filepaths);
    }

    public interface IFileDragLeaveHandler
    {
        void OnDragLeave(string[] filepaths);
    }

    public interface IFileDropHandler
    {
        void OnDrop(string[] filepaths);
    }

    public class FileDragDropHelper
    {
        public static bool GetIsFileDragDropEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFileDragDropEnabledProperty);
        }

        public static void SetIsFileDragDropEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFileDragDropEnabledProperty, value);
        }

        public static bool GetFileDragDropTarget(DependencyObject obj)
        {
            return (bool)obj.GetValue(FileDragDropTargetProperty);
        }

        public static void SetFileDragDropTarget(DependencyObject obj, bool value)
        {
            obj.SetValue(FileDragDropTargetProperty, value);
        }

        public static readonly DependencyProperty IsFileDragDropEnabledProperty = DependencyProperty.RegisterAttached(
            "IsFileDragDropEnabled", typeof(bool), typeof(FileDragDropHelper), new PropertyMetadata(OnFileDragDropEnabled));

        public static readonly DependencyProperty FileDragDropTargetProperty = DependencyProperty.RegisterAttached(
            "FileDragDropTarget", typeof(object), typeof(FileDragDropHelper), null);

        private static void OnFileDragDropEnabled(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue == args.OldValue)
                return;

            if (obj is UIElement element)
            {
                element.DragEnter += OnDragEnter;
                element.DragLeave += OnDragLeave;
                element.Drop += OnDrop;
            }
        }

        private static void OnDragEnter(object sender, DragEventArgs args)
        {
            if (sender is not DependencyObject d)
                return;

            object target = d.GetValue(FileDragDropTargetProperty);
            if (target is IFileDragEnterHandler handler)
            {
                if (!handler.OnDragEnter(GetFilePaths(args)))
                {
                    args.Handled = true;
                }
            }
        }

        private static void OnDragLeave(object sender, DragEventArgs args)
        {
            if (sender is not DependencyObject d)
                return;

            object target = d.GetValue(FileDragDropTargetProperty);
            if (target is IFileDragLeaveHandler handler)
                handler.OnDragLeave(GetFilePaths(args));
        }

        private static void OnDrop(object sender, DragEventArgs args)
        {
            if (sender is not DependencyObject d)
                return;

            object target = d.GetValue(FileDragDropTargetProperty);
            if (target is IFileDropHandler fileTarget)
                fileTarget.OnDrop(GetFilePaths(args));
        }

        private static string[] GetFilePaths(DragEventArgs args)
        {
            return (string[])args.Data.GetData(DataFormats.FileDrop);
        }
    }
}
