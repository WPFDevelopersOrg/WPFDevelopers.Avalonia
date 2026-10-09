using Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class SplitViewDemo : UserControl
    {
        public SplitViewDemo()
        {
            InitializeComponent();
            var splitViewToggle = this.FindControl<Button>("SplitViewToggleBtn");
            var demoSplitView = this.FindControl<SplitView>("DemoSplitView");
            if (splitViewToggle != null && demoSplitView != null)
                splitViewToggle.Click += (_, _) => demoSplitView.IsPaneOpen = !demoSplitView.IsPaneOpen;

            var overlayRadio = this.FindControl<RadioButton>("SplitViewOverlayBtn");
            var inlineRadio = this.FindControl<RadioButton>("SplitViewInlineBtn");
            var compactRadio = this.FindControl<RadioButton>("SplitViewCompactBtn");
            if (demoSplitView != null)
            {
                if (overlayRadio != null) overlayRadio.IsCheckedChanged += (_, _) => { if (overlayRadio.IsChecked == true) demoSplitView.DisplayMode = SplitViewDisplayMode.Overlay; };
                if (inlineRadio != null) inlineRadio.IsCheckedChanged += (_, _) => { if (inlineRadio.IsChecked == true) demoSplitView.DisplayMode = SplitViewDisplayMode.Inline; };
                if (compactRadio != null) compactRadio.IsCheckedChanged += (_, _) => { if (compactRadio.IsChecked == true) demoSplitView.DisplayMode = SplitViewDisplayMode.CompactInline; };
            }
        }
    }
}
