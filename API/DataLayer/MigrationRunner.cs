using DataLayer.Models.Common;
using DataLayer.Repository;
using Npgsql;
using System.Reflection;

namespace DataLayer;

public class MigrationRunner
{
    private readonly IRepository _repository;

    public MigrationRunner(IRepository repository)
    {
        _repository = repository;
    }

    public async Task RunMigrationsAsync()
    {
        await CreateSystemMigrationsIfNotExists();

        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".sql"))
            .OrderBy(name => name)
            .ToList();

        foreach (var resourceName in resourceNames)
        {
            var fileName = resourceName.Substring(
                resourceName.LastIndexOf('.', resourceName.LastIndexOf('.') - 1) + 1);

            var migration = _repository.SetNoTracking<Migration>().FirstOrDefault(m => m.FileName == fileName);

            if (migration != null)
            {
                continue;
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream!);
            var sql = await reader.ReadToEndAsync();

            await using var transaction = await _repository.BeginTransaction();
            try
            {
                await _repository.ExecuteSql(sql);

                await _repository.Add(new Migration
                {
                    MigrationID = Guid.NewGuid(),
                    FileName = fileName,
                    CreatedAt = DateTime.UtcNow
                });

                await _repository.SaveChanges();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Failed: {fileName} - {ex.Message}");
                throw;
            }
        }
    }

    private async Task CreateSystemMigrationsIfNotExists()
    {
        var sql = @"
            CREATE TABLE IF NOT EXISTS ""SystemMigrations"" (
                ""MigrationID"" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
                ""FileName"" VARCHAR(255) NOT NULL UNIQUE,
                ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
            );";

        await _repository.ExecuteSql(sql);
    }
}
