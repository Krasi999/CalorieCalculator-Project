using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Queries;

namespace CalorieCalculator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendarController : ControllerBase
{
    private readonly IServices _services;

    public CalendarController(IServices services)
    {
        _services = services;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCalendarData(Guid userId, [FromQuery] int year, [FromQuery] int month)
    {
        var result = await _services.Mediator.Send(new CalendarDataQuery
        {
            UserID = userId,
            Year = year,
            Month = month
        });

        return Ok(result);
    }
}
