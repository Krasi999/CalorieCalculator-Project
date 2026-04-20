using CalorieCalculator.Models;
using CalorieCalculator.Service;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CalorieCalculator.ViewModels.MainView.Food;

public class FoodProductViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;

    public int CategoryID { get; set; }
    public int ProgramID { get; set; }
    public int MealType { get; set; }
    public int? MealID { get; set; }

    private string _categoryName = "";
    public string CategoryName
    {
        get => _categoryName;
        set { _categoryName = value; OnPropertyChanged(); }
    }

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterProducts();
        }
    }

    public ObservableCollection<FoodProductDTO> AllProducts { get; set; } = new();
    public ObservableCollection<FoodProductDTO> FilteredProducts { get; set; } = new();

    public ICommand LoadCommand { get; }
    public ICommand SelectProductCommand { get; }
    public ICommand GoBackCommand { get; }
    public ICommand CreateProductCommand { get; }

    public FoodProductViewModel(ApiService apiService)
    {
        _apiService = apiService;

        LoadCommand = new Command(async () => await LoadProductsAsync());
        SelectProductCommand = new Command<FoodProductDTO>(SelectProduct);
        GoBackCommand = new Command(GoBack);
        CreateProductCommand = new Command(CreateProduct);
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var products = await _apiService.GetAsync<FoodProductDTO>($"api/food/category/{CategoryID}");

            AllProducts.Clear();
            FilteredProducts.Clear();

            foreach (var product in products)
            {
                AllProducts.Add(product);
                FilteredProducts.Add(product);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void FilterProducts()
    {
        FilteredProducts.Clear();
        var filtered = string.IsNullOrWhiteSpace(SearchText)
            ? AllProducts
            : AllProducts.Where(p => p.Name.ToLower().Contains(SearchText.ToLower()));

        foreach (var product in filtered)
        {
            FilteredProducts.Add(product);
        }
    }

    private async void SelectProduct(FoodProductDTO product)
    {
        await Shell.Current.GoToAsync(
            $"food/details?ProductID={product.ProductID}&ProgramID={ProgramID}&MealType={MealType}&MealID={MealID}");
    }

    private async void GoBack() 
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void CreateProduct()
    {
        await Shell.Current.GoToAsync($"food/create?CategoryID={CategoryID}");
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
