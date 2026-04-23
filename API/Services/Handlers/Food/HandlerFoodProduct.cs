using DataLayer.Models;
using MediatR;
using Services.Queries;


namespace Services.Handlers.Food;

public class HandlerFoodProduct :
    IRequestHandler<FoodProductCommand, Unit>,
    IRequestHandler<FoodToMealCommand, int>,
    IRequestHandler<FoodToMealDeleteCommand, bool>,
    IRequestHandler<FoodProductQuery, FoodProduct?>,
    IRequestHandler<FoodProductsQuery, List<FoodProduct>>,
    IRequestHandler<FoodCategoriesQuery, List<FoodCategory>>,
    IRequestHandler<MealFoodsQuery, List<MealFoodResponse>>
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

        if (request.MealFoodID.HasValue && request.MealFoodID.Value > 0)
        {
            var existingMealFood = _services.Repository.Set<MealFood>()
                .First(mf => mf.MealFoodID == request.MealFoodID.Value);

            existingMealFood.Update(request.Weight);

            _services.Repository.Save(existingMealFood, true);
            await _services.Repository.SaveChanges();

            transaction.Commit();

            return existingMealFood.MealID;
        }

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

        var mealFood = new MealFood(mealId, request.ProductID, request.Weight);

        await _services.Repository.Add(mealFood);
        await _services.Repository.SaveChanges();

        transaction.Commit();

        return mealId;
    }

    public async Task<bool> Handle(FoodToMealDeleteCommand request, CancellationToken cancellationToken)
    {
        using var transaction = await _services.Repository.BeginTransaction();

        var mealFood = _services.Repository.Set<MealFood>().FirstOrDefault(mf => mf.MealFoodID == request.MealFoodID);

        if (mealFood == null)
            return false;

        _services.Repository.Delete(mealFood);
        await _services.Repository.SaveChanges();

        transaction.Commit();

        return true;
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

    public async Task<List<MealFoodResponse>> Handle(MealFoodsQuery request, CancellationToken cancellationToken)
    {
        return _services.Repository.SetNoTracking<MealFood>(nameof(MealFood.FoodProduct))
            .Where(mf => mf.MealID == request.MealID)
            .Select(mf => new MealFoodResponse
            {
                MealFoodID = mf.MealFoodID,
                ProductID = mf.ProductID,
                Name = mf.FoodProduct != null ? mf.FoodProduct.Name ?? "" : "",
                Weight = mf.Weight,
                Calories = (int)Math.Round((mf.FoodProduct != null ? mf.FoodProduct.Calories : 0) * mf.Weight / 100.0),
                Protein = Math.Round((mf.FoodProduct != null ? mf.FoodProduct.Protein : 0) * mf.Weight / 100m, 1),
                Carbs = Math.Round((mf.FoodProduct != null ? mf.FoodProduct.Carbs : 0) * mf.Weight / 100m, 1),
                Fats = Math.Round((mf.FoodProduct != null ? mf.FoodProduct.Fats : 0) * mf.Weight / 100m, 1)
            })
            .ToList();
    }
}
