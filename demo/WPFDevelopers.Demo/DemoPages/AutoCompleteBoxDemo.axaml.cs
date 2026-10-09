using Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class AutoCompleteBoxDemo : UserControl
    {
        public AutoCompleteBoxDemo()
        {
            InitializeComponent();
            var autoComplete = this.FindControl<AutoCompleteBox>("DemoAutoCompleteBox");
            if (autoComplete != null)
            {
                autoComplete.ItemsSource = new[]
                {
                    "Avalonia", "AvaloniaUI", "Button", "CheckBox",
                    "ComboBox", "DataGrid", "Expander", "ListBox", "Menu",
                    "NumericUpDown", "ProgressBar", "RadioButton", "Slider",
                    "TabControl", "TextBox", "TreeView", "SplitView", "TimePicker",
                    "WPFDevelopers","WPF","yanjinhua","WPF开发者"
                };
            }
        }
    }
}
