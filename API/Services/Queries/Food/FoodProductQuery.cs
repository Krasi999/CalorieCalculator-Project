using DataLayer.Models;
using MediatR;

public class FoodProductQuery : IRequest<FoodProduct?>
{
    public int ProductID { get; set; }
}