using DataLayer.Models;
using MediatR;

public class UserGoalQuery : IRequest<UserGoal?>
{
    public Guid UserID { get; set; }

    public string[] Includes { get; set; }
}