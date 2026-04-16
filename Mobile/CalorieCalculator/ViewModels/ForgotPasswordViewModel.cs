using CalorieCalculator.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;

namespace CalorieCalculator.ViewModels;

public partial class ForgotPasswordViewModel : ObservableObject
{
    private readonly AuthApiService _authService;
    private System.Timers.Timer? _countdownTimer;
    private string _resetCode = string.Empty;  // Кодът получен от API

    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy = false;

    // Стъпки
    [ObservableProperty] private bool isStep1Visible = true;
    [ObservableProperty] private bool isStep2Visible = false;
    [ObservableProperty] private bool isStep3Visible = false;

    // 6-цифрен код — всяка цифра отделно
    [ObservableProperty] private string digit1 = string.Empty;
    [ObservableProperty] private string digit2 = string.Empty;
    [ObservableProperty] private string digit3 = string.Empty;
    [ObservableProperty] private string digit4 = string.Empty;
    [ObservableProperty] private string digit5 = string.Empty;
    [ObservableProperty] private string digit6 = string.Empty;

    // Timer
    [ObservableProperty] private string timerText = "05:00";
    [ObservableProperty] private bool isTimerExpired = false;

    // Стъпка 3 — Нова парола
    [ObservableProperty] private string newPassword = string.Empty;
    [ObservableProperty] private string confirmPassword = string.Empty;
    [ObservableProperty] private bool isPasswordVisible = false;
    [ObservableProperty] private bool isConfirmPasswordVisible = false;

    // ISO/IEC 27001 индикатори
    [ObservableProperty] private bool hasMinLength;
    [ObservableProperty] private bool hasUpperCase;
    [ObservableProperty] private bool hasLowerCase;
    [ObservableProperty] private bool hasDigit;
    [ObservableProperty] private bool hasSpecialChar;

    public ForgotPasswordViewModel(AuthApiService authService)
    {
        _authService = authService;
    }

    partial void OnNewPasswordChanged(string value)
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

    // ==========================
    // СТЪПКА 1 — Въвеждане на имейл
    // ==========================

    [RelayCommand]
    private async Task RequestCodeAsync()
    {
        ErrorMessage = string.Empty;

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

        IsBusy = true;

        try
        {
            var (success, error, code) = await _authService.ForgotPasswordAsync(Email);

            if (!success || code == null)
            {
                ErrorMessage = error ?? "Грешка. Опитайте отново.";
                return;
            }

            _resetCode = code;

            // Показваме кода в dialog (симулация на email)
            // TODO: По-късно — замени с реално пращане по email
            await Shell.Current.DisplayAlert(
                "Код за възстановяване",
                $"Вашият код за възстановяване е:\n\n{code}\n\nКодът е валиден 5 минути.",
                "Разбрах");

            // Преминаваме към стъпка 2
            IsStep1Visible = false;
            IsStep2Visible = true;

            StartCountdown();
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ==========================
    // СТЪПКА 2 — Въвеждане на код
    // ==========================

    [RelayCommand]
    private async Task VerifyCodeAsync()
    {
        ErrorMessage = string.Empty;

        var enteredCode = $"{Digit1}{Digit2}{Digit3}{Digit4}{Digit5}{Digit6}";

        if (enteredCode.Length != 6)
        {
            ErrorMessage = "Моля, въведете пълния 6-цифрен код.";
            return;
        }

        if (IsTimerExpired)
        {
            ErrorMessage = "Кодът е изтекъл. Поискайте нов.";
            return;
        }

        IsBusy = true;

        try
        {
            var (success, error) = await _authService.VerifyCodeAsync(Email, enteredCode);

            if (!success)
            {
                ErrorMessage = error ?? "Невалиден код.";
                return;
            }

            // Спираме таймера и преминаваме към стъпка 3
            StopCountdown();
            IsStep2Visible = false;
            IsStep3Visible = true;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ResendCodeAsync()
    {
        ErrorMessage = string.Empty;
        IsBusy = true;

        try
        {
            var (success, error, code) = await _authService.ForgotPasswordAsync(Email);

            if (!success || code == null)
            {
                ErrorMessage = error ?? "Грешка при изпращане на нов код.";
                return;
            }

            _resetCode = code;

            // Изчистваме полетата за код
            Digit1 = Digit2 = Digit3 = Digit4 = Digit5 = Digit6 = string.Empty;

            // Показваме новия код
            await Shell.Current.DisplayAlert(
                "Нов код за възстановяване",
                $"Вашият нов код е:\n\n{code}\n\nКодът е валиден 5 минути.",
                "Разбрах");

            // Рестартираме таймера
            IsTimerExpired = false;
            StartCountdown();
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ==========================
    // СТЪПКА 3 — Нова парола
    // ==========================

    [RelayCommand]
    private async Task ResetPasswordAsync()
    {
        ErrorMessage = string.Empty;

        if (!IsPasswordValid)
        {
            ErrorMessage = "Паролата не покрива всички изисквания.";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "Паролите не съвпадат.";
            return;
        }

        IsBusy = true;

        try
        {
            var enteredCode = $"{Digit1}{Digit2}{Digit3}{Digit4}{Digit5}{Digit6}";

            var (success, error) = await _authService.ResetPasswordAsync(
                Email, enteredCode, NewPassword);

            if (!success)
            {
                ErrorMessage = error ?? "Грешка при смяна на паролата.";
                return;
            }

            await Shell.Current.DisplayAlert(
                "Успех",
                "Паролата е сменена успешно!\nМожете да влезете с новата парола.",
                "Към вход");

            await Shell.Current.GoToAsync("//Login");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // ==========================
    // Toggle и навигация
    // ==========================

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

    [RelayCommand]
    private async Task GoBackAsync()
    {
        if (IsStep3Visible)
        {
            IsStep3Visible = false;
            IsStep2Visible = true;
        }
        else if (IsStep2Visible)
        {
            StopCountdown();
            IsStep2Visible = false;
            IsStep1Visible = true;
            Digit1 = Digit2 = Digit3 = Digit4 = Digit5 = Digit6 = string.Empty;
            ErrorMessage = string.Empty;
        }
        else
        {
            await Shell.Current.GoToAsync("//Login");
        }
    }

    // ==========================
    // Timer логика
    // ==========================

    private void StartCountdown()
    {
        var endTime = DateTime.UtcNow.AddMinutes(5);

        _countdownTimer?.Stop();
        _countdownTimer = new System.Timers.Timer(1000);
        _countdownTimer.Elapsed += (s, e) =>
        {
            var remaining = endTime - DateTime.UtcNow;

            if (remaining <= TimeSpan.Zero)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    TimerText = "00:00";
                    IsTimerExpired = true;
                });

                _countdownTimer?.Stop();
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                TimerText = $"{remaining.Minutes:D2}:{remaining.Seconds:D2}";
            });
        };

        _countdownTimer.Start();
    }

    private void StopCountdown()
    {
        _countdownTimer?.Stop();
        _countdownTimer?.Dispose();
        _countdownTimer = null;
    }
}