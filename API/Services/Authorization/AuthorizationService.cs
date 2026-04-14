using DataLayer.Models.Users;
using DataLayer.Repository;
using Services.Authorization;
using System.Security.Cryptography;
using System.Text;

namespace Services;

public class AuthorizationService : IAuthorization
{
    private readonly IRepository _repository;

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

    public string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hashBytes = sha256.ComputeHash(combined);

        // multiple rounds for extra security
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

    /* Това да се добави във мауито преди да се прати request към контролера - за Краси
    // ISO/IEC 27001 — мин. 8 символа, главна, малка, цифра, специален символ
    private static bool IsPasswordValid(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        return Regex.IsMatch(password, @"[A-Z]") &&
               Regex.IsMatch(password, @"[a-z]") &&
               Regex.IsMatch(password, @"\d") &&
               Regex.IsMatch(password, @"[!@#$%^&*()\-_=+\[\]{};:'"",.<>?/\\|`~]");
    }
    */
}
