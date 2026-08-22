namespace HRMS.Application.Services;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Email { get; }

    string? UserName { get; }

    IReadOnlyCollection<string> Roles { get; }

    bool IsAuthenticated { get; }

    bool IsInRole(string role);
}