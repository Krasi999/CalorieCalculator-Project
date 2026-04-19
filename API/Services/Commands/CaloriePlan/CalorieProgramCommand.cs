using DataLayer.Enums;
using MediatR;

public class CalorieProgramCommand : IRequest<bool>
{
    public Guid UserID;
}