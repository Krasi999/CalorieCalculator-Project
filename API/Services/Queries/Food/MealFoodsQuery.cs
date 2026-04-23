using MediatR;

namespace Services.Queries;

public class MealFoodsQuery : IRequest<List<MealFoodResponse>>
{
    public int MealID { get; set; }
}
