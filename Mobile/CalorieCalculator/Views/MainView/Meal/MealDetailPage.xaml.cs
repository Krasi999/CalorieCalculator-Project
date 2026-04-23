using CalorieCalculator.Models;
using CalorieCalculator.ViewModels.MainView.Meal;

namespace CalorieCalculator.Views.MainView.Meal;

[QueryProperty(nameof(MealID), "MealID")]
[QueryProperty(nameof(MealType), "MealType")]
[QueryProperty(nameof(ProgramID), "ProgramID")]
public partial class MealDetailPage : ContentPage
{
    private readonly MealDetailViewModel _viewModel;

    public string MealID { set => _viewModel.MealID = int.TryParse(value, out var id) ? id : 0; }
    public string MealType { set => _viewModel.MealType = int.Parse(value); }
    public string ProgramID { set => _viewModel.ProgramID = int.Parse(value); }

    public MealDetailPage(MealDetailViewModel viewModel)
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

    private void OnEditWeightTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is MealFoodDTO food)
        {
            _viewModel.EditWeightCommand.Execute(food);
        }
    }

    private void OnDeleteFoodTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is MealFoodDTO food)
        {
            _viewModel.DeleteFoodCommand.Execute(food);
        }
    }
}