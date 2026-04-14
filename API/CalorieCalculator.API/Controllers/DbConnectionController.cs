using DataLayer.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;

namespace CalorieCalculator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DbConnectionController : ControllerBase
{
    private readonly AppDbContext _db;

    public DbConnectionController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("db-connection")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            await _db.Database.OpenConnectionAsync();
            await _db.Database.CloseConnectionAsync();

            return Ok(new { connected = true });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
        }
    }
}
