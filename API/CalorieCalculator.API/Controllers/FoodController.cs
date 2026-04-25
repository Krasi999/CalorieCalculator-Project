using DataLayer.Enums;
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

    [HttpGet("foodcategory")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _services.Mediator.Send(new FoodCategoriesQuery());

        return Ok(result);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId, [FromQuery] string? search)
    {
        var result = await _services.Mediator.Send(new FoodProductsQuery
        {
            CategoryID = categoryId,
            SearchTerm = search
        });

        return Ok(result);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetById(int productId)
    {
        var result = await _services.Mediator.Send(new FoodProductQuery
        {
            ProductID = productId
        });

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromBody] FoodProductRequest request)
    {
        await _services.Mediator.Send(new FoodProductCommand
        {
            ProductID = request.ProductID,
            ProductName = request.ProductName,
            Description = request.Description,
            Calories = request.Calories,
            Weight = request.Weight,
            Fats = request.Fats,
            Protein = request.Protein,
            Carbs = request.Carbs,
            Category = (FoodCategories)request.CategoryID,
            Barcode = request.Barcode
        });

        return Ok(new { message = "Продукта е запазен успешно" });
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode)
    {
        var result = await _services.Mediator.Send(new FoodProductByBarcodeQuery
        {
            Barcode = barcode
        });

        return Ok(result);
    }
}
