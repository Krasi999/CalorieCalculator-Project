using Microsoft.AspNetCore.Mvc;
using Services;

namespace CalorieCalculator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoodController : ControllerBase
{
    private readonly IServices _services;

    public FoodController(IServices services)
    {
        _services = services;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _services.Mediator.Send(new FoodCategoriesQuery());
        return Ok(result);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId, [FromQuery] string? search)
    {
        var result = await _services.Mediator.Send(new ProductsQuery
        {
            CategoryID = categoryId,
            SearchTerm = search
        });
        return Ok(result);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetById(int productId)
    {
        var result = await _services.Mediator.Send(new GetProductByIdQuery
        {
            ProductID = productId
        });

        if (result == null)
            return NotFound();

        return Ok(result);
    }
}
