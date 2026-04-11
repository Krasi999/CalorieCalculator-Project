using DataLayer.Models;
using DataLayer.Repository;
using MediatR;
using Services.Commands;

namespace Services.Handlers;

public class CreateUserGoalHandler
    : IRequestHandler<CreateUserGoalCommand, Guid>
{
    private readonly IUserDetailsRepository _repo;

    public CreateUserGoalHandler(IUserDetailsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(
        CreateUserGoalCommand request,
        CancellationToken ct)
    {
        // Деактивира старата цел преди да създаде нова
        await _repo.DeactivateAllGoalsAsync(request.UserId);

        var goal = new UserGoal();
        goal.Create(
            request.UserId,
            request.GoalType,
            request.TargetWeightKg,
            request.TargetWeightLbs,
            request.StartDate,
            request.EndDate);

        await _repo.CreateGoalAsync(goal);
        await _repo.SaveChangesAsync();
        return goal.Id;
    }
}