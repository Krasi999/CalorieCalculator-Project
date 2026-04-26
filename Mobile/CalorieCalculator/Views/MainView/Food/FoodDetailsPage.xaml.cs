using CalorieCalculator.ViewModels.MainView.Food;

namespace CalorieCalculator.Views.MainView.Food;

[QueryProperty(nameof(ProductID), "ProductID")]
[QueryProperty(nameof(ProgramID), "ProgramID")]
[QueryProperty(nameof(MealType), "MealType")]
[QueryProperty(nameof(MealID), "MealID")]
[QueryProperty(nameof(MealFoodID), "MealFoodID")]
[QueryProperty(nameof(CurrentWeight), "CurrentWeight")]
public partial class FoodDetailsPage : ContentPage
{
    private readonly FoodDetailViewModel _viewModel;

    public string ProductID { get; set; }
    public string ProgramID { get; set; }
    public string MealType { get; set; }
    public string MealID { get; set; }
    public string MealFoodID { get; set; }
    public string CurrentWeight { get; set; }

    public FoodDetailsPage(FoodDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.ProductID = int.Parse(ProductID);
        _viewModel.ProgramID = int.Parse(ProgramID);
        _viewModel.MealType = int.Parse(MealType);
        _viewModel.MealID = int.TryParse(MealID, out var mid) ? mid : null;
        _viewModel.MealFoodID = int.TryParse(MealFoodID, out var mfid) ? mfid : null;
        _viewModel.CurrentWeight = int.TryParse(CurrentWeight, out var cw) ? cw : null;

        _viewModel.LoadCommand.Execute(null);
    }
}