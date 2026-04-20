using System.Text.Json.Serialization;

namespace CalorieCalculator.Models;

public class FoodCategoryDTO
{
    [JsonPropertyName("categoryid")]
    public int CategoryID { get; set; }

    [JsonPropertyName("name")]
    public string CategoryName { get; set; }
}
