using DataLayer.Repository;
using MediatR;
using Services.Authorization;

namespace Services;

public interface IServices
{
    IRepository Repository { get; }
    IMediator Mediator { get; }
    IAuthorization Authorization { get; }
}
