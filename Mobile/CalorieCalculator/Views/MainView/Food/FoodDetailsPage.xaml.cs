using CalorieCalculator.ViewModels.MainView.Food;

namespace CalorieCalculator.Views.MainView.Food;

[QueryProperty(nameof(ProductID), "ProductID")]
[QueryProperty(nameof(ProgramID), "ProgramID")]
[QueryProperty(nameof(MealType), "MealType")]
[QueryProperty(nameof(MealID), "MealID")]
public partial class FoodDetailsPage : ContentPage
{
    private readonly FoodDetailViewModel _viewModel;

    public string ProductID { set => _viewModel.ProductID = int.Parse(value); }
    public string ProgramID { set => _viewModel.ProgramID = int.Parse(value); }
    public string MealType { set => _viewModel.MealType = int.Parse(value); }
    public string MealID { set => _viewModel.MealID = int.TryParse(value, out var id) ? id : null; }

    public FoodDetailsPage(FoodDetailViewModel viewModel)
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