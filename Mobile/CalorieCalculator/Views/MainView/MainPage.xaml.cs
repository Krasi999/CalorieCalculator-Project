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
}
