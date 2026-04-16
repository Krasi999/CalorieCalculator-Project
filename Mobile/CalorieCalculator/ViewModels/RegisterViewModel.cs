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

    // ISO/IEC 27001 парола индикатори — обновяват се в реално време
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

    /// <summary>
    /// Извиква се автоматично при всяка промяна на Password property.
    /// Обновява индикаторите за изискванията на паролата в реално време.
    /// </summary>
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

    /// <summary>
    /// Проверява дали паролата покрива всички ISO изисквания.
    /// Преизползваем метод — може да се извика и от други части на проекта.
    /// </summary>
    public bool IsPasswordValid => HasMinLength && HasUpperCase && HasLowerCase && HasDigit && HasSpecialChar;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = string.Empty;

        // Валидация на имейл
        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "Моля, въведете имейл адрес.";
            return;
        }

        if (!Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            ErrorMessage = "Моля, въведете валиден имейл адрес.";
            return;
        }

        // Валидация на паролата по ISO/IEC 27001
        if (!IsPasswordValid)
        {
            ErrorMessage = "Паролата не покрива всички изисквания.";
            return;
        }

        // Проверка дали паролите съвпадат
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

            await Shell.Current.DisplayAlert(
                "Успешна регистрация",
                "Акаунтът ви е създаден. Моля, влезте с вашите данни.",
                "OK");

            await Shell.Current.GoToAsync("//Login");
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