using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;

namespace WPFDevelopers.Avalonia.Controls
{
    [TemplatePart("PART_ToggleButton", typeof(ToggleButton))]
    [TemplatePart("PART_Menu", typeof(ListBox))]
    [TemplatePart("PART_Popup", typeof(Popup))]
    public class SplitButton : Button
    {
        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<SplitButton, IEnumerable?>(nameof(ItemsSource));

        public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
            AvaloniaProperty.Register<SplitButton, IDataTemplate?>(nameof(ItemTemplate));

        public static readonly StyledProperty<bool> IsDropDownOpenProperty =
            AvaloniaProperty.Register<SplitButton, bool>(nameof(IsDropDownOpen));

        public static readonly RoutedEvent<SplitButtonSelectionChangedEventArgs> SelectionChangedEvent =
            RoutedEvent.Register<SplitButton, SplitButtonSelectionChangedEventArgs>(
                nameof(SelectionChanged), RoutingStrategies.Bubble);

        private ToggleButton? _toggleButton;
        private ListBox? _menu;
        private Popup? _popup;

        public IEnumerable? ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IDataTemplate? ItemTemplate
        {
            get => GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public event EventHandler<SplitButtonSelectionChangedEventArgs> SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            if (_toggleButton != null)
                _toggleButton.Click -= OnToggleButtonClick;
            if (_menu != null)
                _menu.SelectionChanged -= OnMenuSelectionChanged;
            if (_popup != null)
                _popup.Closed -= OnPopupClosed;

            base.OnApplyTemplate(e);

            _toggleButton = e.NameScope.Find<ToggleButton>("PART_ToggleButton");
            _menu = e.NameScope.Find<ListBox>("PART_Menu");
            _popup = e.NameScope.Find<Popup>("PART_Popup");

            if (_toggleButton != null)
                _toggleButton.Click += OnToggleButtonClick;
            if (_menu != null)
                _menu.SelectionChanged += OnMenuSelectionChanged;
            if (_popup != null)
                _popup.Closed += OnPopupClosed;
        }

        private void OnToggleButtonClick(object? sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }

        private void OnMenuSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_menu == null || e.AddedItems.Count == 0)
                return;

            var oldValue = Content;
            var newValue = e.AddedItems[0];
            SetCurrentValue(ContentProperty, newValue);
            IsDropDownOpen = false;
            _menu.SelectedIndex = -1;

            RaiseEvent(new SplitButtonSelectionChangedEventArgs(SelectionChangedEvent, this, oldValue, newValue));
        }

        private void OnPopupClosed(object? sender, EventArgs e)
        {
            IsDropDownOpen = false;
        }
    }

    public class SplitButtonSelectionChangedEventArgs : RoutedEventArgs
    {
        public SplitButtonSelectionChangedEventArgs(
            RoutedEvent routedEvent, object? source, object? oldValue, object? newValue)
            : base(routedEvent, source)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        public object? OldValue { get; }
        public object? NewValue { get; }
    }
}
