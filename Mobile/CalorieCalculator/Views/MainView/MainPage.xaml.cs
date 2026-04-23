using CalorieCalculator.Models;
using CalorieCalculator.ViewModels;

namespace CalorieCalculator.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadCommand.Execute(null);
        NavBar.CurrentPage = "home";
    }

    private async void OnMealTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is MealDTO meal)
        {
            _viewModel.ViewMealCommand.Execute(meal);
        }
    }

    private async void OnAddMealTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is MealDTO meal)
        {
            _viewModel.AddMealCommand.Execute(meal);
        }
    }

    private void OnPreviousDayTapped(object sender, TappedEventArgs e)
    {
        _viewModel.PreviousDayCommand.Execute(null);
    }

    private void OnNextDayTapped(object sender, TappedEventArgs e)
    {
        _viewModel.NextDayCommand.Execute(null);
    }
}
