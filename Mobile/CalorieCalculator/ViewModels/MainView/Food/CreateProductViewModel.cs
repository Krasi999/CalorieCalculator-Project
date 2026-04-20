using CalorieCalculator.Models;
using CalorieCalculator.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalorieCalculator.ViewModels.MainView.Food;

public record FoodProductRequest(int? ProductID, string? ProductName, string? Description, int Calories, 
    int Weight, decimal Fats, decimal Protein, decimal Carbs, int CategoryID);

public class CreateProductViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;

    public int? PreselectedCategoryID { get; set; }

    private string _productName = "";
    public string ProductName
    {
        get => _productName;
        set { _productName = value; OnPropertyChanged(); }
    }

    private string _description = "";
    public string Description
    {
        get => _description;
        set { _description = value; OnPropertyChanged(); }
    }

    private string _calories = "";
    public string Calories
    {
        get => _calories;
        set { _calories = value; OnPropertyChanged(); }
    }

    private string _protein = "";
    public string Protein
    {
        get => _protein;
        set { _protein = value; OnPropertyChanged(); }
    }

    private string _carbs = "";
    public string Carbs
    {
        get => _carbs;
        set { _carbs = value; OnPropertyChanged(); }
    }

    private string _fats = "";
    public string Fats
    {
        get => _fats;
        set { _fats = value; OnPropertyChanged(); }
    }

    private string _weight = "100";
    public string Weight
    {
        get => _weight;
        set { _weight = value; OnPropertyChanged(); }
    }

    public ObservableCollection<FoodCategoryDTO> Categories { get; set; } = new();

    private FoodCategoryDTO? _selectedCategory;
    public FoodCategoryDTO? SelectedCategory
    {
        get => _selectedCategory;
        set { _selectedCategory = value; OnPropertyChanged(); }
    }

    public ICommand LoadCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand GoBackCommand { get; }

    public CreateProductViewModel(ApiService apiService)
    {
        _apiService = apiService;

        LoadCommand = new Command(async () => await LoadCategoriesAsync());
        SaveCommand = new Command(async () => await SaveProductAsync());
        GoBackCommand = new Command(GoBack);
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await _apiService.GetAsync<FoodCategoryDTO>("api/foodcategory");

            Categories.Clear();
            foreach (var cat in categories)
            {
                Categories.Add(cat);
            }

            if (PreselectedCategoryID.HasValue)
            {
                SelectedCategory = Categories.FirstOrDefault(
                    c => c.CategoryID == PreselectedCategoryID.Value);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Грешка", ex.Message, "OK");
        }
    }

    private async Task SaveProductAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ProductName))
            {
                await Shell.Current.DisplayAlert("Грешка", "Въведете име на продукт", "OK");
                return;
            }

            if (SelectedCategory == null)
            {
                await Shell.Current.DisplayAlert("Грешка", "Изберете категория", "OK");
                return;
            }

            if (!int.TryParse(Calories, out var calories))
            {
                await Shell.Current.DisplayAlert("Грешка", "Въведете калории", "OK");
                return;
            }

            var request = new FoodProductRequest(
                null, 
                ProductName, 
                Description, 
                calories, 
                int.TryParse(Weight, out var w) ? w : 100,
                decimal.TryParse(Fats, out var f) ? f : 0,
                decimal.TryParse(Protein, out var p) ? p : 0,
                decimal.TryParse(Carbs, out var c) ? c : 0,
                SelectedCategory.CategoryID
            );

            await _apiService.PostAsync<object>("api/foodproduct/create", request);

            await Shell.Current.DisplayAlert("Успех", "Продуктът е записан", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Грешка", ex.Message, "OK");
        }
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
