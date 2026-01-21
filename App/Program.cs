using Modules.Users.Application.Seed;
using Modules.Users.Persistence;
using SharedKernel.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddUsersPersistence(builder.Configuration);

var app = builder.Build();

using var scope = app.Services.CreateScope();
var seeders = scope.ServiceProvider.GetServices<IModuleSeeder>();

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

app.Run();
