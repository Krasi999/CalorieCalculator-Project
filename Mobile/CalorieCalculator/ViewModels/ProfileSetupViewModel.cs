using CalorieCalculator.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace CalorieCalculator.ViewModels;

public record ProfileDataRequest(Guid UserID, string Nickname, int Gender, DateTime DateOfBirth, decimal? HeightCm, 
    decimal? HeightFt, decimal? WeightKg, decimal? WeightLbs, int ActivityLevel, int CurrentGoal, decimal? TargetWeightKg, decimal? TargetWeightLbs);

public record ProfileDataResponse(bool Success, Guid UserID);

public partial class ProfileSetupViewModel : ObservableObject
{
    private readonly ApiService _api;

    public ProfileSetupViewModel(ApiService api)
    {
        _api = api;
        UpdateStepInfo();
    }


    [ObservableProperty] private int currentStep = 1;
    [ObservableProperty] private double progress = 0.125;
    [ObservableProperty] private string stepTitle = string.Empty;
    [ObservableProperty] private string stepSubtitle = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    [ObservableProperty] private bool isStep1Visible = true;
    [ObservableProperty] private bool isStep2Visible;
    [ObservableProperty] private bool isStep3Visible;
    [ObservableProperty] private bool isStep4Visible;
    [ObservableProperty] private bool isStep5Visible;
    [ObservableProperty] private bool isStep6Visible;
    [ObservableProperty] private bool isStep7Visible;
    [ObservableProperty] private bool isStep8Visible;

    [ObservableProperty] private string nickname = string.Empty;

    [ObservableProperty] private int selectedGender;
    [ObservableProperty] private bool isMaleSelected;
    [ObservableProperty] private bool isFemaleSelected;

    [ObservableProperty] private int selectedAgeIndex = 11; 

    [ObservableProperty] private bool isHeightInCm = true;
    [ObservableProperty] private int selectedHeightCmIndex = 70;
    [ObservableProperty] private int selectedHeightFtIndex = 30;

    [ObservableProperty] private bool isWeightInKg = true;
    [ObservableProperty] private int selectedWeightKgIndex = 40;
    [ObservableProperty] private int selectedWeightLbsIndex = 88;

    [ObservableProperty] private int selectedActivityLevel = 1;

    [ObservableProperty] private bool isSedentarySelected;
    [ObservableProperty] private bool isLightlyActiveSelected;
    [ObservableProperty] private bool isModerateActiveSelected;
    [ObservableProperty] private bool isVeryActiveSelected;
    [ObservableProperty] private bool isExtraActiveSelected;

    [ObservableProperty] private int selectedGoalType = 1;

    [ObservableProperty] private bool isWeightLossSelected;
    [ObservableProperty] private bool isMaintenanceSelected;
    [ObservableProperty] private bool isWeightGainSelected;
    [ObservableProperty] private bool isMuscleGainSelected;

    [ObservableProperty] private bool isTargetWeightInKg = true;
    [ObservableProperty] private int selectedTargetWeightKgIndex = 35;
    [ObservableProperty] private int selectedTargetWeightLbsIndex = 77;

    [ObservableProperty] private bool isEditMode;

    public List<string> AgeOptions { get; } = Enumerable.Range(14, 87)
        .Select(a => $"{a} години").ToList();

    public List<string> HeightCmOptions { get; } = Enumerable.Range(100, 151)
        .Select(h => $"{h} см").ToList();

    public List<string> HeightFtOptions { get; } = GenerateFtOptions();

    public List<string> WeightKgOptions { get; } = Enumerable.Range(30, 221)
        .Select(w => $"{w} кг").ToList();

    public List<string> WeightLbsOptions { get; } = Enumerable.Range(66, 441)
        .Select(w => $"{w} lbs").ToList();

    private static List<string> GenerateFtOptions()
    {
        var options = new List<string>();
        for (int ft = 3; ft <= 7; ft++)
            for (int inch = 0; inch < 12; inch++)
            {
                options.Add($"{ft}'{inch}\"");
                if (ft == 7 && inch == 6) break;
            }
        return options;
    }

    [RelayCommand]
    private void SelectMale()
    {
        SelectedGender = 1;
        IsMaleSelected = true;
        IsFemaleSelected = false;
    }

    [RelayCommand]
    private void SelectFemale()
    {
        SelectedGender = 2;
        IsMaleSelected = false;
        IsFemaleSelected = true;
    }

    [RelayCommand]
    private void SelectHeightCm() => IsHeightInCm = true;

    [RelayCommand]
    private void SelectHeightFt() => IsHeightInCm = false;

    [RelayCommand]
    private void SelectWeightKg() => IsWeightInKg = true;

    [RelayCommand]
    private void SelectWeightLbs() => IsWeightInKg = false;

    [RelayCommand]
    private void SelectTargetWeightKg() => IsTargetWeightInKg = true;

    [RelayCommand]
    private void SelectTargetWeightLbs() => IsTargetWeightInKg = false;

    [RelayCommand]
    private void SelectActivityLevel(string level)
    {
        selectedActivityLevel = level switch
        {
            "Sedentary" => 1,
            "LightlyActive" => 2,
            "ModerateActive" => 3,
            "VeryActive" => 4,
            "ExtraActive" => 5,
            _ => 1
        };
        OnPropertyChanged(nameof(SelectedActivityLevel));

        isSedentarySelected = level == "Sedentary";
        isLightlyActiveSelected = level == "LightlyActive";
        isModerateActiveSelected = level == "ModerateActive";
        isVeryActiveSelected = level == "VeryActive";
        isExtraActiveSelected = level == "ExtraActive";
        OnPropertyChanged(nameof(IsSedentarySelected));
        OnPropertyChanged(nameof(IsLightlyActiveSelected));
        OnPropertyChanged(nameof(IsModerateActiveSelected));
        OnPropertyChanged(nameof(IsVeryActiveSelected));
        OnPropertyChanged(nameof(IsExtraActiveSelected));
    }

    [RelayCommand]
    private void SelectGoalType(string goal)
    {
        selectedGoalType = goal switch
        {
            "WeightLoss" => 1,
            "Maintenance" => 2,
            "WeightGain" => 3,
            "MuscleGain" => 4,
            _ => 2
        };
        OnPropertyChanged(nameof(SelectedGoalType));

        isWeightLossSelected = goal == "WeightLoss";
        isMaintenanceSelected = goal == "Maintenance";
        isWeightGainSelected = goal == "WeightGain";
        isMuscleGainSelected = goal == "MuscleGain";
        OnPropertyChanged(nameof(IsWeightLossSelected));
        OnPropertyChanged(nameof(IsMaintenanceSelected));
        OnPropertyChanged(nameof(IsWeightGainSelected));
        OnPropertyChanged(nameof(IsMuscleGainSelected));
    }

    [RelayCommand]
    private async Task ContinueAsync()
    {
        ErrorMessage = string.Empty;
        if (!ValidateCurrentStep()) return;

        if (CurrentStep < 8)
        {
            SetStepVisibility(CurrentStep, false);
            CurrentStep++;
            SetStepVisibility(CurrentStep, true);
            Progress = CurrentStep / 8.0;
            UpdateStepInfo();
        }
        else
        {
            await SaveProfileAsync();
        }
    }

    [RelayCommand]
    private void GoBack()
    {
        if (CurrentStep > 1)
        {
            SetStepVisibility(CurrentStep, false);
            CurrentStep--;
            SetStepVisibility(CurrentStep, true);
            Progress = CurrentStep / 8.0;
            UpdateStepInfo();
            ErrorMessage = string.Empty;
        }
    }

    private bool ValidateCurrentStep()
    {
        errorMessage = string.Empty;
        OnPropertyChanged(nameof(ErrorMessage));

        switch (currentStep)
        {
            case 1:
                if (string.IsNullOrWhiteSpace(nickname) || nickname.Length < 2)
                {
                    errorMessage = "Псевдонимът трябва да е поне 2 символа.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }
                break;

            case 2:
                if (!isMaleSelected && !isFemaleSelected)
                {
                    errorMessage = "Моля, изберете пол.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }
                break;

            case 3:
                if (selectedAgeIndex < 0)
                {
                    errorMessage = "Моля, изберете възраст.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }
                break;

            case 4:
                if (isHeightInCm && selectedHeightCmIndex < 0)
                {
                    errorMessage = "Моля, изберете ръст.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }
                if (!isHeightInCm && selectedHeightFtIndex < 0)
                {
                    errorMessage = "Моля, изберете ръст.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }
                break;

            case 5:
                if (isWeightInKg && selectedWeightKgIndex < 0)
                {
                    errorMessage = "Моля, изберете тегло.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }
                if (!isWeightInKg && selectedWeightLbsIndex < 0)
                {
                    errorMessage = "Моля, изберете тегло.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }
                break;

            case 6:
                if (!isSedentarySelected && !isLightlyActiveSelected &&
                    !isModerateActiveSelected && !isVeryActiveSelected &&
                    !isExtraActiveSelected)
                {
                    errorMessage = "Моля, изберете ниво на активност.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }
                break;

            case 7:
                if (!isWeightLossSelected && !isMaintenanceSelected &&
                    !isWeightGainSelected && !isMuscleGainSelected)
                {
                    errorMessage = "Моля, изберете цел.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }
                break;

            case 8:
                if (isTargetWeightInKg && selectedTargetWeightKgIndex < 0)
                {
                    errorMessage = "Моля, изберете желано тегло.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }
                if (!isTargetWeightInKg && selectedTargetWeightLbsIndex < 0)
                {
                    errorMessage = "Моля, изберете желано тегло.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    return false;
                }

                var currentWeightKg = isWeightInKg
                    ? selectedWeightKgIndex + 30
                    : (int)Math.Round((selectedWeightLbsIndex + 66) / 2.20462);

                var targetWeightKg = isTargetWeightInKg
                    ? selectedTargetWeightKgIndex + 30
                    : (int)Math.Round((selectedTargetWeightLbsIndex + 66) / 2.20462);

                switch (selectedGoalType)
                {
                    case 1: 
                        if (targetWeightKg >= currentWeightKg)
                        {
                            errorMessage = $"При цел \"Загуба на тегло\" желаното тегло ({targetWeightKg} кг) трябва да е по-малко от текущото ({currentWeightKg} кг).";
                            OnPropertyChanged(nameof(ErrorMessage));
                            return false;
                        }
                        break;

                    case 2: 
                        if (targetWeightKg != currentWeightKg)
                        {
                            errorMessage = $"При цел \"Задържане на теглото\" желаното тегло трябва да е равно на текущото ({currentWeightKg} кг).";
                            OnPropertyChanged(nameof(ErrorMessage));
                            return false;
                        }
                        break;

                    case 3: 
                        if (targetWeightKg <= currentWeightKg)
                        {
                            errorMessage = $"При цел \"Качване на тегло\" желаното тегло ({targetWeightKg} кг) трябва да е по-голямо от текущото ({currentWeightKg} кг).";
                            OnPropertyChanged(nameof(ErrorMessage));
                            return false;
                        }
                        break;

                    case 4: 
                        if (targetWeightKg <= currentWeightKg)
                        {
                            errorMessage = $"При цел \"Качване на мускулна маса\" желаното тегло ({targetWeightKg} кг) трябва да е по-голямо от текущото ({currentWeightKg} кг).";
                            OnPropertyChanged(nameof(ErrorMessage));
                            return false;
                        }
                        break;
                }
                break;
        }
        return true;
    }

    private void SetStepVisibility(int step, bool visible)
    {
        switch (step)
        {
            case 1: IsStep1Visible = visible; break;
            case 2: IsStep2Visible = visible; break;
            case 3: IsStep3Visible = visible; break;
            case 4: IsStep4Visible = visible; break;
            case 5: IsStep5Visible = visible; break;
            case 6: IsStep6Visible = visible; break;
            case 7: IsStep7Visible = visible; break;
            case 8: IsStep8Visible = visible; break;
        }
    }

    private void UpdateStepInfo()
    {
        (StepTitle, StepSubtitle) = CurrentStep switch
        {
            1 => ("Как искаш да те наричаме?", "Въведи псевдоним за профила си."),
            2 => ("Избери своя пол", "Помага ни да изчислим калориите по-точно."),
            3 => ("На колко години си?", "Използваме го за персонализация на плана."),
            4 => ("Колко си висок?", "Използваме го за персонализация на плана."),
            5 => ("Колко тежиш?", "Помага ни да изчислим дневния ти калориен прием."),
            6 => ("Колко активен си?", "Дневното ти ниво на активност помага за по-точно изчисление."),
            7 => ("Каква е целта ти?", "Избери какво искаш да постигнеш."),
            8 => ("Задай желаното тегло", "Помага ни да създадем персонализиран план."),
            _ => ("", "")
        };
    }

    private async Task SaveProfileAsync()
    {
        isBusy = true;
        OnPropertyChanged(nameof(IsBusy));

        try
        {
            var userId = Preferences.Get("user_id", string.Empty);
            System.Diagnostics.Debug.WriteLine($"=== USER ID: '{userId}' ===");

            if (string.IsNullOrEmpty(userId))
            {
                errorMessage = "Грешка: не е намерен потребител.";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            var userIdGuid = Guid.Parse(userId);
            var age = selectedAgeIndex + 14;
            var dateOfBirth = DateTime.UtcNow.AddYears(-age);

            System.Diagnostics.Debug.WriteLine($"=== Nickname: {nickname}, Gender: {selectedGender}, Age: {age} ===");
            System.Diagnostics.Debug.WriteLine($"=== HeightCm: {selectedHeightCmIndex + 100}, WeightKg: {selectedWeightKgIndex + 30} ===");
            System.Diagnostics.Debug.WriteLine($"=== Activity: {selectedActivityLevel}, Goal: {selectedGoalType} ===");

            var profileData = new ProfileDataRequest(
                userIdGuid,
                nickname.Trim(),
                selectedGender,
                dateOfBirth,
                isHeightInCm ? (selectedHeightCmIndex + 100) : null,
                !isHeightInCm ? ParseFtToDecimal(selectedHeightFtIndex) : null,
                isWeightInKg ? (selectedWeightKgIndex + 30) : null,
                !isWeightInKg ? (selectedWeightLbsIndex + 66) : null,
                selectedActivityLevel,
                selectedGoalType,
                isTargetWeightInKg ? (selectedTargetWeightKgIndex + 30) : null,
                !isTargetWeightInKg ? (selectedTargetWeightLbsIndex + 66) : null
            );

            System.Diagnostics.Debug.WriteLine("=== SENDING PROFILE DATA... ===");

            try
            {
                await _api.PostAsync("api/UserDetails/save", profileData);
                System.Diagnostics.Debug.WriteLine("=== PROFILE OK ===");
            }
            catch (Exception profileEx)
            {
                System.Diagnostics.Debug.WriteLine($"!!! PROFILE ERROR: {profileEx.Message}");
                System.Diagnostics.Debug.WriteLine($"!!! STATUS: {(profileEx as HttpRequestException)?.StatusCode}");
                errorMessage = $"Профил грешка: {profileEx.Message}";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            var goalData = new
            {
                UserID = userIdGuid,
                GoalType = selectedGoalType,
                TargetWeightKg = isTargetWeightInKg ? (decimal?)(selectedTargetWeightKgIndex + 30) : null,
                TargetWeightLbs = !isTargetWeightInKg ? (decimal?)(selectedTargetWeightLbsIndex + 66) : null,
                StartDate = DateTime.UtcNow.ToString("O"),
                EndDate = DateTime.UtcNow.AddMonths(3).ToString("O")
            };

            System.Diagnostics.Debug.WriteLine("=== SENDING GOAL DATA... ===");

            try
            {
                await _api.PostAsync("api/UserDetails/goal", goalData);
                System.Diagnostics.Debug.WriteLine("=== GOAL OK ===");
            }
            catch (Exception goalEx)
            {
                System.Diagnostics.Debug.WriteLine($"!!! GOAL ERROR: {goalEx.Message}");
                errorMessage = $"Цел грешка: {goalEx.Message}";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }
            // Уверяваме се, че биометрията е изключена за нов потребител
            Preferences.Set("biometric_enabled", false);
            Preferences.Set("height_unit", isHeightInCm ? "cm" : "ft");
            Preferences.Set("weight_unit", isWeightInKg ? "kg" : "lbs");

            try
            {
                var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (cameraStatus != PermissionStatus.Granted)
                {
                    await Permissions.RequestAsync<Permissions.Camera>();
                    await Task.Delay(500);
                }
            }
            catch { }

            if (isEditMode)
            {
                await Shell.Current.DisplayAlert("Успех", "Профилът е обновен успешно!", "OK");
                await Shell.Current.GoToAsync("//ProfilePage");
            }
            else
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Грешка: {ex.Message}";
            OnPropertyChanged(nameof(ErrorMessage));
            System.Diagnostics.Debug.WriteLine($"!!! GENERAL ERROR: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"!!! STACK: {ex.StackTrace}");
        }
        finally
        {
            isBusy = false;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    private decimal ParseFtToDecimal(int index)
    {
        int ft = 3 + (index / 12);
        int inch = index % 12;
        return ft + (inch / 12.0m);
    }

    public async Task LoadExistingProfileAsync()
    {
        isEditMode = true;
        OnPropertyChanged(nameof(IsEditMode));

        try
        {
            var userId = Preferences.Get("user_id", string.Empty);
            if (string.IsNullOrEmpty(userId)) return;

            var response = await _api.GetSingleAsync<System.Text.Json.JsonElement>(
                $"api/UserDetails/{userId}");

            if (response.ValueKind == System.Text.Json.JsonValueKind.Undefined) return;

            // Попълваме полетата с текущите стойности
            nickname = response.GetProperty("nickname").GetString() ?? string.Empty;
            OnPropertyChanged(nameof(Nickname));

            var gender = response.GetProperty("gender").GetInt32();
            selectedGender = gender;
            OnPropertyChanged(nameof(SelectedGender));
            if (gender == 1) { isMaleSelected = true; isFemaleSelected = false; }
            else { isMaleSelected = false; isFemaleSelected = true; }
            OnPropertyChanged(nameof(IsMaleSelected));
            OnPropertyChanged(nameof(IsFemaleSelected));

            var dob = DateTime.Parse(response.GetProperty("dateOfBirth").GetString() ?? "");
            var age = DateTime.UtcNow.Year - dob.Year;
            selectedAgeIndex = Math.Max(0, age - 14);
            OnPropertyChanged(nameof(SelectedAgeIndex));

            var heightCm = response.GetProperty("heightCm").GetDecimal();
            selectedHeightCmIndex = Math.Max(0, (int)heightCm - 100);
            OnPropertyChanged(nameof(SelectedHeightCmIndex));

            var weightKg = response.GetProperty("weightKg").GetDecimal();
            selectedWeightKgIndex = Math.Max(0, (int)weightKg - 30);
            OnPropertyChanged(nameof(SelectedWeightKgIndex));

            var activity = response.GetProperty("activityLevel").GetInt32();
            selectedActivityLevel = activity;
            OnPropertyChanged(nameof(SelectedActivityLevel));
            // Обновяваме bool-овете за визуална селекция
            isSedentarySelected = activity == 1;
            isLightlyActiveSelected = activity == 2;
            isModerateActiveSelected = activity == 3;
            isVeryActiveSelected = activity == 4;
            isExtraActiveSelected = activity == 5;
            OnPropertyChanged(nameof(IsSedentarySelected));
            OnPropertyChanged(nameof(IsLightlyActiveSelected));
            OnPropertyChanged(nameof(IsModerateActiveSelected));
            OnPropertyChanged(nameof(IsVeryActiveSelected));
            OnPropertyChanged(nameof(IsExtraActiveSelected));

            var goal = response.GetProperty("currentGoal").GetInt32();
            selectedGoalType = goal;
            OnPropertyChanged(nameof(SelectedGoalType));
            isWeightLossSelected = goal == 1;
            isMaintenanceSelected = goal == 2;
            isWeightGainSelected = goal == 3;
            isMuscleGainSelected = goal == 4;
            OnPropertyChanged(nameof(IsWeightLossSelected));
            OnPropertyChanged(nameof(IsMaintenanceSelected));
            OnPropertyChanged(nameof(IsWeightGainSelected));
            OnPropertyChanged(nameof(IsMuscleGainSelected));

            var targetKg = response.GetProperty("targetWeightKg").GetDecimal();
            selectedTargetWeightKgIndex = Math.Max(0, (int)targetKg - 30);
            OnPropertyChanged(nameof(SelectedTargetWeightKgIndex));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load profile error: {ex.Message}");
        }
    }
}