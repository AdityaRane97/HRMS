using HRMS.Application.Contracts;

namespace HRMS.Infrastructure.Services;

/// <summary>
/// In-memory implementation of authorization service for Phase 1 development.
/// This will be replaced with database-backed implementation in Phase 2.
/// Supports role-based, permission-based, and hierarchy-based authorization.
/// </summary>
public class InMemoryAuthorizationService : IAuthorizationService
{
    // In-memory storage for Phase 1 - will use database in Phase 2
    private readonly Dictionary<Guid, List<string>> _userRoles = new();
    private readonly Dictionary<Guid, List<string>> _userPermissions = new();
    private readonly Dictionary<Guid, Guid?> _managerMap = new(); // employeeId -> managerId

    public Task<bool> HasRoleAsync(Guid userId, string roleName, CancellationToken cancellationToken = default)
    {
        if (_userRoles.TryGetValue(userId, out var roles))
            return Task.FromResult(roles.Contains(roleName, StringComparer.OrdinalIgnoreCase));

        return Task.FromResult(false);
    }

    public Task<bool> HasPermissionAsync(Guid userId, string permissionName, CancellationToken cancellationToken = default)
    {
        if (_userPermissions.TryGetValue(userId, out var permissions))
            return Task.FromResult(permissions.Contains(permissionName, StringComparer.OrdinalIgnoreCase));

        return Task.FromResult(false);
    }

    public async Task<bool> CanAccessEmployeeAsync(Guid actorUserId, Guid targetEmployeeId, CancellationToken cancellationToken = default)
    {
        // Admin can access anyone
        if (await HasRoleAsync(actorUserId, "Administrator", cancellationToken))
            return true;

        // HR can access any employee
        if (await HasRoleAsync(actorUserId, "HR", cancellationToken))
            return true;

        // Employee can access themselves
        if (actorUserId == targetEmployeeId)
            return true;

        // Manager can access their direct reports
        if (_managerMap.TryGetValue(targetEmployeeId, out var managerId) && managerId == actorUserId)
            return true;

        return false;
    }

    public Task<IEnumerable<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (_userRoles.TryGetValue(userId, out var roles))
            return Task.FromResult<IEnumerable<string>>(roles.AsReadOnly());

        return Task.FromResult<IEnumerable<string>>(Enumerable.Empty<string>());
    }

    public Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (_userPermissions.TryGetValue(userId, out var permissions))
            return Task.FromResult<IEnumerable<string>>(permissions.AsReadOnly());

        return Task.FromResult<IEnumerable<string>>(Enumerable.Empty<string>());
    }

    public async Task<IEnumerable<Guid>> GetAccessibleEmployeeIdsAsync(Guid actorUserId, CancellationToken cancellationToken = default)
    {
        // Phase 1: Simple in-memory implementation
        // Phase 2: Query database based on org hierarchy, roles, permissions

        var accessible = new List<Guid> { actorUserId }; // Always can access self

        if (await HasRoleAsync(actorUserId, "Administrator", cancellationToken))
        {
            // Admin can access all (but limited for now in Phase 1)
            accessible.AddRange(_managerMap.Keys);
        }
        else if (await HasRoleAsync(actorUserId, "Manager", cancellationToken))
        {
            // Manager can access their direct reports
            var directReports = _managerMap
                .Where(m => m.Value == actorUserId)
                .Select(m => m.Key);
            accessible.AddRange(directReports);
        }

        return accessible;
    }

    public async Task<bool> CanAccessSalaryDataAsync(Guid actorUserId, Guid targetEmployeeId, CancellationToken cancellationToken = default)
    {
        // Stricter access to salary data
        if (await HasPermissionAsync(actorUserId, "Salary.Manage", cancellationToken))
            return true;

        // Employee can access their own salary
        if (actorUserId == targetEmployeeId)
            return await HasPermissionAsync(actorUserId, "Salary.ReadOwn", cancellationToken);

        // Manager can access team salary with specific permission
        if (_managerMap.TryGetValue(targetEmployeeId, out var managerId) && managerId == actorUserId)
            return await HasPermissionAsync(actorUserId, "Salary.ReadTeam", cancellationToken);

        return false;
    }

    public Task<bool> IsAdministratorAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return HasRoleAsync(userId, "Administrator", cancellationToken);
    }

    /// <summary>
    /// For Phase 1 testing - add role to user.
    /// </summary>
    public void AddRoleToUser(Guid userId, string roleName)
    {
        if (!_userRoles.ContainsKey(userId))
            _userRoles[userId] = new List<string>();

        if (!_userRoles[userId].Contains(roleName))
            _userRoles[userId].Add(roleName);
    }

    /// <summary>
    /// For Phase 1 testing - add permission to user.
    /// </summary>
    public void AddPermissionToUser(Guid userId, string permissionName)
    {
        if (!_userPermissions.ContainsKey(userId))
            _userPermissions[userId] = new List<string>();

        if (!_userPermissions[userId].Contains(permissionName))
            _userPermissions[userId].Add(permissionName);
    }

    /// <summary>
    /// For Phase 1 testing - set manager relationship.
    /// </summary>
    public void SetManager(Guid employeeId, Guid managerId)
    {
        _managerMap[employeeId] = managerId;
    }
}
