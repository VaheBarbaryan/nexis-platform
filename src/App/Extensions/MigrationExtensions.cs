using Microsoft.EntityFrameworkCore;
using Modules.Users.Persistence.Contexts;

namespace App.Extensions;

internal static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        using var scope = app.ApplicationServices.CreateScope();
        var provider = scope.ServiceProvider;

        MigrateDb<UsersDbContext>(provider);
    }

    private static void MigrateDb<TContext>(IServiceProvider provider)
        where TContext : DbContext
    {
        using var context = provider.GetRequiredService<TContext>();
        context.Database.Migrate();
    }
}
