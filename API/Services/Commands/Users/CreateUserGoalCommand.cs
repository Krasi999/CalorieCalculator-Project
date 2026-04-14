using DataLayer.Enums;
using MediatR;

namespace Services.Commands.User;

public class CreateUserGoalCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public GoalType GoalType { get; set; }
    public decimal? TargetWeightKg { get; set; }
    public decimal? TargetWeightLbs { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}