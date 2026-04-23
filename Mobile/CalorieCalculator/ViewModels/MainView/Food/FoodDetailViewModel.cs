using CalorieCalculator.Models;
using CalorieCalculator.Service;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalorieCalculator.ViewModels.MainView.Food;

public record FoodToMealRequest(int? MealFoodID, int ProgramID, int MealType, int? MealID, int ProductID, int Weight);

public class FoodDetailViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;

    public int ProductID { get; set; }
    public int ProgramID { get; set; }
    public int MealType { get; set; }
    public int? MealID { get; set; }
    public int? MealFoodID { get; set; }
    public int? CurrentWeight { get; set; }

    private int _categoryID;

    // Base values per 100g
    public int BaseCalories { get; private set; }
    public decimal BaseProtein { get; private set; }
    public decimal BaseCarbs { get; private set; }
    public decimal BaseFats { get; private set; }

    private string _productName = "";
    public string ProductName
    {
        get => _productName;
        set { _productName = value; OnPropertyChanged(); }
    }

    private string _grams = "100";

    public string Grams
    {
        get => _grams;
        set
        {
            _grams = value;
            OnPropertyChanged();
            Recalculate();
        }
    }

    private decimal _calculatedCalories;
    public decimal CalculatedCalories
    {
        get => _calculatedCalories;
        set { _calculatedCalories = value; OnPropertyChanged(); }
    }

    private decimal _calculatedProtein;
    public decimal CalculatedProtein
    {
        get => _calculatedProtein;
        set { _calculatedProtein = value; OnPropertyChanged(); }
    }

    private decimal _calculatedCarbs;
    public decimal CalculatedCarbs
    {
        get => _calculatedCarbs;
        set { _calculatedCarbs = value; OnPropertyChanged(); }
    }

    private decimal _calculatedFats;
    public decimal CalculatedFats
    {
        get => _calculatedFats;
        set { _calculatedFats = value; OnPropertyChanged(); }
    }

    private string _buttonText = "Добави";
    public string ButtonText
    {
        get => _buttonText;
        set { _buttonText = value; OnPropertyChanged(); }
    }

    public ICommand LoadCommand { get; }
    public ICommand AddToMealCommand { get; }
    public ICommand EditProductCommand { get; }
    public ICommand GoBackCommand { get; }

    public FoodDetailViewModel(ApiService apiService)
    {
        _apiService = apiService;

        LoadCommand = new Command(async () => await LoadProductAsync());
        AddToMealCommand = new Command(async () => await SaveAsync());
        EditProductCommand = new Command(EditProduct);
        GoBackCommand = new Command(GoBack);
    }

    private async Task LoadProductAsync()
    {
        try
        {
            var product = await _apiService.GetAsyncT<FoodProductDTO>($"api/food/{ProductID}");

            if (product == null) return;

            ProductName = product.Name;
            _categoryID = product.CategoryID;
            BaseCalories = product.Calories;
            BaseProtein = product.Protein;
            BaseCarbs = product.Carbs;
            BaseFats = product.Fats;

            OnPropertyChanged(nameof(BaseCalories));
            OnPropertyChanged(nameof(BaseProtein));
            OnPropertyChanged(nameof(BaseCarbs));
            OnPropertyChanged(nameof(BaseFats));

            if (CurrentWeight.HasValue)
            {
                ButtonText = "Запази промените";
                Grams = CurrentWeight.Value.ToString();
            }
            else
            {
                ButtonText = "Добави";
                Grams = "100";
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Грешка", ex.Message, "OK");
        }
    }

    private void Recalculate()
    {
        if (!decimal.TryParse(Grams, out var grams) || grams <= 0)
        {
            CalculatedCalories = 0;
            CalculatedProtein = 0;
            CalculatedCarbs = 0;
            CalculatedFats = 0;
            return;
        }

        var multiplier = grams / 100m;

        CalculatedCalories = BaseCalories * multiplier;
        CalculatedProtein = BaseProtein * multiplier;
        CalculatedCarbs = BaseCarbs * multiplier;
        CalculatedFats = BaseFats * multiplier;
    }

    private async Task SaveAsync()
    {
        try
        {
            if (!decimal.TryParse(Grams, out var grams) || grams <= 0)
            {
                await Shell.Current.DisplayAlert("Грешка", "Въведете грамаж", "OK");
                return;
            }

            var request = new FoodToMealRequest(MealFoodID, ProgramID, MealType, MealID, ProductID, (int)grams);

            await _apiService.PostAsync<object>("api/dailyprogram/meal/food-to-meal", request);

            await Shell.Current.GoToAsync("//MainPage");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void EditProduct()
    {
        await Shell.Current.GoToAsync(
            $"food/create?ProductID={ProductID}&CategoryID={_categoryID}");
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
