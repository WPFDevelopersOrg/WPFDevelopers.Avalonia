using Avalonia.Controls;
using Avalonia.Interactivity;
using WPFDevelopers.Avalonia.Windows;

namespace WPFDevelopers.Demo.DemoPages;

public partial class WindowSample : WPFDevelopers.Avalonia.Windows.Window
{
    public WindowSample() : this(TitleBarMode.Normal)
    {
    }

    public WindowSample(TitleBarMode mode, bool isDialog = false)
    {
        InitializeComponent();
        TitleBarMode = mode;
        ShowTheme = true;
        if (mode == TitleBarMode.None)
            TitleHeight = 44;
        if (mode != TitleBarMode.High)
            TitleBarLayout = null;

        Title = isDialog ? "Window · 模态窗口" : $"Window · {mode}";
        ModeText.Text = Title;
        CanResize = !isDialog;
        DialogActions.IsVisible = isDialog;
        CloseButton.IsVisible = !isDialog;
        if (isDialog)
            StatusText.Text = "点击确定或取消，将结果返回示例页面。";
    }

    private void OnToolbarClick(object? sender, RoutedEventArgs e)
    {
        StatusText.Text = "已点击 TitleBarLayout 中的按钮。";
    }

    private void OnMinimizeClick(object? sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    private void OnRestoreClick(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close(false);

    private void OnConfirmClick(object? sender, RoutedEventArgs e) => Close(true);
}
