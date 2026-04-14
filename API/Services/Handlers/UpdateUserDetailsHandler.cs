using DataLayer.Repository;
using MediatR;
using Services.Commands.User;

namespace Services.Handlers;

public class UpdateUserDetailsHandler
    : IRequestHandler<UpdateUserDetailsCommand, bool>
{
    private readonly IUserDetailsRepository _repo;

    public UpdateUserDetailsHandler(IUserDetailsRepository repo)
    {
        _repo = repo;
    }

    public async Task<bool> Handle(
        UpdateUserDetailsCommand request,
        CancellationToken ct)
    {
        var details = await _repo.GetByUserIdAsync(request.UserId)
            ?? throw new KeyNotFoundException("Профилът не е намерен.");

        details.Update(
            request.Nickname,
            request.Gender,
            request.DateOfBirth,
            request.HeightCm,
            request.HeightFt,
            request.WeightKg,
            request.WeightLbs,
            request.ActivityLevel,
            request.CurrentGoal,       
            request.TargetWeightKg,   
            request.TargetWeightLbs);

        await _repo.UpdateAsync(details);
        await _repo.SaveChangesAsync();
        return true;
    }
}