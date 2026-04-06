using DataLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }


    public DbSet<Migration> SystemMigrations { get; set; }

    public DbSet<FoodCategory> FoodCategories { get; set; }

    public DbSet<FoodProduct> FoodProducts { get; set; }
}
