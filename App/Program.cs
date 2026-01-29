using Autofac;
using Autofac.Extensions.DependencyInjection;
using Modules.Users.Endpoints;
using SharedKernel.Application;
using SharedKernel.Infrastructure.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.RegisterModule(new Modules.Users.Infrastructure.Outbox.OutboxModule());
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        Modules.Users.Application.UsersApplicationAssembly.Assembly,
        Modules.Users.Infrastructure.UsersInfrastructureAssembly.Assembly);
});

builder.Services.InstallModulesFromAssemblies(
    builder.Configuration,
    Modules.Users.Infrastructure.UsersInfrastructureAssembly.Assembly);

WebApplication app = builder.Build();

using IServiceScope scope = app.Services.CreateScope();
IEnumerable<IModuleSeeder> seeders = scope.ServiceProvider.GetServices<IModuleSeeder>();

foreach (var seeder in seeders)
{
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthorization();

app.MapEndpoints(UsersEndpointsAssembly.Assembly);

await app.RunAsync();
