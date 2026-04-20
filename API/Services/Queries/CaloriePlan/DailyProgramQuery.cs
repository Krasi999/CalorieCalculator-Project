using MediatR;

namespace Services.Queries;

public class DailyProgramQuery : IRequest<DailyProgramResponse?>
{
    public Guid UserID { get; set; }
    public DateTime Date { get; set; }
}

public class DailyProgramResponse
{
    public int ProgramID { get; set; }
    public int CaloriesPerDay { get; set; }
    public int CarbsPerDay { get; set; }
    public int ProteinPerDay { get; set; }
    public int FatsPerDay { get; set; }
    public List<MealResponse> Meals { get; set; } = new();
}

public class MealResponse
{
    public int MealID { get; set; }
    public int MealType { get; set; }
    public bool Exists { get; set; }
    public List<MealFoodResponse> Foods { get; set; } = new();
    public int TotalCalories => Foods.Sum(f => f.Calories);
}

public class MealFoodResponse
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
