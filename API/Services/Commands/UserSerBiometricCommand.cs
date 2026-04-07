using MediatR;

namespace Services.Commands;

public class UserSetBiometricCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public bool Enable { get; set; }
}