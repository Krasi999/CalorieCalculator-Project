using DataLayer.Repository;
using MediatR;
using Services.Authorization;

namespace Services;

public class ServiceManager : IServices
{
    public IRepository Repository { get; }
    public IMediator Mediator { get; }
    public IAuthorization Authorization { get; }

    public ServiceManager(IRepository repository, IMediator mediator, IAuthorization authorization)
    {
        Repository = repository;
        Mediator = mediator;
        Authorization = authorization;
    }
}