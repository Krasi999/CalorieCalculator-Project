using DataLayer.Models;
using DataLayer.Repository;
using MediatR;
using Services.Queries;

namespace Services.Handlers;

public class GetUserDetailsHandler
    : IRequestHandler<GetUserDetailsQuery, UserDetails?>
{
    private readonly IUserDetailsRepository _repo;

    public GetUserDetailsHandler(IUserDetailsRepository repo)
    {
        _repo = repo;
    }

    public Task<UserDetails?> Handle(
        GetUserDetailsQuery request,
        CancellationToken ct) =>
        _repo.GetByUserIdAsync(request.UserId);
}