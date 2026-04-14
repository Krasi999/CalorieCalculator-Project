using DataLayer.Enums;
using MediatR;

namespace Services.Commands.User;

public class UpdateUserGoalCommand : IRequest<bool>
{
    public Guid GoalId { get; set; }
    public Guid UserId { get; set; }
    public GoalType GoalType { get; set; }
    public decimal? TargetWeightKg { get; set; }
    public decimal? TargetWeightLbs { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}