using DataLayer.Models;
using MediatR;

namespace Services.Queries;

public class GetUserDetailsQuery : IRequest<UserDetails?>
{
    public Guid UserId { get; set; }
}