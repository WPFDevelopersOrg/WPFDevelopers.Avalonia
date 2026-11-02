using Avalonia.Controls;
using Avalonia.Interactivity;

namespace WPFDevelopers.Demo.DemoPages;

public partial class DrawerDemo : UserControl
{
    public DrawerDemo()
    {
        InitializeComponent();
    }

    private void OnLeftOpenClick(object? sender, RoutedEventArgs e) => LeftDrawer.IsOpen = true;

    private void OnTopOpenClick(object? sender, RoutedEventArgs e) => TopDrawer.IsOpen = true;

    private void OnRightOpenClick(object? sender, RoutedEventArgs e) => RightDrawer.IsOpen = true;

    private void OnBottomOpenClick(object? sender, RoutedEventArgs e) => BottomDrawer.IsOpen = true;
}
