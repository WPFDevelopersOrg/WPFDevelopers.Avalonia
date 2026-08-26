using System.Collections.Concurrent;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;

namespace WPFDevelopers.Avalonia.Controls
{
    public static class Toast
    {
        private static readonly ConcurrentDictionary<Window, ToastLayer> _windowLayers = new();
        private static Position _position = Position.Top;

        static ToastLayer GetOrCreateLayer(Window window)
        {
            if (!_windowLayers.TryGetValue(window, out var layer))
            {
                var overlay = OverlayLayer.GetOverlayLayer(window);
                System.Diagnostics.Debug.WriteLine($"Toast: OverlayLayer for window = {overlay != null}");
                if (overlay == null)
                {
                    System.Diagnostics.Debug.WriteLine("Toast: OverlayLayer not found.");
                    return null;
                }

                layer = new ToastLayer();
                overlay.Children.Add(layer);
                _windowLayers[window] = layer;
                System.Diagnostics.Debug.WriteLine($"Toast: New ToastLayer created, added to overlay. Overlay children count = {overlay.Children.Count}");
            }
            else if (layer.Position != _position)
            {
                layer.Position = _position;
            }
            return layer;
        }

        static Window? FindActiveWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                Window? lastWindow = null;
                foreach (var w in desktop.Windows)
                {
                    System.Diagnostics.Debug.WriteLine($"Toast: Window = {w.Title}, IsActive = {w.IsActive}");
                    if (w.IsActive)
                        return w;
                    lastWindow = w;
                }
                return lastWindow;
            }
            System.Diagnostics.Debug.WriteLine("Toast: No desktop lifetime found");
            return null;
        }

        public static void Push(string message, ToastIcon type = ToastIcon.Info, bool center = false)
        {
            System.Diagnostics.Debug.WriteLine($"Toast.Push called with message: {message}");
            var window = FindActiveWindow();
            System.Diagnostics.Debug.WriteLine($"Toast: Found window = {window != null}");
            Push(window, message, type, center);
        }

        public static void Push(Window? window, string message, ToastIcon type = ToastIcon.Info, bool center = false)
        {
            if (window == null || string.IsNullOrWhiteSpace(message)) return;
            var layer = GetOrCreateLayer(window);
            layer?.Push(message, type, center);
            System.Diagnostics.Debug.WriteLine($"Toast: Push complete. Layer children count = {layer?.Children.Count}");
        }

        public static void SetPosition(Position position)
        {
            _position = position;
            foreach (var layer in _windowLayers.Values)
            {
                layer.Position = position;
            }
        }

        public static void Clear(Window? window = null)
        {
            if (window != null && _windowLayers.TryGetValue(window, out var layer))
            {
                layer.Clear();
            }
            else
            {
                foreach (var l in _windowLayers.Values)
                {
                    l.Clear();
                }
            }
        }
    }
}
