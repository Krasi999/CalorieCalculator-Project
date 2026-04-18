using CalorieCalculator.ViewModels;

namespace CalorieCalculator.Views;

public partial class ProfileSetupPage : ContentPage
{
    public ProfileSetupPage(ProfileSetupViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}