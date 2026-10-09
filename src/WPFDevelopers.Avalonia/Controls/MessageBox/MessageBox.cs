using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;

namespace WPFDevelopers.Avalonia.Controls
{
    public static class MessageBox
    {
        public static async Task<MessageBoxResult> Show(string messageBoxText)
        {
            return await ShowInternalAsync(messageBoxText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, null);
        }

        public static async Task<MessageBoxResult> Show(string messageBoxText, Window owner)
        {
            return await ShowInternalAsync(messageBoxText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, owner);
        }

        public static async Task<MessageBoxResult> Show(string messageBoxText, string caption)
        {
            return await ShowInternalAsync(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, null);
        }

        public static async Task<MessageBoxResult> Show(string messageBoxText, string caption, Window owner)
        {
            return await ShowInternalAsync(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, owner);
        }

        public static async Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxButton button)
        {
            return await ShowInternalAsync(messageBoxText, caption, button, MessageBoxImage.None, null);
        }

        public static async Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxButton button, Window owner)
        {
            return await ShowInternalAsync(messageBoxText, caption, button, MessageBoxImage.None, owner);
        }

        public static async Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxImage icon)
        {
            return await ShowInternalAsync(messageBoxText, caption, MessageBoxButton.OK, icon, null);
        }

        public static async Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxImage icon, Window owner)
        {
            return await ShowInternalAsync(messageBoxText, caption, MessageBoxButton.OK, icon, owner);
        }

        public static async Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return await ShowInternalAsync(messageBoxText, caption, button, icon, null);
        }

        public static async Task<MessageBoxResult> Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, Window owner)
        {
            return await ShowInternalAsync(messageBoxText, caption, button, icon, owner);
        }

        //private static async Task<MessageBoxResult> ShowInternalAsync(string message, string caption, MessageBoxButton button, MessageBoxImage icon, Window? owner)
        //{
        //    var msgBox = new WDMessageBox(message, caption, button, icon);
        //    msgBox.WindowStartupLocation = WindowStartupLocation.Manual;
        //    msgBox.ShowInTaskbar = false;
        //    msgBox.CanResize = false;
        //    msgBox.Background = Brushes.Transparent;

        //    msgBox.Opened += (_, _) =>
        //    {
        //        if (owner == null)
        //            return;
        //        var scale = owner.RenderScaling;
        //        var width = msgBox.Bounds.Width;
        //        var height = msgBox.Bounds.Height;
        //        var ownerWidth = owner.Bounds.Width * scale;
        //        var ownerHeight = owner.Bounds.Height * scale;
        //        var x = owner.Position.X +
        //                (int)((ownerWidth - width) / 2);
        //        var y = owner.Position.Y +
        //                (int)((ownerHeight - height) / 2);
        //        msgBox.Position = new PixelPoint(x, y);
        //    };

        //    if (owner != null)
        //    {
        //        owner.IsEnabled = false;
        //        await msgBox.ShowDialog(owner);
        //        owner.IsEnabled = true;
        //        owner.Activate();
        //    }
        //    else
        //    {
        //        await msgBox.ShowDialog(null);
        //    }

        //    return msgBox.Result;
        //}

        private static async Task<MessageBoxResult> ShowInternalAsync(string message, string caption, MessageBoxButton button, MessageBoxImage icon, Window? owner)
        {
            var msgBox = new WDMessageBox(message, caption, button, icon);
            msgBox.WindowStartupLocation = WindowStartupLocation.Manual;
            msgBox.ShowInTaskbar = false;
            msgBox.CanResize = false;
            msgBox.Background = Brushes.Transparent;
            msgBox.Opened += (_, _) =>
            {
                if (owner == null)
                    return;
                var scale = owner.RenderScaling;
                var width = msgBox.Bounds.Width;
                var height = msgBox.Bounds.Height;
                var ownerWidth = owner.Bounds.Width * scale;
                var ownerHeight = owner.Bounds.Height * scale;
                var x = owner.Position.X +
                        (int)((ownerWidth - width) / 2);
                var y = owner.Position.Y +
                        (int)((ownerHeight - height) / 2);
                msgBox.Position = new PixelPoint(x, y);
            };
            if (owner != null)
            {
                var p = owner.Position;
                owner.Position = new PixelPoint(p.X + 1, p.Y);
                owner.Position = p;
                var scale = owner.RenderScaling;
                const int dialogWidth = 380;
                const int dialogHeight = 180;
                msgBox.Width = dialogWidth;
                msgBox.Height = dialogHeight;
                var ownerWidth = owner.Bounds.Width * scale;
                var ownerHeight = owner.Bounds.Height * scale;
                var x = owner.Position.X +
                        (int)((ownerWidth - dialogWidth) / 2);

                var y = owner.Position.Y +
                        (int)((ownerHeight - dialogHeight) / 2);
                msgBox.Position = new PixelPoint(x, y);
                await msgBox.ShowDialog(owner);
                return msgBox.Result;
            }
            await msgBox.ShowDialog((Window)null!);
            return msgBox.Result;
        }


        private static Window? FindActiveWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Window? lastWindow = null;
                foreach (var w in desktop.Windows)
                {
                    if (w.IsActive)
                        return w;
                    lastWindow = w;
                }
                return lastWindow;
            }
            return null;
        }
    }
}
