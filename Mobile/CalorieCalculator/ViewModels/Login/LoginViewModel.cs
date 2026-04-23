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

    // Временно съхраняваме login данните за след биометрията
    private string? _pendingToken;
    private string? _pendingUserId;

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
    private void DismissBiometricPrompt()
    {
        showBiometricPrompt = false;
        OnPropertyChanged(nameof(ShowBiometricPrompt));

        // Ако потребителят откаже биометрията, пускаме го директно
        if (!string.IsNullOrEmpty(_pendingToken) && !string.IsNullOrEmpty(_pendingUserId))
        {
            CompletLogin();
        }
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

            // Запазваме данните временно
            _pendingToken = data.Token;
            _pendingUserId = data.UserId.ToString();

            if (RememberMe)
                Preferences.Set("saved_email", Email);
            else
                Preferences.Remove("saved_email");

            // Проверяваме дали потребителят има включена биометрия
            var biometricEnabled = Preferences.Get("biometric_enabled", false);
            var deviceSupports = await BiometricAuthenticator.IsAvailableAsync();

            if (biometricEnabled && deviceSupports)
            {
                // Показваме биометричен prompt като втора стъпка
                showBiometricPrompt = true;
                OnPropertyChanged(nameof(ShowBiometricPrompt));

                // Автоматично стартираме fingerprint
                await BiometricLoginAsync();
            }
            else
            {
                // Няма биометрия — влизаме директно
                CompletLogin();
            }
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

        var result = await BiometricAuthenticator.AuthenticateAsync(
            "Потвърдете самоличността си с пръстов отпечатък");

        if (!result)
        {
            // Не затваряме prompt-а, потребителят може да опита отново
            // или да натисне хиксчето/линка за вход с данни
            return;
        }

        // Успешна биометрична автентикация
        showBiometricPrompt = false;
        OnPropertyChanged(nameof(ShowBiometricPrompt));

        CompletLogin();
    }

    private async void CompletLogin()
    {
        if (!string.IsNullOrEmpty(_pendingToken) && !string.IsNullOrEmpty(_pendingUserId))
        {
            Preferences.Set("auth_token", _pendingToken);
            Preferences.Set("user_id", _pendingUserId);
            Preferences.Set("last_password_login", DateTime.UtcNow.ToString("O"));

            _pendingToken = null;
            _pendingUserId = null;

            await Shell.Current.GoToAsync("//MainPage");
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