using DataLayer.Enums;
using DataLayer.Models;
using MediatR;
using Services.Queries;


namespace Services.Handlers.Users;

public class HandlerCalorieProgram :
    IRequestHandler<CalorieProgramCommand, bool>,
    IRequestHandler<DailyProgramQuery, DailyProgramResponse?>
{
    private readonly IServices _services;

    public HandlerCalorieProgram(IServices services)
    {
        _services = services;
    }

    public async Task<bool> Handle(CalorieProgramCommand request, CancellationToken cancellationToken)
    {
        var userDetails = _services.Repository.SetNoTracking<UserDetails>().FirstOrDefault(record => record.UserID == request.UserID);

        if (userDetails == null)
        {
            return false;
        }

        if (ValidateCalorieProgram(request.UserID))
        {
            return false;
        }

        using var transaction = await _services.Repository.BeginTransaction();

        var caloriesPerDay = CalculateCaloriesPerDay(
            userDetails.Gender,
            userDetails.WeightKg.Value,
            userDetails.HeightCm.Value,
            userDetails.DateOfBirth,
            userDetails.ActivityLevel,
            userDetails.CurrentGoal);

        var fatsPerDay = CalculateFatsPerDay(caloriesPerDay, userDetails.WeightKg.Value, userDetails.CurrentGoal);
        var proteinPerDay = CalculateProteinPerDay(caloriesPerDay, userDetails.WeightKg.Value, userDetails.CurrentGoal);
        var carbsPerDay = CalculateCarbsPerDay(caloriesPerDay, proteinPerDay, fatsPerDay);

        var lastProgramDate = LastCalorieProgramDate(userDetails.CreatedAt.Date, userDetails.UserID);

        for (var date = lastProgramDate; date <= DateTime.UtcNow.Date; date = date.AddDays(1))
        {
            var calorieProgram = new CalorieProgram(caloriesPerDay, carbsPerDay, proteinPerDay, fatsPerDay, date, request.UserID);

            await _services.Repository.Add(calorieProgram);
            await _services.Repository.SaveChanges();
        }

        transaction.Commit();

        return true;
    }

    public async Task<DailyProgramResponse?> Handle(DailyProgramQuery request, CancellationToken cancellationToken)
    {
        var program = _services.Repository.SetNoTracking<CalorieProgram>(nameof(CalorieProgram.CalorieProgramMeals))
            .Where(p => p.UserID == request.UserID && p.ProgramDate.Date == request.Date.Date).FirstOrDefault();

        if (program == null)
        {
            return null;
        }

        var existingMeals = program.CalorieProgramMeals.ToList();

        var mealIds = existingMeals.Select(m => m.MealID).ToList();
        var mealFoods = mealIds.Any()
            ? _services.Repository.SetNoTracking<MealFood>(nameof(MealFood.FoodProduct))
                .Where(mf => mealIds.Contains(mf.MealID))
                .ToList()
            : new List<MealFood>();

        var allMealTypes = new[] { (int)MealTypes.Breakfast, (int)MealTypes.Lunch, (int)MealTypes.Dinner, (int)MealTypes.Snack };

        var meals = allMealTypes.Select(mealType =>
        {
            var existingMeal = existingMeals.FirstOrDefault(m => m.MealType == mealType);

            return new MealResponse
            {
                MealID = existingMeal?.MealID ?? 0,
                MealType = mealType,
                Exists = existingMeal != null,
                Foods = existingMeal != null
                    ? mealFoods
                        .Where(mf => mf.MealID == existingMeal.MealID)
                        .Select(mf => new MealFoodResponse
                        {
                            MealFoodID = mf.MealFoodID,
                            ProductID = mf.ProductID,
                            Name = mf.FoodProduct?.Name ?? "",
                            Weight = mf.Weight,
                            Calories = (int)Math.Round((mf.FoodProduct?.Calories ?? 0) * mf.Weight / 100.0),
                            Protein = Math.Round((mf.FoodProduct?.Protein ?? 0) * mf.Weight / 100m, 1),
                            Carbs = Math.Round((mf.FoodProduct?.Carbs ?? 0) * mf.Weight / 100m, 1),
                            Fats = Math.Round((mf.FoodProduct?.Fats ?? 0) * mf.Weight / 100m, 1)
                        }).ToList()
                    : new List<MealFoodResponse>()
            };
        }).ToList();

        return new DailyProgramResponse
        {
            ProgramID = program.ProgramID,
            CaloriesPerDay = program.CaloriesPerDay,
            CarbsPerDay = program.CarbsPerDay,
            ProteinPerDay = program.ProteinPerDay,
            FatsPerDay = program.FatsPerDay,
            Meals = meals
        };
    }

    public static int CalculateCaloriesPerDay(Gender gender, decimal weightKg, decimal heightCm, DateTime dateOfBirth, ActivityLevel activityLevel, GoalType goal)
    {
        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

        var bmr = gender switch
        {
            Gender.Male => (10 * weightKg) + (6.25m * heightCm) - (5 * age) + 5,
            Gender.Female => (10 * weightKg) + (6.25m * heightCm) - (5 * age) - 161,
            _ => 0
        };

        var multiplier = activityLevel switch
        {
            ActivityLevel.Sedentary => 1.2m,
            ActivityLevel.LightlyActive => 1.375m,
            ActivityLevel.ModerateActive => 1.55m,
            ActivityLevel.VeryActive => 1.725m,
            ActivityLevel.ExtraActive => 1.9m,
            _ => 1.2m
        };

        var tdee = bmr * multiplier;

        var calories = goal switch
        {
            GoalType.WeightLoss => tdee - 500,
            GoalType.Maintenance => tdee,
            GoalType.WeightGain => tdee + 300,
            GoalType.MuscleGain => tdee + 400,
            _ => tdee
        };

        return (int)Math.Round(calories);
    }

    public static int CalculateFatsPerDay(int caloriesPerDay, decimal weightKg, GoalType goal)
    {
        var fatPercentage = goal switch
        {
            GoalType.WeightLoss => 0.25m,
            GoalType.Maintenance => 0.30m,
            GoalType.WeightGain => 0.28m,
            GoalType.MuscleGain => 0.25m,
            _ => 0.30m
        };

        var fatCalories = (int)Math.Round(caloriesPerDay * fatPercentage);
        return (int)Math.Round(fatCalories / 9m);
    }

    public static int CalculateProteinPerDay(int caloriesPerDay, decimal weightKg, GoalType goal)
    {
        var proteinPerKg = goal switch
        {
            GoalType.WeightLoss => 2.0m,
            GoalType.Maintenance => 1.6m,
            GoalType.WeightGain => 1.8m,
            GoalType.MuscleGain => 2.2m,
            _ => 1.6m
        };

        return (int)Math.Round(weightKg * proteinPerKg);
    }

    public static int CalculateCarbsPerDay(int caloriesPerDay, int proteinPerDay, int fatsPerDay)
    {
        var proteinCalories = proteinPerDay * 4;
        var fatCalories = fatsPerDay * 9;
        var carbsCalories = caloriesPerDay - proteinCalories - fatCalories;
        return (int)Math.Round(carbsCalories / 4m);
    }

    public bool ValidateCalorieProgram(Guid userID)
    {
        var calorieProgram = _services.Repository.SetNoTracking<CalorieProgram>().FirstOrDefault(record => record.UserID == userID && record.ProgramDate == DateTime.UtcNow.Date);

        if (calorieProgram != null)
        {
            return true;
        }

        return false;
    }

    public DateTime LastCalorieProgramDate(DateTime date, Guid userID)
    {
        var caloriePrograms = _services.Repository.SetNoTracking<CalorieProgram>().Where(record => record.UserID == userID && record.ProgramDate.Date == date.Date).OrderByDescending(record => record.ProgramDate);

        if (caloriePrograms.Any() == true)
        {
            return caloriePrograms.First().ProgramDate.Date.AddDays(1);
        }

        return date;
    }
}
