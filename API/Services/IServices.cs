using DataLayer.Repository;
using MediatR;

namespace Services;

public interface IServices
{
    IRepository Repository { get; }
    IMediator Mediator { get; }
}
