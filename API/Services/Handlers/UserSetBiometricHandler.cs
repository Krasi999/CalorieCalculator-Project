using DataLayer.Models;
using DataLayer.Repository;
using MediatR;
using Services.Commands;

namespace Services.Handlers;

public class UserSetBiometricHandler : IRequestHandler<UserSetBiometricCommand, bool>
{
    private readonly IRepository _repository;

    public UserSetBiometricHandler(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(
        UserSetBiometricCommand request,
        CancellationToken cancellationToken)
    {
        var user = _repository
            .Set<User>()
            .FirstOrDefault(u => u.Id == request.UserId);

        if (user == null) return false;

        user.SetBiometric(request.Enable);
        await _repository.SaveChanges();

        return true;
    }
}