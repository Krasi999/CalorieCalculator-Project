using CalorieCalculator.ViewModels;

namespace CalorieCalculator.Views.MainView.Food;


[QueryProperty(nameof(ProgramID), "ProgramID")]
[QueryProperty(nameof(MealType), "MealType")]
[QueryProperty(nameof(MealID), "MealID")]
public partial class FoodCategoryPage : ContentPage
{
    private readonly FoodCategoryViewModel _viewModel;

    public string ProgramID { set => _viewModel.ProgramID = int.Parse(value); }
    public string MealType { set => _viewModel.MealType = int.Parse(value); }
    public string MealID { set => _viewModel.MealID = int.TryParse(value, out var id) ? id : null; }

    public FoodCategoryPage(FoodCategoryViewModel viewModel)
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
}