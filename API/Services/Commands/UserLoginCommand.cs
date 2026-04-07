using MediatR;

namespace Services.Commands;

public class UserLoginCommand : IRequest<UserAuthResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}