using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Represents a role in the authorization system.
/// Roles group permissions and are assigned to users.
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string NormalizedName { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; } // Cannot be deleted if true

    // Relationships
    public virtual ICollection<Permission> Permissions { get; set; } = [];
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];

    public Role()
    {
    }

    public Role(string name, string? description = null, bool isSystemRole = false)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
        IsSystemRole = isSystemRole;
    }
}

/// <summary>
/// Represents association between a user (employee) and a role.
/// </summary>
public class UserRole : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    public Guid RoleId { get; set; }
    public virtual Role? Role { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsCurrentlyActive()
    {
        var now = DateTime.UtcNow;
        return IsActive 
            && (!ValidFrom.HasValue || ValidFrom <= now)
            && (!ValidTo.HasValue || ValidTo > now);
    }
}
