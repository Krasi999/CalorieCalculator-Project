using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

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

        var user = _services.Repository.SetNoTracking<User>()
            .FirstOrDefault(u => u.Email == request.Email);

        var token = Guid.NewGuid().ToString("N");

        await _services.Mediator.Send(new CalorieProgramCommand { UserID = user.ID });

        return Ok(new { userId = user?.ID, token });
    }

    /// Стъпка 1 — генерира 6-цифрен код и го връща на клиента.
    /// По-късно може да се замени с реално пращане на имейл.
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return BadRequest();
        }

        var code = await _services.Authorization.GeneratePasswordResetCode(request.Email);

        if (code == null)
        {
            // Не съществува потребител с този имейл
            return NotFound();
        }

        // TODO: По-късно — пращане на кода по имейл вместо връщане в response-а
        return Ok(new { success = true, code });
    }

    /// Стъпка 2 — проверява дали въведеният код е валиден.
    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest();
        }

        var isValid = await _services.Authorization.VerifyPasswordResetCode(request.Email, request.Code);

        if (!isValid)
        {
            return Unauthorized();
        }

        return Ok();
    }

    /// Стъпка 3 — задаване на нова парола (кодът се проверява отново за сигурност).
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Code) ||
            string.IsNullOrWhiteSpace(request.NewPassword))
        {
            return BadRequest();
        }

        var success = await _services.Authorization.ResetPassword(
            request.Email, request.Code, request.NewPassword);

        if (!success)
        {
            return BadRequest();
        }

        return Ok();
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