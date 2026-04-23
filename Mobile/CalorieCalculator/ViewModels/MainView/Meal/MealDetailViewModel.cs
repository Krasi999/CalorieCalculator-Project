using CalorieCalculator.Models;
using CalorieCalculator.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalorieCalculator.ViewModels.MainView.Meal;

public class MealDetailViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;

    public int MealID { get; set; }
    public int MealType { get; set; }
    public int ProgramID { get; set; }

    public string MealName => MealType switch
    {
        1 => "Закуска",
        2 => "Обяд",
        3 => "Вечеря",
        4 => "Снакс",
        _ => "Ястие"
    };

    private int _totalCalories;
    public int TotalCalories
    {
        get => _totalCalories;
        set { _totalCalories = value; OnPropertyChanged(); }
    }

    private decimal _totalProtein;
    public decimal TotalProtein
    {
        get => _totalProtein;
        set { _totalProtein = value; OnPropertyChanged(); }
    }

    private decimal _totalCarbs;
    public decimal TotalCarbs
    {
        get => _totalCarbs;
        set { _totalCarbs = value; OnPropertyChanged(); }
    }

    private decimal _totalFats;
    public decimal TotalFats
    {
        get => _totalFats;
        set { _totalFats = value; OnPropertyChanged(); }
    }

    public ObservableCollection<MealFoodDTO> Foods { get; set; } = new();

    public ICommand LoadCommand { get; }
    public ICommand EditWeightCommand { get; }
    public ICommand DeleteFoodCommand { get; }
    public ICommand AddMoreFoodCommand { get; }
    public ICommand GoBackCommand { get; }

    public MealDetailViewModel(ApiService apiService)
    {
        _apiService = apiService;

        LoadCommand = new Command(async () => await LoadFoodsAsync());
        EditWeightCommand = new Command<MealFoodDTO>(async (food) => await EditWeightAsync(food));
        DeleteFoodCommand = new Command<MealFoodDTO>(async (food) => await DeleteFoodAsync(food));
        AddMoreFoodCommand = new Command(AddMoreFood);
        GoBackCommand = new Command(GoBack);
    }

    private async Task LoadFoodsAsync()
    {
        try
        {
            OnPropertyChanged(nameof(MealName));

            if (MealID == 0)
            {
                Foods.Clear();
                UpdateTotals();
                return;
            }

            var foods = await _apiService.GetAsync<MealFoodDTO>(
                $"api/dailyprogram/meal/{MealID}/foods");

            Foods.Clear();
            foreach (var food in foods)
            {
                Foods.Add(food);
            }

            UpdateTotals();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Грешка", ex.Message, "OK");
        }
    }

    private async Task EditWeightAsync(MealFoodDTO food)
    {
        var result = await Shell.Current.DisplayPromptAsync(
            "Промени грамаж",
            $"Текущ грамаж: {food.Weight}g",
            "Запази",
            "Отказ",
            placeholder: food.Weight.ToString());

        if (result == null) return;

        if (!int.TryParse(result, out var newWeight) || newWeight <= 0)
        {
            await Shell.Current.DisplayAlert("Грешка", "Въведете валиден грамаж", "OK");
            return;
        }

        try
        {
            await _apiService.PostAsync("api/dailyprogram/meal/update-weight", new
            {
                MealFoodID = food.MealFoodID,
                Weight = newWeight
            });

            await LoadFoodsAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Грешка", ex.Message, "OK");
        }
    }

    private async Task DeleteFoodAsync(MealFoodDTO food)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Изтриване",
            $"Сигурни ли сте, че искате да изтриете {food.Name}?",
            "Изтрий",
            "Отказ");

        if (!confirm) return;

        try
        {
            await _apiService.PostAsync($"api/dailyprogram/meal/remove-food", food.MealFoodID);

            Foods.Remove(food);
            UpdateTotals();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Грешка", ex.Message, "OK");
        }
    }

    private void UpdateTotals()
    {
        TotalCalories = Foods.Sum(f => f.Calories);
        TotalProtein = Foods.Sum(f => f.Protein);
        TotalCarbs = Foods.Sum(f => f.Carbs);
        TotalFats = Foods.Sum(f => f.Fats);
    }

    private async void AddMoreFood()
    {
        await Shell.Current.GoToAsync(
            $"food/categories?ProgramID={ProgramID}&MealType={MealType}&MealID={MealID}");
    }

    private async void GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
