namespace CalorieCalculator.Views;

public partial class ProgressPage : ContentPage
{
    public ProgressPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        NavBar.CurrentPage = "progress";
    }
}