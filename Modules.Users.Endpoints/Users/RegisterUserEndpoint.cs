using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Users.Application.Contracts;
using Modules.Users.Domain.Users.Exceptions;
using Modules.Users.Endpoints.Users.Contracts;
using SharedKernel.Domain.Endpoints;
using SharedKernel.Domain.Exceptions;

namespace Modules.Users.Endpoints.Users;

public class RegisterUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/register", async (
                [FromBody] RegisterUserRequest request,
                IValidator<RegisterUserRequest> validator,
                IRegisterUserService registerUserService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var validationResult = await validator.ValidateAsync(request, cancellationToken);

                    if (!validationResult.IsValid)
                    {
                        return Results.ValidationProblem(validationResult.ToDictionary());
                    }

                    var id = await registerUserService.RegisterAsync(
                        request.Email,
                        request.Username,
                        request.Password,
                        cancellationToken);

                    return Results.Created("/api/register", id);
                }
                catch (EmailAlreadyExistsException ex)
                {
                    return TypedResults.Conflict(new { Title = ex.Message, Status = 409 });
                }
                catch (DomainException ex)
                {
                    return TypedResults.BadRequest(new { Title = ex.Message, Status = 400 });
                }
            })
            .WithTags(Tags.Users)
            .WithName("RegisterUser");
    }
}
