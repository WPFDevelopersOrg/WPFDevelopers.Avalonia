using Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages
{
    public partial class RepeatButtonDemo : UserControl
    {
        public RepeatButtonDemo()
        {
            InitializeComponent();
            var repeatValueText = this.FindControl<TextBlock>("RepeatValueText");
            int repeatCounter = 0;
            var repeatIncrementBtn = this.FindControl<RepeatButton>("RepeatIncrementBtn");
            if (repeatIncrementBtn != null && repeatValueText != null)
                repeatIncrementBtn.Click += (_, _) => { repeatCounter++; repeatValueText.Text = repeatCounter.ToString(); };

            var repeatDecrementBtn = this.FindControl<RepeatButton>("RepeatDecrementBtn");
            if (repeatDecrementBtn != null && repeatValueText != null)
                repeatDecrementBtn.Click += (_, _) => { repeatCounter--; repeatValueText.Text = repeatCounter.ToString(); };
        }
    }
}
