using DataLayer.Models.Users;
using MediatR;

namespace Services.Queries;

public class GetUserDetailsQuery : IRequest<UserDetails?>
{
    public Guid UserId { get; set; }
}