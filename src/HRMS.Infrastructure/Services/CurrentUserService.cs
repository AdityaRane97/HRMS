using System.Security.Claims;
using HRMS.Application.Services;
using Microsoft.AspNetCore.Http;

namespace HRMS.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?
            .User?
            .Identity?
            .IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var userIdClaim =
                _httpContextAccessor.HttpContext?
                    .User?
                    .FindFirst("sub")?
                    .Value;

            return Guid.TryParse(userIdClaim, out var userId)
                ? userId
                : null;
        }
    }

    public string? UserName =>
        _httpContextAccessor.HttpContext?
            .User?
            .Identity?
            .Name;

    public string? Email =>
        _httpContextAccessor.HttpContext?
            .User?
            .FindFirst(ClaimTypes.Email)?
            .Value;

    public IReadOnlyCollection<string> Roles =>
        _httpContextAccessor.HttpContext?
            .User?
            .FindAll(ClaimTypes.Role)
            .Select(x => x.Value)
            .ToList()
        ?? new List<string>();

    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?
            .User?
            .IsInRole(role) ?? false;
    }
}