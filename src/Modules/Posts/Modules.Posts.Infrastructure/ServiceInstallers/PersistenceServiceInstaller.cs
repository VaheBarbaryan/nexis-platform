using System.Diagnostics.CodeAnalysis;
using Autofac;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Posts.Application.Contracts;
using Modules.Posts.Application.Services;
using Modules.Posts.Domain;
using Modules.Posts.Domain.Authors.Repositories;
using Modules.Posts.Domain.Likes.Repositories;
using Modules.Posts.Domain.Posts.Repositories;
using Modules.Posts.Infrastructure.Events;
using Modules.Posts.Persistence;
using Modules.Posts.Persistence.Contexts;
using Modules.Posts.Persistence.Repositories;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.DomainEventsDispatching;
using SharedKernel.Infrastructure.Outbox;

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

        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IPostLikeRepository, PostLikeRepository>();

        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IPostUnitOfWork, PostUnitOfWork>();

        services.AddKeyedScoped<IDomainEventsDispatcher>("posts", (sp, _) =>
        {
            var mediator = sp.GetRequiredService<IMediator>();
            var scope = sp.GetRequiredService<ILifetimeScope>();
            var mapper = sp.GetRequiredKeyedService<IDomainNotificationsMapper>("posts");
            var outbox = sp.GetRequiredKeyedService<IOutbox>("posts");
            var dbContext = sp.GetRequiredService<PostsDbContext>();
            return new DomainEventsDispatcher(mediator, scope, new DomainEventsAccessor(dbContext), mapper, outbox);
        });
    }
}
