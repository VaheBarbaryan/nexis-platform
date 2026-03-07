using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Posts.Persistence.Contexts;
using SharedKernel.Infrastructure;

namespace Modules.Posts.Infrastructure.ServiceInstallers;

[SuppressMessage("Usage", "CA1812", Justification = "Used via IServiceInstaller collection")]
internal sealed class PersistenceServiceInstaller : IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<PostsDbContext>((sp, options) =>
        {
            var connectionString = configuration.GetConnectionString("Database");

            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly(typeof(PostsDbContext).Assembly.FullName);
                npgsqlOptions.EnableRetryOnFailure();
                npgsqlOptions.CommandTimeout(30);
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, PostsSchema.Name);
            });

            // Snake case naming
            options.UseSnakeCaseNamingConvention();
        });
    }
}
