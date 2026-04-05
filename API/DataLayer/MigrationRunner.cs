using Npgsql;
using System.Reflection;

namespace DataLayer;

public class MigrationRunner
{
    private readonly string _connectionString;

    public MigrationRunner(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task RunMigrationsAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await CreateSystemMigrationsIfNotExistsAsync(connection);

        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames()
            .Where(name => name.EndsWith(".sql"))
            .OrderBy(name => name)
            .ToList();

        foreach (var resourceName in resourceNames)
        {
            var fileName = resourceName.Substring(resourceName.LastIndexOf('.', resourceName.LastIndexOf('.') - 1) + 1);

            if (await IsAlreadyExecutedAsync(fileName, connection))
            {
                continue;
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream != null)
            {
                using var reader = new StreamReader(stream);
                var sql = await reader.ReadToEndAsync();

                await using var transaction = await connection.BeginTransactionAsync();
                try
                {
                    await using var command = new NpgsqlCommand(sql, connection, transaction);
                    await command.ExecuteNonQueryAsync();
                    await AddMigrationAsync(fileName, connection, transaction);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Failed: {fileName} - {ex.Message}");
                    throw;
                }
            }

            else
            {
                Console.WriteLine($"Null stream for {fileName}");
            }
        }
    }

    private async Task CreateSystemMigrationsIfNotExistsAsync(NpgsqlConnection connection)
    {
        var sql = @"
        CREATE TABLE IF NOT EXISTS ""SystemMigrations"" (
            ""MigrationID"" uuid DEFAULT gen_random_uuid() PRIMARY KEY,
            ""FileName"" VARCHAR (255) NOT NULL UNIQUE,
            ""CreatedAt"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
        );";

        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task<bool> IsAlreadyExecutedAsync(string fileName, NpgsqlConnection connection)
    {
        var sql = "SELECT COUNT(1) FROM \"SystemMigrations\" WHERE \"FileName\" = @fileName";

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@fileName", fileName);

        return Convert.ToInt32(await command.ExecuteScalarAsync()) > 0;
    }

    private async Task AddMigrationAsync(string fileName, NpgsqlConnection connection, NpgsqlTransaction transaction)
    {
        var sql = "INSERT INTO \"SystemMigrations\" (\"FileName\") VALUES (@fileName)";

        await using var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("@fileName", fileName);

        await command.ExecuteNonQueryAsync();
    }
}
