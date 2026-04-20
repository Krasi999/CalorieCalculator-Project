namespace Services.Authorization;

public interface IAuthorization
{
    Task<bool> SignIn(string email, string password);

    Task<bool> LogIn(string email, string password);

    Task<string?> GeneratePasswordResetCode(string email);

    Task<bool> VerifyPasswordResetCode(string email, string code);

    Task<bool> ResetPassword(string email, string code, string newPassword);

    string HashPassword(string email, string salt);

    string GenerateSalt();
}
