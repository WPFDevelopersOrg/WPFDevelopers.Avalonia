using Avalonia.Controls;
using Avalonia.Interactivity;
using WPFDevelopers.Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages;

public partial class OtpBoxDemo : UserControl
{
    public OtpBoxDemo()
    {
        InitializeComponent();
    }

    private void OnVerificationCompleted(object? sender, RoutedEventArgs e)
    {
        var accepted = VerificationCode.Value == "1234";
        VerificationCode.State = accepted ? OtpBoxState.Success : OtpBoxState.Error;
        VerificationResult.Text = accepted
            ? "Code accepted."
            : "Code rejected. Try 1234 after the fields reset.";
    }

    private void OnCustomCodeCompleted(object? sender, RoutedEventArgs e)
    {
        CustomCode.State = OtpBoxState.Success;
        CustomCodeResult.Text = $"Completed code: {CustomCode.Value}";
    }

    private void OnClearCodesClick(object? sender, RoutedEventArgs e)
    {
        VerificationCode.State = OtpBoxState.None;
        VerificationCode.Value = string.Empty;
        CustomCode.State = OtpBoxState.None;
        CustomCode.Value = string.Empty;
        VerificationResult.Text = string.Empty;
        CustomCodeResult.Text = string.Empty;
    }
}
