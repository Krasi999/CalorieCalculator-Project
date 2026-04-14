using DataLayer.Models.Common;
using DataLayer.Models.FoodDTOs;
using DataLayer.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }


    public DbSet<Migration> SystemMigrations { get; set; }
    public DbSet<FoodCategory> FoodCategories { get; set; }
    public DbSet<FoodProduct> FoodProducts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserDetails> UserDetails { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }
}
