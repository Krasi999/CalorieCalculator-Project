using CalorieCalculator.ViewModels;

namespace CalorieCalculator.Views;

[QueryProperty(nameof(IsEdit), "edit")]
public partial class ProfileSetupPage : ContentPage
{
    private readonly ProfileSetupViewModel _viewModel;

    public string IsEdit
    {
        set
        {
            if (value == "true")
            {
                _ = _viewModel.LoadExistingProfileAsync();
            }
        }
    }

    public ProfileSetupPage(ProfileSetupViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }
}