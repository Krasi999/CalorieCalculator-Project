using CalorieCalculator.Models;
using CalorieCalculator.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalorieCalculator.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;
    private int _programId;
    private DateTime _currentDate;

    private string _dateDisplay = "Днес";
    public string DateDisplay
    {
        get => _dateDisplay;
        set { _dateDisplay = value; OnPropertyChanged(); }
    }

    private bool _hasPrevious;
    public bool HasPrevious
    {
        get => _hasPrevious;
        set { _hasPrevious = value; OnPropertyChanged(); }
    }

    private bool _hasNext;
    public bool HasNext
    {
        get => _hasNext;
        set { _hasNext = value; OnPropertyChanged(); }
    }

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
    public ICommand ViewMealCommand { get; }
    public ICommand PreviousDayCommand { get; }
    public ICommand NextDayCommand { get; }

    public MainViewModel(ApiService apiService)
    {
        _apiService = apiService;
        _currentDate = DateTime.UtcNow.Date;

        LoadCommand = new Command(async () => await LoadDailyProgramAsync());
        AddMealCommand = new Command<MealDTO>(AddMeal);
        ViewMealCommand = new Command<MealDTO>(ViewMeal);
        PreviousDayCommand = new Command(async () => await GoToPreviousDay());
        NextDayCommand = new Command(async () => await GoToNextDay());
    }

    private async Task LoadDailyProgramAsync()
    {
        try
        {
            IsLoading = true;
            var userIdSessions = Preferences.Get("user_id", string.Empty);

            var userId = Guid.Parse(userIdSessions);
            var dateParam = _currentDate.ToString("yyyy-MM-dd");
            var program = await _apiService.GetAsyncT<DailyProgramDTO>($"api/dailyprogram/{userId}?date={dateParam}");

            if (program == null)
            {
                return;
            }

            _programId = program.ProgramID;
            HasPrevious = program.HasPrevious;
            HasNext = program.HasNext;

            if (_currentDate.Date == DateTime.UtcNow.Date)
            {
                DateDisplay = "Днес";
            }
            else if (_currentDate.Date == DateTime.UtcNow.Date.AddDays(-1))
            {
                DateDisplay = "Вчера";
            }
            else
            {
                DateDisplay = _currentDate.ToString("d MMMM", new CultureInfo("bg-BG"));
            }

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
            await Shell.Current.DisplayAlert("Грешка", ex.Message, "OK");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task GoToPreviousDay()
    {
        _currentDate = _currentDate.AddDays(-1);
        await LoadDailyProgramAsync();
    }

    private async Task GoToNextDay()
    {
        _currentDate = _currentDate.AddDays(1);
        await LoadDailyProgramAsync();
    }

    private async void AddMeal(MealDTO meal)
    {
        await Shell.Current.GoToAsync(
            $"food/categories?ProgramID={_programId}&MealType={meal.MealType}&MealID={meal.MealID}");
    }

    private async void ViewMeal(MealDTO meal)
    {
        await Shell.Current.GoToAsync(
            $"meal/detail?MealID={meal.MealID}&MealType={meal.MealType}&ProgramID={_programId}");
    }

    private void UpdateRemaining()
    {
        CaloriesRemaining = _caloriesPerDay - _caloriesEaten;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}