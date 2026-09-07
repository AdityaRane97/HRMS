using HRMS.Domain.Common;

namespace HRMS.Domain.Entities;

/// <summary>
/// Junction entity linking Tasks with Documents for reference attachments
/// This enables many-to-many relationship between Tasks and Documents
/// </summary>
public class TaskDocument : BaseEntity
{
    /// <summary>
    /// Foreign key to Task
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Navigation property to Task
    /// </summary>
    public virtual TaskModel? Task { get; set; }

    /// <summary>
    /// Foreign key to Document
    /// </summary>
    public Guid DocumentId { get; set; }

    /// <summary>
    /// Navigation property to Document
    /// </summary>
    public virtual Document? Document { get; set; }

    /// <summary>
    /// Optional purpose/role of this document in the task context
    /// (e.g., "Specification", "Reference", "Template", "Requirement")
    /// </summary>
    public string? DocumentPurpose { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if this is the primary reference document for the task
    /// </summary>
    public bool IsPrimary { get; set; } = false;

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}
