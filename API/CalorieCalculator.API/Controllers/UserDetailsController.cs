using DataLayer.Enums;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Commands.User;

namespace CalorieCalculator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserDetailsController : ControllerBase
{
    private readonly IServices _services;

    public UserDetailsController(IServices services)
    {
        _services = services;
    }

    [HttpPost("save")]
    public async Task<IActionResult> SaveUserDetails([FromBody] ProfileDataRequest request)
    {
        var result = await _services.Mediator.Send(new UserDetailsCommand
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

        await _services.Mediator.Send(new CalorieProgramCommand { UserID = request.UserID.Value });

        return Ok(new ProfileDataResponse() { Success = result, UserID = request.UserID});
    }

    [HttpPost("goal")]
    public async Task<IActionResult> SaveUserGoal(
        [FromBody] UserGoalCommand command)
    {
        await _services.Mediator.Send(command);
        return Ok();
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserDetails(Guid userId)
    {
        var details = await _services.Mediator.Send(new UserDetailsQuery
        {
            UserID = userId,
            Includes = new string[] { }
        });

        var goal = await _services.Mediator.Send(new UserGoalQuery
        {
            UserID = userId,
            Includes = new string[] { }
        });

        if (details == null)
            return NotFound();

        return Ok(new
        {
            details.Nickname,
            Gender = (int)details.Gender,
            DateOfBirth = details.DateOfBirth.ToString("O"),
            details.HeightCm,
            details.HeightFt,
            details.WeightKg,
            details.WeightLbs,
            ActivityLevel = (int)details.ActivityLevel,
            CurrentGoal = (int)details.CurrentGoal,
            details.TargetWeightKg,
            details.TargetWeightLbs,
            GoalType = goal != null ? (int)goal.GoalType : 0,
            GoalStartDate = goal?.StartDate.ToString("O"),
            GoalEndDate = goal?.EndDate.ToString("O")
        });
    }

    [HttpPost("update-nickname")]
    public async Task<IActionResult> UpdateNickname([FromBody] UpdateNicknameRequest request)
    {
        var userDetails =  _services.Repository.Set<DataLayer.Models.UserDetails>()
            .FirstOrDefault(ud => ud.UserID == request.UserID);

        if (userDetails == null)
            return NotFound();

        userDetails.UpdateNickname(request.Nickname);
        await _services.Repository.SaveChanges();

        return Ok();
    }
}