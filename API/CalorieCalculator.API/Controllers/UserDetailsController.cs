using DataLayer.Enums;
using DataLayer.Models;
using MediatR;
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

    [HttpGet("{userId:guid}/monthly-calories/{year:int}/{month:int}")]
    public async Task<IActionResult> GetMonthlyCalories(Guid userId, int year, int month)
    {
        var details = await _services.Mediator.Send(new UserDetailsQuery
        {
            UserID = userId,
            Includes = new string[] { }
        });

        if (details == null)
            return NotFound();

        // Целеви калории от TDEE
        var tdee = details.CalculateTDEE();
        var goal = details.CurrentGoal;
        decimal targetCalories = tdee ?? 2000m;

        targetCalories = goal switch
        {
            DataLayer.Enums.GoalType.WeightLoss => targetCalories - 500,
            DataLayer.Enums.GoalType.WeightGain => targetCalories + 300,
            DataLayer.Enums.GoalType.MuscleGain => targetCalories + 200,
            _ => targetCalories
        };

        // Реални дневни калории от базата
        var startDate = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1);

        var dailyRecords = _services.Repository.SetNoTracking<DataLayer.Models.Users.DailyCalories>()
            .Where(dc => dc.UserId == userId && dc.Date >= startDate && dc.Date < endDate)
            .ToList();

        var dailyCalories = dailyRecords.ToDictionary(
            dc => dc.Date.ToString("yyyy-MM-dd"),
            dc => dc.CaloriesEaten);

        return Ok(new
        {
            targetCalories = Math.Round(targetCalories),
            dailyCalories
        });
    }

    [HttpPost("daily-calories")]
    public async Task<IActionResult> SaveDailyCalories([FromBody] SaveDailyCaloriesRequest request)
    {
        var existing = _services.Repository.Set<DataLayer.Models.Users.DailyCalories>()
            .FirstOrDefault(dc => dc.UserId == request.UserId
                && dc.Date.Date == request.Date.Date);

        if (existing != null)
        {
            existing.SetCalories(request.CaloriesEaten);
        }
        else
        {
            var record = new DataLayer.Models.Users.DailyCalories(
                request.UserId, request.Date, request.CaloriesEaten);
            await _services.Repository.Add(record);
        }

        await _services.Repository.SaveChanges();
        return Ok();
    }

    // Endpoint за добавяне на калории към деня (от MainPage при добавяне на храна)
    [HttpPost("add-calories")]
    public async Task<IActionResult> AddDailyCalories([FromBody] AddDailyCaloriesRequest request)
    {
        var today = DateTime.UtcNow.Date;

        var existing = _services.Repository.Set<DataLayer.Models.Users.DailyCalories>()
            .FirstOrDefault(dc => dc.UserId == request.UserId && dc.Date.Date == today);

        if (existing != null)
        {
            existing.AddCalories(request.Calories);
        }
        else
        {
            var record = new DataLayer.Models.Users.DailyCalories(
                request.UserId, today, request.Calories);
            await _services.Repository.Add(record);
        }

        await _services.Repository.SaveChanges();
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