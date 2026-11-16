using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using WPFDevelopers.Avalonia;
using WPFDevelopers.Avalonia.Windows;

namespace WPFDevelopers.Demo.DemoPages;

public partial class WindowDemo : UserControl
{
    private bool? _dialogConfirmed;

    public WindowDemo()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        DemoLanguageManager.Instance.PropertyChanged += OnLanguageChanged;
        UpdateDialogResultText();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        DemoLanguageManager.Instance.PropertyChanged -= OnLanguageChanged;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnNormalClick(object? sender, RoutedEventArgs e) => ShowWindow(TitleBarMode.Normal);

    private void OnHighClick(object? sender, RoutedEventArgs e) => ShowWindow(TitleBarMode.High);

    private void OnCustomClick(object? sender, RoutedEventArgs e) => ShowWindow(TitleBarMode.None);

    private void ShowWindow(TitleBarMode mode)
    {
        var owner = GetOwnerWindow();
        var window = new WindowSample(mode) { Icon = owner.Icon };
        window.Show(owner);
    }

    private async void OnDialogClick(object? sender, RoutedEventArgs e)
    {
        var owner = GetOwnerWindow();
        var window = new WindowSample(TitleBarMode.Normal, isDialog: true) { Icon = owner.Icon };
        var confirmed = await window.ShowDialog<bool>(owner);
        _dialogConfirmed = confirmed;
        UpdateDialogResultText();
    }

    private void OnLanguageChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Item[]")
            UpdateDialogResultText();
    }

    private void UpdateDialogResultText()
    {
        if (_dialogConfirmed is { } confirmed)
        {
            var key = confirmed ? "DialogConfirmed" : "DialogCancelled";
            DialogResultText.Text = DemoLanguageManager.Instance[key];
        }
    }

    private global::Avalonia.Controls.Window GetOwnerWindow()
    {
        return TopLevel.GetTopLevel(this) as global::Avalonia.Controls.Window
            ?? throw new InvalidOperationException("The Window demo must be hosted in a desktop window.");
    }
}
