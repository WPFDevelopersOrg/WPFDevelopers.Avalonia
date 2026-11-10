using System;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Platform;
using Avalonia.Threading;

namespace WPFDevelopers.Avalonia.Controls;

[TemplatePart("PART_ItemsPresenter", typeof(ItemsPresenter))]
[TemplatePart("PART_Dots", typeof(StackPanel))]
[TemplatePart("PART_PrevButton", typeof(Button))]
[TemplatePart("PART_NextButton", typeof(Button))]
public class Carousel : ItemsControl
{
    public static readonly StyledProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<Carousel, int>(nameof(SelectedIndex), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<Carousel, object?>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<bool> AutoPlayProperty =
        AvaloniaProperty.Register<Carousel, bool>(nameof(AutoPlay));

    public static readonly StyledProperty<TimeSpan> AutoPlayIntervalProperty =
        AvaloniaProperty.Register<Carousel, TimeSpan>(nameof(AutoPlayInterval), TimeSpan.FromSeconds(3),
            validate: value => value > TimeSpan.Zero);

    public static readonly StyledProperty<double> AnimationDurationProperty =
        AvaloniaProperty.Register<Carousel, double>(nameof(AnimationDuration), 0.5,
            validate: value => double.IsFinite(value) && value >= 0);

    public static readonly StyledProperty<bool> ShowDotsProperty =
        AvaloniaProperty.Register<Carousel, bool>(nameof(ShowDots), true);

    public static readonly StyledProperty<bool> ShowArrowsProperty =
        AvaloniaProperty.Register<Carousel, bool>(nameof(ShowArrows), true);

    public static readonly StyledProperty<string?> DisplayMemberPathProperty =
        AvaloniaProperty.Register<Carousel, string?>(nameof(DisplayMemberPath));

    public static readonly StyledProperty<ICommand?> ItemClickCommandProperty =
        AvaloniaProperty.Register<Carousel, ICommand?>(nameof(ItemClickCommand));

    public static readonly RoutedEvent<RoutedEventArgs> ItemClickEvent =
        RoutedEvent.Register<Carousel, RoutedEventArgs>(nameof(ItemClick), RoutingStrategies.Bubble);

    private readonly DispatcherTimer _autoPlayTimer = new() { Interval = TimeSpan.FromSeconds(3) };
    private readonly DispatcherTimer _animationTimer = new() { Interval = TimeSpan.FromMilliseconds(16) };
    private ItemsPresenter? _itemsPresenter;
    private StackPanel? _dotsPanel;
    private Button? _prevButton;
    private Button? _nextButton;
    private TranslateTransform _translateTransform = new();
    private INotifyCollectionChanged? _itemsSourceNotifier;
    private DateTime _animationStart;
    private double _animationFrom;
    private double _animationTo;
    private bool _isAttached;
    private bool _suppressNextAnimation;
    private bool _wrapNextAnimation;
    private CarouselPanel? _wrapPanel;

    static Carousel()
    {
        SelectedIndexProperty.Changed.AddClassHandler<Carousel>((control, _) => control.OnSelectedIndexChanged());
        SelectedItemProperty.Changed.AddClassHandler<Carousel>((control, _) => control.OnSelectedItemChanged());
        AutoPlayProperty.Changed.AddClassHandler<Carousel>((control, _) => control.UpdateAutoPlay());
        AutoPlayIntervalProperty.Changed.AddClassHandler<Carousel>((control, _) => control.UpdateAutoPlay());
        ShowDotsProperty.Changed.AddClassHandler<Carousel>((control, _) => control.UpdateDotsVisibility());
        DisplayMemberPathProperty.Changed.AddClassHandler<Carousel>((control, _) => control.RefreshItems());
        ItemsSourceProperty.Changed.AddClassHandler<Carousel>((control, args) =>
            control.OnItemsSourceChanged(args.OldValue, args.NewValue));
    }

    public Carousel()
    {
        ItemsPanel = new FuncTemplate<Panel?>(() => new CarouselPanel());
        _translateTransform = new TranslateTransform();
        _autoPlayTimer.Tick += (_, _) => GoToNext();
        _animationTimer.Tick += OnAnimationTick;
        if (Items is INotifyCollectionChanged items)
            items.CollectionChanged += OnItemsCollectionChanged;
    }

    public int SelectedIndex
    {
        get => GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public bool AutoPlay
    {
        get => GetValue(AutoPlayProperty);
        set => SetValue(AutoPlayProperty, value);
    }

    public TimeSpan AutoPlayInterval
    {
        get => GetValue(AutoPlayIntervalProperty);
        set => SetValue(AutoPlayIntervalProperty, value);
    }

    public double AnimationDuration
    {
        get => GetValue(AnimationDurationProperty);
        set => SetValue(AnimationDurationProperty, value);
    }

    public bool ShowDots
    {
        get => GetValue(ShowDotsProperty);
        set => SetValue(ShowDotsProperty, value);
    }

    public bool ShowArrows
    {
        get => GetValue(ShowArrowsProperty);
        set => SetValue(ShowArrowsProperty, value);
    }

    public string? DisplayMemberPath
    {
        get => GetValue(DisplayMemberPathProperty);
        set => SetValue(DisplayMemberPathProperty, value);
    }

    public ICommand? ItemClickCommand
    {
        get => GetValue(ItemClickCommandProperty);
        set => SetValue(ItemClickCommandProperty, value);
    }

    public event EventHandler<RoutedEventArgs> ItemClick
    {
        add => AddHandler(ItemClickEvent, value);
        remove => RemoveHandler(ItemClickEvent, value);
    }

    public void GoToNext()
    {
        var count = Items.Count;
        if (count < 2 || _animationTimer.IsEnabled)
            return;

        SetSelectedIndex(SelectedIndex == count - 1 ? 0 : SelectedIndex + 1,
            suppressAnimation: false, wrapForward: SelectedIndex == count - 1);
    }

    public void GoToPrevious()
    {
        var count = Items.Count;
        if (count < 2 || _animationTimer.IsEnabled)
            return;

        SetSelectedIndex(SelectedIndex == 0 ? count - 1 : SelectedIndex - 1,
            suppressAnimation: SelectedIndex == 0);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        if (DisplayMemberPath is { Length: > 0 } path && item != null)
            return CreateDisplayContainer(item, path);

        if (item is string imagePath && LooksLikeImagePath(imagePath))
            return CreateImage(imagePath);

        return base.CreateContainerForItemOverride(item, index, recycleKey);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        if (item is not Control &&
            (DisplayMemberPath is { Length: > 0 } || item is string imagePath && LooksLikeImagePath(imagePath)))
        {
            recycleKey = null;
            return true;
        }

        return NeedsContainer<ContentPresenter>(item, out recycleKey);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        CompleteAnimation();
        base.OnApplyTemplate(e);

        if (_prevButton != null)
            _prevButton.Click -= OnPreviousClick;
        if (_nextButton != null)
            _nextButton.Click -= OnNextClick;
        if (_itemsPresenter != null)
        {
            _itemsPresenter.PointerReleased -= OnItemsPointerReleased;
            _itemsPresenter.SizeChanged -= OnItemsPresenterSizeChanged;
        }

        _itemsPresenter = e.NameScope.Find<ItemsPresenter>("PART_ItemsPresenter");
        _dotsPanel = e.NameScope.Find<StackPanel>("PART_Dots");
        _prevButton = e.NameScope.Find<Button>("PART_PrevButton");
        _nextButton = e.NameScope.Find<Button>("PART_NextButton");

        if (_prevButton != null)
            _prevButton.Click += OnPreviousClick;
        if (_nextButton != null)
            _nextButton.Click += OnNextClick;
        if (_itemsPresenter != null)
        {
            _itemsPresenter.RenderTransform = _translateTransform;
            _itemsPresenter.PointerReleased += OnItemsPointerReleased;
            _itemsPresenter.SizeChanged += OnItemsPresenterSizeChanged;
        }

        RebuildDots();
        OnSelectedIndexChanged();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _isAttached = true;
        if (_itemsSourceNotifier != null)
            _itemsSourceNotifier.CollectionChanged += OnItemsCollectionChanged;
        UpdateAutoPlay();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _isAttached = false;
        _autoPlayTimer.Stop();
        CompleteAnimation();
        if (_itemsSourceNotifier != null)
            _itemsSourceNotifier.CollectionChanged -= OnItemsCollectionChanged;
        base.OnDetachedFromVisualTree(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BoundsProperty)
            CompleteAnimation();
    }

    private void OnSelectedIndexChanged()
    {
        var count = Items.Count;
        if (count == 0)
        {
            CompleteAnimation();
            if (SelectedIndex != 0)
                SetCurrentValue(SelectedIndexProperty, 0);
            SetCurrentValue(SelectedItemProperty, null);
            UpdateDots();
            return;
        }

        var index = Math.Clamp(SelectedIndex, 0, count - 1);
        if (index != SelectedIndex)
        {
            SetCurrentValue(SelectedIndexProperty, index);
            return;
        }

        if (!Equals(SelectedItem, Items[index]))
            SetCurrentValue(SelectedItemProperty, Items[index]);

        AnimateToIndex(index, _suppressNextAnimation);
        UpdateDots();
    }

    private void OnSelectedItemChanged()
    {
        var index = SelectedItem == null ? 0 : Items.IndexOf(SelectedItem);
        if (index >= 0 && index != SelectedIndex)
            SetCurrentValue(SelectedIndexProperty, index);
    }

    private void OnItemsSourceChanged(object? oldValue, object? newValue)
    {
        if (_itemsSourceNotifier != null)
            _itemsSourceNotifier.CollectionChanged -= OnItemsCollectionChanged;
        _itemsSourceNotifier = newValue as INotifyCollectionChanged;
        if (_isAttached && _itemsSourceNotifier != null)
            _itemsSourceNotifier.CollectionChanged += OnItemsCollectionChanged;
        RefreshItems();
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.UIThread.Post(RefreshItems);
    }

    private void RefreshItems()
    {
        CompleteAnimation();
        RebuildDots();
        OnSelectedIndexChanged();
        UpdateAutoPlay();
        ItemsPanelRoot?.InvalidateMeasure();
    }

    private void RebuildDots()
    {
        if (_dotsPanel == null)
            return;

        _dotsPanel.Children.Clear();
        for (var i = 0; i < Items.Count; i++)
        {
            var dot = new Button
            {
                Classes = { "carousel-dot" },
                Tag = i,
                Focusable = false
            };
            dot.Bind(ThemeProperty, new DynamicResourceExtension("WD.CarouselIndicatorTheme"));
            dot.Click += OnDotClick;
            _dotsPanel.Children.Add(dot);
        }
        UpdateDotsVisibility();
        UpdateDots();
    }

    private void UpdateDotsVisibility()
    {
        if (_dotsPanel != null)
            _dotsPanel.IsVisible = ShowDots && Items.Count > 1;
    }

    private void UpdateDots()
    {
        if (_dotsPanel == null)
            return;

        for (var i = 0; i < _dotsPanel.Children.Count; i++)
        {
            if (_dotsPanel.Children[i] is not Button button)
                continue;

            var selected = i == SelectedIndex;
            button.Classes.Set("selected", selected);
        }
    }

    private void OnDotClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: int index })
            SetCurrentValue(SelectedIndexProperty, index);
    }

    private void OnPreviousClick(object? sender, RoutedEventArgs e) => GoToPrevious();

    private void OnNextClick(object? sender, RoutedEventArgs e) => GoToNext();

    private void OnItemsPresenterSizeChanged(object? sender, SizeChangedEventArgs e)
    {
        CompleteAnimation();
    }

    private void SetSelectedIndex(int index, bool suppressAnimation, bool wrapForward = false)
    {
        _suppressNextAnimation = suppressAnimation;
        _wrapNextAnimation = wrapForward;
        try
        {
            SetCurrentValue(SelectedIndexProperty, index);
        }
        finally
        {
            _suppressNextAnimation = false;
            _wrapNextAnimation = false;
        }
    }

    private void OnItemsPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_itemsPresenter == null || Items.Count == 0 || _itemsPresenter.Bounds.Width <= 0 ||
            _animationTimer.IsEnabled)
            return;

        var index = (int)(e.GetPosition(_itemsPresenter).X / _itemsPresenter.Bounds.Width);
        if (index < 0 || index >= Items.Count)
            return;

        var item = Items[index];
        SetCurrentValue(SelectedIndexProperty, index);
        RaiseEvent(new RoutedEventArgs(ItemClickEvent, this));
        if (ItemClickCommand?.CanExecute(item) == true)
            ItemClickCommand.Execute(item);
    }

    private void AnimateToIndex(int index, bool suppressAnimation)
    {
        if (_wrapPanel != null)
        {
            ResetWrap();
            _translateTransform.X = 0;
        }

        if (_itemsPresenter == null || _itemsPresenter.Bounds.Width <= 0)
            return;

        var target = -index * _itemsPresenter.Bounds.Width;
        if (suppressAnimation)
        {
            _animationTimer.Stop();
            _translateTransform.X = target;
            return;
        }

        if (AnimationDuration <= 0 || Math.Abs(target - _translateTransform.X) < 0.5)
        {
            _animationTimer.Stop();
            _translateTransform.X = target;
            return;
        }

        if (_wrapNextAnimation && ItemsPanelRoot is CarouselPanel panel && Items.Count > 1)
        {
            _wrapPanel = panel;
            panel.WrapFirstItem = true;
            target = -Items.Count * _itemsPresenter.Bounds.Width;
        }

        _animationFrom = _translateTransform.X;
        _animationTo = target;
        _animationStart = DateTime.UtcNow;
        _animationTimer.Start();
    }

    private void OnAnimationTick(object? sender, EventArgs e)
    {
        var progress = (DateTime.UtcNow - _animationStart).TotalSeconds / AnimationDuration;
        if (AnimationDuration <= 0 || progress >= 1)
        {
            CompleteAnimation();
            return;
        }

        var eased = 1 - Math.Pow(1 - progress, 3);
        _translateTransform.X = _animationFrom + (_animationTo - _animationFrom) * eased;
    }

    private void CompleteAnimation()
    {
        _animationTimer.Stop();
        ResetWrap();
        _translateTransform.X = -SelectedIndex * (_itemsPresenter?.Bounds.Width ?? 0);
    }

    private void ResetWrap()
    {
        if (_wrapPanel == null)
            return;

        _wrapPanel.WrapFirstItem = false;
        _wrapPanel = null;
    }

    private void UpdateAutoPlay()
    {
        _autoPlayTimer.Stop();
        if (!_isAttached || !AutoPlay || Items.Count < 2)
            return;

        _autoPlayTimer.Interval = AutoPlayInterval;
        _autoPlayTimer.Start();
    }

    private Control CreateDisplayContainer(object item, string propertyPath)
    {
        var value = GetDisplayValue(item, propertyPath);
        return value is string imagePath && LooksLikeImagePath(imagePath)
            ? CreateImage(imagePath)
            : new ContentPresenter { Content = value };
    }

    private static object? GetDisplayValue(object item, string propertyPath)
    {
        object? value = item;
        foreach (var memberName in propertyPath.Split('.'))
        {
            if (value == null)
                return null;

            var property = value.GetType().GetProperty(memberName,
                BindingFlags.Instance | BindingFlags.Public);
            if (property == null)
                throw new InvalidOperationException(
                    $"DisplayMemberPath '{propertyPath}' was not found on '{value.GetType().FullName}'.");
            value = property.GetValue(value);
        }
        return value;
    }

    private static bool LooksLikeImagePath(string path)
    {
        return Uri.TryCreate(path, UriKind.Absolute, out var uri)
            ? uri.Scheme is "avares" or "file" or "pack"
            : Path.HasExtension(path);
    }

    private static Image CreateImage(string path)
    {
        var source = path.StartsWith("pack://application:,,,/", StringComparison.OrdinalIgnoreCase)
            ? ConvertPackUri(path)
            : new Uri(path, UriKind.RelativeOrAbsolute);

        using var stream = source.IsAbsoluteUri && source.IsFile
            ? File.OpenRead(source.LocalPath)
            : AssetLoader.Open(source);
        return new Image
        {
            Source = new Bitmap(stream),
            Stretch = Stretch.UniformToFill
        };
    }

    private static Uri ConvertPackUri(string path)
    {
        var resource = path["pack://application:,,,/".Length..];
        var componentIndex = resource.IndexOf(";component/", StringComparison.OrdinalIgnoreCase);
        if (componentIndex < 0)
            throw new UriFormatException($"Unsupported WPF pack URI: '{path}'.");

        var assembly = resource[..componentIndex];
        var assetPath = resource[(componentIndex + ";component/".Length)..];
        return new Uri($"avares://{assembly}/{assetPath}");
    }
}
