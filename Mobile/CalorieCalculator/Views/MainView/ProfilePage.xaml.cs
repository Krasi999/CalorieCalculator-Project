using CalorieCalculator.ViewModels;

namespace CalorieCalculator.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnAboutTapped(object? sender, EventArgs e)
    {
        await DisplayAlert(
            "За приложението",
            "Калориен калкулатор\n\nПриложението е създадено.\n\nРазработено от екип CalorieCalculator, 2026.",
            "OK");
    }
}