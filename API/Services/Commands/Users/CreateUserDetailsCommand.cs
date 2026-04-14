using DataLayer.Enums;
using MediatR;

namespace Services.Commands.User;

public class CreateUserDetailsCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public decimal? HeightCm { get; set; }
    public decimal? HeightFt { get; set; }
    public decimal? WeightKg { get; set; }
    public decimal? WeightLbs { get; set; }
    public ActivityLevel ActivityLevel { get; set; }

    public GoalType CurrentGoal { get; set; }
    public decimal? TargetWeightKg { get; set; }
    public decimal? TargetWeightLbs { get; set; }
}