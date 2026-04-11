using DataLayer.Repository;
using MediatR;
using Services.Commands;

namespace Services.Handlers;

public class UpdateUserGoalHandler
    : IRequestHandler<UpdateUserGoalCommand, bool>
{
    private readonly IUserDetailsRepository _repo;

    public UpdateUserGoalHandler(IUserDetailsRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(
        UpdateUserGoalCommand request,
        CancellationToken ct)
    {
        var goal = await _repo.GetActiveGoalByUserIdAsync(request.UserId)
            ?? throw new KeyNotFoundException("Целта не е намерена.");

        goal.Update(
            request.GoalType,
            request.TargetWeightKg,
            request.TargetWeightLbs,
            request.StartDate,
            request.EndDate);

        await _repo.UpdateGoalAsync(goal);
        await _repo.SaveChangesAsync();
        return true;
    }
}