using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace WPFDevelopers.Demo
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            //ScrollViewer.AllowAutoHideProperty.OverrideDefaultValue(typeof(ScrollViewer), false);
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = new MainWindow();
                if (OperatingSystem.IsWindows())
                {
                    if (!GetCursorPos(out var cursor))
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }

                    // CenterScreen selects the monitor containing Position before the first Show.
                    window.Position = new PixelPoint(cursor.X, cursor.Y);
                }

                desktop.MainWindow = window;
                //desktop.MainWindow = new Window1();
            }

            base.OnFrameworkInitializationCompleted();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT point);
    }
}
