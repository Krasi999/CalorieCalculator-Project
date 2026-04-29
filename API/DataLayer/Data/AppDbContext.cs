using DataLayer.Models;
using DataLayer.Models.Common;
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
    public DbSet<CalorieProgram> CaloriePrograms { get; set; }
    public DbSet<CalorieProgramMeal> CalorieProgramMeals { get; set; }
    public DbSet<MealFood> MealFoods { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<UserDetails> UserDetails { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }
    public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        base.OnConfiguring(optionsBuilder);
    }
}
