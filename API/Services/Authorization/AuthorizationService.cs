using DataLayer.Models;
using DataLayer.Repository;
using Services.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace Services;

public class AuthorizationService : IAuthorization
{
    private readonly IRepository _repository;

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

    public async Task<string?> GeneratePasswordResetCode(string email)
    {
        var normalizedEmail = email.ToLowerInvariant().Trim();

        var user = _repository.SetNoTracking<User>()
            .FirstOrDefault(u => u.Email == normalizedEmail);

        if (user == null)
        {
            return null;
        }

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

        var code = GenerateSixDigitCode();
        var expiresAt = DateTime.UtcNow.AddMinutes(ResetCodeValidityMinutes);

        var resetCode = new PasswordResetCode(user.ID, code, expiresAt);

        await _repository.Add(resetCode);
        await _repository.SaveChanges();

        return code;
    }

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

        var salt = GenerateSalt();
        var hash = HashPassword(newPassword, salt);

        user.UpdatePassword(hash, salt);

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

    private static string GenerateSixDigitCode()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);

        var number = BitConverter.ToUInt32(bytes, 0) % 1000000;
        return number.ToString("D6");
    }
}