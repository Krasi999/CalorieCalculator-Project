using DataLayer.Models;
using MediatR;

public class FoodCategoriesQuery : IRequest<List<FoodCategory>>
{
}