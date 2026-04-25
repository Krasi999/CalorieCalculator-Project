using CalorieCalculator.Service;
using CalorieCalculator.Views;
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

    [ObservableProperty] private ImageSource profileImage;
    [ObservableProperty] private bool hasProfileImage;



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

            var photoPath = Preferences.Get("profile_photo_path", string.Empty);
            if (!string.IsNullOrEmpty(photoPath) && File.Exists(photoPath))
            {
                profileImage = ImageSource.FromFile(photoPath);
                OnPropertyChanged(nameof(ProfileImage));
                hasProfileImage = true;
                OnPropertyChanged(nameof(HasProfileImage));
            }
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
    private void CancelEditNickname()
    {
        editNickname = string.Empty;
        OnPropertyChanged(nameof(EditNickname));
        isEditingNickname = false;
        OnPropertyChanged(nameof(IsEditingNickname));
        errorMessage = string.Empty;
        OnPropertyChanged(nameof(ErrorMessage));
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
        if (!isBiometricEnabled)
        {
            var deviceSupports = await BiometricAuthenticator.IsAvailableAsync();
            if (!deviceSupports)
            {
                await Shell.Current.DisplayAlert(
                    "Недостъпно",
                    "Устройството ви не поддържа биометрична автентикация.",
                    "OK");
                return;
            }

            var confirm = await Shell.Current.DisplayAlert(
                "Биометричен вход",
                "Искате ли да използвате пръстов отпечатък като допълнителна стъпка при влизане?",
                "Да",
                "Не");

            if (!confirm) return;

            var authenticated = await BiometricAuthenticator.AuthenticateAsync(
                "Потвърдете самоличността си с пръстов отпечатък");

            if (!authenticated)
            {
                await Shell.Current.DisplayAlert(
                    "Неуспешно",
                    "Биометричната автентикация е неуспешна.",
                    "OK");
                return;
            }

            // Записваме локално ВЕДНАГА (за toggle бутона)
            isBiometricEnabled = true;
            Preferences.Set("biometric_enabled", true);
            OnPropertyChanged(nameof(IsBiometricEnabled));

            // После записваме и в базата
            var userId = Preferences.Get("user_id", string.Empty);
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    await _authService.SetBiometricAsync(Guid.Parse(userId), true);
                    System.Diagnostics.Debug.WriteLine("=== BIOMETRIC SAVED TO DB ===");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"!!! BIOMETRIC DB ERROR: {ex.Message}");
                }
            }

            await Shell.Current.DisplayAlert(
                "Успех",
                "Двуфакторната автентикация с пръстов отпечатък е активирана!",
                "OK");
        }
        else
        {
            var confirm = await Shell.Current.DisplayAlert(
                "Изключване",
                "Сигурни ли сте, че искате да изключите биометричния вход?",
                "Да",
                "Не");

            if (!confirm) return;

            isBiometricEnabled = false;
            Preferences.Set("biometric_enabled", false);
            OnPropertyChanged(nameof(IsBiometricEnabled));

            var userId = Preferences.Get("user_id", string.Empty);
            if (!string.IsNullOrEmpty(userId))
            {
                try
                {
                    await _authService.SetBiometricAsync(Guid.Parse(userId), false);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"!!! BIOMETRIC OFF ERROR: {ex.Message}");
                }
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
        //Preferences.Remove("biometric_enabled");

        await Shell.Current.GoToAsync("//Login");
    }

    private bool _isAboutVisible = false;
    public bool IsAboutVisible
    {
        get => _isAboutVisible;
        set { _isAboutVisible = value; OnPropertyChanged(); }
    }

    public ICommand ToggleAboutCommand => new Command(() => IsAboutVisible = !IsAboutVisible);


    [RelayCommand]
    private async Task ChangePhotoAsync()
    {
        var action = await Shell.Current.DisplayActionSheet(
            "Профилна снимка",
            "Отказ",
            null,
            "Направи селфи",
            "Премахни снимката");

        if (action == "Направи селфи")
        {
            await TakePhotoAsync();
        }
        else if (action == "Премахни снимката")
        {
            RemovePhoto();
        }
    }

    private async Task TakePhotoAsync()
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await Shell.Current.DisplayAlert("Грешка", "Камерата не е налична.", "OK");
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
            {
                Title = "Направи селфи"
            });

            if (photo != null)
            {
                // Уникално име с timestamp за да не кешира MAUI старата снимка
                var timestamp = DateTime.Now.Ticks;
                var fileName = $"profile_{Preferences.Get("user_id", "default")}_{timestamp}.jpg";
                var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                // Изтриваме старата снимка ако съществува
                var oldPath = Preferences.Get("profile_photo_path", string.Empty);
                if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                {
                    try { File.Delete(oldPath); } catch { }
                }

                // Копираме новата снимка
                using (var stream = await photo.OpenReadAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await stream.CopyToAsync(fileStream);
                }

                // Запазваме пътя
                Preferences.Set("profile_photo_path", filePath);

                // Обновяваме UI с нов ImageSource
                profileImage = ImageSource.FromFile(filePath);
                OnPropertyChanged(nameof(ProfileImage));
                hasProfileImage = true;
                OnPropertyChanged(nameof(HasProfileImage));
            }
        }
        catch (PermissionException)
        {
            await Shell.Current.DisplayAlert(
                "Разрешение",
                "Моля, разрешете достъп до камерата от настройките на телефона.",
                "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Camera error: {ex.Message}");
            await Shell.Current.DisplayAlert("Грешка", "Неуспешно заснемане на снимка.", "OK");
        }
    }

    private void RemovePhoto()
    {
        var filePath = Preferences.Get("profile_photo_path", string.Empty);
        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        Preferences.Remove("profile_photo_path");
        profileImage = null;
        OnPropertyChanged(nameof(ProfileImage));
        hasProfileImage = false;
        OnPropertyChanged(nameof(HasProfileImage));
    }
}