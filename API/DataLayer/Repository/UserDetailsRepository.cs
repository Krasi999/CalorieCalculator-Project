using DataLayer.Data;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repository;

public class UserDetailsRepository : IUserDetailsRepository
{
    private readonly AppDbContext _db;

    public UserDetailsRepository(AppDbContext db)
    {
        _db = db;
    }

    // ── UserDetails ──────────────────────────────

    public Task<UserDetails?> GetByUserIdAsync(Guid userId) =>
        _db.UserDetails
           .FirstOrDefaultAsync(x => x.UserId == userId);

    public async Task CreateAsync(UserDetails userDetails) =>
        await _db.UserDetails.AddAsync(userDetails);

    public Task UpdateAsync(UserDetails userDetails)
    {
        _db.UserDetails.Update(userDetails);
        return Task.CompletedTask;
    }

    // ── UserGoals ────────────────────────────────

    public Task<UserGoal?> GetActiveGoalByUserIdAsync(Guid userId) =>
        _db.UserGoals
           .FirstOrDefaultAsync(x => x.UserId == userId && x.IsActive);

    public async Task<IEnumerable<UserGoal>> GetAllGoalsByUserIdAsync(Guid userId) =>
        await _db.UserGoals
                 .Where(x => x.UserId == userId)
                 .OrderByDescending(x => x.CreatedAt)
                 .ToListAsync();

    public async Task CreateGoalAsync(UserGoal goal) =>
        await _db.UserGoals.AddAsync(goal);

    public async Task DeactivateAllGoalsAsync(Guid userId)
    {
        var goals = await _db.UserGoals
                             .Where(x => x.UserId == userId && x.IsActive)
                             .ToListAsync();
        foreach (var g in goals)
            g.Deactivate();
    }

    public Task UpdateGoalAsync(UserGoal goal)
    {
        _db.UserGoals.Update(goal);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}