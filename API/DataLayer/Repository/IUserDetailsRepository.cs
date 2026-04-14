using DataLayer.Models.Users;

namespace DataLayer.Repository;

public interface IUserDetailsRepository
{
    Task<UserDetails?> GetByUserIdAsync(Guid userId);
    Task CreateAsync(UserDetails userDetails);
    Task UpdateAsync(UserDetails userDetails);

    Task<UserGoal?> GetActiveGoalByUserIdAsync(Guid userId);
    Task<IEnumerable<UserGoal>> GetAllGoalsByUserIdAsync(Guid userId);
    Task CreateGoalAsync(UserGoal goal);
    Task DeactivateAllGoalsAsync(Guid userId);
    Task UpdateGoalAsync(UserGoal goal);

    Task SaveChangesAsync();
}