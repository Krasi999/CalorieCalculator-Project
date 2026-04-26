using CalorieCalculator.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
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

    [ObservableProperty] private bool hasMinLength;
    [ObservableProperty] private bool hasUpperCase;
    [ObservableProperty] private bool hasLowerCase;
    [ObservableProperty] private bool hasDigit;
    [ObservableProperty] private bool hasSpecialChar;
    [ObservableProperty] private bool isPasswordVisible;
    [ObservableProperty] private bool isConfirmPasswordVisible;

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }

    [RelayCommand]
    private void ToggleConfirmPasswordVisibility()
    {
        IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
    }



    public RegisterViewModel(AuthApiService authService)
    {
        _authService = authService;
    }

    partial void OnPasswordChanged(string value)
    {
        ValidatePasswordRequirements(value);
    }

    private void ValidatePasswordRequirements(string pwd)
    {
        HasMinLength = !string.IsNullOrEmpty(pwd) && pwd.Length >= 8;
        HasUpperCase = Regex.IsMatch(pwd ?? string.Empty, @"[A-Z]");
        HasLowerCase = Regex.IsMatch(pwd ?? string.Empty, @"[a-z]");
        HasDigit = Regex.IsMatch(pwd ?? string.Empty, @"\d");
        HasSpecialChar = Regex.IsMatch(pwd ?? string.Empty, @"[!@#$%^&*()\-_=+\[\]{};:'"",.<>?/\\|`~]");
    }

    public bool IsPasswordValid => HasMinLength && HasUpperCase && HasLowerCase && HasDigit && HasSpecialChar;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Моля, въведете имейл адрес.";
            return;
        }

        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            ErrorMessage = "Моля, въведете валиден имейл адрес или парола.";
            return;
        }

        if (Email != Email.ToLowerInvariant())
        {
            ErrorMessage = "Имейлът трябва да съдържа само малки букви.";
            return;
        }

        if (!IsPasswordValid)
        {
            ErrorMessage = "Паролата не покрива всички изисквания.";
            return;
        }

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
                ErrorMessage = error ?? "Грешка при регистрация. Опитайте отново.";
                return;
            }

            Preferences.Set("user_id", userId.ToString()!);
            // Изчистваме биометрия за новия потребител
            Preferences.Set($"biometric_enabled_{userId}", false);

            await Shell.Current.GoToAsync("//ProfileSetup");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Възникна неочаквана грешка. Опитайте отново.";
            System.Diagnostics.Debug.WriteLine($"Register error: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToLogin()
    {
        await Shell.Current.GoToAsync("//Login");
    }
}