using Avalonia;
using Avalonia.Controls.Primitives;

namespace WPFDevelopers.Avalonia.Controls
{
    public class SwitchButton : ToggleButton
    {
        public static readonly StyledProperty<object?> CheckedContentProperty =
            AvaloniaProperty.Register<SwitchButton, object?>(nameof(CheckedContent));

        public static readonly StyledProperty<object?> UncheckedContentProperty =
            AvaloniaProperty.Register<SwitchButton, object?>(nameof(UncheckedContent));

        public object? CheckedContent
        {
            get => GetValue(CheckedContentProperty);
            set => SetValue(CheckedContentProperty, value);
        }

        public object? UncheckedContent
        {
            get => GetValue(UncheckedContentProperty);
            set => SetValue(UncheckedContentProperty, value);
        }
    }
}
