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
                "api/Authentication/signin",
                new RegisterRequest(email, password));

            return (true, null, result?.UserId);
        }
        catch (HttpRequestException ex)
        {
            return (false, GetFriendlyError(ex, isLogin: false), null);
        }
        catch (Exception)
        {
            return (false, "Няма връзка със сървъра. Провери интернет връзката.", null);
        }
    }

    public async Task<(bool Success, string? Error, LoginResponse? Data)> LoginAsync(
        string email, string password)
    {
        try
        {
            var result = await _api.PostAsync<LoginResponse>(
                "api/Authentication/login",
                new LoginRequest(email, password));

            return (true, null, result);
        }
        catch (HttpRequestException ex)
        {
            return (false, GetFriendlyError(ex, isLogin: true), null);
        }
        catch (Exception)
        {
            return (false, "Няма връзка със сървъра. Провери интернет връзката.", null);
        }
    }

    public async Task<bool> SetBiometricAsync(Guid userId, bool enable)
    {
        try
        {
            await _api.PostAsync<object>(
                "api/Authentication/biometric",
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

    /// <summary>
    /// Превръща HTTP грешка в разбираемо за потребителя съобщение.
    /// </summary>
    private static string GetFriendlyError(HttpRequestException ex, bool isLogin)
    {
        return ex.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "Невалиден имейл или парола.",
            System.Net.HttpStatusCode.BadRequest when isLogin => "Невалиден имейл или парола.",
            System.Net.HttpStatusCode.BadRequest => "Потребител с този имейл вече съществува.",
            System.Net.HttpStatusCode.NotFound => "Услугата не е намерена. Провери връзката.",
            _ => "Няма връзка със сървъра. Опитай отново."
        };
    }

    private record RegisterResultDto(Guid UserId);
}