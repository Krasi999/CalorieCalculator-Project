using DataLayer;
using DataLayer.Data;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using Services;
using Services.Authorization;

namespace CalorieCalculator.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Добави това тук:
        //builder.WebHost.UseUrls("http://192.168.115.136:5083");
        builder.WebHost.UseUrls("http://192.168.115.203:5083");

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddScoped<IRepository, Repository>();
        builder.Services.AddScoped<IAuthorization, AuthorizationService>();

        builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(IServices).Assembly));
        builder.Services.AddScoped<IServices, ServiceManager>();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        var app = builder.Build();

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