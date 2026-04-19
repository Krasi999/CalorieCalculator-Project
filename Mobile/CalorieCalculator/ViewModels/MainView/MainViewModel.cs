using CalorieCalculator.Models;
using CalorieCalculator.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalorieCalculator.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;

    private int _caloriesPerDay;
    public int CaloriesPerDay
    {
        get => _caloriesPerDay;
        set { _caloriesPerDay = value; OnPropertyChanged(); UpdateRemaining(); }
    }

    private int _caloriesEaten;
    public int CaloriesEaten
    {
        get => _caloriesEaten;
        set { _caloriesEaten = value; OnPropertyChanged(); UpdateRemaining(); }
    }

    private int _caloriesBurned;
    public int CaloriesBurned
    {
        get => _caloriesBurned;
        set { _caloriesBurned = value; OnPropertyChanged(); UpdateRemaining(); }
    }

    private int _caloriesRemaining;
    public int CaloriesRemaining
    {
        get => _caloriesRemaining;
        set { _caloriesRemaining = value; OnPropertyChanged(); }
    }

    private int _carbsCurrent;
    public int CarbsCurrent
    {
        get => _carbsCurrent;
        set { _carbsCurrent = value; OnPropertyChanged(); OnPropertyChanged(nameof(CarbsProgress)); }
    }

    private int _carbsGoal;
    public int CarbsGoal
    {
        get => _carbsGoal;
        set { _carbsGoal = value; OnPropertyChanged(); OnPropertyChanged(nameof(CarbsProgress)); }
    }
    public double CarbsProgress => CarbsGoal > 0 ? (double)CarbsCurrent / CarbsGoal : 0;

    private int _proteinCurrent;
    public int ProteinCurrent
    {
        get => _proteinCurrent;
        set { _proteinCurrent = value; OnPropertyChanged(); OnPropertyChanged(nameof(ProteinProgress)); }
    }

    private int _proteinGoal;
    public int ProteinGoal
    {
        get => _proteinGoal;
        set { _proteinGoal = value; OnPropertyChanged(); OnPropertyChanged(nameof(ProteinProgress)); }
    }
    public double ProteinProgress => ProteinGoal > 0 ? (double)ProteinCurrent / ProteinGoal : 0;

    private int _fatCurrent;
    public int FatCurrent
    {
        get => _fatCurrent;
        set { _fatCurrent = value; OnPropertyChanged(); OnPropertyChanged(nameof(FatProgress)); }
    }

    private int _fatGoal;
    public int FatGoal
    {
        get => _fatGoal;
        set { _fatGoal = value; OnPropertyChanged(); OnPropertyChanged(nameof(FatProgress)); }
    }
    public double FatProgress => FatGoal > 0 ? (double)FatCurrent / FatGoal : 0;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    public ObservableCollection<MealDTO> Meals { get; set; } = new();

    public ICommand LoadCommand { get; }
    public ICommand AddMealCommand { get; }

    public MainViewModel(ApiService apiService)
    {
        _apiService = apiService;

        LoadCommand = new Command(async () => await LoadDailyProgramAsync());
        AddMealCommand = new Command<string>(async (mealName) =>
        {
            await Shell.Current.DisplayAlert("Add Food", $"Add food to {mealName}", "OK");
        });
    }

    private async Task LoadDailyProgramAsync()
    {
        try
        {
            IsLoading = true;
            var userIdSessions = Preferences.Get("user_id", string.Empty);

            var userId = Guid.Parse(userIdSessions);
            var program = await _apiService.GetAsyncT<DailyProgramDTO>(
                $"api/dailyprogram/{userId}");

            if (program == null) return;

            CaloriesPerDay = program.CaloriesPerDay;
            CarbsGoal = program.CarbsPerDay;
            ProteinGoal = program.ProteinPerDay;
            FatGoal = program.FatsPerDay;

            Meals.Clear();
            int totalCalories = 0;
            int totalCarbs = 0;
            int totalProtein = 0;
            int totalFat = 0;

            foreach (var meal in program.Meals.OrderBy(m => m.MealType))
            {
                Meals.Add(meal);
                totalCalories += meal.TotalCalories;
                totalCarbs += (int)meal.Foods.Sum(f => f.Carbs);
                totalProtein += (int)meal.Foods.Sum(f => f.Protein);
                totalFat += (int)meal.Foods.Sum(f => f.Fats);
            }

            CaloriesEaten = totalCalories;
            CarbsCurrent = totalCarbs;
            ProteinCurrent = totalProtein;
            FatCurrent = totalFat;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateRemaining()
    {
        CaloriesRemaining = _caloriesPerDay - _caloriesEaten + _caloriesBurned;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}