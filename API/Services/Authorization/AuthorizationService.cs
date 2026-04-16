using DataLayer.Models.Users;
using DataLayer.Repository;
using Services.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace Services;

public class AuthorizationService : IAuthorization
{
    private readonly IRepository _repository;

    // Валидност на кода за възстановяване в минути
    private const int ResetCodeValidityMinutes = 5;

    public AuthorizationService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> SignIn(string email, string password)
    {
        var exists = _repository.SetNoTracking<User>().Where(u => u.Email == email).Any();

        if (exists)
        {
            return false;
        }

        var salt = GenerateSalt();
        var hash = HashPassword(password, salt);

        var user = new User();

        user.Update(email, hash, salt);

        await _repository.Add(user);
        await _repository.SaveChanges();

        return true;
    }

    public async Task<bool> LogIn(string email, string password)
    {
        var user = _repository.SetNoTracking<User>().FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            return false;
        }

        var hash = HashPassword(password, user.Salt);

        return hash == user.PasswordHash;
    }

    public async Task<bool> SetBiometric(string email)
    {
        var user = _repository.SetNoTracking<User>().FirstOrDefault(u => u.Email == email);

        if (user == null)
        {
            return false;
        }

        user.SetBiometric(true);
        await _repository.SaveChanges();

        return true;
    }

    /// Генерира нов 6-цифрен код за възстановяване на парола.
    /// Деактивира всички предишни кодове за този потребител.
    public async Task<string?> GeneratePasswordResetCode(string email)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();

        var user = _repository.SetNoTracking<User>()
            .FirstOrDefault(u => u.Email == normalizedEmail);

        if (user == null)
        {
            return null;
        }

        // Деактивираме всички предишни неизползвани кодове на този потребител
        var oldCodes = await _repository.Find<PasswordResetCode>(
            c => c.UserId == user.ID && !c.IsUsed);

        foreach (var oldCode in oldCodes)
        {
            oldCode.MarkAsUsed();
            _repository.Update(oldCode);
        }

        if (oldCodes.Any())
        {
            await _repository.SaveChanges();
        }

        // Генерираме нов 6-цифрен код
        var code = GenerateSixDigitCode();
        var expiresAt = DateTime.UtcNow.AddMinutes(ResetCodeValidityMinutes);

        var resetCode = new PasswordResetCode(user.ID, code, expiresAt);

        await _repository.Add(resetCode);
        await _repository.SaveChanges();

        return code;
    }

    /// Проверява дали даденият код е валиден за този потребител.
    /// НЕ маркира кода като използван — само проверка.
    public async Task<bool> VerifyPasswordResetCode(string email, string code)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();

        var user = _repository.SetNoTracking<User>()
            .FirstOrDefault(u => u.Email == normalizedEmail);

        if (user == null)
        {
            return false;
        }

        var resetCode = _repository.SetNoTracking<PasswordResetCode>()
            .FirstOrDefault(c => c.UserId == user.ID && c.Code == code);

        return resetCode != null && resetCode.IsValid();
    }

    /// Сменя паролата след верификация на кода.
    /// Маркира кода като използван, за да не може да се ползва повторно.
    public async Task<bool> ResetPassword(string email, string code, string newPassword)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();

        var user = _repository.Set<User>()
            .FirstOrDefault(u => u.Email == normalizedEmail);

        if (user == null)
        {
            return false;
        }

        var resetCode = _repository.Set<PasswordResetCode>()
            .FirstOrDefault(c => c.UserId == user.ID && c.Code == code);

        if (resetCode == null || !resetCode.IsValid())
        {
            return false;
        }

        // Нов salt + хеш
        var salt = GenerateSalt();
        var hash = HashPassword(newPassword, salt);

        user.UpdatePassword(hash, salt);

        // Маркираме кода като използван
        resetCode.MarkAsUsed();

        await _repository.SaveChanges();

        return true;
    }

    public string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hashBytes = sha256.ComputeHash(combined);

        for (int i = 0; i < 10000; i++)
        {
            hashBytes = sha256.ComputeHash(
                Encoding.UTF8.GetBytes(Convert.ToBase64String(hashBytes) + salt));
        }

        return Convert.ToBase64String(hashBytes);
    }

    public string GenerateSalt()
    {
        var saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);

        return Convert.ToBase64String(saltBytes);
    }

    /// <summary>
    /// Генерира криптографски сигурен 6-цифрен код.
    /// </summary>
    private static string GenerateSixDigitCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);

        var number = BitConverter.ToUInt32(bytes, 0) % 1000000;
        return number.ToString("D6");
    }
}