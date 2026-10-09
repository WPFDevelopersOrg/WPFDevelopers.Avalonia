using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.VisualTree;
using WPFDevelopers.Avalonia;
using WPFDevelopers.Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class MessageBoxDemo : UserControl
    {
        public MessageBoxDemo()
        {
            InitializeComponent();
            var msgBoxInfoBtn = this.FindControl<Button>("MsgBoxInfoBtn");
            if (msgBoxInfoBtn != null) msgBoxInfoBtn.Click += (_, _) => MessageBox.Show("This is an information message.", "Information", MessageBoxImage.Information, GetOwnerWindow());

            var msgBoxWarningBtn = this.FindControl<Button>("MsgBoxWarningBtn");
            if (msgBoxWarningBtn != null) msgBoxWarningBtn.Click += (_, _) => MessageBox.Show("This is a warning message.", "Warning", MessageBoxImage.Warning, GetOwnerWindow());

            var msgBoxErrorBtn = this.FindControl<Button>("MsgBoxErrorBtn");
            if (msgBoxErrorBtn != null) msgBoxErrorBtn.Click += (_, _) => MessageBox.Show("An error has occurred.", "Error", MessageBoxImage.Error, GetOwnerWindow());


            var msgBoxQuestionBtn = this.FindControl<Button>("MsgBoxQuestionBtn");
            if (msgBoxQuestionBtn != null) msgBoxQuestionBtn.Click += (_, _) => MessageBox.Show("Are you sure?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question, GetOwnerWindow());

            var msgBoxYesNoCancelBtn = this.FindControl<Button>("MsgBoxYesNoCancelBtn");
            if (msgBoxYesNoCancelBtn != null) msgBoxYesNoCancelBtn.Click += async (_, _) =>
            {
                var result = await MessageBox.Show("Do you want to save changes?", "Confirm", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, GetOwnerWindow());
                if (result == MessageBoxResult.Yes)
                {
                    Toast.Push($"Result: {result}", ToastIcon.Info);
                }
            };
        }

        private Window? GetOwnerWindow()
        {
            if (this.GetVisualRoot() is Window visualRoot)
            {
                return visualRoot;
            }

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.MainWindow;
            }

            return null;
        }
    }
}
