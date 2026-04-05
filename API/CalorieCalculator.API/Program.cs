using DataLayer;
using DataLayer.Data;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using Services;

namespace CalorieCalculator.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // add controllers
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddScoped<IRepository, Repository>();

        // add mediator
        builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(IServices).Assembly));
        builder.Services.AddScoped<IServices, ServiceManager>();

        // register DbContext
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        var app = builder.Build();

        // run new sql migrations
        using (var scope = app.Services.CreateScope())
        {
            var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
            var migrationRunner = new MigrationRunner(repository);
            await migrationRunner.RunMigrationsAsync();
        }

        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
