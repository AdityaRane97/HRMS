using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents a permission in the authorization system.
/// Permissions define individual capabilities and are grouped into roles.
/// </summary>
public class Permission : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string NormalizedName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g., "Employee", "Salary", "Timesheet", "Admin"
    public bool IsSystemPermission { get; set; } // Cannot be deleted if true

    // Relationships
    public virtual ICollection<Role> Roles { get; set; } = [];

    public Permission()
    {
    }

    public Permission(string name, string category, string? description = null, bool isSystemPermission = false)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        Category = category;
        IsSystemPermission = isSystemPermission;
    }
}

/// <summary>
/// Association between a role and permissions.
/// </summary>
public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public virtual Role? Role { get; set; }

    public Guid PermissionId { get; set; }
    public virtual Permission? Permission { get; set; }
}
