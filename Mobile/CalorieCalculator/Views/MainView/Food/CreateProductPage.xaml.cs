using CalorieCalculator.ViewModels.MainView.Food;

namespace CalorieCalculator.Views.MainView.Food;


[QueryProperty(nameof(CategoryID), "CategoryID")]
public partial class CreateProductPage : ContentPage
{
    private readonly CreateProductViewModel _viewModel;

    public string CategoryID
    {
        set
        {
            if (int.TryParse(value, out var id))
                _viewModel.PreselectedCategoryID = id;
        }
    }

    public CreateProductPage(CreateProductViewModel viewModel)
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