using DataLayer.Models.Users;
using MediatR;


namespace Services.Handlers.Users;

public class HandlerUser : IRequest
{
    private readonly IServices _services;

    public HandlerUser(IServices services)
    {
        _services = services;
    }
}
