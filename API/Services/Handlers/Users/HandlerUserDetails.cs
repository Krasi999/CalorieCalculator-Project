using DataLayer.Models;
using MediatR;


namespace Services.Handlers.Users;

public class HandlerUserDetails :
    IRequestHandler<UserDetailsCommand, bool>,
    IRequestHandler<UserDetailsQuery, UserDetails?>
{
    private readonly IServices _services;

    public HandlerUserDetails(IServices services)
    {
        _services = services;
    }

    public async Task<bool> Handle(UserDetailsCommand request, CancellationToken cancellationToken)
    {
        var userDetails = request.ID != null
            ? _services.Repository.Set<UserDetails>().First(record => record.UserID == request.ID)
            : new UserDetails(request.UserID);

        using var transaction = await _services.Repository.BeginTransaction();

        userDetails.Update(
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

        _services.Repository.Save(userDetails, request.ID.HasValue);

        await _services.Repository.SaveChanges();
        transaction.Commit();

        return userDetails != null;
    }

    public async Task<UserDetails?> Handle(UserDetailsQuery request, CancellationToken cancellationToken)
    {
        var userDetails = 
            _services.Repository.SetNoTracking<UserDetails>(request.Includes).FirstOrDefault(record => record.UserID == request.UserID);

        return userDetails;
    }
}
