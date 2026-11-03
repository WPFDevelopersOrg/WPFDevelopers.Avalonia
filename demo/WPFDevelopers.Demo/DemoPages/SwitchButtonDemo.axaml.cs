using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.VisualTree;
using WPFDevelopers.Avalonia;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class SwitchButtonDemo : UserControl
    {
        public SwitchButtonDemo()
        {
            InitializeComponent();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            var app = Application.Current
                ?? throw new InvalidOperationException("The application is not initialized.");
            app.ActualThemeVariantChanged += OnActualThemeVariantChanged;
            UpdateThemeSwitch();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            if (Application.Current is { } app)
                app.ActualThemeVariantChanged -= OnActualThemeVariantChanged;

            base.OnDetachedFromVisualTree(e);
        }

        private void OnActualThemeVariantChanged(object? sender, EventArgs e)
        {
            UpdateThemeSwitch();
        }

        private void UpdateThemeSwitch()
        {
            ThemeSwitch.IsChecked = Application.Current?.ActualThemeVariant == ThemeVariant.Dark;
        }

        private void OnThemeSwitchClick(object? sender, RoutedEventArgs e)
        {
            var app = Application.Current
                ?? throw new InvalidOperationException("The application is not initialized.");
            app.SetTheme(ThemeSwitch.IsChecked == true ? ThemeType.Dark : ThemeType.Light);
        }
    }
}
