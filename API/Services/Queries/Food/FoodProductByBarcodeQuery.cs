using DataLayer.Models;
using MediatR;
using System.Text.Json.Serialization;

public class FoodProductByBarcodeQuery : IRequest<BarcodeProductResponse>
{
    public string Barcode { get; set; }
}

public class BarcodeProductResponse
{
    public bool Found { get; set; }
    public bool FromLocal { get; set; }
    public int? ProductID { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public int Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fats { get; set; }
    public int Weight { get; set; }
    public int CategoryID { get; set; }
    public string Barcode { get; set; }
}

public class OpenFoodFactsResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("product")]
    public OpenFoodFactsProduct? Product { get; set; }
}

public class OpenFoodFactsProduct
{
    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    [JsonPropertyName("brands")]
    public string? Brands { get; set; }

    [JsonPropertyName("nutriments")]
    public OpenFoodFactsNutriments? Nutriments { get; set; }
}

public class OpenFoodFactsNutriments
{
    [JsonPropertyName("energy-kcal_100g")]
    public double? EnergyKcal100g { get; set; }

    [JsonPropertyName("proteins_100g")]
    public double? Proteins100g { get; set; }

    [JsonPropertyName("carbohydrates_100g")]
    public double? Carbohydrates100g { get; set; }

    [JsonPropertyName("fat_100g")]
    public double? Fat100g { get; set; }
}
