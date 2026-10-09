using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using WPFDevelopers.Avalonia;
using WPFDevelopers.Demo.DemoPages;

namespace WPFDevelopers.Demo
{
    public partial class MainWindow : Window
    {
        private readonly Dictionary<string, Func<UserControl>> _demoPages = new()
        {
            ["Theme"] = () => new ThemeDemo(),
            ["Buttons"] = () => new ButtonsDemo(),
            ["Icons"] = () => new IconsDemo(),
            ["Inputs"] = () => new InputsDemo(),
            ["CheckBox"] = () => new CheckBoxDemo(),
            ["RadioButton"] = () => new RadioButtonDemo(),
            ["ToggleButton"] = () => new ToggleButtonDemo(),
            ["Tags"] = () => new TagsDemo(),
            ["Mask"] = () => new MaskDemo(),
            ["Loading"] = () => new LoadingDemo(),
            ["Badge"] = () => new BadgeDemo(),
            ["Progress"] = () => new ProgressDemo(),
            ["Slider"] = () => new SliderDemo(),
            ["Expander"] = () => new ExpanderDemo(),
            ["ComboBox"] = () => new ComboBoxDemo(),
            ["DataGrid"] = () => new DataGridDemo(),
            ["DateTime"] = () => new DateTimeDemo(),
            ["ListBox"] = () => new ListBoxDemo(),
            ["Menu"] = () => new MenuDemo(),
            ["TabControl"] = () => new TabControlDemo(),
            ["TreeView"] = () => new TreeViewDemo(),
            ["GroupBox"] = () => new GroupBoxDemo(),
            ["RepeatButton"] = () => new RepeatButtonDemo(),
            ["LabelSeparator"] = () => new LabelSeparatorDemo(),
            ["NumericUpDown"] = () => new NumericUpDownDemo(),
            ["AutoCompleteBox"] = () => new AutoCompleteBoxDemo(),
            ["SplitView"] = () => new SplitViewDemo(),
            ["ContextMenu"] = () => new ContextMenuDemo(),
            ["Toast"] = () => new ToastDemo(),
            ["MessageBox"] = () => new MessageBoxDemo()
        };

        public MainWindow()
        {
            InitializeComponent();
            InitializeDemoNavigation();

            if (Application.Current is { } app)
            {
                var trayIcons = TrayIcon.GetIcons(app);
                if (trayIcons == null || trayIcons.Count == 0)
                {
                    var showItem = new NativeMenuItem("Show");
                    var exitItem = new NativeMenuItem("Exit");
                    showItem.Click += TrayShow_Click;
                    exitItem.Click += TrayExit_Click;

                    var tray = new TrayIcon
                    {
                        Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://WPFDevelopers.Demo/Assets/WPFDevelopers.ico"))),
                        Menu = new NativeMenu
                        {
                            showItem,
                            exitItem
                        }
                    };
                    TrayIcon.SetIcons(app, [tray]);
                }
            }
        }

        private void InitializeDemoNavigation()
        {
            var navListBox = this.FindControl<ListBox>("DemoNavListBox");
            if (navListBox == null)
            {
                return;
            }

            navListBox.ItemsSource = _demoPages.Keys.Select(key => new DemoNavItem(key)).ToList();
            navListBox.SelectedIndex = 0;

            var host = this.FindControl<ContentControl>("DemoPageHost");
            if (host != null && _demoPages.TryGetValue(navListBox.SelectedItem is DemoNavItem item ? item.Name : _demoPages.Keys.First(), out var createPage))
            {
                host.Content = createPage();
            }
        }

        private void DemoNavListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var navListBox = sender as ListBox ?? this.FindControl<ListBox>("DemoNavListBox");
            if (navListBox?.SelectedItem is not DemoNavItem selectedItem)
            {
                return;
            }

            var host = this.FindControl<ContentControl>("DemoPageHost");
            if (host == null)
            {
                return;
            }

            if (_demoPages.TryGetValue(selectedItem.Name, out var createPage))
            {
                var currentContent = host.Content;
                host.Content = null;
                if (currentContent is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                var page = createPage();
                page.IsVisible = true;
                host.Content = page;
            }
        }

        private void TrayShow_Click(object? sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void TrayExit_Click(object? sender, EventArgs e)
        {
            Environment.Exit(0);
        }
    }

    public class DemoNavItem
    {
        public DemoNavItem(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
