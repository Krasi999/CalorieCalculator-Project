using DataLayer.Models.Users;
using MediatR;
using Services.Commands.User;


namespace Services.Handlers.Users;

public class HandlerUserGoal :
    IRequestHandler<UserGoalCommand, Unit>,
    IRequestHandler<UserGoalQuery, UserGoal?>
{
    private readonly IServices _services;

    public HandlerUserGoal(IServices services)
    {
        _services = services;
    }

    public async Task<Unit> Handle(UserGoalCommand request, CancellationToken cancellationToken)
    {
        var userGoal = request.ID != null
            ? _services.Repository.Set<UserGoal>().First(record => record.UserID == request.ID)
            : new UserGoal();

        using var transaction = await _services.Repository.BeginTransaction();

        if (request.ID == null)
        {
            userGoal.MapToUser(request.UserID);
        }

        userGoal.Update(
            request.GoalType,
            request.TargetWeightKg,
            request.TargetWeightLbs,
            request.StartDate,
            request.EndDate);

        _services.Repository.Save(userGoal, request.ID.HasValue);

        await _services.Repository.SaveChanges();

        return Unit.Value;
    }

    public async Task<UserGoal?> Handle(UserGoalQuery request, CancellationToken cancellationToken)
    {
        var userGoal = 
            _services.Repository.SetNoTracking<UserGoal>(request.Includes).FirstOrDefault(record => record.UserID == request.UserID);

        return userGoal;
    }
}
