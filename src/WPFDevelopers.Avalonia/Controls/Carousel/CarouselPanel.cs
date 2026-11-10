using Avalonia;
using Avalonia.Controls;

namespace WPFDevelopers.Avalonia.Controls;

public class CarouselPanel : Panel
{
    private bool _wrapFirstItem;

    internal bool WrapFirstItem
    {
        get => _wrapFirstItem;
        set
        {
            if (_wrapFirstItem == value)
                return;

            _wrapFirstItem = value;
            InvalidateArrange();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var constraintWidth = double.IsFinite(availableSize.Width) ? availableSize.Width : double.PositiveInfinity;
        var constraintHeight = double.IsFinite(availableSize.Height) ? availableSize.Height : double.PositiveInfinity;
        var desiredWidth = 0.0;
        var desiredHeight = 0.0;
        foreach (var child in Children)
        {
            child.Measure(new Size(constraintWidth, constraintHeight));
            desiredWidth = Math.Max(desiredWidth, child.DesiredSize.Width);
            desiredHeight = Math.Max(desiredHeight, child.DesiredSize.Height);
        }
        return new Size(double.IsFinite(availableSize.Width) ? availableSize.Width : desiredWidth,
            double.IsFinite(availableSize.Height) ? availableSize.Height : desiredHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        for (var i = 0; i < Children.Count; i++)
        {
            // Reuse the first slide after the last instead of cloning its visual tree.
            var position = WrapFirstItem && i == 0 ? Children.Count : i;
            Children[i].Arrange(new Rect(position * finalSize.Width, 0, finalSize.Width, finalSize.Height));
        }
        return finalSize;
    }
}
