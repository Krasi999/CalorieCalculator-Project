using DataLayer.Models;
using DataLayer.Models.Users;
using DataLayer.Repository;
using MediatR;
using Services.Commands.User;

namespace Services.Handlers;

public class CreateUserDetailsHandler
    : IRequestHandler<CreateUserDetailsCommand, Guid>
{
    private readonly IUserDetailsRepository _repo;

    public CreateUserDetailsHandler(IUserDetailsRepository repo)
    {
        _repo = repo;
    }

    public async Task<Guid> Handle(
        CreateUserDetailsCommand request,
        CancellationToken ct)
    {
        var existing = await _repo.GetByUserIdAsync(request.UserId);
        if (existing is not null)
            throw new InvalidOperationException(
                "Потребителят вече има попълнен профил.");

        var details = new UserDetails();
        details.Create(
            request.UserId,
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

        await _repo.CreateAsync(details);
        await _repo.SaveChangesAsync();
        return details.Id;
    }
}