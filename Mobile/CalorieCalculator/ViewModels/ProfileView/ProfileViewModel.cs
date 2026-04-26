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
            isBiometricEnabled = Preferences.Get($"biometric_enabled_{userId}", false);
            OnPropertyChanged(nameof(IsBiometricEnabled));

            var photoPath = Preferences.Get($"profile_photo_path_{userId}", string.Empty);
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
            var userId = Preferences.Get("user_id", string.Empty);
            Preferences.Set($"biometric_enabled_{userId}", true);
            OnPropertyChanged(nameof(IsBiometricEnabled));

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
            var userId = Preferences.Get("user_id", string.Empty);
            Preferences.Set($"biometric_enabled_{userId}", false);
            OnPropertyChanged(nameof(IsBiometricEnabled));

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
            // Искаме разрешение ПРЕДИ всичко друго
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Camera>();
                    if (status != PermissionStatus.Granted)
                    {
                        await Shell.Current.DisplayAlert(
                            "Разрешение",
                            "Моля, разрешете достъп до камерата от настройките.",
                            "OK");
                        return;
                    }
                    // Чакаме MAUI да се стабилизира
                    await Task.Delay(1000);
                }
            }
            catch { }

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

            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo == null) return;

            var userId = Preferences.Get("user_id", "default");
            var timestamp = DateTime.Now.Ticks;
            var fileName = $"profile_{userId}_{timestamp}.jpg";
            var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            // Изтриваме старата снимка
            try
            {
                var oldPath = Preferences.Get($"profile_photo_path_{userId}", string.Empty);
                if (!string.IsNullOrEmpty(oldPath) && File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }
            }
            catch { }

            // Копираме новата
            using (var sourceStream = await photo.OpenReadAsync())
            using (var destStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await sourceStream.CopyToAsync(destStream);
            }

            Preferences.Set($"profile_photo_path_{userId}", filePath);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                profileImage = ImageSource.FromFile(filePath);
                OnPropertyChanged(nameof(ProfileImage));
                hasProfileImage = true;
                OnPropertyChanged(nameof(HasProfileImage));
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"!!! Camera error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"!!! Stack: {ex.StackTrace}");
        }
    }

    private void RemovePhoto()
    {
        var userId = Preferences.Get("user_id", "default");
        var filePath = Preferences.Get($"profile_photo_path_{userId}", string.Empty);

        if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
        {
            try { File.Delete(filePath); } catch { }
        }

        Preferences.Remove($"profile_photo_path_{userId}");
        profileImage = null;
        OnPropertyChanged(nameof(ProfileImage));
        hasProfileImage = false;
        OnPropertyChanged(nameof(HasProfileImage));
    }

    [RelayCommand]
    private async Task EditProfileAsync()
    {
        await Shell.Current.GoToAsync("//ProfileSetup?edit=true");
    }

    [RelayCommand]
    private async Task EditHeightAsync()
    {
        var result = await Shell.Current.DisplayPromptAsync(
            "Ръст",
            "Въведете ръст в сантиметри:",
            "OK", "Отказ",
            keyboard: Microsoft.Maui.Keyboard.Numeric,
            initialValue: heightText.Replace(" см", ""));

        if (string.IsNullOrEmpty(result)) return;

        if (decimal.TryParse(result, out var newHeight) && newHeight >= 100 && newHeight <= 250)
        {
            await UpdateProfileFieldAsync("HeightCm", newHeight);
            heightText = $"{newHeight:F0} см";
            OnPropertyChanged(nameof(HeightText));
        }
        else
        {
            await Shell.Current.DisplayAlert("Грешка", "Моля, въведете валиден ръст (100-250 см).", "OK");
        }
    }

    [RelayCommand]
    private async Task EditWeightAsync()
    {
        var result = await Shell.Current.DisplayPromptAsync(
            "Тегло",
            "Въведете тегло в килограми:",
            "OK", "Отказ",
            keyboard: Microsoft.Maui.Keyboard.Numeric,
            initialValue: weightText.Replace(" кг", ""));

        if (string.IsNullOrEmpty(result)) return;

        if (decimal.TryParse(result, out var newWeight) && newWeight >= 30 && newWeight <= 250)
        {
            await UpdateProfileFieldAsync("WeightKg", newWeight);
            weightText = $"{newWeight:F0} кг";
            OnPropertyChanged(nameof(WeightText));
        }
        else
        {
            await Shell.Current.DisplayAlert("Грешка", "Моля, въведете валидно тегло (30-250 кг).", "OK");
        }
    }

    [RelayCommand]
    private async Task EditTargetWeightAsync()
    {
        var result = await Shell.Current.DisplayPromptAsync(
            "Желано тегло",
            "Въведете желано тегло в килограми:",
            "OK", "Отказ",
            keyboard: Microsoft.Maui.Keyboard.Numeric,
            initialValue: targetWeightText.Replace(" кг", ""));

        if (string.IsNullOrEmpty(result)) return;

        if (!decimal.TryParse(result, out var newTarget) || newTarget < 30 || newTarget > 250)
        {
            await Shell.Current.DisplayAlert("Грешка", "Моля, въведете валидно тегло (30-250 кг).", "OK");
            return;
        }

        var currentWeight = decimal.Parse(weightText.Replace(" кг", ""));
        var validationError = ValidateWeightVsGoal(currentWeight, newTarget);
        if (validationError != null)
        {
            await Shell.Current.DisplayAlert("Грешка", validationError, "OK");
            return;
        }

        await UpdateProfileFieldAsync("TargetWeightKg", newTarget);
        targetWeightText = $"{newTarget:F0} кг";
        OnPropertyChanged(nameof(TargetWeightText));
    }

    [RelayCommand]
    private void ReloadPhoto()
    {
        var userId = Preferences.Get("user_id", string.Empty);
        if (string.IsNullOrEmpty(userId)) return;

        var photoPath = Preferences.Get($"profile_photo_path_{userId}", string.Empty);
        if (!string.IsNullOrEmpty(photoPath) && File.Exists(photoPath))
        {
            profileImage = ImageSource.FromFile(photoPath);
            OnPropertyChanged(nameof(ProfileImage));
            hasProfileImage = true;
            OnPropertyChanged(nameof(HasProfileImage));
        }
    }

    [RelayCommand]
    private async Task EditActivityAsync()
    {
        var result = await Shell.Current.DisplayActionSheet(
            "Ниво на активност",
            "Отказ",
            null,
            "Заседнал",
            "Леко активен",
            "Умерено активен",
            "Много активен",
            "Изключително активен");

        if (string.IsNullOrEmpty(result) || result == "Отказ") return;

        var level = result switch
        {
            "Заседнал" => 1,
            "Леко активен" => 2,
            "Умерено активен" => 3,
            "Много активен" => 4,
            "Изключително активен" => 5,
            _ => 1
        };

        await UpdateProfileFieldAsync("ActivityLevel", level);
        activityText = result;
        OnPropertyChanged(nameof(ActivityText));
    }

    [RelayCommand]
    private async Task EditGoalAsync()
    {
        var result = await Shell.Current.DisplayActionSheet(
            "Цел",
            "Отказ",
            null,
            "Загуба на тегло",
            "Задържане на теглото",
            "Качване на тегло",
            "Качване на мускулна маса");

        if (string.IsNullOrEmpty(result) || result == "Отказ") return;

        var goal = result switch
        {
            "Загуба на тегло" => 1,
            "Задържане на теглото" => 2,
            "Качване на тегло" => 3,
            "Качване на мускулна маса" => 4,
            _ => 2
        };

        // Проверка спрямо текущото и желаното тегло
        var currentWeight = decimal.Parse(weightText.Replace(" кг", ""));
        var targetWeight = decimal.Parse(targetWeightText.Replace(" кг", ""));
        var validationError = ValidateGoalVsWeights(goal, currentWeight, targetWeight);
        if (validationError != null)
        {
            await Shell.Current.DisplayAlert("Грешка", validationError, "OK");
            return;
        }

        await UpdateProfileFieldAsync("CurrentGoal", goal);
        goalText = result;
        OnPropertyChanged(nameof(GoalText));
    }

    private string? ValidateWeightVsGoal(decimal currentWeight, decimal targetWeight)
    {
        // Определяме текущата цел
        var goalCode = goalText switch
        {
            "Загуба на тегло" => 1,
            "Задържане на теглото" => 2,
            "Качване на тегло" => 3,
            "Качване на мускулна маса" => 4,
            _ => 0
        };

        return ValidateGoalVsWeights(goalCode, currentWeight, targetWeight);
    }

    /// <summary>
    /// Проверява дали целта е съвместима с теглата.
    /// </summary>
    private string? ValidateGoalVsWeights(int goalCode, decimal currentWeight, decimal targetWeight)
    {
        switch (goalCode)
        {
            case 1: // Загуба на тегло
                if (targetWeight >= currentWeight)
                    return $"При цел \"Загуба на тегло\" желаното тегло ({targetWeight:F0} кг) трябва да е по-малко от текущото ({currentWeight:F0} кг).";
                break;

            case 2: // Задържане на теглото
                if (targetWeight != currentWeight)
                    return $"При цел \"Задържане на теглото\" желаното тегло трябва да е равно на текущото ({currentWeight:F0} кг).";
                break;

            case 3: // Качване на тегло
                if (targetWeight <= currentWeight)
                    return $"При цел \"Качване на тегло\" желаното тегло ({targetWeight:F0} кг) трябва да е по-голямо от текущото ({currentWeight:F0} кг).";
                break;

            case 4: // Качване на мускулна маса
                if (targetWeight <= currentWeight)
                    return $"При цел \"Качване на мускулна маса\" желаното тегло ({targetWeight:F0} кг) трябва да е по-голямо от текущото ({currentWeight:F0} кг).";
                break;
        }

        return null; // Няма грешка
    }

    /// <summary>
    /// Обновява едно поле в базата данни.
    /// </summary>
    private async Task UpdateProfileFieldAsync(string fieldName, object value)
    {
        try
        {
            var userId = Preferences.Get("user_id", string.Empty);
            if (string.IsNullOrEmpty(userId)) return;

            await _api.PostAsync("api/UserDetails/update-field",
                new
                {
                    UserID = Guid.Parse(userId),
                    FieldName = fieldName,
                    Value = value.ToString()
                });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Update field error: {ex.Message}");
            await Shell.Current.DisplayAlert("Грешка", "Неуспешно обновяване на данните.", "OK");
        }
    }
}