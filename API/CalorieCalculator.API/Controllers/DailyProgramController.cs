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
    public async Task<IActionResult> DailyProgram(Guid userId)
    {
        var result = await _services.Mediator.Send(new DailyProgramQuery
        {
            UserID = userId,
            Date = DateTime.UtcNow.Date
        });

        if (result == null)
        {
            await _services.Mediator.Send(new CalorieProgramCommand { UserID = userId });
            
            result = await _services.Mediator.Send(new DailyProgramQuery
            {
                UserID = userId,
                Date = DateTime.UtcNow.Date
            });
        }

        return Ok(result);
    }
}
