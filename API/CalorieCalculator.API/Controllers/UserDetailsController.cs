using DataLayer.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Commands.User;

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

    // POST api/userdetails/save
    [HttpPost("save")]
    public async Task<IActionResult> SaveUserDetails([FromBody] ProfileDataRequest request)
    {
        var result = await _mediator.Send(new UserDetailsCommand
        {
            UserID = request.UserID.Value,
            Nickname = request.Nickname,
            Gender = (Gender)request.Gender.Value,
            DateOfBirth = request.DateOfBirth.Value,
            HeightCm = request.HeightCm,
            HeightFt = request.HeightFt,
            WeightKg = request.WeightKg,
            WeightLbs = request.WeightLbs,
            ActivityLevel = (ActivityLevel)request.ActivityLevel.Value,
            CurrentGoal = (GoalType)request.CurrentGoal.Value,
            TargetWeightKg = request.TargetWeightKg,
            TargetWeightLbs = request.TargetWeightLbs,
        });

        return Ok(new ProfileDataResponse() { Success = result, UserID = request.UserID});
    }

    // POST api/userdetails/goal
    [HttpPost("goal")]
    public async Task<IActionResult> SaveUserGoal(
        [FromBody] UserGoalCommand command)
    {
        await _mediator.Send(command);
        return Ok();
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
    }

    // PUT api/userdetails
    [HttpPut]
    public async Task<IActionResult> UpdateUserDetails(
        [FromBody] UpdateUserDetailsCommand command)
    {
        var success = await _mediator.Send(command);
        return Ok(new { success });
    }

    // GET api/userdetails/{userId}/goal
    [HttpGet("{userId:guid}/goal")]
    public async Task<IActionResult> GetUserGoal(Guid userId)
    {
        var query = new GetUserGoalQuery { UserId = userId };
        var result = await _mediator.Send(query);
        if (result is null)
            return NotFound(new { message = "Няма активна цел." });
        return Ok(result);
    }

    // PUT api/userdetails/goal
    [HttpPut("goal")]
    public async Task<IActionResult> UpdateUserGoal(
        [FromBody] UpdateUserGoalCommand command)
    {
        var success = await _mediator.Send(command);
        return Ok(new { success });
    }
    */
}