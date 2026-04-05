using DataLayer;
using DataLayer.Data;
using Microsoft.EntityFrameworkCore;

namespace CalorieCalculator.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // add controllers
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // register DbContext
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        var app = builder.Build();

        // run new sql migrations
        var migrationRunner = new MigrationRunner(connectionString!);
        await migrationRunner.RunMigrationsAsync();

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
