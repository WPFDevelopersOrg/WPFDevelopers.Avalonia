using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace WPFDevelopers.Avalonia.Controls
{
    public static class Loading
    {
        public static readonly AttachedProperty<bool> IsShowProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>("IsShow", typeof(Loading));

        static Loading()
        {
            IsShowProperty.Changed.AddClassHandler<Control>((c, _) => UpdateLoading(c));
        }

        public static bool GetIsShow(Control element) => element.GetValue(IsShowProperty);
        public static void SetIsShow(Control element, bool value) => element.SetValue(IsShowProperty, value);

        private static void UpdateLoading(Control control)
        {
            var isShow = GetIsShow(control);

            if (isShow)
            {
                var ring = new DefaultLoading
                {
                    Width = 25,
                    Height = 25,
                    ArcThickness = 1.5,
                    StrokeBrush = ThemeManager.Instance.PrimaryBrush ?? Brushes.White,
                    IsActive = true
                };

                Mask.SetMaskContent(control, ring);
                Mask.SetIsShow(control, true);
            }
            else
            {
                Mask.SetIsShow(control, false);
                Mask.SetMaskContent(control, null!);
            }
        }
    }
}
