using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace WPFDevelopers.Avalonia.Controls
{
    public class Dialog : ContentControl
    {
        public static readonly StyledProperty<bool> IsOpenProperty =
            AvaloniaProperty.Register<Dialog, bool>(nameof(IsOpen), defaultBindingMode: BindingMode.TwoWay);
        public static readonly StyledProperty<ICommand?> ClosedCommandProperty =
            AvaloniaProperty.Register<Dialog, ICommand?>(nameof(ClosedCommand));
        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<Dialog, string>(nameof(Title), "Dialog");
        public static readonly StyledProperty<Window?> OwnerProperty =
            AvaloniaProperty.Register<Dialog, Window?>(nameof(Owner));
        public static readonly StyledProperty<double> DialogWidthProperty =
            AvaloniaProperty.Register<Dialog, double>(nameof(DialogWidth), 440, validate: IsValidDimension);
        public static readonly StyledProperty<double> DialogHeightProperty =
            AvaloniaProperty.Register<Dialog, double>(nameof(DialogHeight), double.NaN, validate: IsValidDimension);
        public static readonly StyledProperty<bool> ShowFooterProperty =
            AvaloniaProperty.Register<Dialog, bool>(nameof(ShowFooter), true);
        public static readonly StyledProperty<string?> ConfirmTextProperty =
            AvaloniaProperty.Register<Dialog, string?>(nameof(ConfirmText));
        public static readonly StyledProperty<string?> CancelTextProperty =
            AvaloniaProperty.Register<Dialog, string?>(nameof(CancelText));
        public static readonly StyledProperty<bool> IsConfirmEnabledProperty =
            AvaloniaProperty.Register<Dialog, bool>(nameof(IsConfirmEnabled), true);

        private static readonly Dictionary<Window, Dialog> OpenDialogs = new();
        private OverlayLayer? _layer;
        private DialogOverlay? _overlay;
        private Window? _owner;
        private Control? _background;
        private bool _backgroundEnabled;
        private IInputElement? _previousFocus;
        private TaskCompletionSource<DialogResult>? _completion;
        private bool _changingIsOpen;
        private bool _closing;
        private Button? _confirmButton;
        private Button? _cancelButton;
        private Button? _closeButton;

        static Dialog()
        {
            ContentProperty.OverrideMetadata<Dialog>(new StyledPropertyMetadata<object?>(
                coerce: (target, value) =>
                {
                    var dialog = (Dialog)target;
                    if (dialog._overlay != null && !ReferenceEquals(dialog.Content, value))
                        throw new InvalidOperationException("Dialog.Content cannot be changed while the dialog is open.");
                    if (value != null && value is not UserControl)
                        throw new ArgumentException("Dialog.Content must be a UserControl.");
                    return value;
                }));
        }

        public Dialog()
        {
            Focusable = true;
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Cycle);
        }

        public event EventHandler<DialogClosingEventArgs>? Closing;
        public event EventHandler? Closed;

        public bool IsOpen { get => GetValue(IsOpenProperty); set => SetValue(IsOpenProperty, value); }
        public ICommand? ClosedCommand { get => GetValue(ClosedCommandProperty); set => SetValue(ClosedCommandProperty, value); }
        public string Title { get => GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public Window? Owner { get => GetValue(OwnerProperty); set => SetValue(OwnerProperty, value); }
        public double DialogWidth { get => GetValue(DialogWidthProperty); set => SetValue(DialogWidthProperty, value); }
        public double DialogHeight { get => GetValue(DialogHeightProperty); set => SetValue(DialogHeightProperty, value); }
        public bool ShowFooter { get => GetValue(ShowFooterProperty); set => SetValue(ShowFooterProperty, value); }
        public string? ConfirmText { get => GetValue(ConfirmTextProperty); set => SetValue(ConfirmTextProperty, value); }
        public string? CancelText { get => GetValue(CancelTextProperty); set => SetValue(CancelTextProperty, value); }
        public bool IsConfirmEnabled { get => GetValue(IsConfirmEnabledProperty); set => SetValue(IsConfirmEnabledProperty, value); }

        public Task<DialogResult> ShowAsync(Window? owner = null)
        {
            Dispatcher.UIThread.VerifyAccess();
            if (_overlay != null)
                throw new InvalidOperationException("This Dialog is already open.");
            if (Content is not UserControl content)
                throw new InvalidOperationException("Dialog.Content must be a UserControl.");
            if (Parent != null || this.GetVisualParent() != null)
                throw new InvalidOperationException("Dialog must not be attached to another container.");
            if ((content.Parent != null && !ReferenceEquals(content.Parent, this)) ||
                (content.GetVisualParent() != null && !content.GetVisualAncestors().Contains(this)))
                throw new InvalidOperationException("Dialog.Content is already attached to another container.");

            owner ??= Owner ?? FindOwner();
            if (owner == null || !owner.IsVisible)
                throw new InvalidOperationException("Dialog requires a visible owner Window.");
            if (owner.Content is not Control background)
                throw new InvalidOperationException("The owner Window must have a Control as its content.");
            var layer = OverlayLayer.GetOverlayLayer(owner)
                ?? throw new InvalidOperationException("The owner Window requires an OverlayLayer.");
            if (OpenDialogs.ContainsKey(owner))
                throw new InvalidOperationException("The owner Window already has an open Dialog.");

            _owner = owner;
            _background = background;
            _backgroundEnabled = background.IsEnabled;
            _previousFocus = owner.FocusManager?.GetFocusedElement();
            _layer = layer;
            _completion = new TaskCompletionSource<DialogResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            var task = _completion.Task;
            try
            {
                _overlay = new DialogOverlay(this);
                OpenDialogs.Add(owner, this);
                background.DetachedFromVisualTree += OnBackgroundDetached;
                background.PropertyChanged += OnBackgroundPropertyChanged;
                owner.Closed += OnOwnerClosed;
                owner.AddHandler(KeyDownEvent, OnOwnerKeyDown, RoutingStrategies.Tunnel);
                owner.AddHandler(GotFocusEvent, OnOwnerGotFocus, RoutingStrategies.Tunnel);
                background.SetCurrentValue(IsEnabledProperty, false);
                layer.Children.Add(_overlay);
                SetOpen(true);
                Dispatcher.UIThread.Post(() =>
                {
                    if (_overlay != null)
                        FocusFirst();
                }, DispatcherPriority.Input);
                return task;
            }
            catch
            {
                Cleanup();
                throw;
            }
        }

        public void Close(DialogCloseReason reason = DialogCloseReason.Cancel)
        {
            Dispatcher.UIThread.VerifyAccess();
            if (!Enum.IsDefined(reason))
                throw new ArgumentOutOfRangeException(nameof(reason));
            if (_overlay == null)
                throw new InvalidOperationException("This Dialog is not open.");
            if (reason == DialogCloseReason.Confirm && !IsConfirmEnabled)
                throw new InvalidOperationException("Confirmation is disabled.");
            if (_closing)
                throw new InvalidOperationException("A dialog close operation is already in progress.");

            _closing = true;
            try
            {
                var valid = reason != DialogCloseReason.Confirm || UpdateSourcesAndValidate();
                var args = new DialogClosingEventArgs(reason) { Cancel = !valid };
                Closing?.Invoke(this, args);
                if (valid && !args.Cancel && _overlay != null)
                    Complete(reason == DialogCloseReason.Confirm ? DialogResult.Confirmed : DialogResult.Canceled);
            }
            finally
            {
                _closing = false;
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            if (_confirmButton != null) _confirmButton.Click -= OnConfirm;
            if (_cancelButton != null) _cancelButton.Click -= OnCancel;
            if (_closeButton != null) _closeButton.Click -= OnCloseButton;
            base.OnApplyTemplate(e);
            _confirmButton = e.NameScope.Find<Button>("PART_ConfirmButton");
            _cancelButton = e.NameScope.Find<Button>("PART_CancelButton");
            _closeButton = e.NameScope.Find<Button>("PART_CloseButton");
            if (_confirmButton != null) _confirmButton.Click += OnConfirm;
            if (_cancelButton != null) _cancelButton.Click += OnCancel;
            if (_closeButton != null) _closeButton.Click += OnCloseButton;
            UpdateTextClasses();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == IsOpenProperty && !_changingIsOpen)
            {
                if (IsOpen && _overlay == null)
                {
                    try { _ = ShowAsync(); }
                    catch { SetOpen(false); throw; }
                }
                else if (!IsOpen && _overlay != null)
                {
                    try { Close(); }
                    finally { if (_overlay != null) SetOpen(true); }
                }
            }
            else if (change.Property == DialogWidthProperty || change.Property == DialogHeightProperty)
                _overlay?.InvalidateMeasure();
            else if (change.Property == ConfirmTextProperty || change.Property == CancelTextProperty)
                UpdateTextClasses();
        }

        private static bool IsValidDimension(double value) => double.IsNaN(value) || (value > 0 && double.IsFinite(value));

        private static Window? FindOwner()
        {
            if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                return null;
            return desktop.Windows.FirstOrDefault(window => window.IsActive) ?? desktop.MainWindow;
        }

        private void UpdateTextClasses()
        {
            PseudoClasses.Set(":custom-confirm", ConfirmText != null);
            PseudoClasses.Set(":custom-cancel", CancelText != null);
        }

        private void SetOpen(bool value)
        {
            _changingIsOpen = true;
            try { SetCurrentValue(IsOpenProperty, value); }
            finally { _changingIsOpen = false; }
        }

        private void OnConfirm(object? sender, RoutedEventArgs e) => Close(DialogCloseReason.Confirm);
        private void OnCancel(object? sender, RoutedEventArgs e) => Close(DialogCloseReason.Cancel);
        private void OnCloseButton(object? sender, RoutedEventArgs e) => Close(DialogCloseReason.CloseButton);

        private void OnOwnerKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close(DialogCloseReason.Escape);
                e.Handled = true;
            }
        }

        private void OnOwnerGotFocus(object? sender, FocusChangedEventArgs e)
        {
            if (e.Source is Visual visual && !ReferenceEquals(visual, this) &&
                !visual.GetVisualAncestors().Contains(this))
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (_overlay != null)
                        FocusFirst();
                }, DispatcherPriority.Input);
            }
        }

        private void FocusFirst()
        {
            var first = this.GetVisualDescendants().OfType<Control>()
                .FirstOrDefault(control => control.Focusable && control.IsEffectivelyEnabled && control.IsEffectivelyVisible);
            if (first?.Focus() != true)
                Focus();
        }

        private bool UpdateSourcesAndValidate()
        {
            var controls = this.GetVisualDescendants().OfType<Control>().Prepend(this).ToArray();
            foreach (var control in controls)
            {
                foreach (var property in AvaloniaPropertyRegistry.Instance.GetRegistered(control))
                    BindingOperations.GetBindingExpressionBase(control, property)?.UpdateSource();
            }
            return controls.All(control => !control.GetValue(DataValidationErrors.HasErrorsProperty));
        }

        private void OnBackgroundDetached(object? sender, VisualTreeAttachmentEventArgs e)
        {
            if (_overlay != null)
                Complete(DialogResult.Canceled);
        }

        private void OnBackgroundPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == IsEnabledProperty && _background?.IsEnabled == true)
                _background.SetCurrentValue(IsEnabledProperty, false);
        }

        private void OnOwnerClosed(object? sender, EventArgs e)
        {
            if (_overlay != null)
                Complete(DialogResult.Canceled);
        }

        private void Complete(DialogResult result)
        {
            var completion = _completion!;
            Cleanup();
            try
            {
                Closed?.Invoke(this, EventArgs.Empty);
                if (ClosedCommand?.CanExecute(result) == true)
                    ClosedCommand.Execute(result);
            }
            finally { completion.TrySetResult(result); }
        }

        private void Cleanup()
        {
            var owner = _owner!;
            var background = _background!;
            background.DetachedFromVisualTree -= OnBackgroundDetached;
            background.PropertyChanged -= OnBackgroundPropertyChanged;
            owner.Closed -= OnOwnerClosed;
            owner.RemoveHandler(KeyDownEvent, OnOwnerKeyDown);
            owner.RemoveHandler(GotFocusEvent, OnOwnerGotFocus);
            if (_overlay != null)
            {
                _layer?.Children.Remove(_overlay);
                _overlay.Children.Clear();
            }
            background.SetCurrentValue(IsEnabledProperty, _backgroundEnabled);
            BindingOperations.GetBindingExpressionBase(background, IsEnabledProperty)?.UpdateTarget();
            OpenDialogs.Remove(owner);
            _overlay = null;
            _layer = null;
            _owner = null;
            _background = null;
            _completion = null;
            SetOpen(false);
            if (owner.IsVisible && _previousFocus is Control previous && previous.IsEffectivelyVisible && previous.IsEffectivelyEnabled)
                previous.Focus();
            _previousFocus = null;
        }

        private sealed class DialogOverlay : Panel
        {
            private readonly Dialog _dialog;
            private readonly Border _mask;

            public DialogOverlay(Dialog dialog)
            {
                _dialog = dialog;
                _mask = new Border { Background = new SolidColorBrush(Color.FromArgb(110, 0, 0, 0)) };
                HorizontalAlignment = HorizontalAlignment.Stretch;
                VerticalAlignment = VerticalAlignment.Stretch;
                Children.Add(_mask);
                Children.Add(dialog);
            }

            protected override Size MeasureOverride(Size availableSize)
            {
                _mask.Measure(availableSize);
                var width = double.IsNaN(_dialog.DialogWidth) ? availableSize.Width : Math.Min(availableSize.Width, _dialog.DialogWidth);
                var height = double.IsNaN(_dialog.DialogHeight) ? availableSize.Height : Math.Min(availableSize.Height, _dialog.DialogHeight);
                _dialog.Measure(new Size(width, height));
                return availableSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                _mask.Arrange(new Rect(finalSize));
                var width = double.IsNaN(_dialog.DialogWidth) ? Math.Max(240, _dialog.DesiredSize.Width) : _dialog.DialogWidth;
                var height = double.IsNaN(_dialog.DialogHeight) ? Math.Max(120, _dialog.DesiredSize.Height) : _dialog.DialogHeight;
                width = Math.Min(width, finalSize.Width);
                height = Math.Min(height, finalSize.Height);
                _dialog.Arrange(new Rect((finalSize.Width - width) / 2, (finalSize.Height - height) / 2, width, height));
                return finalSize;
            }
        }
    }
}
