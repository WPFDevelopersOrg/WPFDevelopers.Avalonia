using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using WPFDevelopers.Avalonia;
using WPFDevelopers.Avalonia.Controls;
using WPFDevelopers.Demo.DemoPages;

namespace WPFDevelopers.Demo
{
    public partial class MainWindow : WPFDevelopers.Avalonia.Windows.Window
    {
        internal static string LibraryVersion { get; } = GetLibraryVersion();

        private static string GetLibraryVersion()
        {
            var assembly = typeof(WPFDevelopers.Avalonia.Controls.Tag).Assembly;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? assembly.GetName().Version?.ToString()
                ?? throw new InvalidOperationException("The control library assembly has no version.");
            return version.Split('+')[0];
        }

        private static readonly Dictionary<string, string> DemoIconNames = new()
        {
            ["Buttons"] = "Button",
            ["Icons"] = "Vector",
            ["Inputs"] = "Text",
            ["ColorPicker"] = "Palette",
            ["CheckBox"] = "Checkbox",
            ["RadioButton"] = "RadioButton",
            ["Segmented"] = "Selection",
            ["SplitButton"] = "Selection",
            ["SwitchButton"] = "ToggleButton",
            ["Timeline"] = "Activity",
            ["Carousel"] = "Image",
            ["OtpBox"] = "Shield",
            ["GestureUnlock"] = "Shield",
            ["ToggleButton"] = "ToggleButton",
            ["Tags"] = "Tag",
            ["Mask"] = "Image",
            ["Loading"] = "Spinner",
            ["Badge"] = "Activity",
            ["Progress"] = "Progress",
            ["Step"] = "Steps",
            ["Drawer"] = "Navigation",
            ["Slider"] = "Slider",
            ["Expander"] = "Expander",
            ["ComboBox"] = "ComboBox",
            ["DataGrid"] = "DataGrid",
            ["DateTime"] = "DateTimePicker",
            ["ListBox"] = "ListBox",
            ["Menu"] = "Navigation",
            ["TabControl"] = "TabControl",
            ["TreeView"] = "TreeView",
            ["GroupBox"] = "GroupBox",
            ["GridSplitter"] = "GridSplitter",
            ["RepeatButton"] = "Button",
            ["LabelSeparator"] = "Text",
            ["NumericUpDown"] = "Selection",
            ["AutoCompleteBox"] = "Search",
            ["SplitView"] = "Navigation",
            ["ContextMenu"] = "Navigation",
            ["Window"] = "GroupBox",
            ["Toast"] = "Chat",
            ["MessageBox"] = "Chat",
            ["MultiSelectComboBox"] = "Selection",
            ["Pagination"] = "Selection"
        };

        private static readonly HashSet<string> BasicMenuItems = new()
        {
            "Buttons",
            "Inputs",
            "CheckBox",
            "RadioButton",
            "ToggleButton",
            "Progress",
            "Slider",
            "Expander",
            "ComboBox",
            "DataGrid",
            "DateTime",
            "ListBox",
            "Menu",
            "TabControl",
            "TreeView",
            "GroupBox",
            "GridSplitter",
            "RepeatButton",
            "LabelSeparator",
            "NumericUpDown",
            "AutoCompleteBox",
            "SplitView",
            "ContextMenu",
            "Window"
        };

        private readonly Dictionary<string, Func<UserControl>> _demoPages = new()
        {
            ["Home"] = () => new HomeDemo(),
            ["Buttons"] = () => new ButtonsDemo(),
            ["Icons"] = () => new IconsDemo(),
            ["Inputs"] = () => new InputsDemo(),
            ["ColorPicker"] = () => new ColorPickerDemo(),
            ["CheckBox"] = () => new CheckBoxDemo(),
            ["RadioButton"] = () => new RadioButtonDemo(),
            ["Segmented"] = () => new SegmentedDemo(),
            ["SplitButton"] = () => new SplitButtonDemo(),
            ["SwitchButton"] = () => new SwitchButtonDemo(),
            ["Timeline"] = () => new TimelineDemo(),
            ["Carousel"] = () => new CarouselDemo(),
            ["OtpBox"] = () => new OtpBoxDemo(),
            ["GestureUnlock"] = () => new GestureUnlockDemo(),
            ["ToggleButton"] = () => new ToggleButtonDemo(),
            ["Tags"] = () => new TagsDemo(),
            ["Mask"] = () => new MaskDemo(),
            ["Loading"] = () => new LoadingDemo(),
            ["Badge"] = () => new BadgeDemo(),
            ["Progress"] = () => new ProgressDemo(),
            ["Step"] = () => new StepDemo(),
            ["Drawer"] = () => new DrawerDemo(),
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
            ["GridSplitter"] = () => new GridSplitterDemo(),
            ["RepeatButton"] = () => new RepeatButtonDemo(),
            ["LabelSeparator"] = () => new LabelSeparatorDemo(),
            ["NumericUpDown"] = () => new NumericUpDownDemo(),
            ["AutoCompleteBox"] = () => new AutoCompleteBoxDemo(),
            ["SplitView"] = () => new SplitViewDemo(),
            ["ContextMenu"] = () => new ContextMenuDemo(),
            ["Window"] = () => new WindowDemo(),
            ["Toast"] = () => new ToastDemo(),
            ["MessageBox"] = () => new MessageBoxDemo(),
            ["MultiSelectComboBox"] = () => new MultiSelectComboBoxDemo(),
            ["Pagination"] = () => new PaginationDemo(),

        };

        private string? _selectedDemoName;
        private bool _isExitConfirmationOpen;
        private bool _isExitConfirmed;

        public MainWindow()
        {
            InitializeComponent();
            DemoLanguageManager.Instance.PropertyChanged += OnLanguageChanged;
            UpdateTitle();
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

        private void OnLanguageChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Item[]")
            {
                UpdateTitle();
                UpdateDemoNavigation(selectFirst: false);
                UpdateSourceTitle();
            }
        }

        private void UpdateTitle()
        {
            Title = $"{DemoLanguageManager.Instance["MainWindowTitle"]} · v{LibraryVersion}";
        }

        private void InitializeDemoNavigation()
        {
            MenuCategorySegmented.SelectionChanged += MenuCategorySegmented_SelectionChanged;
            MenuSearchTextBox.TextChanged += MenuSearchTextBox_TextChanged;
            UpdateDemoNavigation(selectFirst: false);
            ShowDemoPage("Home");
        }

        private void HomeButton_Click(object? sender, RoutedEventArgs e)
        {
            DemoNavListBox.SelectedItem = null;
            ShowDemoPage("Home");
        }

        private void MenuCategorySegmented_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            UpdateDemoNavigation(selectFirst: true);
        }

        private void MenuSearchTextBox_TextChanged(object? sender, TextChangedEventArgs e)
        {
            UpdateDemoNavigation(selectFirst: false);
        }

        private void UpdateDemoNavigation(bool selectFirst)
        {
            var isBasicMenu = MenuCategorySegmented.SelectedIndex == 1;
            var searchText = MenuSearchTextBox.Text ?? string.Empty;
            var items = _demoPages.Keys
                .Where(key => key != "Home")
                .Where(key => BasicMenuItems.Contains(key) == isBasicMenu)
                .Select(key => new DemoNavItem(key, DemoIconNames[key]))
                .Where(item => item.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                    item.DisplayName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();
            DemoNavListBox.ItemsSource = items;
            DemoNavListBox.SelectedItem = selectFirst
                ? items.FirstOrDefault()
                : items.FirstOrDefault(item => item.Name == _selectedDemoName);
        }

        private void DemoNavListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var navListBox = sender as ListBox ?? this.FindControl<ListBox>("DemoNavListBox");
            if (navListBox?.SelectedItem is not DemoNavItem selectedItem)
            {
                return;
            }

            ShowDemoPage(selectedItem.Name);
        }

        private void ShowDemoPage(string name)
        {
            var host = this.FindControl<ContentControl>("DemoPageHost");
            if (host == null)
            {
                return;
            }

            if (_demoPages.TryGetValue(name, out var createPage))
            {
                if (_selectedDemoName == name)
                {
                    return;
                }

                _selectedDemoName = name;
                HomeButton.Classes.Set("selected", name == "Home");
                CodeDrawer.IsOpen = false;
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

        private void ViewSourceButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_selectedDemoName is not { } selectedDemoName)
            {
                return;
            }

            var pageName = $"{selectedDemoName}Demo";
            XamlSource.Text = ReadSource($"{pageName}.axaml");
            CSharpSource.Text = ReadSource($"{pageName}.axaml.cs");
            if (selectedDemoName == "Home")
            {
                XamlSource.Text += $"{Environment.NewLine}{Environment.NewLine}<!-- ThemeDemo.axaml -->{Environment.NewLine}{ReadSource("ThemeDemo.axaml")}";
                CSharpSource.Text += $"{Environment.NewLine}{Environment.NewLine}// ThemeDemo.axaml.cs{Environment.NewLine}{ReadSource("ThemeDemo.axaml.cs")}";
            }
            else if (selectedDemoName == "Window")
            {
                XamlSource.Text += $"{Environment.NewLine}{Environment.NewLine}<!-- WindowSample.axaml -->{Environment.NewLine}{ReadSource("WindowSample.axaml")}";
                CSharpSource.Text += $"{Environment.NewLine}{Environment.NewLine}// WindowSample.axaml.cs{Environment.NewLine}{ReadSource("WindowSample.axaml.cs")}";
            }
            UpdateSourceTitle();
            SourceTabs.SelectedIndex = 0;
            CodeDrawer.IsOpen = true;
        }

        private void CloseSourceButton_Click(object? sender, RoutedEventArgs e)
        {
            CodeDrawer.IsOpen = false;
        }

        private void UpdateSourceTitle()
        {
            if (_selectedDemoName is { } name)
                SourceTitle.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    DemoLanguageManager.Instance["SourceCodeTitle"],
                    DemoLanguageManager.Instance.TranslateText(name));
        }

        protected override async void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
            if (e.Cancel || _isExitConfirmed)
                return;

            e.Cancel = true;
            if (_isExitConfirmationOpen)
                return;

            _isExitConfirmationOpen = true;
            try
            {
                var result = await MessageBox.Show(
                    DemoLanguageManager.Instance["ExitConfirmation"],
                    DemoLanguageManager.Instance["ExitCaption"],
                    MessageBoxButton.YesNo,
                    this);
                if (result == MessageBoxResult.Yes)
                {
                    _isExitConfirmed = true;
                    Close();
                }
            }
            finally
            {
                _isExitConfirmationOpen = false;
                _isExitConfirmed = false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            DemoLanguageManager.Instance.PropertyChanged -= OnLanguageChanged;
            if (DemoPageHost.Content is IDisposable disposable)
                disposable.Dispose();

            base.OnClosed(e);
        }

        private static string ReadSource(string fileName)
        {
            var resourceName = $"WPFDevelopers.Demo.DemoPages.{fileName}";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
                ?? throw new FileNotFoundException($"Embedded demo source '{resourceName}' was not found.");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        private void TrayShow_Click(object? sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void TrayExit_Click(object? sender, EventArgs e)
        {
            TrayShow_Click(sender, e);
            Dispatcher.UIThread.Post(Close, DispatcherPriority.Background);
        }
    }

    public class DemoNavItem
    {
        public DemoNavItem(string name, string iconName)
        {
            Name = name;
            IconPath = $"/Assets/MenuIcons/{iconName}.svg";
        }

        public string Name { get; }
        public string DisplayName => DemoLanguageManager.Instance.TranslateText(Name);
        public string IconPath { get; }
    }

    public sealed class SvgBrushToCssConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value is not SolidColorBrush brush)
            {
                return null;
            }

            var color = brush.Color;
            return $"path {{ stroke: #{color.R:X2}{color.G:X2}{color.B:X2}; }}";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
