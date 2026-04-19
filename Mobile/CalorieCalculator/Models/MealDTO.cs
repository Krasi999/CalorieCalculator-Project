namespace CalorieCalculator.Models;

public class MealDTO
{
    public int MealID { get; set; }
    public int MealType { get; set; }
    public bool Exists { get; set; }
    public string MealName => MealType switch
    {
        1 => "Breakfast",
        2 => "Lunch",
        3 => "Dinner",
        4 => "Snacks",
        _ => "Other"
    };
    public string Icon => MealType switch
    {
        1 => "🥣",
        2 => "🍲",
        3 => "🥗",
        4 => "🍎",
        _ => "🍽"
    };
    public List<MealFoodDTO> Foods { get; set; } = new();
    public int TotalCalories => Foods.Sum(f => f.Calories);
}
