using DataLayer.Models.Users;
using MediatR;


namespace Services.Handlers.Users;

public class HandlerUserDetails :
    IRequestHandler<UserDetailsCommand, Unit>,
    IRequestHandler<UserDetailsQuery, UserDetails?>
{
    private readonly IServices _services;

    public HandlerUserDetails(IServices services)
    {
        _services = services;
    }

    public async Task<Unit> Handle(UserDetailsCommand request, CancellationToken cancellationToken)
    {
        var userDetails = request.ID != null
            ? _services.Repository.Set<UserDetails>().First(record => record.UserID == request.ID)
            : new UserDetails();

        using var transaction = await _services.Repository.BeginTransaction();

        if (request.ID == null)
        {
            userDetails.MapToUser(request.UserID);
        }

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

        return Unit.Value;
    }

    public async Task<UserDetails?> Handle(UserDetailsQuery request, CancellationToken cancellationToken)
    {
        var userDetails = 
            _services.Repository.SetNoTracking<UserDetails>(request.Includes).FirstOrDefault(record => record.UserID == request.UserID);

        return userDetails;
    }
}
