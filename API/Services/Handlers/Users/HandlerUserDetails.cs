using MediatR;

namespace Services.Handlers.Users;

public class HandlerUserDetails : IRequest
{
    private readonly IServices _services;

    public HandlerUserDetails(IServices services)
    {
        _services = services;
    }
}
