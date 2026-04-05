using DataLayer.Repository;
using MediatR;

namespace Services;

public class ServiceManager : IServices
{
    public IRepository Repository { get; }
    public IMediator Mediator { get; }

    public ServiceManager(IRepository repository, IMediator mediator)
    {
        Repository = repository;
        Mediator = mediator;
    }
}