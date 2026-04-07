using MediatR;

namespace Services.Commands;

public class UserRegisterCommand : IRequest<UserAuthResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

// Споделен резултат за Register и Login
public class UserAuthResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Guid? UserId { get; set; }
    public string? Token { get; set; }

    // Казва на мобилното приложение дали да иска парола
    // въпреки че биометрията е включена
    public bool RequiresPasswordReauth { get; set; }
}