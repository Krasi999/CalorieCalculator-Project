using DataLayer.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Commands.User;

namespace CalorieCalculator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IServices _services;

    public AuthenticationController(IServices services)
    {
        _services = services;
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        var result = await _services.Authorization.SignIn(request.Email, request.Password);

        if (!result)
        {
            return BadRequest();
        }

        // Вземаме UserId от базата, за да го върнем на мобилния клиент
        var user = _services.Repository.SetNoTracking<User>()
            .FirstOrDefault(u => u.Email == request.Email);

        return Ok(new { userId = user?.ID });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LogIn([FromBody] LogInRequest request)
    {
        var result = await _services.Authorization.LogIn(request.Email, request.Password);

        if (!result)
        {
            return Unauthorized();
        }

        // Вземаме UserId от базата след успешен login
        var user = _services.Repository.SetNoTracking<User>()
            .FirstOrDefault(u => u.Email == request.Email);

        // TODO: реален JWT token по-късно
        var token = Guid.NewGuid().ToString("N");

        return Ok(new { userId = user?.ID, token });
    }

    /* TODO да се добави подобна логика при желание за активиране на биометрия от потребителя
    [HttpPatch("biometric")]
    public async Task<IActionResult> SetBiometric([FromBody] UserSetBiometricCommand command)
    {
        var success = await _services.Mediator.Send(command);

        return success ? Ok() : NotFound();
    }
    */
}