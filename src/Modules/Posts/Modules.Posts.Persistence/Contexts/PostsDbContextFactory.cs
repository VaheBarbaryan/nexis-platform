using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace Modules.Posts.Persistence.Contexts;

public class PostsDbContextFactory
    : IDesignTimeDbContextFactory<PostsDbContext>
{
    public PostsDbContext CreateDbContext(string[] args)
    {
        // Build configuration manually
        var basePath = Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("Database")
                               ?? "Host=localhost;Port=5432;Database=users;Username=postgres;Password=postgres;";

        var optionsBuilder = new DbContextOptionsBuilder<PostsDbContext>();

        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(PostsDbContext).Assembly.FullName);
            npgsqlOptions.EnableRetryOnFailure();
            npgsqlOptions.CommandTimeout(30);

            // Ensure __EFMigrationsHistory is inside users schema
            npgsqlOptions.MigrationsHistoryTable(
                HistoryRepository.DefaultTableName,
                PostsSchema.Name);
        });

        optionsBuilder.UseSnakeCaseNamingConvention();

        return new PostsDbContext(optionsBuilder.Options);
    }
}
