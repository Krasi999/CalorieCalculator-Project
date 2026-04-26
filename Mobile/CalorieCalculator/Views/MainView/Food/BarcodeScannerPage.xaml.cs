using CalorieCalculator.Models;
using CalorieCalculator.Service;
using System.Linq;
using ZXing.Net.Maui;

namespace CalorieCalculator.Views.MainView.Food;

[QueryProperty(nameof(ProgramID), "ProgramID")]
[QueryProperty(nameof(MealType), "MealType")]
[QueryProperty(nameof(MealID), "MealID")]
public partial class BarcodeScannerPage : ContentPage
{
    private readonly ApiService _apiService;
    private bool _isProcessing;

    public string? ProgramID { get; set; }
    public string? MealType { get; set; }
    public string? MealID { get; set; }

    public BarcodeScannerPage(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;

        BarcodeReader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.OneDimensional,
            AutoRotate = true,
            Multiple = false
        };
    }

    private void OnBarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        if (_isProcessing) return;
        _isProcessing = true;

        var barcode = e.Results.FirstOrDefault()?.Value;
        if (string.IsNullOrEmpty(barcode))
        {
            _isProcessing = false;
            return;
        }

        BarcodeReader.IsDetecting = false;

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            try
            {
                var result = await _apiService.GetAsyncT<BarcodeProductDTO>(
                    $"api/food/barcode/{barcode}");

                if (result != null && result.Found && result.FromLocal && result.ProductID.HasValue)
                {
                    await Shell.Current.GoToAsync(
                        $"food/details?ProductID={result.ProductID}" +
                        $"&ProgramID={ProgramID}&MealType={MealType}&MealID={MealID}");
                }
                else if (result != null && result.Found && !result.FromLocal)
                {
                    await Shell.Current.GoToAsync(
                        $"food/create?Barcode={barcode}" +
                        $"&PrefilledName={Uri.EscapeDataString(result.ProductName ?? "")}" +
                        $"&PrefilledDescription={Uri.EscapeDataString(result.Description ?? "")}" +
                        $"&PrefilledCalories={result.Calories}" +
                        $"&PrefilledProtein={result.Protein}" +
                        $"&PrefilledCarbs={result.Carbs}" +
                        $"&PrefilledFats={result.Fats}");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Не е намерен",
                        $"Баркод {barcode} не е намерен. Създайте продукта.", "OK");
                    await Shell.Current.GoToAsync($"food/create?Barcode={barcode}");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Грешка", ex.Message, "OK");
                BarcodeReader.IsDetecting = true;
                _isProcessing = false;
            }
        });
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        BarcodeReader.IsDetecting = true;
        _isProcessing = false;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        BarcodeReader.IsDetecting = false;
    }
}