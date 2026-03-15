using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SharedKernel.Application.Auth;

namespace SharedKernel.Infrastructure.Auth;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? Id
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User
                            .FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? _httpContextAccessor.HttpContext?.User
                            .FindFirstValue("sub");

            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public string? Email
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User
                            .FindFirstValue(ClaimTypes.Email)
                        ?? _httpContextAccessor.HttpContext?.User
                            .FindFirstValue("email");

            return !string.IsNullOrEmpty(claim) ? claim : null;
        }
    }

    public string? Username
    {
        get
        {
            var claim = _httpContextAccessor.HttpContext?.User.FindFirstValue("username");

            return !string.IsNullOrEmpty(claim) ? claim : null;
        }
    }
}
