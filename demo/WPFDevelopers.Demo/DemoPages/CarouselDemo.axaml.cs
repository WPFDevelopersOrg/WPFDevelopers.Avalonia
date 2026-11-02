using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Interactivity;
using WPFDevelopers.Avalonia.Controls;

namespace WPFDevelopers.Demo.DemoPages;

public partial class CarouselDemo : UserControl
{
    public CarouselDemo()
    {
        InitializeComponent();
        ImagePaths =
        [
            new CarouselSlideModel("Slide 1", "avares://WPFDevelopers.Demo/Assets/Carousel/Slide0.png"),
            new CarouselSlideModel("Slide 2", "avares://WPFDevelopers.Demo/Assets/Carousel/Slide1.png"),
            new CarouselSlideModel("Slide 3", "avares://WPFDevelopers.Demo/Assets/Carousel/Slide2.png"),
            new CarouselSlideModel("Slide 4", "avares://WPFDevelopers.Demo/Assets/Carousel/Slide3.png")
        ];
        DataContext = this;
    }

    public ObservableCollection<CarouselSlideModel> ImagePaths { get; }

    public ICommand CarouselClickCommand => new DemoCommand(parameter =>
    {
        if (parameter is CarouselSlideModel model)
            Toast.Push($"Command: clicked {model.Title}", ToastIcon.Success, true);
    });

    private void Carousel_ItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is WPFDevelopers.Avalonia.Controls.Carousel carousel)
            Toast.Push($"Event: clicked slide {carousel.SelectedIndex + 1}", ToastIcon.Success, true);
    }

    public sealed record CarouselSlideModel(string Title, string URL);

    private sealed class DemoCommand(Action<object?> execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter) => execute(parameter);
    }
}
