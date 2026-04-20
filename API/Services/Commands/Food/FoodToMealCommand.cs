using MediatR;

public class FoodToMealCommand : IRequest<int>
{
    public int ProgramID { get; set; }

    public int MealType { get; set; }

    public int? MealID { get; set; }

    public int ProductID { get; set; }
}
