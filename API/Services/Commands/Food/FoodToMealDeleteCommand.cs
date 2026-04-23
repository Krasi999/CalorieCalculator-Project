using MediatR;

public class FoodToMealDeleteCommand : IRequest<bool>
{
    public int MealFoodID { get; set; }
}
