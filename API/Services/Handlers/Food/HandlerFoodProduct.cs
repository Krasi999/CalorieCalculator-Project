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

        transaction.Commit();

        return Unit.Value;
    }

    public async Task<int> Handle(FoodToMealCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _services.Repository.BeginTransaction();

        int mealId;

        if (request.MealID.HasValue && request.MealID.Value > 0)
        {
            mealId = request.MealID.Value;
        }
        else
        {
            var meal = new CalorieProgramMeal(request.MealType, request.ProgramID);

            await _services.Repository.Add(meal);
            await _services.Repository.SaveChanges();
            mealId = meal.MealID;
        }

        var mealFood = new MealFood(mealId, request.ProductID);

        await _services.Repository.Add(mealFood);
        await _services.Repository.SaveChanges();

        transaction.Commit();

        return mealId;
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
                query = query.Where(recod => recod.Name.ToLower().Contains(search));
            }

            return query.OrderBy(recod => recod.Name).ToList();
        }

        return new List<FoodProduct>();
    }

    public async Task<List<FoodCategory>> Handle(FoodCategoriesQuery request, CancellationToken cancellationToken)
    {
        return _services.Repository.SetNoTracking<FoodCategory>().OrderBy(recod => recod.Name).ToList();
    }
}
