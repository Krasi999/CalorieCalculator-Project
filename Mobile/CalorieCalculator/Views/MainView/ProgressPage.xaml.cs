namespace CalorieCalculator.Views;

public partial class ProgressPage : ContentPage
{
    public ProgressPage()
    {
        InitializeComponent();
    }

    private async void OnProfileTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProfilePage");
    }

    private async void OnTodayTapped(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//MainPage");
    }
}