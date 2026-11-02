using Avalonia.Controls;
using Avalonia.Interactivity;
using WPFDevelopers.Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages;

public partial class GestureUnlockDemo : UserControl
{
    public GestureUnlockDemo()
    {
        InitializeComponent();
    }

    private void OnGestureCompleted(object? sender, RoutedEventArgs e)
    {
        if (Unlock.Password == "0426")
        {
            Unlock.State = ControlState.Success;
            ResultText.Text = "Pattern accepted.";
            Toast.Push("Pattern accepted.", ToastIcon.Success);
            return;
        }

        Unlock.State = ControlState.Error;
        Toast.Push("Pattern rejected. Try again.", ToastIcon.Error);
    }
}
