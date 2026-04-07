using System.Net.Http.Json;

namespace CalorieCalculator.Service;

// DTO-та за комуникация с API-то
public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
public record LoginResponse(Guid UserId, string Token, bool RequiresPasswordReauth);
public record SetBiometricRequest(Guid UserId, bool Enable);

public class AuthApiService
{
    private readonly ApiService _api;

    public AuthApiService(ApiService api)
    {
        _api = api;
    }

    public async Task<(bool Success, string? Error, Guid? UserId)> RegisterAsync(
        string email, string password)
    {
        try
        {
            var result = await _api.PostAsync<RegisterResultDto>(
                "api/auth/register",
                new RegisterRequest(email, password));

            return (true, null, result?.UserId);
        }
        catch (HttpRequestException ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<(bool Success, string? Error, LoginResponse? Data)> LoginAsync(
        string email, string password)
    {
        try
        {
            var result = await _api.PostAsync<LoginResponse>(
                "api/auth/login",
                new LoginRequest(email, password));

            return (true, null, result);
        }
        catch (HttpRequestException ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<bool> SetBiometricAsync(Guid userId, bool enable)
    {
        try
        {
            await _api.PostAsync<object>(
                "api/auth/biometric",
                new SetBiometricRequest(userId, enable));
            return true;
        }
        catch
        {
            return false;
        }
    }

    // Проверка дали са минали 72 часа
    public static bool RequiresPasswordReauth()
    {
        var stored = Preferences.Get("last_password_login", string.Empty);
        if (string.IsNullOrEmpty(stored)) return true;

        var lastLogin = DateTime.Parse(stored, null,
            System.Globalization.DateTimeStyles.RoundtripKind);
        return (DateTime.UtcNow - lastLogin).TotalHours >= 72;
    }

    private record RegisterResultDto(Guid UserId);
}