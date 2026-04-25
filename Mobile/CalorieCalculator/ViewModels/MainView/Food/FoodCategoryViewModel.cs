using CalorieCalculator.Models;
using CalorieCalculator.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalorieCalculator.ViewModels;

public class FoodCategoryDisplayItem
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; }
    public string Icon => CategoryID switch
    {
        1 => "🥩",
        2 => "🐟",
        3 => "🥛",
        4 => "🌾",
        5 => "🥬",
        6 => "🧂",
        _ => "🍽"
    };
}

public class FoodCategoryViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;

    public int ProgramID { get; set; }
    public int MealType { get; set; }
    public int? MealID { get; set; }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterCategories();
        }
    }

    public ObservableCollection<FoodCategoryDisplayItem> AllCategories { get; set; } = new();
    public ObservableCollection<FoodCategoryDisplayItem> FilteredCategories { get; set; } = new();

    public ICommand LoadCommand { get; }
    public ICommand SelectCategoryCommand { get; }
    public ICommand GoBackCommand { get; }
    public ICommand CreateProductCommand { get; }
    public ICommand ScanBarcodeCommand { get; }

    public FoodCategoryViewModel(ApiService apiService)
    {
        _apiService = apiService;

        LoadCommand = new Command(async () => await LoadCategoriesAsync());
        SelectCategoryCommand = new Command<FoodCategoryDisplayItem>(SelectCategory);
        GoBackCommand = new Command(GoBack);
        CreateProductCommand = new Command(CreateProduct);
        ScanBarcodeCommand = new Command(ScanBarcode);
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await _apiService.GetAsync<FoodCategoryDTO>("api/food/foodcategory");

            AllCategories.Clear();
            FilteredCategories.Clear();

            foreach (var cat in categories)
            {
                var item = new FoodCategoryDisplayItem
                {
                    CategoryID = cat.CategoryID,
                    CategoryName = cat.CategoryName,
                };
                AllCategories.Add(item);
                FilteredCategories.Add(item);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void FilterCategories()
    {
        FilteredCategories.Clear();
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? AllCategories
            : AllCategories.Where(c => c.CategoryName.ToLower().Contains(SearchText.ToLower()));

        foreach (var cat in filtered)
        {
            FilteredCategories.Add(cat);
        }
    }

    private async void SelectCategory(FoodCategoryDisplayItem category)
    {
        await Shell.Current.GoToAsync(
                $"food/products?CategoryID={category.CategoryID}&CategoryName={category.CategoryName}&ProgramID={ProgramID}&MealType={MealType}&MealID={MealID}");
    }

    private async void GoBack()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void CreateProduct()
    {
        await Shell.Current.GoToAsync("food/create");
    }

    private async void ScanBarcode()
    {
        await Shell.Current.GoToAsync(
            $"barcode/scan?ProgramID={ProgramID}&MealType={MealType}&MealID={MealID}");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
