using System.Collections.Generic;
using Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class MultiSelectComboBoxDemo : UserControl
    {
        public MultiSelectComboBoxDemo()
        {
            InitializeComponent();

            var source = new List<DemoItem>
            {
                new() { Name = "Alice", Age = 28, IsActive = true, Role = "Admin" },
                new() { Name = "Bob", Age = 34, IsActive = true, Role = "Editor" },
                new() { Name = "Charlie", Age = 22, IsActive = false, Role = "Viewer" },
                new() { Name = "Diana", Age = 41, IsActive = true, Role = "Admin" },
                new() { Name = "Eve", Age = 30, IsActive = false, Role = "Editor" },
                new() { Name = "Frank", Age = 26, IsActive = true, Role = "Viewer" }
            };

            var defaultSelected = new List<object> { source[0], source[3] };

            var textModeCombo = this.FindControl<WPFDevelopers.Avalonia.Controls.MultiSelectComboBox>("TextModeCombo");
            if (textModeCombo != null)
            {
                textModeCombo.ItemsSource = source;
                textModeCombo.SelectedItemsExt = new List<object>(defaultSelected);
            }

            var tagModeCombo = this.FindControl<WPFDevelopers.Avalonia.Controls.MultiSelectComboBox>("TagModeCombo");
            if (tagModeCombo != null)
            {
                tagModeCombo.ItemsSource = source;
                tagModeCombo.SelectedItemsExt = new List<object>(defaultSelected);
            }

            var multiColumnCombo = this.FindControl<WPFDevelopers.Avalonia.Controls.MultiSelectComboBox>("MultiColumnCombo");
            if (multiColumnCombo != null)
            {
                multiColumnCombo.ItemsSource = source;
                multiColumnCombo.SelectedItemsExt = new List<object>(defaultSelected);
            }
        }
    }
}
