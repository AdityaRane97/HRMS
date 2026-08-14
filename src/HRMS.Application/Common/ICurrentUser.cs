namespace HRMS.Application.Common;

public interface ICurrentUser
{
    string? UserId { get; }
    string? UserName { get; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
}
