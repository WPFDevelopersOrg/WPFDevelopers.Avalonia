using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace WPFDevelopers.Avalonia.Controls
{
    [TemplatePart("PART_Marker", typeof(Border))]
    public class StepItem : ContentControl
    {
        public static readonly StyledProperty<int> IndexProperty =
            AvaloniaProperty.Register<StepItem, int>(nameof(Index), -1);

        public static readonly StyledProperty<StepState> StatusProperty =
            AvaloniaProperty.Register<StepItem, StepState>(nameof(Status), StepState.Waiting);

        internal static readonly StyledProperty<Orientation> OrientationProperty =
            AvaloniaProperty.Register<StepItem, Orientation>(nameof(Orientation), Orientation.Horizontal);

        internal Border? Marker { get; private set; }

        static StepItem()
        {
            StatusProperty.Changed.AddClassHandler<StepItem>((item, _) => item.UpdateStatusClasses());
            OrientationProperty.Changed.AddClassHandler<StepItem>((item, _) => item.UpdateOrientationClass());
        }

        public StepItem()
        {
            UpdateStatusClasses();
            UpdateOrientationClass();
        }

        public int Index
        {
            get => GetValue(IndexProperty);
            internal set => SetValue(IndexProperty, value);
        }

        public StepState Status
        {
            get => GetValue(StatusProperty);
            internal set => SetValue(StatusProperty, value);
        }

        internal Orientation Orientation
        {
            get => GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            Marker = e.NameScope.Find<Border>("PART_Marker");
        }

        private void UpdateStatusClasses()
        {
            PseudoClasses.Set(":waiting", Status == StepState.Waiting);
            PseudoClasses.Set(":inprogress", Status == StepState.InProgress);
            PseudoClasses.Set(":complete", Status == StepState.Complete);
        }

        private void UpdateOrientationClass()
        {
            PseudoClasses.Set(":vertical", Orientation == Orientation.Vertical);
        }
    }
}
