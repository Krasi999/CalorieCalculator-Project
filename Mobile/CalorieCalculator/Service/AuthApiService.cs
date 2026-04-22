using System.Text.Json.Serialization;

namespace CalorieCalculator.Service;

public record RegisterRequest(string Email, string Password);

public record LoginRequest(string Email, string Password);

public record LoginResponse(Guid UserId, string Token, bool RequiresPasswordReauth);

public record SetBiometricRequest(Guid UserId, bool Enable);

public record ForgotPasswordRequest(string Email);

public record ForgotPasswordResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("code")] string Code);

public record VerifyCodeRequest(string Email, string Code);

public record ResetPasswordApiRequest(string Email, string Code, string NewPassword);

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

    public async Task<(bool Success, string? Error, string? Code)> ForgotPasswordAsync(string email)
    {
        try
        {
            var result = await _api.PostAsync<ForgotPasswordResponse>(
                "api/Authentication/forgot-password",
                new ForgotPasswordRequest(email));

            return (true, null, result?.Code);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                return (false, "Няма регистриран потребител с този имейл.", null);

            return (false, "Няма връзка със сървъра.", null);
        }
        catch (Exception)
        {
            return (false, "Няма връзка със сървъра.", null);
        }
    }

    public async Task<(bool Success, string? Error)> VerifyCodeAsync(string email, string code)
    {
        try
        {
            await _api.PostAsync(
                "api/Authentication/verify-code",
                new VerifyCodeRequest(email, code));

            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return (false, "Невалиден или изтекъл код. Опитайте отново.");

            return (false, "Няма връзка със сървъра.");
        }
        catch (Exception)
        {
            return (false, "Няма връзка със сървъра.");
        }
    }

    public async Task<(bool Success, string? Error)> ResetPasswordAsync(
        string email, string code, string newPassword)
    {
        try
        {
            await _api.PostAsync(
                "api/Authentication/reset-password",
                new ResetPasswordApiRequest(email, code, newPassword));

            return (true, null);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                return (false, "Невалиден код или грешка при смяна на паролата.");

            return (false, "Няма връзка със сървъра.");
        }
        catch (Exception)
        {
            return (false, "Няма връзка със сървъра.");
        }
    }

    public static bool RequiresPasswordReauth()
    {
        var stored = Preferences.Get("last_password_login", string.Empty);
        if (string.IsNullOrEmpty(stored)) return true;

        var lastLogin = DateTime.Parse(stored, null,
            System.Globalization.DateTimeStyles.RoundtripKind);
        return (DateTime.UtcNow - lastLogin).TotalHours >= 72;
    }

    private static string GetFriendlyError(HttpRequestException ex, bool isLogin)
    {
        return ex.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "Невалиден имейл или парола.",
            System.Net.HttpStatusCode.BadRequest when isLogin => "Невалиден имейл или парола.",
            System.Net.HttpStatusCode.BadRequest => "Потребител с този имейл вече съществува.",
            System.Net.HttpStatusCode.NotFound => "Услугата не е намерена.",
            _ => "Няма връзка със сървъра. Опитай отново."
        };
    }

    private record RegisterResultDto(Guid UserId);


}