using HRMS.Application.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace HRMS.Api.Identity;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => User?.Identity?.Name;
    public IEnumerable<string> Roles => User?.FindAll(ClaimTypes.Role).Select(c => c.Value) ?? Enumerable.Empty<string>();
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
