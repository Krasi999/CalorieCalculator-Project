using DataLayer.Models;
using DataLayer.Repository;
using MediatR;
using Services.Commands;

namespace Services.Handlers;

public class HandlerFoodProduct : 
    IRequestHandler<FoodProductCreateCommand, Unit>
{
    private readonly IRepository _repository;

    public HandlerFoodProduct(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(FoodProductCreateCommand request, CancellationToken cancellationToken)
    {
        var product = request.ProductID != null 
            ? _repository.Set<FoodProduct>().First(record => record.ProductID == request.ProductID) 
            : new FoodProduct();

        using var transaction = await _repository.BeginTransaction();

        product.Update(
            request.ProductName,
            request.Description,
            request.Calories,
            request.Weight,
            request.Fats,
            request.Protein,
            request.Carbs,
            request.Category);

        _repository.Save(product, request.ProductID.HasValue);

        await _repository.SaveChanges();

        return Unit.Value;
    }
}
