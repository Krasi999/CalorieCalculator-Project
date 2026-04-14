using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Commands.User;
using Services.Queries;

namespace CalorieCalculator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserDetailsController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserDetailsController(IMediator mediator)
    {
        _mediator = mediator;
    }
    /*
    // GET api/userdetails/{userId}
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserDetails(Guid userId)
    {
        var query = new GetUserDetailsQuery { UserId = userId };
        var result = await _mediator.Send(query);

        if (result is null)
            return NotFound(new { message = "Profile not found." });

        return Ok(result);
    }*/

    // POST api/userdetails
    [HttpPost]
    public async Task<IActionResult> CreateUserDetails(
        [FromBody] CreateUserDetailsCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    // POST api/userdetails
    [HttpPost]
    public async Task<IActionResult> UpdateUserDetails(
        [FromBody] UpdateUserDetailsCommand command)
    {
        var success = await _mediator.Send(command);
        return Ok(new { success });
    }
    /*
    // GET api/userdetails/{userId}/goal
    [HttpGet("{userId:guid}/goal")]
    public async Task<IActionResult> GetUserGoal(Guid userId)
    {
        var query = new GetUserGoalQuery { UserId = userId };
        var result = await _mediator.Send(query);

        if (result is null)
            return NotFound(new { message = "Няма активна цел." });

        return Ok(result);
    }*/

    // POST api/userdetails/goal
    [HttpPost("goal")]
    public async Task<IActionResult> CreateUserGoal(
        [FromBody] CreateUserGoalCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    // POST api/userdetails/goal
    [HttpPost("goal")]
    public async Task<IActionResult> UpdateUserGoal(
        [FromBody] UpdateUserGoalCommand command)
    {
        var success = await _mediator.Send(command);
        return Ok(new { success });
    }
}