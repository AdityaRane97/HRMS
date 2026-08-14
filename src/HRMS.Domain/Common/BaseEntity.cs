namespace HRMS.Domain.Common;

/// <summary>
/// Base entity with common properties for all domain entities.
/// Supports optimistic concurrency and audit trails.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public string? DeletedBy { get; set; }

    /// <summary>
    /// Row version for optimistic concurrency control.
    /// </summary>
    public byte[]? RowVersion { get; set; }
}
