using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ShapePath = Avalonia.Controls.Shapes.Path;

namespace WPFDevelopers.Avalonia.Windows
{
    public enum TitleBarMode
    {
        Normal,
        High,
        None
    }

    [TemplatePart("PART_MinimizeButton", typeof(Button))]
    [TemplatePart("PART_RestoreButton", typeof(Button))]
    [TemplatePart("PART_CloseButton", typeof(Button))]
    [TemplatePart("PART_TitleBarIcon", typeof(Image))]
    [TemplatePart("PART_ThemeSwitchButton", typeof(Button))]
    [TemplatePart("PART_ThemePopup", typeof(Popup))]
    [PseudoClasses(":maximized", ":fullscreen")]
    public class Window : global::Avalonia.Controls.Window
    {
        public static readonly StyledProperty<double> TitleHeightProperty =
            AvaloniaProperty.Register<Window, double>(nameof(TitleHeight), 32,
                validate: value => double.IsFinite(value) && value >= 0);

        public static readonly StyledProperty<IBrush?> TitleBackgroundProperty =
            AvaloniaProperty.Register<Window, IBrush?>(nameof(TitleBackground));

        public static readonly StyledProperty<object?> TitleBarLayoutProperty =
            AvaloniaProperty.Register<Window, object?>(nameof(TitleBarLayout));

        public static readonly StyledProperty<object?> TitleBarProperty =
            AvaloniaProperty.Register<Window, object?>(nameof(TitleBar));

        public static readonly StyledProperty<TitleBarMode> TitleBarModeProperty =
            AvaloniaProperty.Register<Window, TitleBarMode>(nameof(TitleBarMode));

        public static readonly StyledProperty<bool> ShowThemeProperty =
            AvaloniaProperty.Register<Window, bool>(nameof(ShowTheme));

        private Button? _minimizeButton;
        private Button? _restoreButton;
        private Button? _closeButton;
        private Button? _themeSwitchButton;
        private Button? _systemThemeButton;
        private Button? _lightThemeButton;
        private Button? _darkThemeButton;
        private PathIcon? _systemThemeCheck;
        private PathIcon? _lightThemeCheck;
        private PathIcon? _darkThemeCheck;
        private ShapePath? _themeGlyph;
        private Popup? _themePopup;
        private Control? _themePopupChild;
        private DispatcherTimer? _themeCloseTimer;
        private Image? _titleBarIcon;
        private Bitmap? _iconBitmap;

        public Window()
        {
            if (OperatingSystem.IsWindows())
            {
                SetCurrentValue(ExtendClientAreaToDecorationsHintProperty, true);
                SetCurrentValue(WindowDecorationsProperty, WindowDecorations.BorderOnly);
                SetCurrentValue(ExtendClientAreaTitleBarHeightHintProperty, TitleHeight);
            }
        }

        protected override Type StyleKeyOverride => OperatingSystem.IsWindows()
            ? typeof(Window)
            : typeof(global::Avalonia.Controls.Window);

        public double TitleHeight
        {
            get => GetValue(TitleHeightProperty);
            set => SetValue(TitleHeightProperty, value);
        }

        public IBrush? TitleBackground
        {
            get => GetValue(TitleBackgroundProperty);
            set => SetValue(TitleBackgroundProperty, value);
        }

        public object? TitleBarLayout
        {
            get => GetValue(TitleBarLayoutProperty);
            set => SetValue(TitleBarLayoutProperty, value);
        }

        public object? TitleBar
        {
            get => GetValue(TitleBarProperty);
            set => SetValue(TitleBarProperty, value);
        }

        public TitleBarMode TitleBarMode
        {
            get => GetValue(TitleBarModeProperty);
            set => SetValue(TitleBarModeProperty, value);
        }

        public bool ShowTheme
        {
            get => GetValue(ShowThemeProperty);
            set => SetValue(ShowThemeProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            DetachCaptionButtons();
            DetachThemeMenu();
            if (_titleBarIcon != null)
                _titleBarIcon.PointerPressed -= OnIconPointerPressed;

            base.OnApplyTemplate(e);

            _minimizeButton = e.NameScope.Find<Button>("PART_MinimizeButton");
            _restoreButton = e.NameScope.Find<Button>("PART_RestoreButton");
            _closeButton = e.NameScope.Find<Button>("PART_CloseButton");
            _titleBarIcon = e.NameScope.Find<Image>("PART_TitleBarIcon");
            _themeSwitchButton = e.NameScope.Find<Button>("PART_ThemeSwitchButton");
            _themePopup = e.NameScope.Find<Popup>("PART_ThemePopup");
            _systemThemeButton = e.NameScope.Find<Button>("PART_SystemThemeButton");
            _lightThemeButton = e.NameScope.Find<Button>("PART_LightThemeButton");
            _darkThemeButton = e.NameScope.Find<Button>("PART_DarkThemeButton");
            _systemThemeCheck = e.NameScope.Find<PathIcon>("PART_SystemThemeCheck");
            _lightThemeCheck = e.NameScope.Find<PathIcon>("PART_LightThemeCheck");
            _darkThemeCheck = e.NameScope.Find<PathIcon>("PART_DarkThemeCheck");
            _themeGlyph = e.NameScope.Find<ShapePath>("PART_ThemeGlyph");
            if (OperatingSystem.IsWindows())
            {
                if (_minimizeButton != null)
                    _minimizeButton.Click += OnMinimizeClick;
                if (_restoreButton != null)
                    _restoreButton.Click += OnRestoreClick;
                if (_closeButton != null)
                    _closeButton.Click += OnCloseClick;
                if (_titleBarIcon != null)
                    _titleBarIcon.PointerPressed += OnIconPointerPressed;
                UpdateIcon();
            }
            AttachThemeMenu();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ShowThemeProperty || change.Property == TitleBarModeProperty ||
                change.Property == WindowStateProperty || change.Property == IsVisibleProperty ||
                change.Property == IsEnabledProperty)
            {
                if (!ShowTheme || TitleBarMode == TitleBarMode.None || WindowState == WindowState.FullScreen ||
                    !IsVisible || !IsEnabled)
                    CloseThemeMenu();
            }

            if (!OperatingSystem.IsWindows())
                return;

            if (change.Property == TitleHeightProperty)
                SetCurrentValue(ExtendClientAreaTitleBarHeightHintProperty, TitleHeight);
            else if (change.Property == IconProperty)
                UpdateIcon();
            else if (change.Property == WindowStateProperty)
            {
                PseudoClasses.Set(":maximized", WindowState == WindowState.Maximized);
                PseudoClasses.Set(":fullscreen", WindowState == WindowState.FullScreen);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            DetachCaptionButtons();
            DetachThemeMenu();
            if (_titleBarIcon != null)
            {
                _titleBarIcon.PointerPressed -= OnIconPointerPressed;
                _titleBarIcon.Source = null;
            }
            _iconBitmap?.Dispose();
            _iconBitmap = null;
            base.OnClosed(e);
        }

        private void UpdateIcon()
        {
            if (_titleBarIcon == null)
                return;

            Bitmap? bitmap = null;
            if (Icon != null)
            {
                using var stream = new MemoryStream();
                Icon.Save(stream);
                stream.Position = 0;
                bitmap = new Bitmap(stream);
            }

            _titleBarIcon.Source = bitmap;
            _titleBarIcon.IsVisible = bitmap != null;
            _iconBitmap?.Dispose();
            _iconBitmap = bitmap;
        }

        private void DetachCaptionButtons()
        {
            if (_minimizeButton != null)
                _minimizeButton.Click -= OnMinimizeClick;
            if (_restoreButton != null)
                _restoreButton.Click -= OnRestoreClick;
            if (_closeButton != null)
                _closeButton.Click -= OnCloseClick;
        }

        private void AttachThemeMenu()
        {
            if (_themeSwitchButton == null || _themePopup == null)
                return;

            _themePopup.PlacementTarget = _themeSwitchButton;
            _themeSwitchButton.Click += OnThemeSwitchClick;
            _themeSwitchButton.KeyDown += OnThemeSwitchKeyDown;
            _themeSwitchButton.PointerEntered += OnThemePointerEntered;
            _themeSwitchButton.PointerExited += OnThemePointerExited;
            _themeSwitchButton.PropertyChanged += OnThemeButtonPropertyChanged;
            _themePopupChild = _themePopup.Child;
            if (_themePopupChild != null)
            {
                _themePopupChild.PointerEntered += OnThemePointerEntered;
                _themePopupChild.PointerExited += OnThemePointerExited;
            }
            _themeCloseTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(150) };
            _themeCloseTimer.Tick += OnThemeCloseTimer;
            Deactivated += OnThemeWindowDeactivated;
            if (_systemThemeButton != null)
            {
                _systemThemeButton.Click += OnSystemThemeClick;
                _systemThemeButton.KeyDown += OnThemeMenuKeyDown;
            }
            if (_lightThemeButton != null)
            {
                _lightThemeButton.Click += OnLightThemeClick;
                _lightThemeButton.KeyDown += OnThemeMenuKeyDown;
            }
            if (_darkThemeButton != null)
            {
                _darkThemeButton.Click += OnDarkThemeClick;
                _darkThemeButton.KeyDown += OnThemeMenuKeyDown;
            }

            ThemeManager.Instance.ThemeModeChanged += SynchronizeThemeMenu;
            SynchronizeThemeMenu(ThemeManager.Instance.Theme);
        }

        private void DetachThemeMenu()
        {
            CloseThemeMenu();
            if (_themeCloseTimer != null)
            {
                _themeCloseTimer.Tick -= OnThemeCloseTimer;
                _themeCloseTimer = null;
            }
            Deactivated -= OnThemeWindowDeactivated;
            if (_themePopupChild != null)
            {
                _themePopupChild.PointerEntered -= OnThemePointerEntered;
                _themePopupChild.PointerExited -= OnThemePointerExited;
                _themePopupChild = null;
            }
            if (_themeSwitchButton != null)
            {
                _themeSwitchButton.Click -= OnThemeSwitchClick;
                _themeSwitchButton.KeyDown -= OnThemeSwitchKeyDown;
                _themeSwitchButton.PointerEntered -= OnThemePointerEntered;
                _themeSwitchButton.PointerExited -= OnThemePointerExited;
                _themeSwitchButton.PropertyChanged -= OnThemeButtonPropertyChanged;
            }
            if (_systemThemeButton != null)
            {
                _systemThemeButton.Click -= OnSystemThemeClick;
                _systemThemeButton.KeyDown -= OnThemeMenuKeyDown;
            }
            if (_lightThemeButton != null)
            {
                _lightThemeButton.Click -= OnLightThemeClick;
                _lightThemeButton.KeyDown -= OnThemeMenuKeyDown;
            }
            if (_darkThemeButton != null)
            {
                _darkThemeButton.Click -= OnDarkThemeClick;
                _darkThemeButton.KeyDown -= OnThemeMenuKeyDown;
            }

            ThemeManager.Instance.ThemeModeChanged -= SynchronizeThemeMenu;
        }

        private void OnThemeSwitchClick(object? sender, RoutedEventArgs e)
        {
            OpenThemeMenu(_themeSwitchButton?.IsPointerOver != true);
            e.Handled = true;
        }

        private void OpenThemeMenu(bool focusSelected = false)
        {
            if (_themePopup == null || _themeSwitchButton?.IsEffectivelyVisible != true ||
                !_themeSwitchButton.IsEffectivelyEnabled)
                return;

            _themeCloseTimer?.Stop();
            _themePopup.IsOpen = true;
            if (focusSelected)
                Dispatcher.UIThread.Post(() =>
                {
                    if (_themePopup?.IsOpen == true)
                        GetSelectedThemeButton()?.Focus();
                }, DispatcherPriority.Input);
        }

        private void CloseThemeMenu()
        {
            _themeCloseTimer?.Stop();
            if (_themePopup != null)
                _themePopup.IsOpen = false;
        }

        private void OnThemePointerEntered(object? sender, PointerEventArgs e)
        {
            _themeCloseTimer?.Stop();
            if (sender == _themeSwitchButton)
                OpenThemeMenu();
        }

        private void OnThemePointerExited(object? sender, PointerEventArgs e) => _themeCloseTimer?.Start();

        private void OnThemeCloseTimer(object? sender, EventArgs e)
        {
            _themeCloseTimer?.Stop();
            if (_themeSwitchButton?.IsPointerOver != true && _themePopupChild?.IsPointerOver != true)
                CloseThemeMenu();
        }

        private void OnThemeButtonPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (_themeSwitchButton?.IsEffectivelyVisible != true ||
                _themeSwitchButton?.IsEffectivelyEnabled != true)
                CloseThemeMenu();
        }

        private void OnThemeWindowDeactivated(object? sender, EventArgs e) => CloseThemeMenu();

        private void OnThemeSwitchKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                OpenThemeMenu(focusSelected: true);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape && _themePopup?.IsOpen == true)
            {
                CloseThemeMenu();
                e.Handled = true;
            }
        }

        private void OnThemeMenuKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseThemeMenu();
                _themeSwitchButton?.Focus();
                e.Handled = true;
                return;
            }

            if (e.Key is not (Key.Down or Key.Up))
                return;

            var buttons = new[] { _systemThemeButton, _lightThemeButton, _darkThemeButton };
            var index = Array.IndexOf(buttons, sender);
            if (index >= 0)
            {
                var offset = e.Key == Key.Down ? 1 : buttons.Length - 1;
                buttons[(index + offset) % buttons.Length]?.Focus();
                e.Handled = true;
            }
        }

        private void OnSystemThemeClick(object? sender, RoutedEventArgs e) => SetTheme(ThemeMode.System, e);

        private void OnLightThemeClick(object? sender, RoutedEventArgs e) => SetTheme(ThemeMode.Light, e);

        private void OnDarkThemeClick(object? sender, RoutedEventArgs e) => SetTheme(ThemeMode.Dark, e);

        private void SetTheme(ThemeMode theme, RoutedEventArgs e)
        {
            CloseThemeMenu();
            ThemeManager.Instance.Theme = theme;
            e.Handled = true;
        }

        private Button? GetSelectedThemeButton() => ThemeManager.Instance.Theme switch
        {
            ThemeMode.System => _systemThemeButton,
            ThemeMode.Light => _lightThemeButton,
            ThemeMode.Dark => _darkThemeButton,
            _ => throw new ArgumentOutOfRangeException(nameof(ThemeManager.Instance.Theme))
        };

        private void SynchronizeThemeMenu(ThemeMode mode)
        {
            if (_themeGlyph != null)
            {
                var resourceKey = mode switch
                {
                    ThemeMode.System => "WD.SystemThemeGeometry",
                    ThemeMode.Light => "WD.SunGeometry",
                    ThemeMode.Dark => "WD.MoonGeometry",
                    _ => throw new ArgumentOutOfRangeException(nameof(mode))
                };
                _themeGlyph.Data = this.FindResource(resourceKey) as Geometry
                    ?? throw new InvalidOperationException($"Missing theme icon resource '{resourceKey}'.");
                _themeGlyph.StrokeThickness = mode == ThemeMode.Light ? 0 : 1.1;
                _themeGlyph.Classes.Set("light", mode == ThemeMode.Light);
            }
            _systemThemeButton?.Classes.Set("selected", mode == ThemeMode.System);
            _lightThemeButton?.Classes.Set("selected", mode == ThemeMode.Light);
            _darkThemeButton?.Classes.Set("selected", mode == ThemeMode.Dark);
            if (_systemThemeCheck != null)
                _systemThemeCheck.IsVisible = mode == ThemeMode.System;
            if (_lightThemeCheck != null)
                _lightThemeCheck.IsVisible = mode == ThemeMode.Light;
            if (_darkThemeCheck != null)
                _darkThemeCheck.IsVisible = mode == ThemeMode.Dark;
        }

        private void OnMinimizeClick(object? sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Handled = true;
        }

        private void OnRestoreClick(object? sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            e.Handled = true;
        }

        private void OnCloseClick(object? sender, RoutedEventArgs e)
        {
            Close();
            e.Handled = true;
        }

        private void OnIconPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (e.ClickCount == 2 && e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                Close();
                e.Handled = true;
            }
        }
    }
}
