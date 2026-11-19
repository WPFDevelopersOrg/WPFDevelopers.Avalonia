using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace WPFDevelopers.Avalonia.Controls
{
    [TemplatePart("PART_ProgressTrack", typeof(Border))]
    [TemplatePart("PART_ProgressFill", typeof(Border))]
    public class Step : ItemsControl
    {
        public static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<Step, Orientation>(nameof(Orientation), Orientation.Horizontal);

        public static readonly StyledProperty<int> StepIndexProperty =
            AvaloniaProperty.Register<Step, int>(nameof(StepIndex), 0);

        private Border? _progressTrack;
        private Border? _progressFill;
        private TranslateTransform? _progressTrackTransform;
        private TranslateTransform? _progressFillTransform;

        static Step()
        {
            OrientationProperty.Changed.AddClassHandler<Step>((step, _) => step.OnOrientationChanged());
            StepIndexProperty.Changed.AddClassHandler<Step>((step, _) =>
            {
                step.UpdateStepItems();
                step.UpdateProgress();
            });
        }

        public Step()
        {
            UpdateOrientationClass();
            UpdateItemsPanel();
            LayoutUpdated += OnLayoutUpdated;
            SizeChanged += OnSizeChanged;
        }

        public Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public int StepIndex
        {
            get => GetValue(StepIndexProperty);
            set => SetValue(StepIndexProperty, value);
        }

        public void Next()
        {
            var count = ItemCount;
            if (count <= 0)
                return;

            var index = Math.Clamp(StepIndex, 0, count - 1);
            if (index < count - 1)
                StepIndex = index + 1;
        }

        public void Previous()
        {
            var count = ItemCount;
            if (count <= 0)
                return;

            var index = Math.Clamp(StepIndex, 0, count - 1);
            if (index > 0)
                StepIndex = index - 1;
        }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return item as StepItem ?? new StepItem();
        }

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            return NeedsContainer<StepItem>(item, out recycleKey);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            _progressTrack = e.NameScope.Find<Border>("PART_ProgressTrack");
            _progressFill = e.NameScope.Find<Border>("PART_ProgressFill");
            _progressTrackTransform = _progressTrack?.RenderTransform as TranslateTransform;
            _progressFillTransform = _progressFill?.RenderTransform as TranslateTransform;
            UpdateStepItems();
            UpdateProgress();
        }

        private void OnOrientationChanged()
        {
            UpdateOrientationClass();
            UpdateItemsPanel();
            UpdateStepItems();
            UpdateProgress();
        }

        private void UpdateOrientationClass()
        {
            PseudoClasses.Set(":vertical", Orientation == Orientation.Vertical);
        }

        private void UpdateItemsPanel()
        {
            ItemsPanel = new FuncTemplate<Panel?>(() => Orientation == Orientation.Horizontal
                ? new UniformGrid { Rows = 1 }
                : new UniformGrid { Columns = 1 });
        }

        private void OnLayoutUpdated(object? sender, System.EventArgs e)
        {
            UpdateStepItems();
            UpdateProgress();
        }

        private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            UpdateProgress();
        }

        private void UpdateStepItems()
        {
            var count = ItemCount;
            if (count <= 0)
                return;

            var index = Math.Clamp(StepIndex, 0, count - 1);
            if (index != StepIndex)
            {
                SetCurrentValue(StepIndexProperty, index);
                return;
            }

            for (var i = 0; i < count; i++)
            {
                if (ContainerFromIndex(i) is not StepItem item)
                    continue;

                item.Index = i + 1;
                item.Orientation = Orientation;
                item.Status = i < index
                    ? StepState.Complete
                    : i == index ? StepState.InProgress : StepState.Waiting;
            }
        }

        private void UpdateProgress()
        {
            if (_progressTrack == null || _progressFill == null ||
                _progressTrackTransform == null || _progressFillTransform == null ||
                _progressTrack.Parent is not Visual progressLayer)
                return;

            var count = ItemCount;
            if (count <= 1)
            {
                _progressTrack.IsVisible = false;
                _progressFill.IsVisible = false;
                return;
            }

            var currentIndex = Math.Clamp(StepIndex, 0, count - 1);
            var markerCenters = new Point[count];
            for (var i = 0; i < count; i++)
            {
                if (ContainerFromIndex(i) is not StepItem { Marker: { } marker })
                    return;

                var markerCenter = marker.TranslatePoint(
                    new Point(marker.Bounds.Width / 2, marker.Bounds.Height / 2),
                    progressLayer);
                if (markerCenter == null)
                    return;

                markerCenters[i] = markerCenter.Value;
            }

            _progressTrack.IsVisible = true;
            _progressFill.IsVisible = true;
            if (Orientation == Orientation.Horizontal)
            {
                var start = markerCenters[0];
                var end = markerCenters[count - 1];
                var current = markerCenters[currentIndex];
                _progressTrack.Width = Math.Max(0, end.X - start.X);
                _progressTrack.Height = 1;
                _progressTrackTransform.X = start.X;
                _progressTrackTransform.Y = start.Y - 0.5;
                _progressFill.Width = Math.Max(0, current.X - start.X);
                _progressFill.Height = 1;
                _progressFillTransform.X = start.X;
                _progressFillTransform.Y = start.Y - 0.5;
            }
            else
            {
                var start = markerCenters[0];
                var end = markerCenters[count - 1];
                var current = markerCenters[currentIndex];
                _progressTrack.Width = 1;
                _progressTrack.Height = Math.Max(0, end.Y - start.Y);
                _progressTrackTransform.X = start.X - 0.5;
                _progressTrackTransform.Y = start.Y;
                _progressFill.Width = 1;
                _progressFill.Height = Math.Max(0, current.Y - start.Y);
                _progressFillTransform.X = start.X - 0.5;
                _progressFillTransform.Y = start.Y;
            }
        }
    }
}
