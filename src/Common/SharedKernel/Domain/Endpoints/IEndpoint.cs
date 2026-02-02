using Microsoft.AspNetCore.Routing;

namespace SharedKernel.Domain.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}