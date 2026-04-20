using CalorieCalculator.ViewModels.MainView.Food;

namespace CalorieCalculator.Views.MainView.Food;

[QueryProperty(nameof(CategoryID), "CategoryID")]
[QueryProperty(nameof(CategoryName), "CategoryName")]
[QueryProperty(nameof(ProgramID), "ProgramID")]
[QueryProperty(nameof(MealType), "MealType")]
[QueryProperty(nameof(MealID), "MealID")]
public partial class FoodProductPage : ContentPage
{
    private readonly FoodProductViewModel _viewModel;

    public string CategoryID { set => _viewModel.CategoryID = int.Parse(value); }
    public string CategoryName { set => _viewModel.CategoryName = Uri.UnescapeDataString(value); }
    public string ProgramID { set => _viewModel.ProgramID = int.Parse(value); }
    public string MealType { set => _viewModel.MealType = int.Parse(value); }
    public string MealID { set => _viewModel.MealID = int.TryParse(value, out var id) ? id : null; }

    public FoodProductPage(FoodProductViewModel viewModel)
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