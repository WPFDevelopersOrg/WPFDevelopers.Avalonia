using System.Globalization;
using System.Net.Mail;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using WPFDevelopers.Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class DialogDemo : UserControl
    {
        public DialogDemo() => InitializeComponent();

        private async void OnOpenClick(object? sender, RoutedEventArgs e)
        {
            var owner = TopLevel.GetTopLevel(this) as Window
                ?? throw new InvalidOperationException("The dialog demo requires an owner window.");
            var language = DemoLanguageManager.Instance;
            var name = new TextBox();
            var email = new TextBox();
            var form = new Grid
            {
                ColumnDefinitions = new ColumnDefinitions("Auto,*"),
                RowDefinitions = new RowDefinitions("Auto,Auto"),
                ColumnSpacing = 12,
                RowSpacing = 8
            };
            var nameLabel = new Label { Content = language["Name"], Target = name, VerticalAlignment = VerticalAlignment.Center };
            var emailLabel = new Label { Content = language["Email"], Target = email, VerticalAlignment = VerticalAlignment.Center };
            Grid.SetColumn(name, 1);
            Grid.SetRow(emailLabel, 1);
            Grid.SetRow(email, 1);
            Grid.SetColumn(email, 1);
            form.Children.AddRange([nameLabel, name, emailLabel, email]);

            var dialog = new Dialog
            {
                Owner = owner,
                Title = language["Edit"],
                ConfirmText = language["Save"],
                DialogWidth = 400,
                IsConfirmEnabled = false,
                Content = new UserControl { Content = form }
            };
            void UpdateSaveEnabled() =>
                dialog.IsConfirmEnabled = !string.IsNullOrWhiteSpace(name.Text) && !string.IsNullOrWhiteSpace(email.Text);

            name.TextChanged += (_, _) => UpdateSaveEnabled();
            email.TextChanged += (_, _) => UpdateSaveEnabled();
            dialog.Closing += (_, args) =>
            {
                if (args.Reason != DialogCloseReason.Confirm || IsValidEmail(email.Text))
                    return;

                args.Cancel = true;
                Toast.Push(owner, language["Invalid email address."], ToastIcon.Error, center: true);
                email.Focus();
            };
            var result = await dialog.ShowAsync();
            ResultText.Text = result == DialogResult.Confirmed
                ? $"{language["Name"]}: {name.Text} · {language["Email"]}: {email.Text}"
                : language["Cancel"];
        }

        private static bool IsValidEmail(string? value)
        {
            if (!MailAddress.TryCreate(value, out var address) ||
                !string.Equals(address.Address, value, StringComparison.OrdinalIgnoreCase))
                return false;

            try
            {
                var host = new IdnMapping { UseStd3AsciiRules = true }.GetAscii(address.Host);
                // This form expects a dotted DNS domain, not a local host or an IP literal.
                return host.Contains('.') && !host.EndsWith('.') && Uri.CheckHostName(host) == UriHostNameType.Dns;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
