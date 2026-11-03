using Avalonia.Controls;
using Avalonia.Interactivity;

namespace WPFDevelopers.Demo.DemoPages;

public partial class StepDemo : UserControl
{
    public StepDemo()
    {
        InitializeComponent();
    }

    private void OnPreviousClick(object? sender, RoutedEventArgs e)
    {
        HorizontalStep.Previous();
    }

    private void OnNextClick(object? sender, RoutedEventArgs e)
    {
        HorizontalStep.Next();
    }
}
