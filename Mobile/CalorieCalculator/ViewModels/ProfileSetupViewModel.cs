using CalorieCalculator.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace CalorieCalculator.ViewModels;

public partial class ProfileSetupViewModel : ObservableObject
{
    private readonly ApiService _api;

    public ProfileSetupViewModel(ApiService api)
    {
        _api = api;
        UpdateStepInfo();
    }

    // ==========================
    // Стъпки и навигация
    // ==========================

    [ObservableProperty] private int currentStep = 1;
    [ObservableProperty] private double progress = 0.125;
    [ObservableProperty] private string stepTitle = string.Empty;
    [ObservableProperty] private string stepSubtitle = string.Empty;
    [ObservableProperty] private string errorMessage = string.Empty;
    [ObservableProperty] private bool isBusy;

    // Step visibility
    [ObservableProperty] private bool isStep1Visible = true;
    [ObservableProperty] private bool isStep2Visible;
    [ObservableProperty] private bool isStep3Visible;
    [ObservableProperty] private bool isStep4Visible;
    [ObservableProperty] private bool isStep5Visible;
    [ObservableProperty] private bool isStep6Visible;
    [ObservableProperty] private bool isStep7Visible;
    [ObservableProperty] private bool isStep8Visible;

    // Стъпка 1 — Псевдоним
    [ObservableProperty] private string nickname = string.Empty;

    // Стъпка 2 — Пол (1=Мъж, 2=Жена)
    [ObservableProperty] private int selectedGender;
    [ObservableProperty] private bool isMaleSelected;
    [ObservableProperty] private bool isFemaleSelected;

    // Стъпка 3 — Възраст
    [ObservableProperty] private int selectedAgeIndex = 11; // 14+11=25

    // Стъпка 4 — Ръст
    [ObservableProperty] private bool isHeightInCm = true;
    [ObservableProperty] private int selectedHeightCmIndex = 70; // 100+70=170cm
    [ObservableProperty] private int selectedHeightFtIndex = 30; // ~5'6"

    // Стъпка 5 — Тегло
    [ObservableProperty] private bool isWeightInKg = true;
    [ObservableProperty] private int selectedWeightKgIndex = 40;
    [ObservableProperty] private int selectedWeightLbsIndex = 88;

    // Стъпка 6 — Ниво на активност (1-5)
    [ObservableProperty] private int selectedActivityLevel = 1;

    // Стъпка 6 — кой е избран
    [ObservableProperty] private bool isSedentarySelected;
    [ObservableProperty] private bool isLightlyActiveSelected;
    [ObservableProperty] private bool isModerateActiveSelected;
    [ObservableProperty] private bool isVeryActiveSelected;
    [ObservableProperty] private bool isExtraActiveSelected;

    // Стъпка 7 — Цел (1-4)
    [ObservableProperty] private int selectedGoalType = 1;

    // Стъпка 7 — коя цел е избрана
    [ObservableProperty] private bool isWeightLossSelected;
    [ObservableProperty] private bool isMaintenanceSelected;
    [ObservableProperty] private bool isWeightGainSelected;
    [ObservableProperty] private bool isMuscleGainSelected;

    // Стъпка 8 — Желано тегло
    [ObservableProperty] private bool isTargetWeightInKg = true;
    [ObservableProperty] private int selectedTargetWeightKgIndex = 35;
    [ObservableProperty] private int selectedTargetWeightLbsIndex = 77;



    // ==========================
    // Списъци за Picker-ите (read-only, не са ObservableProperty)
    // ==========================

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

    // ==========================
    // Commands — Пол
    // ==========================

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

    // ==========================
    // Commands — Toggles
    // ==========================

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

    // ==========================
    // Commands — Активност и цел
    // ==========================

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

    // ==========================
    // Навигация между стъпките
    // ==========================

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
        switch (CurrentStep)
        {
            case 1:
                if (string.IsNullOrWhiteSpace(Nickname) || Nickname.Length < 2)
                {
                    ErrorMessage = "Псевдонимът трябва да е поне 2 символа.";
                    return false;
                }
                break;
            case 2:
                if (!IsMaleSelected && !IsFemaleSelected)
                {
                    ErrorMessage = "Моля, изберете пол.";
                    return false;
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

    // ==========================
    // Запис в базата
    // ==========================

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

            // Пробваме първо само profile data
            var profileData = new
            {
                UserID = userIdGuid,
                Nickname = nickname.Trim(),
                Gender = selectedGender,
                DateOfBirth = dateOfBirth.ToString("O"),
                HeightCm = isHeightInCm ? (decimal?)(selectedHeightCmIndex + 100) : null,
                HeightFt = !isHeightInCm ? (decimal?)ParseFtToDecimal(selectedHeightFtIndex) : null,
                WeightKg = isWeightInKg ? (decimal?)(selectedWeightKgIndex + 30) : null,
                WeightLbs = !isWeightInKg ? (decimal?)(selectedWeightLbsIndex + 66) : null,
                ActivityLevel = selectedActivityLevel,
                CurrentGoal = selectedGoalType,
                TargetWeightKg = isTargetWeightInKg ? (decimal?)(selectedTargetWeightKgIndex + 30) : null,
                TargetWeightLbs = !isTargetWeightInKg ? (decimal?)(selectedTargetWeightLbsIndex + 66) : null
            };

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

            // После goal data
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

            await Shell.Current.GoToAsync("//MainPage");
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
}