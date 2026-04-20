using DataLayer.Models;
using MediatR;

public class FoodProductsQuery : IRequest<List<FoodProduct>>
{
    public int CategoryID { get; set; }

    public string? SearchTerm { get; set; }
}