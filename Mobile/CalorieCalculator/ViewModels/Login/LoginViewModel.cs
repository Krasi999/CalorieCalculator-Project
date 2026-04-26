using CalorieCalculator.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CalorieCalculator.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly AuthApiService _authService;

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string password = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy = false;
    [ObservableProperty] private bool rememberMe = false;
    [ObservableProperty] private bool isPasswordVisible;

    public LoginViewModel(AuthApiService authService)
    {
        _authService = authService;
        LoadSavedEmail();
    }

    private void LoadSavedEmail()
    {
        var savedEmail = Preferences.Get("saved_email", string.Empty);
        if (!string.IsNullOrEmpty(savedEmail))
        {
            Email = savedEmail;
            RememberMe = true;
        }
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Моля, въведете имейл и парола.";
            return;
        }

        if (Email != Email.ToLowerInvariant())
        {
            ErrorMessage = "Имейлът трябва да съдържа само малки букви.";
            return;
        }

        IsBusy = true;

        try
        {
            var (success, error, data) = await _authService.LoginAsync(Email, Password);

            if (!success || data == null)
            {
                ErrorMessage = error ?? "Грешка при вход. Проверете данните си.";
                return;
            }

            if (RememberMe)
                Preferences.Set("saved_email", Email);
            else
                Preferences.Remove("saved_email");

            var biometricEnabled = Preferences.Get($"biometric_enabled_{data.UserId}", false);
            var deviceSupports = await BiometricAuthenticator.IsAvailableAsync();

            if (biometricEnabled && deviceSupports)
            {
                var fingerResult = await BiometricAuthenticator.AuthenticateAsync(
                    "Потвърдете самоличността си с пръстов отпечатък");

                if (!fingerResult)
                {
                    ErrorMessage = "Биометричната автентикация е неуспешна. Опитайте отново.";
                    return;
                }
            }

            Preferences.Set("auth_token", data.Token);
            Preferences.Set("user_id", data.UserId.ToString());
            Preferences.Set("last_password_login", DateTime.UtcNow.ToString("O"));

            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Възникна неочаквана грешка. Опитайте отново.";
            System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToRegister()
    {
        await Shell.Current.GoToAsync("//Register");
    }

    [RelayCommand]
    private async Task ForgotPassword()
    {
        await Shell.Current.GoToAsync("//ForgotPassword");
    }
}