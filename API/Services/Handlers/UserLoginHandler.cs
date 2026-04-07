using DataLayer.Models;
using DataLayer.Repository;
using BCrypt.Net;
using MediatR;
using Services.Commands;

namespace Services.Handlers;

public class UserLoginHandler : IRequestHandler<UserLoginCommand, UserAuthResult>
{
    private readonly IRepository _repository;

    public UserLoginHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserAuthResult> Handle(
        UserLoginCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Намираме потребителя
        var user = _repository
            .Set<User>()   // AsTracking — ще го обновяваме
            .FirstOrDefault(u => u.Email == request.Email.ToLowerInvariant().Trim());

        if (user == null || !user.IsActive)
            return new UserAuthResult
            {
                Success = false,
                ErrorMessage = "Невалиден имейл или парола."
            };

        // 2. Сравняване на хешовете
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return new UserAuthResult
            {
                Success = false,
                ErrorMessage = "Невалиден имейл или парола."
            };

        // 3. Обновяваме LastPasswordLogin
        user.UpdateLastPasswordLogin();
        await _repository.SaveChanges();

        // 4. Генерираме прост JWT токен
        //    (ще разширим с JwtService когато стигнем до него)
        var token = JwtHelper.GenerateToken(user.Id, user.Email);

        return new UserAuthResult
        {
            Success = true,
            UserId = user.Id,
            Token = token,
            RequiresPasswordReauth = false
        };
    }
}