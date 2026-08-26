using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;

namespace WPFDevelopers.Avalonia.Controls
{
    public class ToastListBoxItem : ContentControl
    {
        private const double DismissDelayMs = 10000;
        private CancellationTokenSource? _dismissToken;
        private Border? _border;
        private Rectangle? _rectangle;
        private Animation? _currentAnimation;
        private bool _isClosedByButton = false;

        public static readonly StyledProperty<ToastIcon> ToastIconProperty =
            AvaloniaProperty.Register<ToastListBoxItem, ToastIcon>(nameof(ToastIcon));

        public static readonly StyledProperty<bool> IsCenterProperty =
            AvaloniaProperty.Register<ToastListBoxItem, bool>(nameof(IsCenter));

        public ToastIcon ToastIcon
        {
            get => GetValue(ToastIconProperty);
            set => SetValue(ToastIconProperty, value);
        }

        public bool IsCenter
        {
            get => GetValue(IsCenterProperty);
            set => SetValue(IsCenterProperty, value);
        }

        static ToastListBoxItem()
        {
            AffectsArrange<ToastListBoxItem>(ToastIconProperty, IsCenterProperty);

            ToastIconProperty.Changed.AddClassHandler<ToastListBoxItem>((item, e) =>
            {
                var icon = (ToastIcon)e.NewValue!;
                item.PseudoClasses.Set(":info", icon == ToastIcon.Info);
                item.PseudoClasses.Set(":success", icon == ToastIcon.Success);
                item.PseudoClasses.Set(":warning", icon == ToastIcon.Warning);
                item.PseudoClasses.Set(":error", icon == ToastIcon.Error);
            });

            IsCenterProperty.Changed.AddClassHandler<ToastListBoxItem>((item, e) =>
                item.PseudoClasses.Set(":center", (bool)e.NewValue!));
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _border = e.NameScope.Find<Border>("PART_Border");
            if (_border != null)
            {
                _border.RenderTransform = new ScaleTransform(1, 1);
                _border.RenderTransformOrigin = new RelativePoint(0.5, 0, RelativeUnit.Relative);
            }
            _rectangle = e.NameScope.Find<Rectangle>("PART_Rectangle");

            var closeBtn = e.NameScope.Find<Button>("PART_CloseButton");
            if (closeBtn != null)
            {
                closeBtn.Click += (_, _) => CloseByButton();
            }
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            Close();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            StartAutoDismissTimer();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            _dismissToken?.Cancel();
            _dismissToken = null;
            base.OnDetachedFromVisualTree(e);
        }

        private void StartAutoDismissTimer()
        {
            _dismissToken?.Cancel();
            _dismissToken = new CancellationTokenSource();
            var token = _dismissToken.Token;

            Task.Delay(TimeSpan.FromMilliseconds(DismissDelayMs), token).ContinueWith(t =>
            {
                if (t.IsCanceled) return;
                Dispatcher.UIThread.Post(() =>
                {
                    if (IsVisible)
                        RemoveSelf();
                });
            }, CancellationToken.None);
        }

        public void Close(bool isAnimation = true)
        {
            _dismissToken?.Cancel();
            _dismissToken = null;
            if (_rectangle != null && isAnimation && !_isClosedByButton)
            {
                _rectangle.RenderTransform = new ScaleTransform(1, 1);
                _rectangle.RenderTransformOrigin = new RelativePoint(0, 0.5, RelativeUnit.Relative);
                var anim = new Animation
                {
                    Duration = TimeSpan.FromSeconds(10),
                    FillMode = FillMode.Forward,
                    Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters = { new Setter { Property = ScaleTransform.ScaleXProperty, Value = 1d } }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        Setters = { new Setter { Property = ScaleTransform.ScaleXProperty, Value = 0d } }
                    }
                }
                };
                _currentAnimation = anim;
                _ = AnimateAsync(anim, _rectangle);
            }
            else
            {
                RemoveSelf();
            }
        }

        private void CloseByButton()
        {
            _isClosedByButton = true;

            if (_currentAnimation != null)
            {
                if (_rectangle != null)
                {
                    _rectangle.RenderTransform = null;
                }
                _currentAnimation = null;
            }

            Close(false);
        }

        private async Task AnimateAsync(Animation anim, Animatable target)
        {
            try
            {
                await anim.RunAsync(target);
                Dispatcher.UIThread.Post(async () =>
                {
                    await RemoveSelfWithAnimation();
                });
                //RemoveSelf();
            }
            catch
            {
                RemoveSelf();
            }
        }

        private async Task RemoveSelfWithAnimation()
        {
            if(_border != null)
            {
                var anim = new Animation
                {
                    Duration = TimeSpan.FromMilliseconds(200),
                    FillMode = FillMode.Forward,
                    Children =
                    {
                        new KeyFrame
                        {
                            Cue = new Cue(0d),
                            Setters =
                            {
                                new Setter { Property = ScaleTransform.ScaleYProperty, Value = 1d },
                                new Setter { Property = Visual.OpacityProperty, Value = 1d }
                            }
                        },
                        new KeyFrame
                        {
                            Cue = new Cue(1d),
                            Setters =
                            {
                                new Setter { Property = ScaleTransform.ScaleYProperty, Value = 0d },
                                new Setter { Property = Visual.OpacityProperty, Value = 0d }
                            }
                        }
                    }
                };

                _currentAnimation = anim;
                await anim.RunAsync(_border);
                RemoveSelf();
            }
        }

        private void RemoveSelf()
        {
            _isClosedByButton = false;
            _currentAnimation = null;
            (Parent as Panel)?.Children.Remove(this);
        }
    }
}
