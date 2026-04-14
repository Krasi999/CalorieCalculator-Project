using DataLayer.Models.Users;
using MediatR;

public class UserDetailsQuery : IRequest<UserDetails?>
{
    public Guid UserID { get; set; }

    public string[] Includes { get; set; }
}