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

    /// <summary>
    /// Gets the primary role of the user (first role in the list)
    /// </summary>
    public string Role => Roles.FirstOrDefault() ?? "EMPLOYEE";

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Check if user has manager or admin role
    /// </summary>
    public bool IsManagerOrAdmin() => Role == "MANAGER" || Role == "ADMIN";

    // Get Employee ID from claims
    public Guid EmployeeId
    {
        get
        {
            var employeeIdStr = User?.FindFirstValue("EmployeeId");
            if (Guid.TryParse(employeeIdStr, out var employeeId))
            {
                return employeeId;
            }
            // Fallback to UserId if EmployeeId not found
            if (Guid.TryParse(UserId, out var userId))
            {
                return userId;
            }
            return Guid.Empty;
        }
    }

    // Get Email from claims
    public string? Email => User?.FindFirstValue(ClaimTypes.Email);
}
