using CalorieCalculator.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.PlatformUI;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace CalorieCalculator.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly AuthApiService _authService;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy = false;

    public RegisterViewModel(AuthApiService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Паролите не съвпадат.";
            return;
        }

        IsBusy = true;

        try
        {
            var (success, error, userId) = await _authService.RegisterAsync(Email, Password);

            if (!success)
            {
                ErrorMessage = error ?? "Грешка при регистрация.";
                return;
            }

            Preferences.Set("user_id", userId.ToString()!);
            await Shell.Current.GoToAsync("//Login");
        }
        finally
        {
            IsBusy = false;
        }
    }
}