using CalorieCalculator.ViewModels;

namespace CalorieCalculator.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private void OnAboutTapped(object? sender, EventArgs e)
    {
        var vm = BindingContext as ProfileViewModel;
        if (vm != null)
            vm.ToggleAboutCommand.Execute(null);
    }

}