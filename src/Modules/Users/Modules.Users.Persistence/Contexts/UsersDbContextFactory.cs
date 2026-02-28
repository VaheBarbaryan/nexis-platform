using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace Modules.Users.Persistence.Contexts;

public class UsersDbContextFactory
    : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        // Build configuration manually
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("UsersDb")
                               ?? "Host=localhost;Port=5432;Database=users;Username=postgres;Password=postgres;";

        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();

        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(UsersDbContext).Assembly.FullName);
            npgsqlOptions.EnableRetryOnFailure();
            npgsqlOptions.CommandTimeout(30);

            // Ensure __EFMigrationsHistory is inside users schema
            npgsqlOptions.MigrationsHistoryTable(
                HistoryRepository.DefaultTableName,
                UsersSchema.Name);
        });

        optionsBuilder.UseSnakeCaseNamingConvention();

        return new UsersDbContext(optionsBuilder.Options);
    }
}
