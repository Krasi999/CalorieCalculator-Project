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
            "За CalorieTracker",
            "CalorieTracker v1.0\n\nТвоят персонален хранителен дневник.\n\nРазработено от екип CalorieCalculator, 2026.",
            "OK");
    }
}