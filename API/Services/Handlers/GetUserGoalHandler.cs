using DataLayer.Models.Users;
using DataLayer.Repository;
using MediatR;
using Services.Queries;

namespace Services.Handlers;

public class GetUserGoalHandler
    : IRequestHandler<GetUserGoalQuery, UserGoal?>
{
    private readonly IUserDetailsRepository _repo;

    public GetUserGoalHandler(IUserDetailsRepository repo)
    {
        _repo = repo;
    }

    public Task<UserGoal?> Handle(
        GetUserGoalQuery request,
        CancellationToken ct) =>
        _repo.GetActiveGoalByUserIdAsync(request.UserId);
}