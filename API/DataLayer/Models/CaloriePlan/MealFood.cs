using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models;

[Table("MealFoods")]
public class MealFood
{
    public MealFood() { }

    public MealFood(int mealID, int productID, int weight)
    {
        MealID = mealID;
        ProductID = productID;
        Weight = weight;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MealFoodID { get; private set; }

    public int MealID { get; private set; }

    [ForeignKey("MealID")]
    public CalorieProgramMeal? CalorieProgramMeal { get; set; }

    public int ProductID { get; private set; }

    [ForeignKey("ProductID")]
    public FoodProduct? FoodProduct { get; set; }

    public int Weight { get; private set; }
}
