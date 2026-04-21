using CalorieCalculator.ViewModels;

namespace CalorieCalculator.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is MainViewModel vm)
        {
            vm.LoadCommand.Execute(null);
        }
    }

    private async void OnProfileTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProfilePage");
    }

    private async void OnProgressTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProgressPage");
    }
}
