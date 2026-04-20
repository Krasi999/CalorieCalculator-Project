namespace CalorieCalculator.Models;

public class MealFoodDTO
{
    public int MealFoodID { get; set; }

    public int ProductID { get; set; }

    public string Name { get; set; }

    public int Weight { get; set; }

    public int Calories { get; set; }

    public decimal Protein { get; set; }

    public decimal Carbs { get; set; }

    public decimal Fats { get; set; }
}
