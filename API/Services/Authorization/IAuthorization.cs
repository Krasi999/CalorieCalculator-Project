namespace Services.Authorization;

public interface IAuthorization
{
    Task<bool> SignIn(string email, string password);

    Task<bool> LogIn(string email, string password);

    /// Генерира нов 6-цифрен код за възстановяване на парола.
    /// Връща кода само ако потребителят съществува, иначе null.
    Task<string?> GeneratePasswordResetCode(string email);

    /// Проверява дали кодът е валиден за дадения имейл.
    Task<bool> VerifyPasswordResetCode(string email, string code);

    /// Сменя паролата след успешна верификация на кода.
    Task<bool> ResetPassword(string email, string code, string newPassword);

    string HashPassword(string email, string salt);

    string GenerateSalt();
}
