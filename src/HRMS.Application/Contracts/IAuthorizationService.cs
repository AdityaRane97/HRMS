namespace HRMS.Application.Contracts;

/// <summary>
/// Authorization service for role-based, permission-based, and hierarchy-based authorization.
/// Centralizes all authorization logic to ensure consistent enforcement across the application.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Check if a user has a specific role.
    /// </summary>
    Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a user has a specific permission.
    /// </summary>
    Task<bool> HasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a user can access another user's data based on reporting hierarchy.
    /// Used for manager access to team members' data, HR access to broader employee population.
    /// </summary>
    Task<bool> CanAccessEmployeeAsync(Guid actorUserId, Guid targetEmployeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active roles for a user.
    /// </summary>
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all active permissions for a user.
    /// </summary>
    Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the set of employees that a user can access.
    /// Considers role, permissions, and reporting hierarchy.
    /// </summary>
    Task<IEnumerable<Guid>> GetAccessibleEmployeeIdsAsync(Guid actorUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify user access to sensitive salary data.
    /// Stricter than general employee access.
    /// </summary>
    Task<bool> CanAccessSalaryDataAsync(Guid actorUserId, Guid targetEmployeeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify user can perform administrative actions.
    /// </summary>
    Task<bool> IsAdministratorAsync(Guid userId, CancellationToken cancellationToken = default);
}
