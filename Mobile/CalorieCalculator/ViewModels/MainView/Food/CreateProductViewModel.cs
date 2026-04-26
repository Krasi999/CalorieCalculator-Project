using CalorieCalculator.Models;
using CalorieCalculator.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalorieCalculator.ViewModels.MainView.Food;

public record FoodProductRequest(int? ProductID, string? ProductName, string? Description, int Calories, 
    int Weight, decimal Fats, decimal Protein, decimal Carbs, int CategoryID, string? Barcode);

public class CreateProductViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;

    public int? PreselectedCategoryID { get; set; }
    public int? EditProductID { get; set; }
    public string? Barcode { get; set; }
    public string? PrefilledName { get; set; }
    public string? PrefilledDescription { get; set; }
    public string? PrefilledCalories { get; set; }
    public string? PrefilledProtein { get; set; }
    public string? PrefilledCarbs { get; set; }
    public string? PrefilledFats { get; set; }

    private string _pageTitle = "Нов продукт";
    public string PageTitle
    {
        get => _pageTitle;
        set { _pageTitle = value; OnPropertyChanged(); }
    }

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

    private string _saveButtonText = "Запази продукт";
    public string SaveButtonText
    {
        get => _saveButtonText;
        set { _saveButtonText = value; OnPropertyChanged(); }
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

        LoadCommand = new Command(async () => await LoadAsync());
        SaveCommand = new Command(async () => await SaveProductAsync());
        GoBackCommand = new Command(GoBack);
    }

    private async Task LoadAsync()
    {
        try
        {
            var categories = await _apiService.GetAsync<FoodCategoryDTO>("api/food/foodcategory");

            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }

            if (EditProductID.HasValue)
            {
                PageTitle = "Редактиране";
                SaveButtonText = "Запази промените";

                var product = await _apiService.GetAsyncT<FoodProductDTO>(
                    $"api/food/{EditProductID.Value}");

                if (product != null)
                {
                    ProductName = product.Name;
                    Description = product.Description ?? "";
                    Calories = product.Calories.ToString();
                    Protein = product.Protein.ToString();
                    Carbs = product.Carbs.ToString();
                    Fats = product.Fats.ToString();
                    Weight = "100";
                    SelectedCategory = Categories.FirstOrDefault(c => c.CategoryID == product.CategoryID);
                }
            }
            else if (!string.IsNullOrEmpty(PrefilledName) || !string.IsNullOrWhiteSpace(Barcode))
            {
                PageTitle = "Нов продукт";
                SaveButtonText = "Запази продукт";
                ProductName = PrefilledName;
                Description = PrefilledDescription ?? "";
                Calories = PrefilledCalories ?? "";
                Protein = PrefilledProtein ?? "";
                Carbs = PrefilledCarbs ?? "";
                Fats = PrefilledFats ?? "";
            }
            else
            {
                PageTitle = "Нов продукт";
                SaveButtonText = "Запази продукт";
                ClearForm();

                if (PreselectedCategoryID.HasValue)
                {
                    SelectedCategory = Categories.FirstOrDefault(
                        c => c.CategoryID == PreselectedCategoryID.Value);
                }
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

            var request = new FoodProductRequest(
                EditProductID, 
                ProductName, 
                Description,
                int.TryParse(Calories, out var cal) ? cal : 0, 
                100,
                decimal.TryParse(Fats, out var f) ? f : 0,
                decimal.TryParse(Protein, out var p) ? p : 0,
                decimal.TryParse(Carbs, out var c) ? c : 0,
                SelectedCategory.CategoryID,
                Barcode
            );

            await _apiService.PostAsync<object>("api/food/create", request);
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Грешка", ex.Message, "OK");
        }
    }

    private void ClearForm()
    {
        ProductName = "";
        Description = "";
        Calories = "";
        Protein = "";
        Carbs = "";
        Fats = "";
        Weight = "100";
        SelectedCategory = null;
        EditProductID = null;
        Barcode = null;
        PrefilledName = null;
        PrefilledDescription = null;
        PrefilledCalories = null;
        PrefilledProtein = null;
        PrefilledCarbs = null;
        PrefilledFats = null;
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
