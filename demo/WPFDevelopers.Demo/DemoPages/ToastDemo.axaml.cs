using Avalonia.Controls;
using Avalonia.Interactivity;
using WPFDevelopers.Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class ToastDemo : UserControl
    {
        public ToastDemo()
        {
            InitializeComponent();
            var toastInfoBtn = this.FindControl<Button>("ToastInfoBtn");
            if (toastInfoBtn != null) toastInfoBtn.Click += (_, _) => Toast.Push("This is an info toast message.", ToastIcon.Info);

            var toastSuccessBtn = this.FindControl<Button>("ToastSuccessBtn");
            if (toastSuccessBtn != null) toastSuccessBtn.Click += (_, _) => Toast.Push("Operation completed successfully!", ToastIcon.Success);

            var toastWarningBtn = this.FindControl<Button>("ToastWarningBtn");
            if (toastWarningBtn != null) toastWarningBtn.Click += (_, _) => Toast.Push("Warning: please check your input.", ToastIcon.Warning);

            var toastErrorBtn = this.FindControl<Button>("ToastErrorBtn");
            if (toastErrorBtn != null) toastErrorBtn.Click += (_, _) => Toast.Push("An error occurred. Please try again.", ToastIcon.Error);

            var toastClearBtn = this.FindControl<Button>("ToastClearBtn");
            if (toastClearBtn != null) toastClearBtn.Click += (_, _) => Toast.Clear();
        }
       
    }
}
