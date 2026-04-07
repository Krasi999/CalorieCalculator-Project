using MediatR;
using Microsoft.AspNetCore.Mvc;
using Services.Commands;

namespace CalorieCalculator.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Success)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(new { userId = result.UserId });
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Success)
            return Unauthorized(new { error = result.ErrorMessage });

        return Ok(new
        {
            userId = result.UserId,
            token = result.Token,
            requiresPasswordReauth = result.RequiresPasswordReauth
        });
    }

    // PATCH api/auth/biometric
    [HttpPatch("biometric")]
    public async Task<IActionResult> SetBiometric([FromBody] UserSetBiometricCommand command)
    {
        var success = await _mediator.Send(command);
        return success ? Ok() : NotFound();
    }
}