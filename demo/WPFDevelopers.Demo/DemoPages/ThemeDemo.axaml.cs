using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using WPFDevelopers.Avalonia;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class ThemeDemo : UserControl
    {
        private bool _isDark;
        private List<ColorOption> _colorOptions = new();

        public ThemeDemo()
        {
            InitializeComponent();
            InitializeColorOptions();

            var themeToggle = this.FindControl<Button>("ThemeToggle");
            if (themeToggle != null)
            {
                themeToggle.Click += OnThemeToggleClick;
            }
        }

        private void InitializeColorOptions()
        {
            _colorOptions = new List<ColorOption>
            {
                new() { Name = "玫瑰红", Color = new SolidColorBrush(Color.Parse("#FF4D6D")), ColorCode = "#FF4D6D" },
                new() { Name = "珊瑚橙", Color = new SolidColorBrush(Color.Parse("#FF6D3A")), ColorCode = "#FF6D3A" },
                new() { Name = "向日葵黄", Color = new SolidColorBrush(Color.Parse("#FFD166")), ColorCode = "#FFD166" },
                new() { Name = "草绿色", Color = new SolidColorBrush(Color.Parse("#06D6A0")), ColorCode = "#06D6A0" },
                new() { Name = "天空蓝", Color = new SolidColorBrush(Color.Parse("#118AB2")), ColorCode = "#118AB2" },
                new() { Name = "薰衣草紫", Color = new SolidColorBrush(Color.Parse("#9B5DE5")), ColorCode = "#9B5DE5" },
                new() { Name = "樱花粉", Color = new SolidColorBrush(Color.Parse("#FFB7B2")), ColorCode = "#FFB7B2" },
                new() { Name = "薄荷绿", Color = new SolidColorBrush(Color.Parse("#A7E0E0")), ColorCode = "#A7E0E0" }
            };

            var itemsControl = this.FindControl<ItemsControl>("ColorItemsControl");
            if (itemsControl != null)
            {
                itemsControl.ItemsSource = _colorOptions;
            }
        }

        private void OnThemeToggleClick(object? sender, RoutedEventArgs e)
        {
            _isDark = !_isDark;
            var themeToggle = sender as Button;
            if (themeToggle != null)
            {
                themeToggle.Content = _isDark ? "Switch to Light" : "Switch to Dark";
            }
            Application.Current?.SetTheme(_isDark ? ThemeType.Dark : ThemeType.Light);
        }

        private void OnColorRadioButtonChecked(object? sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true && rb.Tag is ColorOption selectedColor)
            {
                var appResources = Application.Current?.Resources as Resources;
                if (appResources != null)
                {
                    appResources.Color = Color.Parse(selectedColor.ColorCode);
                }
            }
        }
    }

    public class ColorOption
    {
        public string Name { get; set; } = string.Empty;
        public IBrush Color { get; set; } = Brushes.Transparent;
        public string ColorCode { get; set; } = string.Empty;
    }
}
