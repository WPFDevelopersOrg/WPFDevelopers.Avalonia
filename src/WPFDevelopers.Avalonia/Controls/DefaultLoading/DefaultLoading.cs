using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

namespace WPFDevelopers.Avalonia.Controls
{
    public class DefaultLoading : Border
    {
        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<DefaultLoading, bool>(nameof(IsActive), defaultValue: true);

        public static readonly StyledProperty<IBrush?> StrokeBrushProperty =
            AvaloniaProperty.Register<DefaultLoading, IBrush?>(nameof(StrokeBrush));

        public static readonly StyledProperty<double> ArcThicknessProperty =
            AvaloniaProperty.Register<DefaultLoading, double>(nameof(ArcThickness), defaultValue: 1.5);

        static DefaultLoading()
        {
            IsActiveProperty.Changed.AddClassHandler<DefaultLoading>((r, _) => r.OnIsActiveChanged());
        }

        public bool IsActive
        {
            get => GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public IBrush? StrokeBrush
        {
            get => GetValue(StrokeBrushProperty);
            set => SetValue(StrokeBrushProperty, value);
        }

        public double ArcThickness
        {
            get => GetValue(ArcThicknessProperty);
            set => SetValue(ArcThicknessProperty, value);
        }

        private readonly Grid _grid;
        private readonly global::Avalonia.Controls.Shapes.Path _arcPath;
        private DispatcherTimer? _gridRotationTimer;
        private DispatcherTimer? _arcRotationTimer;
        private DispatcherTimer? _sweepTimer;
        private double _gridAngle;
        private double _arcAngle;
        private double _arcProgress;
        private bool _expanding;

        public DefaultLoading()
        {
            Width = 25;
            Height = 25;

            _grid = new Grid
            {
                Width = 25,
                Height = 25,
                HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
                RenderTransform = new RotateTransform(0),
                RenderTransformOrigin = new RelativePoint(12.5, 12.5, RelativeUnit.Absolute)
            };

            _arcPath = new global::Avalonia.Controls.Shapes.Path
            {
                Width = 25,
                Height = 25,
                Stretch = Stretch.None,
                HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
                RenderTransform = new RotateTransform(0),
                RenderTransformOrigin = new RelativePoint(12.5, 12.5, RelativeUnit.Absolute)
            };

            _grid.Children.Add(_arcPath);
            Child = _grid;
            ApplyStroke();
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            if (IsActive) StartAnimation();
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
            StopAnimation();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);
            if (change.Property == StrokeBrushProperty || change.Property == ArcThicknessProperty)
            {
                ApplyStroke();
            }
        }

        private void ApplyStroke()
        {
            _arcPath.Fill = StrokeBrush ?? Brushes.White;
            UpdateArcGeometry();
        }

        private void OnIsActiveChanged()
        {
            if (IsActive) StartAnimation();
            else StopAnimation();
        }

        private void StartAnimation()
        {
            StopAnimation();

            _gridAngle = 0;
            _arcAngle = 0;
            _arcProgress = 0;
            _expanding = true;

            _gridRotationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _gridRotationTimer.Tick += (_, _) =>
            {
                _gridAngle += 360.0 * 16.0 / 784.0;
                _grid.RenderTransform = new RotateTransform(_gridAngle);
            };
            _gridRotationTimer.Start();

            _arcRotationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _arcRotationTimer.Tick += (_, _) =>
            {
                _arcAngle += 1080.0 * 16.0 / 5332.0;
                _arcPath.RenderTransform = new RotateTransform(_arcAngle);
            };
            _arcRotationTimer.Start();

            _sweepTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _sweepTimer.Tick += (_, _) =>
            {
                var halfDuration = 666.5;
                var delta = 16.0 / halfDuration;

                if (_expanding)
                {
                    _arcProgress += delta;
                    if (_arcProgress >= 1.0)
                    {
                        _arcProgress = 1.0;
                        _expanding = false;
                    }
                }
                else
                {
                    _arcProgress -= delta;
                    if (_arcProgress <= 0.0)
                    {
                        _arcProgress = 0.0;
                        _expanding = true;
                    }
                }

                UpdateArcGeometry();
            };
            _sweepTimer.Start();
        }

        private void UpdateArcGeometry()
        {
            const double w = 25;
            const double h = 25;

            var cx = w / 2;
            var cy = h / 2;
            var outerR = Math.Min(cx, cy);
            var innerR = outerR - ArcThickness;
            if (innerR <= 0) innerR = 1;

            var startDeg = -5 + (-125) * _arcProgress;
            var endDeg = 5 + 125 * _arcProgress;

            var radS = startDeg * Math.PI / 180.0;
            var radE = endDeg * Math.PI / 180.0;

            var x1o = cx + outerR * Math.Cos(radS);
            var y1o = cy + outerR * Math.Sin(radS);
            var x2o = cx + outerR * Math.Cos(radE);
            var y2o = cy + outerR * Math.Sin(radE);
            var x2i = cx + innerR * Math.Cos(radE);
            var y2i = cy + innerR * Math.Sin(radE);
            var x1i = cx + innerR * Math.Cos(radS);
            var y1i = cy + innerR * Math.Sin(radS);

            var sweepDeg = endDeg - startDeg;

            var outerArc = new ArcSegment
            {
                Point = new Point(x2o, y2o),
                Size = new Size(outerR, outerR),
                SweepDirection = SweepDirection.Clockwise,
                IsLargeArc = sweepDeg > 180
            };

            var innerArc = new ArcSegment
            {
                Point = new Point(x1i, y1i),
                Size = new Size(innerR, innerR),
                SweepDirection = SweepDirection.CounterClockwise,
                IsLargeArc = sweepDeg > 180
            };

            var figure = new PathFigure
            {
                StartPoint = new Point(x1o, y1o),
                IsClosed = true
            };
            figure.Segments!.Add(outerArc);
            figure.Segments.Add(new LineSegment { Point = new Point(x2i, y2i) });
            figure.Segments.Add(innerArc);
            figure.Segments.Add(new LineSegment { Point = new Point(x1o, y1o) });

            var geometry = new PathGeometry();
            geometry.Figures!.Add(figure);
            _arcPath.Data = geometry;
        }

        private void StopAnimation()
        {
            _gridRotationTimer?.Stop();
            _gridRotationTimer = null;
            _arcRotationTimer?.Stop();
            _arcRotationTimer = null;
            _sweepTimer?.Stop();
            _sweepTimer = null;
        }
    }
}
