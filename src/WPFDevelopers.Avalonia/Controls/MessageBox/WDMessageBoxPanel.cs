using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace WPFDevelopers.Avalonia.Controls
{
    public class WDMessageBoxPanel : TemplatedControl
    {
        private TextBlock? _titleText;
        private TextBlock? _messageText;
        private Button? _closeButton;
        private Button? _buttonCancel;
        private Button? _buttonOK;
        private Button? _buttonYes;
        private Button? _buttonNo;
        private PathIcon? _pathIcon;

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<WDMessageBoxPanel, string>(nameof(Title));

        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<WDMessageBoxPanel, string>(nameof(Message));

        public string Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly StyledProperty<MessageBoxButton> ButtonsProperty =
            AvaloniaProperty.Register<WDMessageBoxPanel, MessageBoxButton>(nameof(Buttons));

        public MessageBoxButton Buttons
        {
            get => GetValue(ButtonsProperty);
            set => SetValue(ButtonsProperty, value);
        }

        public static readonly StyledProperty<MessageBoxImage> IconProperty =
            AvaloniaProperty.Register<WDMessageBoxPanel, MessageBoxImage>(nameof(Icon));

        public MessageBoxImage Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly StyledProperty<bool> IsDefaultProperty =
            AvaloniaProperty.Register<WDMessageBoxPanel, bool>(nameof(IsDefault), defaultValue: true);

        public bool IsDefault
        {
            get => GetValue(IsDefaultProperty);
            set => SetValue(IsDefaultProperty, value);
        }

        public MessageBoxResult Result { get; private set; }

        public event EventHandler<MessageBoxResult>? Finished;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _titleText = e.NameScope.Find<TextBlock>("PART_Title");
            _messageText = e.NameScope.Find<TextBlock>("PART_Message");
            _pathIcon = e.NameScope.Find<PathIcon>("PART_Path");

            UpdateContent();

            _closeButton = e.NameScope.Find<Button>("PART_CloseButton");
            if (_closeButton != null)
                _closeButton.Click += CloseButton_Click;

            _buttonOK = e.NameScope.Find<Button>("PART_ButtonOK");
            if (_buttonOK != null)
                _buttonOK.Click += ButtonOK_Click;

            _buttonCancel = e.NameScope.Find<Button>("PART_ButtonCancel");
            if (_buttonCancel != null)
                _buttonCancel.Click += ButtonCancel_Click;

            _buttonYes = e.NameScope.Find<Button>("PART_ButtonYes");
            if (_buttonYes != null)
                _buttonYes.Click += ButtonYes_Click;

            _buttonNo = e.NameScope.Find<Button>("PART_ButtonNo");
            if (_buttonNo != null)
                _buttonNo.Click += ButtonNo_Click;

            ApplyButtons(Buttons);
        }


        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TitleProperty || change.Property == MessageProperty)
                UpdateContent();

            if (change.Property == ButtonsProperty && change.NewValue is MessageBoxButton button)
                ApplyButtons(button);

            if (change.Property == IconProperty && change.NewValue is MessageBoxImage icon)
                ApplyIcon(icon);
        }

        private void UpdateContent()
        {
            if (_titleText != null)
                _titleText.Text = Title;
            if (_messageText != null)
                _messageText.Text = Message;
            if (_pathIcon != null)
                ApplyIcon(Icon);
        }

        private void ApplyIcon(MessageBoxImage icon)
        {
            if (_pathIcon == null || Application.Current == null) return;

            StreamGeometry? geometry = icon switch
            {
                MessageBoxImage.Warning => GetResource<StreamGeometry>("WD.WarningGeometry"),
                MessageBoxImage.Error => GetResource<StreamGeometry>("WD.ErrorGeometry"),
                MessageBoxImage.Information => GetResource<StreamGeometry>("WD.InfoGeometry"),
                MessageBoxImage.Question => GetResource<StreamGeometry>("WD.QuestionGeometry"),
                _ => GetResource<StreamGeometry>("WD.InfoGeometry")
            };

            IBrush? foreground = icon switch
            {
                MessageBoxImage.Warning => GetResource<IBrush>("WD.WarningBrush"),
                MessageBoxImage.Error => GetResource<IBrush>("WD.DangerBrush"),
                MessageBoxImage.Information => GetResource<IBrush>("WD.PrimaryBrush"),
                MessageBoxImage.Question => GetResource<IBrush>("WD.PrimaryBrush"),
                _ => GetResource<IBrush>("WD.PrimaryBrush")
            };

            if (geometry != null)
                _pathIcon.Data = geometry;
            if (foreground != null)
                _pathIcon.Foreground = foreground;
        }

        private T? GetResource<T>(string key) where T : class
        {
            if (Application.Current?.Resources.TryGetResource(key, null, out var value) == true && value is T typed)
                return typed;
            return null;
        }

        private void ApplyButtons(MessageBoxButton button)
        {
            var okVisible = false;
            var cancelVisible = false;
            var yesVisible = false;
            var noVisible = false;

            switch (button)
            {
                case MessageBoxButton.OKCancel:
                    okVisible = true;
                    cancelVisible = true;
                    break;
                case MessageBoxButton.YesNo:
                    yesVisible = true;
                    noVisible = true;
                    break;
                case MessageBoxButton.YesNoCancel:
                    yesVisible = true;
                    noVisible = true;
                    cancelVisible = true;
                    break;
                default:
                    okVisible = true;
                    break;
            }

            if (_buttonOK != null) _buttonOK.IsVisible = okVisible;
            if (_buttonCancel != null) _buttonCancel.IsVisible = cancelVisible;
            if (_buttonYes != null) _buttonYes.IsVisible = yesVisible;
            if (_buttonNo != null) _buttonNo.IsVisible = noVisible;
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            Finish(MessageBoxResult.None);
        }

        private void ButtonOK_Click(object? sender, RoutedEventArgs e)
        {
            Finish(MessageBoxResult.OK);
        }

        private void ButtonCancel_Click(object? sender, RoutedEventArgs e)
        {
            Finish(MessageBoxResult.Cancel);
        }

        private void ButtonYes_Click(object? sender, RoutedEventArgs e)
        {
            Finish(MessageBoxResult.Yes);
        }

        private void ButtonNo_Click(object? sender, RoutedEventArgs e)
        {
            Finish(MessageBoxResult.No);
        }

        private void Finish(MessageBoxResult result)
        {
            Result = result;
            Finished?.Invoke(this, result);
        }
    }
}
