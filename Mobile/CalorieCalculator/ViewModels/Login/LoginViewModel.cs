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
    [ObservableProperty] private bool isBiometricAvailable = false;
    [ObservableProperty] private bool isPasswordVisible;
    [ObservableProperty] private bool showBiometricPrompt = false;

    public LoginViewModel(AuthApiService authService)
    {
        _authService = authService;
        LoadSavedEmail();
        _ = CheckAndShowBiometricAsync();
    }

    private async Task CheckAndShowBiometricAsync()
    {
        var deviceSupports = await BiometricAuthenticator.IsAvailableAsync();
        var userEnabled = Preferences.Get("biometric_enabled", false);
        isBiometricAvailable = deviceSupports && userEnabled;
        OnPropertyChanged(nameof(IsBiometricAvailable));

        // Ако биометрията е налична и не са минали 72 часа → показваме prompt
        if (isBiometricAvailable && !AuthApiService.RequiresPasswordReauth())
        {
            showBiometricPrompt = true;
            OnPropertyChanged(nameof(showBiometricPrompt));

            // Автоматично стартираме биометрична автентикация
            await BiometricLoginAsync();
        }
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
    private void DismissBiometricPrompt()
    {
        showBiometricPrompt = false;
        OnPropertyChanged(nameof(showBiometricPrompt));
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

            Preferences.Set("auth_token", data.Token);
            Preferences.Set("user_id", data.UserId.ToString());
            Preferences.Set("last_password_login", DateTime.UtcNow.ToString("O"));

            if (RememberMe)
                Preferences.Set("saved_email", Email);
            else
                Preferences.Remove("saved_email");

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
    private async Task BiometricLoginAsync()
    {
        ErrorMessage = string.Empty;

        if (AuthApiService.RequiresPasswordReauth())
        {
            ErrorMessage = "Изминаха 72 часа. Моля, влезте с имейл и парола.";
            showBiometricPrompt = false;
            OnPropertyChanged(nameof(showBiometricPrompt));
            return;
        }

        var result = await BiometricAuthenticator.AuthenticateAsync(
            "Влезте с пръстов отпечатък");

        if (!result)
        {
            ErrorMessage = "Биометричната автентикация е неуспешна.";
            return;
        }

        // Успешна биометрична автентикация
        Preferences.Set("last_password_login", DateTime.UtcNow.ToString("O"));
        showBiometricPrompt = false;
        OnPropertyChanged(nameof(showBiometricPrompt));

        await Shell.Current.GoToAsync("//MainPage");
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