using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLayer.Models;

[Table("CalorieProgramMeals")]
public class CalorieProgramMeal
{
    public CalorieProgramMeal() { }

    public CalorieProgramMeal(int mealType, int calorieProgramID)
    {
        MealType = mealType;
        CalorieProgramID = calorieProgramID;
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MealID { get; private set; }

    public int MealType { get; private set; }

    public int CalorieProgramID { get; private set; }

    [ForeignKey("CalorieProgramID")]
    public CalorieProgram? CalorieProgram { get; set; }

    public ICollection<MealFood> MealFoods { get; set; } = new List<MealFood>();

    
}
