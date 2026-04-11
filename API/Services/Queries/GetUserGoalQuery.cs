using DataLayer.Models;
using MediatR;

namespace Services.Queries;

public class GetUserGoalQuery : IRequest<UserGoal?>
{
    public Guid UserId { get; set; }
}