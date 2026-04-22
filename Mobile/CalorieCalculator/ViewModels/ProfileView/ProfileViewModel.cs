using CalorieCalculator.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using System.Windows.Input;

namespace CalorieCalculator.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly ApiService _api;
    private readonly AuthApiService _authService;

    public ProfileViewModel(ApiService api, AuthApiService authService)
    {
        _api = api;
        _authService = authService;
        _ = LoadProfileAsync();
    }

    [ObservableProperty] private string nickname = string.Empty;
    [ObservableProperty] private string profileInitial = "?";
    [ObservableProperty] private Color profileColor = Color.FromArgb("#3B82F6");
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage = string.Empty;

    [ObservableProperty] private string genderText = string.Empty;
    [ObservableProperty] private string ageText = string.Empty;
    [ObservableProperty] private string heightText = string.Empty;
    [ObservableProperty] private string weightText = string.Empty;
    [ObservableProperty] private string activityText = string.Empty;
    [ObservableProperty] private string goalText = string.Empty;
    [ObservableProperty] private string targetWeightText = string.Empty;

    [ObservableProperty] private bool isBiometricEnabled;
    [ObservableProperty] private bool isBiometricAvailable;

    [ObservableProperty] private bool isDetailsVisible;

    [ObservableProperty] private string todayDate = string.Empty;
    [ObservableProperty] private string dayOfWeek = string.Empty;
    [ObservableProperty] private string monthYear = string.Empty;

    [ObservableProperty] private bool isEditingNickname;
    [ObservableProperty] private string editNickname = string.Empty;

    [ObservableProperty] private bool isDay1Today;
    [ObservableProperty] private bool isDay2Today;
    [ObservableProperty] private bool isDay3Today;
    [ObservableProperty] private bool isDay4Today;
    [ObservableProperty] private bool isDay5Today;
    [ObservableProperty] private bool isDay6Today;
    [ObservableProperty] private bool isDay7Today;
    


    private async Task LoadProfileAsync()
    {
        isBusy = true;
        OnPropertyChanged(nameof(IsBusy));

        try
        {
            var userId = Preferences.Get("user_id", string.Empty);
            if (string.IsNullOrEmpty(userId)) return;

            var response = await _api.GetSingleAsync<JsonElement>(
                $"api/UserDetails/{userId}");

            if (response.ValueKind != JsonValueKind.Undefined)
            {
                var nick = response.GetProperty("nickname").GetString() ?? "Потребител";
                nickname = nick;
                OnPropertyChanged(nameof(Nickname));

                profileInitial = nick.Length > 0 ? nick[0].ToString().ToUpper() : "?";
                OnPropertyChanged(nameof(ProfileInitial));

                var gender = response.GetProperty("gender").GetInt32();
                profileColor = gender == 2
                    ? Color.FromArgb("#EC4899") 
                    : Color.FromArgb("#3B82F6");
                OnPropertyChanged(nameof(ProfileColor));

                genderText = gender == 1 ? "Мъж" : "Жена";
                OnPropertyChanged(nameof(GenderText));

                var dob = DateTime.Parse(response.GetProperty("dateOfBirth").GetString() ?? "");
                var age = DateTime.UtcNow.Year - dob.Year;
                ageText = $"{age} години";
                OnPropertyChanged(nameof(AgeText));

                var heightCm = response.GetProperty("heightCm").GetDecimal();
                heightText = $"{heightCm:F0} см";
                OnPropertyChanged(nameof(HeightText));

                var weightKg = response.GetProperty("weightKg").GetDecimal();
                weightText = $"{weightKg:F0} кг";
                OnPropertyChanged(nameof(WeightText));

                var activity = response.GetProperty("activityLevel").GetInt32();
                activityText = activity switch
                {
                    1 => "Заседнал",
                    2 => "Леко активен",
                    3 => "Умерено активен",
                    4 => "Много активен",
                    5 => "Изключително активен",
                    _ => "Неизвестно"
                };
                OnPropertyChanged(nameof(ActivityText));

                var goal = response.GetProperty("currentGoal").GetInt32();
                goalText = goal switch
                {
                    1 => "Загуба на тегло",
                    2 => "Задържане на теглото",
                    3 => "Качване на тегло",
                    4 => "Качване на мускулна маса",
                    _ => "Неизвестно"
                };
                OnPropertyChanged(nameof(GoalText));

                var targetKg = response.GetProperty("targetWeightKg").GetDecimal();
                targetWeightText = $"{targetKg:F0} кг";
                OnPropertyChanged(nameof(TargetWeightText));
            }

            isBiometricAvailable = await BiometricAuthenticator.IsAvailableAsync();
            OnPropertyChanged(nameof(IsBiometricAvailable));
            isBiometricEnabled = Preferences.Get("biometric_enabled", false);
            OnPropertyChanged(nameof(IsBiometricEnabled));

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Profile load error: {ex.Message}");
        }
        finally
        {
            isBusy = false;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    [RelayCommand]
    private void ToggleDetails()
    {
        isDetailsVisible = !isDetailsVisible;
        OnPropertyChanged(nameof(IsDetailsVisible));
    }

    [RelayCommand]
    private void StartEditNickname()
    {
        editNickname = nickname;
        OnPropertyChanged(nameof(EditNickname));
        isEditingNickname = true;
        OnPropertyChanged(nameof(IsEditingNickname));
    }

    [RelayCommand]
    private async Task SaveNicknameAsync()
    {
        if (string.IsNullOrWhiteSpace(editNickname) || editNickname.Length < 2)
        {
            errorMessage = "Псевдонимът трябва да е поне 2 символа.";
            OnPropertyChanged(nameof(ErrorMessage));
            return;
        }

        try
        {
            var userId = Preferences.Get("user_id", string.Empty);
            if (string.IsNullOrEmpty(userId)) return;

            await _api.PostAsync("api/UserDetails/update-nickname",
                new { UserID = Guid.Parse(userId), Nickname = editNickname.Trim() });

            nickname = editNickname.Trim();
            OnPropertyChanged(nameof(Nickname));
            profileInitial = nickname[0].ToString().ToUpper();
            OnPropertyChanged(nameof(ProfileInitial));

            isEditingNickname = false;
            OnPropertyChanged(nameof(IsEditingNickname));
            errorMessage = string.Empty;
            OnPropertyChanged(nameof(ErrorMessage));
        }
        catch (Exception ex)
        {
            errorMessage = "Грешка при запазване на псевдонима.";
            OnPropertyChanged(nameof(ErrorMessage));
            System.Diagnostics.Debug.WriteLine($"Nickname save error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ToggleBiometricAsync()
    {
        var newValue = !isBiometricEnabled;
        var userId = Preferences.Get("user_id", string.Empty);

        if (!string.IsNullOrEmpty(userId))
        {
            var success = await _authService.SetBiometricAsync(Guid.Parse(userId), newValue);
            if (success)
            {
                isBiometricEnabled = newValue;
                Preferences.Set("biometric_enabled", newValue);
                OnPropertyChanged(nameof(IsBiometricEnabled));
            }
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Изход",
            "Сигурни ли сте, че искате да излезете от профила си?",
            "Да",
            "Не");

        if (!confirm) return;

        Preferences.Remove("auth_token");
        Preferences.Remove("user_id");
        Preferences.Remove("last_password_login");
        Preferences.Remove("biometric_enabled");

        await Shell.Current.GoToAsync("//Login");
    }

    private bool _isAboutVisible = false;
    public bool IsAboutVisible
    {
        get => _isAboutVisible;
        set { _isAboutVisible = value; OnPropertyChanged(); }
    }

    public ICommand ToggleAboutCommand => new Command(() => IsAboutVisible = !IsAboutVisible);
}