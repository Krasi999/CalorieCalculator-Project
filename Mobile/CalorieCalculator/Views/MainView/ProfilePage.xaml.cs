using CalorieCalculator.ViewModels;

namespace CalorieCalculator.Views;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _viewModel;
    private bool _firstAppearing = true;

    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    private void OnAboutTapped(object? sender, EventArgs e)
    {
        var vm = BindingContext as ProfileViewModel;
        if (vm != null)
            vm.ToggleAboutCommand.Execute(null);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!_firstAppearing)
        {
            _viewModel.ReloadPhotoCommand.Execute(null);
        }
        _firstAppearing = false;

        NavBar.CurrentPage = "profile";
    }
}