using CalorieCalculator.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualStudio.PlatformUI;
using ObservableObject = CommunityToolkit.Mvvm.ComponentModel.ObservableObject;

namespace CalorieCalculator.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthApiService _authService;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy = false;

    public LoginViewModel(AuthApiService authService)
    {
        _authService = authService;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;
        IsBusy = true;

        try
        {
            var (success, error, data) = await _authService.LoginAsync(Email, Password);

            if (!success || data == null)
            {
                ErrorMessage = error ?? "Грешка при вход.";
                return;
            }

            Preferences.Set("auth_token", data.Token);
            Preferences.Set("user_id", data.UserId.ToString());
            Preferences.Set("last_password_login", DateTime.UtcNow.ToString("O"));

            await Shell.Current.GoToAsync("//Dashboard");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task BiometricLoginAsync()
    {
        ErrorMessage = string.Empty;

        if (AuthApiService.RequiresPasswordReauth())
        {
            ErrorMessage = "Изминаха 72 часа. Моля, влезте с имейл и парола.";
            return;
        }

        var result = await BiometricAuthenticator.AuthenticateAsync(
            "Потвърди самоличността си");

        if (!result)
        {
            ErrorMessage = "Биометричната автентикация неуспешна.";
            return;
        }

        await Shell.Current.GoToAsync("//Dashboard");
    }
}