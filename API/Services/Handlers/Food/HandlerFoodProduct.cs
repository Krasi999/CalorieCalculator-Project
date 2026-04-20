using DataLayer.Models;
using MediatR;


namespace Services.Handlers.Food;

public class HandlerFoodProduct :
    IRequestHandler<FoodProductCommand, Unit>,
    IRequestHandler<FoodProductQuery, FoodProduct?>,
    IRequestHandler<FoodProductsQuery, List<FoodProduct>>,
    IRequestHandler<FoodCategoriesQuery, List<FoodCategory>>
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

    public async Task<FoodProduct?> Handle(FoodProductQuery request, CancellationToken cancellationToken)
    {
        return _services.Repository.SetNoTracking<FoodProduct>().Where(p => p.ProductID == request.ProductID).FirstOrDefault();
    }

    public async Task<List<FoodProduct>> Handle(FoodProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _services.Repository.SetNoTracking<FoodProduct>().Where(p => p.CategoryID == request.CategoryID);

        if (query.Any() == true)
        {
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(recod => recod.ProductName.ToLower().Contains(search));
            }

            return query.OrderBy(recod => recod.ProductName).ToList();
        }

        return new List<FoodProduct>();
    }

    public async Task<List<FoodCategory>> Handle(FoodCategoriesQuery request, CancellationToken cancellationToken)
    {
        return _services.Repository.SetNoTracking<FoodCategory>().OrderBy(recod => recod.Name).ToList();
    }
}
