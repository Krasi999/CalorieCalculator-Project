using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Queries;

namespace CalorieCalculator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DailyProgramController : ControllerBase
{
    private readonly IServices _services;

    public DailyProgramController(IServices services)
    {
        _services = services;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> DailyProgram(Guid userId, [FromQuery] string? date)
    {
        var targetDate = date != null? DateTime.Parse(date) : DateTime.UtcNow.Date;

        var result = await _services.Mediator.Send(new DailyProgramQuery
        {
            UserID = userId,
            Date = targetDate
        });

        if (result == null)
        {
            await _services.Mediator.Send(new CalorieProgramCommand { UserID = userId });
            
            result = await _services.Mediator.Send(new DailyProgramQuery
            {
                UserID = userId,
                Date = targetDate
            });
        }

        return Ok(result);
    }

    [HttpPost("meal/food-to-meal")]
    public async Task<IActionResult> FoodToMeal([FromBody] FoodToMealRequest request)
    {
        var mealId = await _services.Mediator.Send(new FoodToMealCommand
        {
            MealFoodID = request.MealFoodID,
            ProgramID = request.ProgramID,
            MealType = request.MealType,
            MealID = request.MealID,
            ProductID = request.ProductID,
            Weight = request.Weight
        });

        return Ok(new { mealId });
    }

    [HttpPost("meal/delete-food")]
    public async Task<IActionResult> RemoveFoodFromMeal([FromBody] int mealFoodId)
    {
        var result = await _services.Mediator.Send(new FoodToMealDeleteCommand
        {
            MealFoodID = mealFoodId
        });

        return Ok(new { removed = result });
    }

    [HttpGet("meal/{mealId}/foods")]
    public async Task<IActionResult> GetMealFoods(int mealId)
    {
        var result = await _services.Mediator.Send(new MealFoodsQuery
        {
            MealID = mealId
        });

        return Ok(result);
    }
}
