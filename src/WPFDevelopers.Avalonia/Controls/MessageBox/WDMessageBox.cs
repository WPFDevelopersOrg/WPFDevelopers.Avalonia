using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace WPFDevelopers.Avalonia.Controls
{
    public class WDMessageBox : Window
    {
        private readonly WDMessageBoxPanel _panel;

        public MessageBoxResult Result { get; set; }

        public WDMessageBox(string message, double buttonCornerRadius = 0)
            : this(message, string.Empty, MessageBoxButton.OK, MessageBoxImage.None)
        {
        }

        public WDMessageBox(string message, string caption, double buttonCornerRadius = 0)
            : this(message, caption, MessageBoxButton.OK, MessageBoxImage.None)
        {
        }

        public WDMessageBox(string message, string caption, MessageBoxButton button, double buttonCornerRadius = 0)
            : this(message, caption, button, MessageBoxImage.None)
        {
        }

        public WDMessageBox(string message, string caption, MessageBoxImage icon, double buttonCornerRadius = 0)
            : this(message, caption, MessageBoxButton.OK, icon)
        {
        }

        //public WDMessageBox(string message, string caption, MessageBoxButton button, MessageBoxImage icon, double buttonCornerRadius = 0)
        //{
        //    SizeToContent = SizeToContent.WidthAndHeight;
        //    SystemDecorations = SystemDecorations.None;
        //    ExtendClientAreaToDecorationsHint = true;
        //    ExtendClientAreaChromeHints = global::Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
        //    ExtendClientAreaTitleBarHeightHint = -1;
        //    Background = Brushes.Transparent;
        //    ClipToBounds = false;
        //    _panel = new WDMessageBoxPanel
        //    {
        //        Title = caption,
        //        Message = message,
        //        Buttons = button,
        //        Icon = icon,
        //    };
        //    _panel.Finished += (_, result) => { Result = result; Close(); };
        //    Content = _panel;
        //    KeyDown += OnWindowKeyDown;
        //}
        public WDMessageBox(string message,string caption,MessageBoxButton button,MessageBoxImage icon,double buttonCornerRadius = 0)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            SystemDecorations = SystemDecorations.None;
            Background = Brushes.Transparent;
            CanResize = false;
            ShowInTaskbar = false;
            ClipToBounds = false;
            _panel = new WDMessageBoxPanel
            {
                Title = caption,
                Message = message,
                Buttons = button,
                Icon = icon,
            };
            _panel.Finished += (_, result) => { Result = result; Close(); };
            Content = _panel;
            KeyDown += OnWindowKeyDown;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var source = e.Source as StyledElement;
            while (source != null)
            {
                if (source is Button)
                    return;
                source = source.Parent;
            }

            var point = e.GetPosition(this);
            if (point.Y >= 0 && point.Y < 40 && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                BeginMoveDrag(e);
            }
        }

        private void OnWindowKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Result = MessageBoxResult.Cancel;
                Close();
                e.Handled = true;
            }
        }
    }
}
