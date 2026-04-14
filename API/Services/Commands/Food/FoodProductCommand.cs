using DataLayer.Enums;
using MediatR;

public class FoodProductCommand : IRequest<Unit>
{
    public int? ProductID { get; set; }

    public string ProductName { get; set; }

    public string Description { get; set; }

    public int Calories { get; set; }

    public decimal Fats { get; set; }

    public decimal Protein { get; set; }

    public decimal Carbs { get; set; }

    public int Weight { get; set; }

    public FoodCategories Category { get; set; }
}
