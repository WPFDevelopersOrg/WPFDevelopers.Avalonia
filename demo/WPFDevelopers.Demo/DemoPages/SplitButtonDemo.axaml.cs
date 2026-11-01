using Avalonia.Controls;
using Avalonia.Interactivity;
using WPFDevelopers.Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages;

public partial class SplitButtonDemo : UserControl
{
    public string[] MenuItems { get; } =
    [
        "Save as PDF",
        "Save as Word",
        "Save as Excel",
        "Save as Image"
    ];

    public SplitButtonDemo()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OnMainClick(object? sender, RoutedEventArgs e)
    {
        if (sender is WPFDevelopers.Avalonia.Controls.SplitButton button)
            StatusText.Text = $"Main button clicked: {button.Content}";
    }

    private void OnSelectionChanged(object? sender, SplitButtonSelectionChangedEventArgs e)
    {
        StatusText.Text = $"Selection changed: {e.OldValue} -> {e.NewValue}";
    }
}
