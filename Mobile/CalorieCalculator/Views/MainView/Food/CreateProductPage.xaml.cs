using CalorieCalculator.ViewModels.MainView.Food;

namespace CalorieCalculator.Views.MainView.Food;


[QueryProperty(nameof(CategoryID), "CategoryID")]
[QueryProperty(nameof(ProductID), "ProductID")]
[QueryProperty(nameof(Barcode), "Barcode")]
[QueryProperty(nameof(PrefilledName), "PrefilledName")]
[QueryProperty(nameof(PrefilledDescription), "PrefilledDescription")]
[QueryProperty(nameof(PrefilledCalories), "PrefilledCalories")]
[QueryProperty(nameof(PrefilledProtein), "PrefilledProtein")]
[QueryProperty(nameof(PrefilledCarbs), "PrefilledCarbs")]
[QueryProperty(nameof(PrefilledFats), "PrefilledFats")]
public partial class CreateProductPage : ContentPage
{
    private readonly CreateProductViewModel _viewModel;
    private bool _paramsApplied;

    public string CategoryID { get; set; }
    public string ProductID { get; set; }
    public string Barcode { get; set; }
    public string PrefilledName { get; set; }
    public string PrefilledDescription { get; set; }
    public string PrefilledCalories { get; set; }
    public string PrefilledProtein { get; set; }
    public string PrefilledCarbs { get; set; }
    public string PrefilledFats { get; set; }

    public CreateProductPage(CreateProductViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!_paramsApplied)
        {
            if (int.TryParse(CategoryID, out var catId))
                _viewModel.PreselectedCategoryID = catId;

            if (int.TryParse(ProductID, out var prodId))
                _viewModel.EditProductID = prodId;

            _viewModel.Barcode = Barcode;
            _viewModel.PrefilledName = PrefilledName != null ? Uri.UnescapeDataString(PrefilledName) : null;
            _viewModel.PrefilledDescription = PrefilledDescription != null ? Uri.UnescapeDataString(PrefilledDescription) : null;
            _viewModel.PrefilledCalories = PrefilledCalories;
            _viewModel.PrefilledProtein = PrefilledProtein;
            _viewModel.PrefilledCarbs = PrefilledCarbs;
            _viewModel.PrefilledFats = PrefilledFats;

            _paramsApplied = true;
        }

        _viewModel.LoadCommand.Execute(null);
    }
}