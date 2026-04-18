using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalorieCalculator.ViewModels;

public class MealItem
{
    public string Name { get; set; }
    public string Icon { get; set; }
    public int CurrentCalories { get; set; }
    public int GoalCalories { get; set; }
}

public class MainViewModel : INotifyPropertyChanged
{
    // Calories
    private int _caloriesEaten = 1291;
    public int CaloriesEaten
    {
        get => _caloriesEaten;
        set { _caloriesEaten = value; OnPropertyChanged(); UpdateRemaining(); }
    }

    private int _caloriesBurned = 244;
    public int CaloriesBurned
    {
        get => _caloriesBurned;
        set { _caloriesBurned = value; OnPropertyChanged(); UpdateRemaining(); }
    }

    private int _caloriesGoal = 2117;
    public int CaloriesGoal
    {
        get => _caloriesGoal;
        set { _caloriesGoal = value; OnPropertyChanged(); UpdateRemaining(); }
    }

    private int _caloriesRemaining;
    public int CaloriesRemaining
    {
        get => _caloriesRemaining;
        set { _caloriesRemaining = value; OnPropertyChanged(); }
    }

    // Macros
    public int CarbsCurrent { get; set; } = 206;
    public int CarbsGoal { get; set; } = 258;
    public double CarbsProgress => CarbsGoal > 0 ? (double)CarbsCurrent / CarbsGoal : 0;

    public int ProteinCurrent { get; set; } = 35;
    public int ProteinGoal { get; set; } = 103;
    public double ProteinProgress => ProteinGoal > 0 ? (double)ProteinCurrent / ProteinGoal : 0;

    public int FatCurrent { get; set; } = 32;
    public int FatGoal { get; set; } = 68;
    public double FatProgress => FatGoal > 0 ? (double)FatCurrent / FatGoal : 0;

    // Meals
    public ObservableCollection<MealItem> Meals { get; set; }

    // Commands
    public ICommand AddMealCommand { get; }

    public MainViewModel()
    {
        UpdateRemaining();

        Meals = new ObservableCollection<MealItem>
        {
            new MealItem { Name = "Breakfast", Icon = "🥣", CurrentCalories = 56, GoalCalories = 635 },
            new MealItem { Name = "Lunch", Icon = "🍲", CurrentCalories = 856, GoalCalories = 847 },
            new MealItem { Name = "Dinner", Icon = "🥗", CurrentCalories = 379, GoalCalories = 635 },
            new MealItem { Name = "Snacks", Icon = "🍎", CurrentCalories = 0, GoalCalories = 200 }
        };

        AddMealCommand = new Command<string>(async (mealName) =>
        {
            await Shell.Current.DisplayAlert("Add Food", $"Add food to {mealName}", "OK");
        });
    }

    private void UpdateRemaining()
    {
        CaloriesRemaining = _caloriesGoal - _caloriesEaten + _caloriesBurned;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
