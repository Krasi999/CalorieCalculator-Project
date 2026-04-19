using DataLayer.Models;
using MediatR;


namespace Services.Handlers.Food;

public class HandlerFoodProduct : 
    IRequestHandler<FoodProductCommand, Unit>
{
    private readonly IServices _services;

    public HandlerFoodProduct(IServices services)
    {
        _services = services;
    }

    public async Task<Unit> Handle(FoodProductCommand request, CancellationToken cancellationToken)
    {
        var product = request.ProductID != null 
            ? _services.Repository.Set<FoodProduct>().First(record => record.ProductID == request.ProductID) 
            : new FoodProduct();

        using var transaction = await _services.Repository.BeginTransaction();

        product.Update(
            request.ProductName,
            request.Description,
            request.Calories,
            request.Weight,
            request.Fats,
            request.Protein,
            request.Carbs,
            request.Category);

        _services.Repository.Save(product, request.ProductID.HasValue);

        await _services.Repository.SaveChanges();

        return Unit.Value;
    }
}
