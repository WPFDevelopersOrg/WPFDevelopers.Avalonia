using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace WPFDevelopers.Avalonia.Controls
{
    public static class Mask
    {
        public static readonly AttachedProperty<bool> IsShowProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>("IsShow", typeof(Mask));

        public static readonly AttachedProperty<object> MaskContentProperty =
            AvaloniaProperty.RegisterAttached<Control, object>("MaskContent", typeof(Mask));

        public static readonly AttachedProperty<IBrush> BackgroundProperty =
            AvaloniaProperty.RegisterAttached<Control, IBrush>("Background", typeof(Mask));

        public static readonly AttachedProperty<CornerRadius> CornerRadiusProperty =
            AvaloniaProperty.RegisterAttached<Control, CornerRadius>("CornerRadius", typeof(Mask));

        private static readonly AttachedProperty<object?> OverlayDataProperty =
            AvaloniaProperty.RegisterAttached<Control, object?>("OverlayData", typeof(Mask));

        static Mask()
        {
            IsShowProperty.Changed.AddClassHandler<Control>((c, _) =>
                Dispatcher.UIThread.Post(() => UpdateMask(c)));
            MaskContentProperty.Changed.AddClassHandler<Control>((c, _) =>
                Dispatcher.UIThread.Post(() => UpdateContent(c)));
        }

        public static bool GetIsShow(Control element) => element.GetValue(IsShowProperty);
        public static void SetIsShow(Control element, bool value) => element.SetValue(IsShowProperty, value);

        public static object GetMaskContent(Control element) => element.GetValue(MaskContentProperty);
        public static void SetMaskContent(Control element, object value) => element.SetValue(MaskContentProperty, value);

        public static IBrush GetBackground(Control element) => element.GetValue(BackgroundProperty);
        public static void SetBackground(Control element, IBrush value) => element.SetValue(BackgroundProperty, value);

        public static CornerRadius GetCornerRadius(Control element)
        {
            var value = element.GetValue(CornerRadiusProperty);
            if (value != new CornerRadius())
                return value;

            if (element is Border border)
                return border.CornerRadius;
            if (element is Button button)
                return button.CornerRadius;

            return new CornerRadius();
        }
        public static void SetCornerRadius(Control element, CornerRadius value) => element.SetValue(CornerRadiusProperty, value);

        private static void UpdateMask(Control control)
        {
            if (control.GetValue(IsShowProperty))
                ShowMask(control);
            else
                HideMask(control);
        }

        private static void UpdateContent(Control control)
        {
            var data = control.GetValue(OverlayDataProperty);
            if (data is OverlayInfo info && info.OverlayGrid is Grid g && g.Children.Count > 1 && g.Children[1] is ContentPresenter cp)
            {
                cp.Content = control.GetValue(MaskContentProperty);
            }
        }

        private static void ShowMask(Control control)
        {
            if (control.GetValue(OverlayDataProperty) != null) return;

            control.DetachedFromVisualTree -= OnDetachedFromVisualTree;
            control.DetachedFromVisualTree += OnDetachedFromVisualTree;

            var background = control.GetValue(BackgroundProperty)
                ?? control.FindResource("WD.ChartFillBrush") as IBrush
                ?? Brushes.White;
            var cornerRadius = GetCornerRadius(control);
            var maskContent = control.GetValue(MaskContentProperty);

            var overlayGrid = new Grid
            {
                Width = control.Bounds.Width,
                Height = control.Bounds.Height
            };

            var backgroundLayer = new Border
            {
                Background = background,
                CornerRadius = cornerRadius,
                Opacity = 0.6,
                IsHitTestVisible = true
            };

            var contentLayer = new ContentPresenter
            {
                HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
                Content = maskContent
            };

            overlayGrid.Children.Add(backgroundLayer);
            overlayGrid.Children.Add(contentLayer);

            var vlm = FindVisualLayerManager(control);
            if (vlm != null)
            {
                vlm.AdornerLayer.Children.Add(overlayGrid);
                AdornerLayer.SetAdornedElement(overlayGrid, control);
                control.SetValue(OverlayDataProperty, new OverlayInfo { OverlayGrid = overlayGrid, Mode = OverlayMode.Adorner });
                control.LayoutUpdated += OnLayoutUpdated;
                return;
            }

            TryInsertOverlay(control, overlayGrid);
        }

        private static void HideMask(Control control)
        {
            var data = control.GetValue(OverlayDataProperty);
            if (data is not OverlayInfo info) return;

            control.LayoutUpdated -= OnLayoutUpdated;
            control.DetachedFromVisualTree -= OnDetachedFromVisualTree;

            if (info.Mode == OverlayMode.Adorner && info.OverlayGrid?.Parent is AdornerLayer al)
            {
                al.Children.Remove(info.OverlayGrid);
            }
            else if (info.OverlayGrid != null)
            {
                RestoreGridChild(info.OverlayGrid.GetVisualParent() as Grid, control);
            }

            control.SetValue(OverlayDataProperty, null);
        }

        private static void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
        {
            if (sender is not Control control)
            {
                return;
            }

            if (control.GetValue(IsShowProperty))
            {
                HideMask(control);
            }
        }

        private static void OnLayoutUpdated(object? sender, System.EventArgs e)
        {
            if (sender is Control control)
            {
                var data = control.GetValue(OverlayDataProperty);
                if (data is OverlayInfo info && info.OverlayGrid != null)
                {
                    info.OverlayGrid.Width = control.Bounds.Width;
                    info.OverlayGrid.Height = control.Bounds.Height;
                }
            }
        }

        private static VisualLayerManager? FindVisualLayerManager(Control control)
        {
            Visual? current = control;
            while (current != null)
            {
                if (current is VisualLayerManager vlm)
                    return vlm;
                current = current.GetVisualParent();
            }
            return null;
        }

        private static void TryInsertOverlay(Control control, Grid overlay)
        {
            Visual? current = control.GetVisualParent();
            Control? childToReplace = control;

            while (current != null)
            {
                if (current is Panel panel)
                {
                    var index = panel.Children.IndexOf(childToReplace);
                    if (index < 0) break;

                    panel.Children.RemoveAt(index);
                    var grid = new Grid { Children = { control, overlay } };
                    panel.Children.Insert(index, grid);
                    control.SetValue(OverlayDataProperty, new OverlayInfo
                    {
                        OverlayGrid = overlay,
                        Mode = OverlayMode.GridWrap
                    });
                    control.LayoutUpdated += OnLayoutUpdated;
                    return;
                }

                if (current is ContentControl cc)
                {
                    if (!ReferenceEquals(cc.Content, childToReplace)) break;
                    var grid = new Grid { Children = { control, overlay } };
                    cc.Content = grid;
                    control.SetValue(OverlayDataProperty, new OverlayInfo
                    {
                        OverlayGrid = overlay,
                        Mode = OverlayMode.GridWrap
                    });
                    control.LayoutUpdated += OnLayoutUpdated;
                    return;
                }

                if (current is Decorator dec)
                {
                    if (!ReferenceEquals(dec.Child, childToReplace)) break;
                    var grid = new Grid { Children = { control, overlay } };
                    dec.Child = grid;
                    control.SetValue(OverlayDataProperty, new OverlayInfo
                    {
                        OverlayGrid = overlay,
                        Mode = OverlayMode.GridWrap
                    });
                    control.LayoutUpdated += OnLayoutUpdated;
                    return;
                }

                if (current is ContentPresenter cp)
                {
                    if (!ReferenceEquals(cp.Content, childToReplace)) break;
                    var grid = new Grid { Children = { control, overlay } };
                    cp.Content = grid;
                    control.SetValue(OverlayDataProperty, new OverlayInfo
                    {
                        OverlayGrid = overlay,
                        Mode = OverlayMode.GridWrap
                    });
                    control.LayoutUpdated += OnLayoutUpdated;
                    return;
                }

                childToReplace = (Control)current;
                current = current.GetVisualParent();
            }
        }

        private static void RestoreGridChild(Grid? wrapperGrid, Control control)
        {
            if (wrapperGrid == null) return;

            wrapperGrid.Children.Remove(control);
            wrapperGrid.Children.Clear();

            var parent = wrapperGrid.GetVisualParent();
            if (parent is Panel panel)
            {
                var index = panel.Children.IndexOf(wrapperGrid);
                panel.Children.RemoveAt(index);
                panel.Children.Insert(index, control);
            }
            else if (parent is ContentControl cc)
            {
                cc.Content = control;
            }
            else if (parent is Decorator dec)
            {
                dec.Child = control;
            }
            else if (parent is ContentPresenter cp)
            {
                cp.Content = control;
            }
        }

        private class OverlayInfo
        {
            public required Grid OverlayGrid;
            public required OverlayMode Mode;
        }

        private enum OverlayMode { Adorner, GridWrap }
    }
}
