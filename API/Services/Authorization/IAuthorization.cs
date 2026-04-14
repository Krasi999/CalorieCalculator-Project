namespace Services.Authorization;

public interface IAuthorization
{
    Task<bool> SignIn(string email, string password);

    Task<bool> LogIn(string email, string password);

    string HashPassword(string email, string salt);

    string GenerateSalt();
}
