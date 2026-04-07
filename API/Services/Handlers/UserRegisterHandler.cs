using DataLayer.Models;
using DataLayer.Repository;
using MediatR;
using BCrypt.Net;
using Services.Commands;
using System.Text.RegularExpressions;

namespace Services.Handlers;

public class UserRegisterHandler : IRequestHandler<UserRegisterCommand, UserAuthResult>
{
    private readonly IRepository _repository;

    public UserRegisterHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserAuthResult> Handle(
        UserRegisterCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Валидация на парола
        if (!IsPasswordValid(request.Password))
            return new UserAuthResult
            {
                Success = false,
                ErrorMessage = "Паролата трябва да съдържа поне 8 символа, " +
                               "главна и малка буква, цифра и специален символ."
            };

        // 2. Проверка за съществуващ имейл
        var exists = _repository
            .SetNoTracking<User>()
            .Any(u => u.Email == request.Email.ToLowerInvariant().Trim());

        if (exists)
            return new UserAuthResult
            {
                Success = false,
                ErrorMessage = "Имейлът вече е регистриран."
            };

        // 3. Хеширане на паролата
        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12);

        // 4. Създаване на потребител
        var user = new User();
        user.Create(request.Email, hash);

        await _repository.Add(user);
        await _repository.SaveChanges();

        return new UserAuthResult
        {
            Success = true,
            UserId = user.Id
        };
    }

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
}