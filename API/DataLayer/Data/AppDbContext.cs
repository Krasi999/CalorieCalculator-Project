using Microsoft.EntityFrameworkCore;

namespace DataLayer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    //public DbSet<Users> FoodItems { get; set; }
}
