namespace CalorieCalculator.Models;

public class MealDTO
{
    public int MealID { get; set; }
    public int MealType { get; set; }
    public bool Exists { get; set; }
    public string MealName => MealType switch
    {
        1 => "Закуска",
        2 => "Обяд",
        3 => "Вечеря",
        4 => "Снакс",
        _ => "Ястие"
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

    public int TotalCalories => Foods.Sum(food => food.Calories);
}
